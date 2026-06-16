using UnityEngine;
using QuadTreeSystem;

/// <summary>
/// QuadTree 사용 예시 — MonoBehaviour 에 붙여서 테스트
/// </summary>
public class QuadTreeExample : MonoBehaviour
{
    [Header("QuadTree 설정")]
    [SerializeField] private Vector2 worldCenter   = Vector2.zero;
    [SerializeField] private Vector2 worldHalfSize = new Vector2(50f, 50f);
    [SerializeField] private int     capacity      = 4;
    [SerializeField] private int     spawnCount    = 200;

    [Header("쿼리 설정")]
    [SerializeField] private Vector2 queryCenter   = Vector2.zero;
    [SerializeField] private float   queryRadius   = 10f;
    [SerializeField] private int     kNearestCount = 5;

    private QuadTree _tree;

    private void Start()
    {
        // 1. 트리 생성
        var boundary = new AABB(worldCenter, worldHalfSize);
        _tree = new QuadTree(boundary, capacity);

        // 2. 랜덤 포인트 삽입
        for (int i = 0; i < spawnCount; i++)
        {
            var pos = new Vector2(
                Random.Range(-worldHalfSize.x, worldHalfSize.x),
                Random.Range(-worldHalfSize.y, worldHalfSize.y));
            _tree.Insert(pos, 0 , $"object_{i}");
        }
        Debug.Log($"[QuadTree] 삽입 완료 — 총 포인트: {_tree.TotalCount}, 최대 깊이: {_tree.MaxDepth}");

        // 3. 원형 범위 쿼리
        var inCircle = _tree.QueryCircle(queryCenter, queryRadius);
        Debug.Log($"[QueryCircle] 반경 {queryRadius} 내 포인트 수: {inCircle.Count}");

        // 4. AABB 범위 쿼리
        var inRect = _tree.QueryRect(queryCenter, new Vector2(queryRadius, queryRadius));
        Debug.Log($"[QueryRect]  사각 범위 내 포인트 수: {inRect.Count}");

        // 5. 최근접 이웃
        var nearest = _tree.FindNearest(queryCenter);
        if (nearest != null)
            Debug.Log($"[Nearest]   가장 가까운 포인트: {nearest}, 데이터: {nearest.Data}");

        // 6. K-최근접 이웃
        var kNearest = _tree.FindKNearest(queryCenter, kNearestCount , 0);
        Debug.Log($"[KNearest]  상위 {kNearestCount}개 포인트:");
        foreach (var p in kNearest)
            Debug.Log($"  → {p}  dist={Vector2.Distance(p.Position, queryCenter):F2}");

        // 7. 트리 재구성 (동적 이동 후 사용 권장)
        _tree.Rebuild();
        Debug.Log($"[Rebuild]   재구성 후 포인트 수: {_tree.TotalCount}");
    }

    // Gizmo 시각화 (Scene 뷰)
    private void OnDrawGizmos()
    {
        _tree?.DrawGizmos(Color.cyan, Color.yellow);

        // 쿼리 범위 표시
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(
            new Vector3(queryCenter.x, queryCenter.y, 0), queryRadius);
    }
}
