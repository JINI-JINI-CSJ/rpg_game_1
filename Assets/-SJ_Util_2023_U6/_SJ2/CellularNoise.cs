using System.Collections.Generic;
using UnityEngine;

public class CellularNoise : MonoBehaviour
{
    [Header("Cellular Noise Settings")]
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;
    [SerializeField] private int numPoints = 20;
    [SerializeField] private float scale = 1f;
    [SerializeField] private bool showBoundaries = true;
    [SerializeField] private bool showPoints = true;
    
    [Header("Visualization")]
    [SerializeField] private Material boundaryMaterial;
    [SerializeField] private Material pointMaterial;
    [SerializeField] private float boundaryWidth = 0.1f;
    [SerializeField] private float pointSize = 0.5f;

    public string layerName_boundary;
    
    // 보로노이 포인트들
    private Vector2[] voronoiPoints;
    // 경계선 정보
    public List<Vector3> boundaryVertices = new List<Vector3>();
    private List<int> boundaryIndices = new List<int>();
    // 시각화용 객체들
    private GameObject boundaryParent;
    private GameObject pointsParent;
    
    // 각 셀의 경계면 정보를 저장하는 구조체
    [System.Serializable]
    public struct CellBoundary
    {
        public int cellIndex;
        public Vector2 centerPoint;
        public List<Vector2> boundaryPoints;
        public Bounds2D bounds;
    }
    
    // 2D 바운드 구조체
    [System.Serializable]
    public struct Bounds2D
    {
        public Vector2 min;
        public Vector2 max;
        public Vector2 center;
        public Vector2 size;
        
        public Bounds2D(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
            this.center = (min + max) * 0.5f;
            this.size = max - min;
        }
        
        public bool Contains(Vector2 point)
        {
            return point.x >= min.x && point.x <= max.x && 
                   point.y >= min.y && point.y <= max.y;
        }
    }
    
    public List<CellBoundary> cellBoundaries = new List<CellBoundary>();
    
    void Start()
    {
        // GenerateCellularNoise();
        // VisualizeNoise();
    }

    public float GetWidth(){return width;}
    public float GetHeight(){return height;}

    [ContextMenu("Menu_Make")]
    public void Menu_Make()
    {
        GenerateCellularNoise();
        VisualizeNoise();
    }
    
    public void GenerateCellularNoise()
    {
        // 랜덤 보로노이 포인트 생성
        voronoiPoints = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            voronoiPoints[i] = new Vector2(
                Random.Range(0f, width * scale),
                Random.Range(0f, height * scale)
            );
        }
        
