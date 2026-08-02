using UnityEngine;

/// <summary>
/// 인스펙터에서 스케일 값을 입력하고 버튼(또는 컨텍스트 메뉴)으로 적용할 수 있게 해주는 컴포넌트.
/// "현재 객체(부모객체)에 원하는 스케일을 입력하면 하위 객체들이 그 효과를 낸다"에 대응.
/// </summary>
[DisallowMultipleComponent]
public class RectTransformScaleApplier : MonoBehaviour
{
    [Tooltip("적용할 배율. 1 = 원본, 1.5 = 150% 확대")]
    public float scale = 1f;

    [Tooltip("스케일 기준점 (0~1). 기본 (0.5,0.5) = 이 오브젝트의 rect 중앙")]
    public Vector2 pivot01 = new Vector2(0.5f, 0.5f);

    [Tooltip("체크하면 이 오브젝트 자신의 크기/위치도 함께 조정한다 (기본: 하위 객체만 조정)")]
    public bool includeSelf = false;

    RectTransform RT => transform as RectTransform;

    [ContextMenu("스케일 적용")]
    public void Apply()
    {
        if (RT == null)
        {
            Debug.LogWarning("RectTransform이 아닌 오브젝트에는 적용할 수 없습니다.", this);
            return;
        }
        RectTransformScaler.ScaleHierarchy(RT, scale, pivot01, includeSelf);
    }
}
