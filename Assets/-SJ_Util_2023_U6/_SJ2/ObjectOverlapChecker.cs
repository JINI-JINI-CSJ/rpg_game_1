using UnityEngine;

public class ObjectOverlapChecker : MonoBehaviour
{
    [Header("확인할 객체들")]
    public GameObject object1;
    public GameObject object2;
    
    [Header("디버그 설정")]
    public bool showDebugInfo = true;
    
    void Update()
    {
        if (object1 != null && object2 != null)
        {
            // 각 방법으로 겹침 확인
            bool boundsOverlap = CheckBoundsOverlap(object1, object2);
            bool rendererOverlap = CheckRendererBoundsOverlap(object1, object2);
            bool screenOverlap = CheckScreenSpaceOverlap(object1, object2);
            
            if (showDebugInfo)
            {
                Debug.Log($"Bounds 겹침: {boundsOverlap}");
                Debug.Log($"Renderer Bounds 겹침: {rendererOverlap}");
                Debug.Log($"Screen Space 겹침: {screenOverlap}");
            }
        }
    }
    
    /// <summary>
    /// Collider 또는 Renderer의 Bounds를 이용한 겹침 확인
    /// </summary>
    public static bool CheckBoundsOverlap(GameObject obj1, GameObject obj2)
    {
        Bounds bounds1 = GetObjectBounds(obj1);
        Bounds bounds2 = GetObjectBounds(obj2);
        
        if (bounds1.size == Vector3.zero || bounds2.size == Vector3.zero)
            return false;
            
        return bounds1.Intersects(bounds2);
    }
    
    /// <summary>
    /// Renderer의 Bounds만을 이용한 겹침 확인 (렌더링 기준)
    /// 본체 및 하위 자식 객체의 모든 Renderer 고려
    /// </summary>
    public static bool CheckRendererBoundsOverlap(GameObject obj1, GameObject obj2)
    {
        Bounds bounds1 = GetCombinedRendererBounds(obj1);
        Bounds bounds2 = GetCombinedRendererBounds(obj2);
        
        // 유효한 bounds가 없으면 겹치지 않음
        if (bounds1.size == Vector3.zero || bounds2.size == Vector3.zero)
            return false;
            
        return bounds1.Intersects(bounds2);
    }
    
