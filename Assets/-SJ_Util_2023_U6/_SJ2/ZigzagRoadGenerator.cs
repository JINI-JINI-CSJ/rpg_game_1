using UnityEngine;
using System;

// 사용법 & 팁

// 프리팹 방향: 프리팹의 Forward(+Z) 방향이 길의 진행 방향이 되도록 모델을 배치하세요. 길이 조절은 localScale.z 로 이뤄집니다. (X=폭, Y=두께)

// 빈 GameObject에 위 스크립트를 붙이고, roadPrefab 지정 → Inspector에서 Generate(ContextMenu) 실행.

// startEdge를 Random으로 두면 각 경로가 임의의 변에서 시작해 자동으로 반대편으로 향합니다.

// maxTurnAngleDeg를 키우면 더 날카롭고 지그재그가 심해지고, targetBias를 키우면 전체적으로 직진 성향이 강해집니다(반대편 변에 더 빨리 도달).

// 서로 교차 허용이라 충돌/중복 체크 없음. 필요하면 이후에 Physics.CheckBox 또는 셀 점검으로 중복 배치 방지 레이어를 얹을 수 있어요.

// 길이 조각이 경계선을 넘어갈 때 정확히 변에서 절단되어 깔끔하게 끝납니다.


// 메쉬 정렬
// 전체 길이 1 을 Z 방향
// 메쉬는 센터

public class ZigzagRoadGenerator : MonoBehaviour
{
    public enum Edge { Left, Right, Top, Bottom, Random }

    [Header("Area (center = this.transform.position)")]
    public Vector2 areaSize = new Vector2(100, 80);   // X = width, Z = height
    public float groundY = 0f;

    [Header("Road Prefab (forward=+Z)")]
    public GameObject roadPrefab;
    [Tooltip("프리팹의 가로(X) 두께")]
    public float roadWidth = 3f;
    [Tooltip("프리팹의 세로(Y) 두께")]
    public float roadThickness = 0.2f;

    [Header("Path Settings")]
    public int pathCount = 3;
    public Edge startEdge = Edge.Left;   // Random 선택 가능
    [Tooltip("한 경로의 최대 조각(세그먼트) 수")]
    public int maxSegmentsPerPath = 200;

    [Header("Segment Randomization")]
    public float minSegmentLength = 4f;
    public float maxSegmentLength = 12f;
    [Tooltip("매 세그먼트마다 좌/우로 꺾는 최대 각도(도). 0~90 권장")]
    public float maxTurnAngleDeg = 45f;
    [Tooltip("타깃 방향으로의 최소 전진 각도 가중(크면 타깃쪽으로 더 가려함) 0~1")]
    [Range(0f, 1f)] public float targetBias = 0.4f;

    [Header("Random Seed (optional)")]
    public bool useSeed = false;
    public int seed = 12345;

    [Header("Housekeeping")]
    public bool clearChildrenOnGenerate = true;

    System.Random rng;

    Bounds AreaBounds
    {
        get
        {
            Vector3 c = transform.position;
            return new Bounds(c, new Vector3(areaSize.x, 1f, areaSize.y));
        }
    }

    [ContextMenu("삭제")]
    public void ClearChild()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (roadPrefab == null)
        {
            Debug.LogError("roadPrefab 이 비어 있습니다.");
            return;
        }

        if (useSeed) rng = new System.Random(seed);
        else rng = new System.Random();

        if (clearChildrenOnGenerate)
        {
            // for (int i = transform.childCount - 1; i >= 0; --i)
            //     DestroyImmediate(transform.GetChild(i).gameObject);
            ClearChild();
        }

