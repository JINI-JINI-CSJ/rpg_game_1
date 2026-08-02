using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// localScale을 조정하지 않고, 하위 RectTransform들의 width/height(sizeDelta)와
/// anchoredPosition을 직접 재계산하여 "부모를 스케일한 것과 동일한 시각 효과"를 만든다.
///
/// 원리:
///  1) 대상(루트) 오브젝트의 로컬 원점(피벗)을 스케일 기준점으로 사용한다.
///     -> 실제 localScale을 조정했을 때와 동일한 기준점.
///  2) 하위 계층 전체의 "원본 월드 좌표 4모서리"를 먼저 전부 캐싱한다(아직 아무것도 변경하기 전).
///  3) 부모 -> 자식 순서로 내려가며, 각 오브젝트의 목표 코너를
///     center + (원본코너 - center) * scale 로 계산한다.
///  4) sizeDelta/anchoredPosition을 직접 계산하지 않고 offsetMin/offsetMax로 대입한다.
///     -> 앵커가 stretch든 point든 상관없이 항상 정확히 맞는다(Unity가 내부적으로
///        sizeDelta/anchoredPosition을 역산해줌).
///  5) 부모를 먼저 갱신한 뒤 "갱신된 rect"를 기준으로 자식을 계산해야
///     앵커 위치가 이동한 부분까지 정확히 반영된다. (그래서 부모->자식 재귀 순서가 중요)
///
/// 주의:
///  - static class이므로 컴포넌트로 붙지 않는다. 코드에서 직접 호출하거나
///    RectTransformScaleApplier 컴포넌트를 통해 사용한다.
///  - Canvas의 renderMode/scaleFactor 등으로 인한 world<->screen 변환은
///    RectTransform.GetWorldCorners / InverseTransformPoint가 알아서 처리하므로
///    Canvas 스케일과 무관하게 정확하다.
/// </summary>
public static class RectTransformScaler
{
    /// <summary>
    /// 하위 RectTransform 전체를 root 기준으로 scale배만큼 확대/축소한다.
    /// </summary>
    /// <param name="root">기준(부모) 오브젝트</param>
    /// <param name="scale">배율 (1.0 = 원본, 1.5 = 150%)</param>
    /// <param name="pivot01">
    /// 스케일 기준점 (root의 rect 내 0~1 비율). null이면 (0.5,0.5) = 지오메트릭 중앙.
    /// 실제 localScale과 동일한 기준(피벗)을 쓰고 싶으면 root.pivot을 넘기면 된다.
    /// </param>
    /// <param name="includeRootItself">
    /// true면 root 자신의 sizeDelta/anchoredPosition(정확히는 root의 부모 기준 offset)도 함께 조정한다.
    /// 기본은 false: root는 크기가 그대로 유지되고, 그 안의 내용물(하위 오브젝트)만 커진다.
    /// (localScale로 부모를 스케일했을 때, 부모 자신의 rect 크기는 안 변하는 것과 동일한 동작)
    /// </param>
    public static void ScaleHierarchy(RectTransform root, float scale, Vector2? pivot01 = null, bool includeRootItself = false)
    {
        if (root == null || Mathf.Approximately(scale, 1f)) return;

        Vector2 p01 = pivot01 ?? new Vector2(0.5f, 0.5f);
        Rect rootRect = root.rect;
        Vector2 centerLocal = new Vector2(
            Mathf.Lerp(rootRect.xMin, rootRect.xMax, p01.x),
            Mathf.Lerp(rootRect.yMin, rootRect.yMax, p01.y));
        Vector3 centerWorld = root.TransformPoint(centerLocal);

        // 1) 원본 월드 코너를 전부 먼저 캐싱 (아무것도 변경하기 전)
        var originalCorners = new Dictionary<RectTransform, Vector3[]>();
        CacheCorners(root, originalCorners);

        // 2) 부모 -> 자식 순서로 재귀 적용
        if (includeRootItself)
        {
            ApplyOne(root, scale, centerWorld, originalCorners);
        }

        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i) as RectTransform;
            if (child != null) ApplyRecursive(child, scale, centerWorld, originalCorners);
        }
    }

    static void CacheCorners(RectTransform rt, Dictionary<RectTransform, Vector3[]> dict)
    {
        Vector3[] c = new Vector3[4];
        rt.GetWorldCorners(c);
        dict[rt] = c;
        for (int i = 0; i < rt.childCount; i++)
        {
            var child = rt.GetChild(i) as RectTransform;
            if (child != null) CacheCorners(child, dict);
        }
    }

    static void ApplyRecursive(RectTransform rt, float scale, Vector3 centerWorld,
        Dictionary<RectTransform, Vector3[]> originalCorners)
    {
        ApplyOne(rt, scale, centerWorld, originalCorners);

        // rt가 갱신된 뒤, 그 결과(rt.rect)를 기준으로 자식을 계산해야 정확함
        for (int i = 0; i < rt.childCount; i++)
        {
            var child = rt.GetChild(i) as RectTransform;
            if (child != null) ApplyRecursive(child, scale, centerWorld, originalCorners);
        }
    }

    static void ApplyOne(RectTransform rt, float scale, Vector3 centerWorld,
        Dictionary<RectTransform, Vector3[]> originalCorners)
    {
        if (!originalCorners.TryGetValue(rt, out var orig)) return;

        // 목표 월드 코너 = center + (원본 - center) * scale
        Vector3[] target = new Vector3[4];
        for (int i = 0; i < 4; i++)
            target[i] = centerWorld + (orig[i] - centerWorld) * scale;

        RectTransform parentRT = rt.parent as RectTransform;
        if (parentRT == null) return; // 부모가 RectTransform이 아니면 처리 불가

        // 부모의 로컬 공간으로 변환 (0:bottom-left, 2:top-right)
        Vector2 bl = parentRT.InverseTransformPoint(target[0]);
        Vector2 tr = parentRT.InverseTransformPoint(target[2]);

        float xMin = Mathf.Min(bl.x, tr.x), xMax = Mathf.Max(bl.x, tr.x);
        float yMin = Mathf.Min(bl.y, tr.y), yMax = Mathf.Max(bl.y, tr.y);

        // 앵커 기준점을 "부모의 현재(=이미 갱신됐을 수도 있는) rect" 기준으로 계산
        Rect pRect = parentRT.rect;
        Vector2 anchorMinPos = new Vector2(
            pRect.xMin + rt.anchorMin.x * pRect.width,
            pRect.yMin + rt.anchorMin.y * pRect.height);
        Vector2 anchorMaxPos = new Vector2(
            pRect.xMin + rt.anchorMax.x * pRect.width,
            pRect.yMin + rt.anchorMax.y * pRect.height);

        // offsetMin/offsetMax로 대입하면 Unity가 sizeDelta/anchoredPosition을 알아서 역산해준다.
        // point 앵커든 stretch 앵커든 항상 정확히 맞는다.
        rt.offsetMin = new Vector2(xMin - anchorMinPos.x, yMin - anchorMinPos.y);
        rt.offsetMax = new Vector2(xMax - anchorMaxPos.x, yMax - anchorMaxPos.y);
    }
}