    /// <summary>
    /// 객체와 모든 하위 자식 객체의 Renderer Bounds를 결합
    /// </summary>
    static Bounds GetCombinedRendererBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);
        
        // 첫 번째 renderer의 bounds로 시작
        Bounds combinedBounds = renderers[0].bounds;
        
        // 나머지 renderer들의 bounds를 결합
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }
        
        return combinedBounds;
    }
    
    /// <summary>
    /// 스크린 공간에서의 겹침 확인 (카메라 기준)
    /// </summary>
    public static bool CheckScreenSpaceOverlap(GameObject obj1, GameObject obj2, Camera cam = null)
    {
        if (cam == null)
            cam = Camera.main;
            
        if (cam == null)
            return false;
            
        // 각 객체의 스크린 상 바운딩 박스 계산
        Rect screenRect1 = GetScreenBounds(obj1, cam);
        Rect screenRect2 = GetScreenBounds(obj2, cam);
        
        // 스크린 rect가 유효한지 확인
        if (screenRect1.width == 0 || screenRect1.height == 0 || 
            screenRect2.width == 0 || screenRect2.height == 0)
            return false;
            
        return screenRect1.Overlaps(screenRect2);
    }
    
    /// <summary>
    /// 객체의 Bounds를 가져오는 함수 (Collider 우선, 없으면 Renderer)
    /// </summary>
    static Bounds GetObjectBounds(GameObject obj)
    {
        // Collider가 있으면 Collider bounds 사용
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
            return col.bounds;
            
        // Collider가 없으면 Renderer bounds 사용
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
            return renderer.bounds;
            
        // 둘 다 없으면 자식 객체들의 bounds 계산
        return GetCombinedBounds(obj);
    }
    
    /// <summary>
    /// 자식 객체들을 포함한 전체 Bounds 계산
    /// </summary>
    static Bounds GetCombinedBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);
            
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
    }
    
    /// <summary>
    /// 스크린 공간에서의 객체 바운딩 박스 계산
    /// </summary>
    static Rect GetScreenBounds(GameObject obj, Camera cam)
    {
        Bounds bounds = GetObjectBounds(obj);
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        
        // 바운딩 박스의 8개 코너 점 계산
        Vector3[] corners = new Vector3[8];
        corners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
        corners[1] = center + new Vector3(extents.x, -extents.y, -extents.z);
        corners[2] = center + new Vector3(-extents.x, extents.y, -extents.z);
        corners[3] = center + new Vector3(extents.x, extents.y, -extents.z);
        corners[4] = center + new Vector3(-extents.x, -extents.y, extents.z);
        corners[5] = center + new Vector3(extents.x, -extents.y, extents.z);
        corners[6] = center + new Vector3(-extents.x, extents.y, extents.z);
        corners[7] = center + new Vector3(extents.x, extents.y, extents.z);
        
        // 스크린 좌표로 변환
        Vector3[] screenCorners = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            screenCorners[i] = cam.WorldToScreenPoint(corners[i]);
        }
        
        // 스크린 좌표에서 min/max 계산
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        
        for (int i = 0; i < 8; i++)
        {
            if (screenCorners[i].z > 0) // 카메라 앞쪽에 있는 점만 고려
            {
                minX = Mathf.Min(minX, screenCorners[i].x);
                maxX = Mathf.Max(maxX, screenCorners[i].x);
                minY = Mathf.Min(minY, screenCorners[i].y);
                maxY = Mathf.Max(maxY, screenCorners[i].y);
            }
        }
        
        if (minX == float.MaxValue) // 모든 점이 카메라 뒤쪽에 있음
            return new Rect(0, 0, 0, 0);
            
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
    
    /// <summary>
    /// 더 정확한 렌더러 겹침 확인 - 각 렌더러 간의 개별 겹침 체크
    /// </summary>
    public static bool CheckDetailedRendererOverlap(GameObject obj1, GameObject obj2)
    {
        Renderer[] renderers1 = obj1.GetComponentsInChildren<Renderer>();
        Renderer[] renderers2 = obj2.GetComponentsInChildren<Renderer>();
        
        if (renderers1.Length == 0 || renderers2.Length == 0)
            return false;
        
        // 각 렌더러 조합을 모두 확인
        for (int i = 0; i < renderers1.Length; i++)
        {
            for (int j = 0; j < renderers2.Length; j++)
            {
                if (renderers1[i].bounds.Intersects(renderers2[j].bounds))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 겹치는 렌더러 쌍의 정보를 반환
    /// </summary>
    public static bool CheckRendererOverlapWithDetails(GameObject obj1, GameObject obj2, out string overlapInfo)
    {
        overlapInfo = "";
        Renderer[] renderers1 = obj1.GetComponentsInChildren<Renderer>();
        Renderer[] renderers2 = obj2.GetComponentsInChildren<Renderer>();
        
        if (renderers1.Length == 0 || renderers2.Length == 0)
        {
            overlapInfo = "렌더러를 찾을 수 없습니다.";
            return false;
        }
        
        bool hasOverlap = false;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        for (int i = 0; i < renderers1.Length; i++)
        {
            for (int j = 0; j < renderers2.Length; j++)
            {
                if (renderers1[i].bounds.Intersects(renderers2[j].bounds))
                {
                    hasOverlap = true;
                    sb.AppendLine($"겹침 발견: {renderers1[i].name} <-> {renderers2[j].name}");
                }
            }
        }
        
        overlapInfo = sb.ToString();
        return hasOverlap;
    }
}

// 사용 예시 클래스
public class OverlapExample : MonoBehaviour
{
    public GameObject targetObject;
    
    void Start()
    {
        // 다른 모든 객체와 겹침 확인
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj != targetObject && obj != gameObject)
            {
                // 기본 렌더러 bounds 겹침 확인
                if (ObjectOverlapChecker.CheckRendererBoundsOverlap(targetObject, obj))
                {
                    Debug.Log($"{targetObject.name}과 {obj.name}이 겹침! (통합 bounds)");
                }
                
                // 상세한 렌더러 겹침 확인
                if (ObjectOverlapChecker.CheckDetailedRendererOverlap(targetObject, obj))
                {
                    Debug.Log($"{targetObject.name}과 {obj.name}이 겹침! (개별 renderer 확인)");
                    
                    // 겹치는 렌더러 상세 정보
                    string overlapInfo;
                    if (ObjectOverlapChecker.CheckRendererOverlapWithDetails(targetObject, obj, out overlapInfo))
                    {
                        Debug.Log($"겹침 상세정보:\n{overlapInfo}");
                    }
                }
            }
        }
    }
}