        for (int p = 0; p < pathCount; p++)
        {
            Edge sEdge = startEdge == Edge.Random ? (Edge)rng.Next(0, 4) : startEdge;
            Edge tEdge = Opposite(sEdge);

            // 시작점/초기 헤딩
            Vector3 startPos = GetRandomPointOnEdge(sEdge);
            Vector3 targetDir = EdgeDirection(tEdge); // 전체적인 목표 방향 (반대편 변의 법선 방향 쪽)
            Vector3 heading = (targetDir + RandomHorizontalDir(0.4f)).normalized;

            Vector3 curr = startPos;

            for (int i = 0; i < maxSegmentsPerPath; i++)
            {
                float segLen = Rand(minSegmentLength, maxSegmentLength);

                // 타깃 쪽으로 약간 편향
                heading = SteerWithBias(heading, targetDir, targetBias);

                // 좌/우 랜덤 턴
                float turn = (float)(rng.NextDouble() * 2 - 1) * maxTurnAngleDeg;
                heading = Quaternion.Euler(0f, turn, 0f) * heading;
                heading.y = 0f; heading.Normalize();

                // 다음 세그먼트의 이론적 끝점
                Vector3 desiredEnd = curr + heading * segLen;

                // 경계 밖으로 나가는지 체크하고, 나가면 반사 또는 절단
                bool hitBoundary;
                float trimLen = TrimLengthToBounds(curr, heading, segLen, out hitBoundary);
                float usedLen = trimLen;

                // 반대편 목표 변을 통과하는지 체크: 통과 시 정확히 거기서 끝
                bool reachedOpp = TryTrimToTargetEdge(curr, heading, usedLen, Opposite(sEdge), out float toTargetLen);
                if (reachedOpp) usedLen = Mathf.Min(usedLen, toTargetLen);

                // 세그먼트 배치
                PlaceSegment(curr, heading, usedLen, p, i);

                curr = curr + heading * usedLen;

                if (reachedOpp)
                    break;

                // 경계에 부딪혔다면 헤딩을 벽에 반사
                if (hitBoundary)
                {
                    heading = ReflectHeadingIfTouchingBoundary(curr, heading);
                }
            }
        }
    }

    void PlaceSegment(Vector3 start, Vector3 dir, float length, int pathIndex, int segIndex)
    {
        if (length <= 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        Vector3 center = start + dir * (length * 0.5f);
        center.y = groundY;

        GameObject go = Instantiate(roadPrefab, center, rot, transform);
        Vector3 s = go.transform.localScale;
        go.transform.localScale = new Vector3(roadWidth, roadThickness, length);
        go.name = $"Road_p{pathIndex:00}_s{segIndex:000}";
    }

    // --- Geometry helpers ---

    Vector3 GetRandomPointOnEdge(Edge e)
    {
        Bounds b = AreaBounds;
        float x = 0, z = 0;
        switch (e)
        {
            case Edge.Left:
                x = b.min.x;
                z = Rand(b.min.z, b.max.z);
                break;
            case Edge.Right:
                x = b.max.x;
                z = Rand(b.min.z, b.max.z);
                break;
            case Edge.Top:   // +Z
                z = b.max.z;
                x = Rand(b.min.x, b.max.x);
                break;
            case Edge.Bottom: // -Z
                z = b.min.z;
                x = Rand(b.min.x, b.max.x);
                break;
        }
        return new Vector3(x, groundY, z);
    }

    Edge Opposite(Edge e)
    {
        switch (e)
        {
            case Edge.Left: return Edge.Right;
            case Edge.Right: return Edge.Left;
            case Edge.Top: return Edge.Bottom;
            case Edge.Bottom: return Edge.Top;
        }
        return Edge.Right;
    }

    Vector3 EdgeDirection(Edge e)
    {
        switch (e)
        {
            case Edge.Left: return Vector3.left;
            case Edge.Right: return Vector3.right;
            case Edge.Top: return Vector3.forward;
            case Edge.Bottom: return Vector3.back;
        }
        return Vector3.right;
    }

    Vector3 RandomHorizontalDir(float jitter)
    {
        // jitter: 0~1, 값이 클수록 방향이 더 랜덤
        float angle = (float)(rng.NextDouble() * 360.0);
        Vector3 d = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        return Vector3.Lerp(Vector3.zero, d, Mathf.Clamp01(jitter)).normalized;
    }

    Vector3 SteerWithBias(Vector3 current, Vector3 targetDir, float bias)
    {
        // current 와 targetDir 사이를 보간하여 타깃 방향으로 조금 더 가도록 함
        return Vector3.Slerp(current, targetDir.normalized, Mathf.Clamp01(bias)).normalized;
    }

    // 경계에 닿을 때 세그먼트 길이를 줄여 잘라냄
    float TrimLengthToBounds(Vector3 start, Vector3 dir, float desiredLen, out bool hitBoundary)
    {
        Bounds b = AreaBounds;
        hitBoundary = false;
        float tMax = desiredLen;

        // X 방향
        if (dir.x > 1e-4f)
        {
            float t = (b.max.x - start.x) / dir.x;
            if (t >= 0f) { tMax = Mathf.Min(tMax, t); if (t < desiredLen) hitBoundary = true; }
        }
        else if (dir.x < -1e-4f)
        {
            float t = (b.min.x - start.x) / dir.x;
            if (t >= 0f) { tMax = Mathf.Min(tMax, t); if (t < desiredLen) hitBoundary = true; }
        }

        // Z 방향
        if (dir.z > 1e-4f)
        {
            float t = (b.max.z - start.z) / dir.z;
            if (t >= 0f) { tMax = Mathf.Min(tMax, t); if (t < desiredLen) hitBoundary = true; }
        }
        else if (dir.z < -1e-4f)
        {
            float t = (b.min.z - start.z) / dir.z;
            if (t >= 0f) { tMax = Mathf.Min(tMax, t); if (t < desiredLen) hitBoundary = true; }
        }

        return Mathf.Max(0f, tMax);
    }

    // 목표(반대편) 변을 지나가면 그 변에 맞춰 절단
    bool TryTrimToTargetEdge(Vector3 start, Vector3 dir, float currentLen, Edge targetEdge, out float toTargetLen)
    {
        Bounds b = AreaBounds;
        toTargetLen = currentLen;

        switch (targetEdge)
        {
            case Edge.Right:
                if (dir.x > 1e-4f)
                {
                    float t = (b.max.x - start.x) / dir.x;
                    if (t >= 0f && t <= currentLen) { toTargetLen = t; return true; }
                }
                break;
            case Edge.Left:
                if (dir.x < -1e-4f)
                {
                    float t = (b.min.x - start.x) / dir.x;
                    if (t >= 0f && t <= currentLen) { toTargetLen = t; return true; }
                }
                break;
            case Edge.Top:
                if (dir.z > 1e-4f)
                {
                    float t = (b.max.z - start.z) / dir.z;
                    if (t >= 0f && t <= currentLen) { toTargetLen = t; return true; }
                }
                break;
            case Edge.Bottom:
                if (dir.z < -1e-4f)
                {
                    float t = (b.min.z - start.z) / dir.z;
                    if (t >= 0f && t <= currentLen) { toTargetLen = t; return true; }
                }
                break;
        }
        return false;
    }

    // 경계에서 튕기듯 방향 반사
    Vector3 ReflectHeadingIfTouchingBoundary(Vector3 pos, Vector3 dir)
    {
        Bounds b = AreaBounds;

        Vector3 n = Vector3.zero;
        const float eps = 0.001f;

        if (Mathf.Abs(pos.x - b.min.x) < eps) n = Vector3.right;
        else if (Mathf.Abs(pos.x - b.max.x) < eps) n = Vector3.left;
        else if (Mathf.Abs(pos.z - b.min.z) < eps) n = Vector3.forward;
        else if (Mathf.Abs(pos.z - b.max.z) < eps) n = Vector3.back;

        if (n != Vector3.zero)
        {
            Vector3 r = Vector3.Reflect(dir, n);
            r.y = 0f;
            return r.normalized;
        }
        return dir;
    }

    // --- Utility RNG ---
    float Rand(float a, float b)
    {
        return (float)(a + (b - a) * rng.NextDouble());
    }

    // --- Gizmos ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Bounds b = AreaBounds;
        Gizmos.DrawWireCube(b.center, b.size);

        // 시작/목표 변 표시
        if (startEdge != Edge.Random)
        {
            DrawEdgeGizmo(startEdge, Color.green);
            DrawEdgeGizmo(Opposite(startEdge), Color.cyan);
        }
    }

    void DrawEdgeGizmo(Edge e, Color c)
    {
        Gizmos.color = c;
        Bounds b = AreaBounds;
        Vector3 a, d;
        switch (e)
        {
            case Edge.Left:
                a = new Vector3(b.min.x, groundY, b.min.z);
                d = new Vector3(b.min.x, groundY, b.max.z);
                Gizmos.DrawLine(a, d);
                break;
            case Edge.Right:
                a = new Vector3(b.max.x, groundY, b.min.z);
                d = new Vector3(b.max.x, groundY, b.max.z);
                Gizmos.DrawLine(a, d);
                break;
            case Edge.Top:
                a = new Vector3(b.min.x, groundY, b.max.z);
                d = new Vector3(b.max.x, groundY, b.max.z);
                Gizmos.DrawLine(a, d);
                break;
            case Edge.Bottom:
                a = new Vector3(b.min.x, groundY, b.min.z);
                d = new Vector3(b.max.x, groundY, b.min.z);
                Gizmos.DrawLine(a, d);
                break;
        }
    }
}