        GenerateBoundaries();
    }
    
    void GenerateBoundaries()
    {
        cellBoundaries.Clear();
        boundaryVertices.Clear();
        
        float resolution = 0.5f; // 경계선 해상도
        
        // 각 보로노이 포인트에 대한 경계면 계산
        for (int pointIndex = 0; pointIndex < voronoiPoints.Length; pointIndex++)
        {
            CellBoundary cell = new CellBoundary
            {
                cellIndex = pointIndex,
                centerPoint = voronoiPoints[pointIndex],
                boundaryPoints = new List<Vector2>()
            };
            
            Vector2 minBounds = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 maxBounds = new Vector2(float.MinValue, float.MinValue);
            
            // 그리드를 순회하며 경계점들 찾기
            for (float x = 0; x < width * scale; x += resolution)
            {
                for (float z = 0; z < height * scale; z += resolution)
                {
                    Vector2 currentPoint = new Vector2(x, z);
                    int closestPointIndex = GetClosestPointIndex(currentPoint);
                    
                    if (closestPointIndex == pointIndex)
                    {
                        // 이웃한 점들을 확인하여 경계인지 판단
                        bool isBoundary = false;
                        Vector2[] neighbors = {
                            new Vector2(x + resolution, z),
                            new Vector2(x - resolution, z),
                            new Vector2(x, z + resolution),
                            new Vector2(x, z - resolution)
                        };
                        
                        foreach (Vector2 neighbor in neighbors)
                        {
                            if (neighbor.x >= 0 && neighbor.x < width * scale && 
                                neighbor.y >= 0 && neighbor.y < height * scale)
                            {
                                int neighborClosest = GetClosestPointIndex(neighbor);
                                if (neighborClosest != pointIndex)
                                {
                                    isBoundary = true;
                                    break;
                                }
                            }
                        }
                        
                        if (isBoundary)
                        {
                            cell.boundaryPoints.Add(currentPoint);
                            minBounds = Vector2.Min(minBounds, currentPoint);
                            maxBounds = Vector2.Max(maxBounds, currentPoint);
                            
                            // 시각화를 위한 버텍스 추가
                            boundaryVertices.Add(new Vector3(currentPoint.x, 0, currentPoint.y));
                        }
                    }
                }
            }
            
            cell.bounds = new Bounds2D(minBounds, maxBounds);
            cellBoundaries.Add(cell);
        }
    }
    
    int GetClosestPointIndex(Vector2 point)
    {
        int closestIndex = 0;
        float closestDistance = Vector2.Distance(point, voronoiPoints[0]);
        
        for (int i = 1; i < voronoiPoints.Length; i++)
        {
            float distance = Vector2.Distance(point, voronoiPoints[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        
        return closestIndex;
    }
    
    void VisualizeNoise()
    {
        // 기존 시각화 객체 정리
        // if (boundaryParent != null) DestroyImmediate(boundaryParent);
        // if (pointsParent != null) DestroyImmediate(pointsParent);

        SJ_Unity.Delete_Child( transform );
        
        // 경계선 시각화
        if (showBoundaries)
        {
            boundaryParent = new GameObject("Boundary Lines");
            boundaryParent.transform.parent = transform;
            
            for (int i = 0; i < cellBoundaries.Count; i++)
            {
                CreateBoundaryVisualization(cellBoundaries[i], i);
            }
        }
        
        // 보로노이 포인트 시각화
        if (showPoints)
        {
            pointsParent = new GameObject("Voronoi Points");
            pointsParent.transform.parent = transform;
            
            for (int i = 0; i < voronoiPoints.Length; i++)
            {
                CreatePointVisualization(voronoiPoints[i], i);
            }
        }
    }


    
    void CreateBoundaryVisualization(CellBoundary cell, int cellIndex)
    {
        GameObject cellBoundaryObject = new GameObject($"Cell_{cellIndex}_Boundary");
        cellBoundaryObject.transform.parent = boundaryParent.transform;
        
        foreach (Vector2 boundaryPoint in cell.boundaryPoints)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Cube);
            point.transform.parent = cellBoundaryObject.transform;
            point.transform.position = new Vector3(boundaryPoint.x, 0.1f, boundaryPoint.y);
            point.transform.localScale = Vector3.one * boundaryWidth;
            
            if (boundaryMaterial != null)
            {
                point.GetComponent<Renderer>().material = boundaryMaterial;
            }
            
            // 랜덤 색상 적용
            Color cellColor = Color.HSVToRGB((cellIndex * 0.618033988749f) % 1f, 0.7f, 0.8f);
            point.GetComponent<Renderer>().material.color = cellColor;

            // 레이어 이름
            if( string.IsNullOrEmpty( layerName_boundary ) == false )
                SJ_Unity.SetLayer_Obj( point , layerName_boundary , false );
        }
    }
    
    void CreatePointVisualization(Vector2 point, int index)
    {
        GameObject pointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointObject.name = $"VoronoiPoint_{index}";
        pointObject.transform.parent = pointsParent.transform;
        pointObject.transform.position = new Vector3(point.x, 0.2f, point.y);
        pointObject.transform.localScale = Vector3.one * pointSize;
        
        if (pointMaterial != null)
        {
            pointObject.GetComponent<Renderer>().material = pointMaterial;
        }
        
        // 포인트 색상 설정
        Color pointColor = Color.HSVToRGB((index * 0.618033988749f) % 1f, 1f, 1f);
        pointObject.GetComponent<Renderer>().material.color = pointColor;
    }
    
    // 특정 객체가 어떤 셀 경계면에 포함되는지 확인하는 함수
    public List<int> CheckObjectInCellBoundaries(GameObject obj)
    {
        List<int> containingCells = new List<int>();
        
        // 객체의 바운드 또는 콜리더 정보 가져오기
        Bounds2D objBounds2D = GetObjectBounds2D(obj);
        
        for (int i = 0; i < cellBoundaries.Count; i++)
        {
            if (IsObjectInCell(objBounds2D, cellBoundaries[i]))
            {
                containingCells.Add(i);
            }
        }
        
        return containingCells;
    }
    
    // 객체가 경계면 선에 걸치는지 정확히 확인하는 함수
    public bool IsObjectCrossingBoundary(GameObject obj)
    {
        Bounds2D objBounds = GetObjectBounds2D(obj);
        return IsObjectCrossingBoundary(objBounds);
    }
    
    // 2D 바운드가 경계면 선에 걸치는지 확인
    public bool IsObjectCrossingBoundary(Bounds2D objBounds)
    {
        float resolution = 0.2f; // 경계 검사 해상도
        
        // 객체 바운드의 모든 모서리를 검사
        Vector2[] boundaryEdges = {
            new Vector2(objBounds.min.x, objBounds.min.y), // 좌하
            new Vector2(objBounds.max.x, objBounds.min.y), // 우하
            new Vector2(objBounds.max.x, objBounds.max.y), // 우상
            new Vector2(objBounds.min.x, objBounds.max.y)  // 좌상
        };
        
        // 객체 바운드의 각 모서리 선분을 검사
        for (int i = 0; i < boundaryEdges.Length; i++)
        {
            Vector2 start = boundaryEdges[i];
            Vector2 end = boundaryEdges[(i + 1) % boundaryEdges.Length];
            
            if (IsLineCrossingBoundary(start, end, resolution))
            {
                return true;
            }
        }
        
        // 객체 내부를 관통하는 경계선이 있는지 검사
        return IsAreaCrossingBoundary(objBounds, resolution);
    }
    
    // 선분이 경계면을 지나는지 확인
    bool IsLineCrossingBoundary(Vector2 start, Vector2 end, float resolution)
    {
        float distance = Vector2.Distance(start, end);
        int steps = Mathf.CeilToInt(distance / resolution);
        
        int previousCellIndex = GetClosestPointIndex(start);
        
        for (int step = 1; step <= steps; step++)
        {
            float t = (float)step / steps;
            Vector2 currentPoint = Vector2.Lerp(start, end, t);
            
            // 맵 경계 확인
            if (currentPoint.x < 0 || currentPoint.x >= width * scale ||
                currentPoint.y < 0 || currentPoint.y >= height * scale)
                continue;
                
            int currentCellIndex = GetClosestPointIndex(currentPoint);
            
            if (currentCellIndex != previousCellIndex)
            {
                return true; // 경계면을 지남
            }
            
            previousCellIndex = currentCellIndex;
        }
        
        return false;
    }
    
    // 영역이 경계면을 지나는지 확인 (내부를 관통하는 경계선 체크)
    bool IsAreaCrossingBoundary(Bounds2D bounds, float resolution)
    {
        // 객체 영역 내부의 그리드 포인트들을 샘플링
        for (float x = bounds.min.x; x <= bounds.max.x; x += resolution)
        {
            for (float y = bounds.min.y; y <= bounds.max.y; y += resolution)
            {
                Vector2 currentPoint = new Vector2(x, y);
                
                // 맵 경계 확인
                if (currentPoint.x < 0 || currentPoint.x >= width * scale ||
                    currentPoint.y < 0 || currentPoint.y >= height * scale)
                    continue;
                
                // 이웃 포인트들과 셀 인덱스 비교
                Vector2[] neighbors = {
                    new Vector2(x + resolution, y),
                    new Vector2(x, y + resolution)
                };
                
                int currentCellIndex = GetClosestPointIndex(currentPoint);
                
                foreach (Vector2 neighbor in neighbors)
                {
                    if (neighbor.x <= bounds.max.x && neighbor.y <= bounds.max.y &&
                        neighbor.x >= 0 && neighbor.x < width * scale &&
                        neighbor.y >= 0 && neighbor.y < height * scale)
                    {
                        int neighborCellIndex = GetClosestPointIndex(neighbor);
                        if (neighborCellIndex != currentCellIndex)
                        {
                            return true; // 경계면이 객체 내부를 관통
                        }
                    }
                }
            }
        }
        
        return false;
    }
    
    // 객체가 특정 셀들 사이의 경계선에 걸치는지 확인
    public bool IsObjectCrossingSpecificBoundary(GameObject obj, int cellIndex1, int cellIndex2)
    {
        if (cellIndex1 < 0 || cellIndex1 >= cellBoundaries.Count ||
            cellIndex2 < 0 || cellIndex2 >= cellBoundaries.Count ||
            cellIndex1 == cellIndex2)
        {
            return false;
        }
        
        Bounds2D objBounds = GetObjectBounds2D(obj);
        return IsObjectCrossingSpecificBoundary(objBounds, cellIndex1, cellIndex2);
    }
    
    // 2D 바운드가 특정 두 셀 사이의 경계선에 걸치는지 확인
    bool IsObjectCrossingSpecificBoundary(Bounds2D objBounds, int cellIndex1, int cellIndex2)
    {
        float resolution = 0.2f;
        
        // 객체 영역 내에서 두 셀의 경계가 있는지 확인
        for (float x = objBounds.min.x; x <= objBounds.max.x; x += resolution)
        {
            for (float y = objBounds.min.y; y <= objBounds.max.y; y += resolution)
            {
                Vector2 currentPoint = new Vector2(x, y);
                
                // 맵 경계 확인
                if (currentPoint.x < 0 || currentPoint.x >= width * scale ||
                    currentPoint.y < 0 || currentPoint.y >= height * scale)
                    continue;
                
                int currentCellIndex = GetClosestPointIndex(currentPoint);
                
                // 현재 점이 지정된 셀 중 하나에 속하는지 확인
                if (currentCellIndex == cellIndex1 || currentCellIndex == cellIndex2)
                {
                    // 이웃 점들 확인
                    Vector2[] neighbors = {
                        new Vector2(x + resolution, y),
                        new Vector2(x - resolution, y),
                        new Vector2(x, y + resolution),
                        new Vector2(x, y - resolution)
                    };
                    
                    foreach (Vector2 neighbor in neighbors)
                    {
                        if (neighbor.x >= objBounds.min.x && neighbor.x <= objBounds.max.x &&
                            neighbor.y >= objBounds.min.y && neighbor.y <= objBounds.max.y &&
                            neighbor.x >= 0 && neighbor.x < width * scale &&
                            neighbor.y >= 0 && neighbor.y < height * scale)
                        {
                            int neighborCellIndex = GetClosestPointIndex(neighbor);
                            
                            // 두 지정된 셀 사이의 경계인지 확인
                            if ((currentCellIndex == cellIndex1 && neighborCellIndex == cellIndex2) ||
                                (currentCellIndex == cellIndex2 && neighborCellIndex == cellIndex1))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        
        return false;
    }
    
    // 객체와 교차하는 모든 경계선 정보를 반환
    public List<BoundaryIntersection> GetBoundaryIntersections(GameObject obj)
    {
        List<BoundaryIntersection> intersections = new List<BoundaryIntersection>();
        Bounds2D objBounds = GetObjectBounds2D(obj);
        
        float resolution = 0.2f;
        HashSet<string> processedBoundaries = new HashSet<string>();
        
        for (float x = objBounds.min.x; x <= objBounds.max.x; x += resolution)
        {
            for (float y = objBounds.min.y; y <= objBounds.max.y; y += resolution)
            {
                Vector2 currentPoint = new Vector2(x, y);
                
                if (currentPoint.x < 0 || currentPoint.x >= width * scale ||
                    currentPoint.y < 0 || currentPoint.y >= height * scale)
                    continue;
                
                int currentCellIndex = GetClosestPointIndex(currentPoint);
                
                Vector2[] neighbors = {
                    new Vector2(x + resolution, y),
                    new Vector2(x - resolution, y),
                    new Vector2(x, y + resolution),
                    new Vector2(x, y - resolution)
                };
                
                foreach (Vector2 neighbor in neighbors)
                {
                    if (neighbor.x >= objBounds.min.x && neighbor.x <= objBounds.max.x &&
                        neighbor.y >= objBounds.min.y && neighbor.y <= objBounds.max.y &&
                        neighbor.x >= 0 && neighbor.x < width * scale &&
                        neighbor.y >= 0 && neighbor.y < height * scale)
                    {
                        int neighborCellIndex = GetClosestPointIndex(neighbor);
                        
                        if (neighborCellIndex != currentCellIndex)
                        {
                            // 중복 처리 방지
                            string boundaryKey = $"{Mathf.Min(currentCellIndex, neighborCellIndex)}-{Mathf.Max(currentCellIndex, neighborCellIndex)}";
                            
                            if (!processedBoundaries.Contains(boundaryKey))
                            {
                                processedBoundaries.Add(boundaryKey);
                                
                                BoundaryIntersection intersection = new BoundaryIntersection
                                {
                                    cellIndex1 = currentCellIndex,
                                    cellIndex2 = neighborCellIndex,
                                    intersectionPoint = Vector2.Lerp(currentPoint, neighbor, 0.5f),
                                    boundaryKey = boundaryKey
                                };
                                
                                intersections.Add(intersection);
                            }
                        }
                    }
                }
            }
        }
        
        return intersections;
    }
    
    // 경계선 교차 정보를 담는 구조체
    [System.Serializable]
    public struct BoundaryIntersection
    {
        public int cellIndex1;
        public int cellIndex2;
        public Vector2 intersectionPoint;
        public string boundaryKey;
        
        public override string ToString()
        {
            return $"경계선 {cellIndex1}-{cellIndex2} at ({intersectionPoint.x:F2}, {intersectionPoint.y:F2})";
        }
    }
    
    // 객체의 XZ축 바운드를 계산
    Bounds2D GetObjectBounds2D(GameObject obj)
    {
        Bounds bounds;
        
        // 콜리더가 있는 경우
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            bounds = collider.bounds;
        }
        // 렌더러가 있는 경우
        else if (obj.GetComponent<Renderer>() != null)
        {
            bounds = obj.GetComponent<Renderer>().bounds;
        }
        // 기본적으로 Transform 위치 사용
        else
        {
            Vector3 pos = obj.transform.position;
            bounds = new Bounds(pos, Vector3.zero);
        }
        
        Vector2 min2D = new Vector2(bounds.min.x, bounds.min.z);
        Vector2 max2D = new Vector2(bounds.max.x, bounds.max.z);
        
        return new Bounds2D(min2D, max2D);
    }
    
    // 객체가 특정 셀에 포함되는지 확인
    bool IsObjectInCell(Bounds2D objBounds, CellBoundary cell)
    {
        // 바운드 박스 기반 빠른 검사
        if (!BoundsOverlap2D(objBounds, cell.bounds))
        {
            return false;
        }
        
        // 객체의 중심점이 해당 셀에 속하는지 확인
        Vector2 objCenter = objBounds.center;
        int closestCellIndex = GetClosestPointIndex(objCenter);
        
        return closestCellIndex == cell.cellIndex;
    }
    
    // 두 2D 바운드가 겹치는지 확인
    bool BoundsOverlap2D(Bounds2D bounds1, Bounds2D bounds2)
    {
        return bounds1.min.x <= bounds2.max.x && bounds1.max.x >= bounds2.min.x &&
               bounds1.min.y <= bounds2.max.y && bounds1.max.y >= bounds2.min.y;
    }
    
    // 디버그용: 경계면 정보 출력
    [ContextMenu("Print Cell Boundaries Info")]
    public void PrintCellBoundariesInfo()
    {
        Debug.Log($"총 {cellBoundaries.Count}개의 셀이 생성되었습니다.");
        
        for (int i = 0; i < cellBoundaries.Count; i++)
        {
            CellBoundary cell = cellBoundaries[i];
            Debug.Log($"셀 {i}: 중심점({cell.centerPoint.x:F2}, {cell.centerPoint.y:F2}), " +
                     $"경계점 수: {cell.boundaryPoints.Count}, " +
                     $"바운드: Min({cell.bounds.min.x:F2}, {cell.bounds.min.y:F2}) " +
                     $"Max({cell.bounds.max.x:F2}, {cell.bounds.max.y:F2})");
        }
    }
    
    // 특정 위치가 어떤 셀에 속하는지 확인
    public int GetCellAtPosition(Vector3 worldPosition)
    {
        Vector2 point2D = new Vector2(worldPosition.x, worldPosition.z);
        return GetClosestPointIndex(point2D);
    }
    
    // 셀 경계면 정보 반환
    public List<CellBoundary> GetCellBoundaries()
    {
        return cellBoundaries;
    }
    
    // 보로노이 포인트들 반환
    public Vector2[] GetVoronoiPoints()
    {
        return voronoiPoints;
    }
    
    void OnValidate()
    {
        // // 인스펙터에서 값 변경 시 자동 재생성
        // if (Application.isPlaying)
        // {
        //     GenerateCellularNoise();
        //     VisualizeNoise();
        // }
    }
    
    void OnDrawGizmos()
    {
        if (cellBoundaries == null || cellBoundaries.Count == 0) return;
        
        // 각 셀의 바운드 박스 그리기
        for (int i = 0; i < cellBoundaries.Count; i++)
        {
            CellBoundary cell = cellBoundaries[i];
            Color gizmoColor = Color.HSVToRGB((i * 0.618033988749f) % 1f, 0.5f, 0.7f);
            gizmoColor.a = 0.3f;
            Gizmos.color = gizmoColor;
            
            Vector3 center = new Vector3(cell.bounds.center.x, 0, cell.bounds.center.y);
            Vector3 size = new Vector3(cell.bounds.size.x, 0.1f, cell.bounds.size.y);
            
            Gizmos.DrawCube(center, size);
            
            // 중심점 그리기
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(new Vector3(cell.centerPoint.x, 0, cell.centerPoint.y), 0.2f);
        }
    }

    // 경계면 위치 랜덤
    public Vector3 GetRandomPos_Boundaries()
    {
        Transform tr = transform.Find( "Boundary Lines" );
        if( tr == null ) return Vector3.zero;

        BoxCollider[] boxs = tr.GetComponentsInChildren<BoxCollider>();
        int idx = UnityEngine.Random.Range( 0 , boxs.Length );
        BoxCollider sel = boxs[idx];

        return sel.transform.position;
    }

    public Transform Get_BoundaryLines()
    {
        return transform.Find( "Boundary Lines" );
    }

    // 실제 라인 표시 큐브가 있는 
    public List<Transform> Get_BoundaryLinesList()
    {
        List<Transform> lt = new List<Transform>();
        Transform tr = transform.Find( "Boundary Lines" );
        if( tr == null ) return lt;
        for( int i = 0 ; i < tr.childCount ; i++ )
        {
            lt.Add( tr.GetChild(i) );
        }
        return lt;
    }
}