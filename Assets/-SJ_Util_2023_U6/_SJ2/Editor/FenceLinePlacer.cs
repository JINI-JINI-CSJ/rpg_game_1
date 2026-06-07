using UnityEngine;
using UnityEditor;

public class FenceLinePlacer : UnityEditor.EditorWindow
{
    [SerializeField] private GameObject fencePrefab;
    [SerializeField] private Vector3 startPosition = Vector3.zero;
    [SerializeField] private Vector3 endPosition = Vector3.forward * 10;
    [SerializeField] private bool alignToTerrain = false;
    [SerializeField] private LayerMask terrainLayerMask = 1;
    [SerializeField] private bool useManualLength = false;
    [SerializeField] private float manualFenceLength = 2f;
    
    private SerializedObject serializedObject;
    private SerializedProperty fencePrefabProperty;
    private SerializedProperty startPositionProperty;
    private SerializedProperty endPositionProperty;
    private SerializedProperty alignToTerrainProperty;
    private SerializedProperty terrainLayerMaskProperty;
    private SerializedProperty useManualLengthProperty;
    private SerializedProperty manualFenceLengthProperty;
    
    [MenuItem("Tools/Fence Line Placer")]
    public static void ShowWindow()
    {
        GetWindow<FenceLinePlacer>("Fence Line Placer");
    }
    
    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        fencePrefabProperty = serializedObject.FindProperty("fencePrefab");
        startPositionProperty = serializedObject.FindProperty("startPosition");
        endPositionProperty = serializedObject.FindProperty("endPosition");
        alignToTerrainProperty = serializedObject.FindProperty("alignToTerrain");
        terrainLayerMaskProperty = serializedObject.FindProperty("terrainLayerMask");
        
        SceneView.duringSceneGui += OnSceneGUI;
    }
    
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    
    private void OnGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Fence Line Placer", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(fencePrefabProperty, new GUIContent("Fence Prefab ( 프리펩 등록된것만 )"));
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Positions", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(startPositionProperty, new GUIContent("Start Position"));
        EditorGUILayout.PropertyField(endPositionProperty, new GUIContent("End Position"));
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(alignToTerrainProperty, new GUIContent("Align to Terrain"));
        if (alignToTerrain)
        {
            EditorGUILayout.PropertyField(terrainLayerMaskProperty, new GUIContent("Terrain Layer Mask"));
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Set Start Position from Selection"))
        {
            if (Selection.activeTransform != null)
            {
                startPosition = Selection.activeTransform.position;
            }
        }
        
        if (GUILayout.Button("Set End Position from Selection"))
        {
            if (Selection.activeTransform != null)
            {
                endPosition = Selection.activeTransform.position;
            }
        }
        
        EditorGUILayout.Space();
        
        // 거리와 예상 개수 표시
        float distance = Vector3.Distance(startPosition, endPosition);
        EditorGUILayout.LabelField($"Distance: {distance:F2} units");
        
        if (fencePrefab != null)
        {
            float fenceLength = GetFenceLength(fencePrefab);

            //Debug.Log($"Calculated fence length: {fenceLength}");
            if (fenceLength > 0)
            {
                int estimatedCount = Mathf.RoundToInt(distance / fenceLength);
                EditorGUILayout.LabelField($"Estimated fence count: {estimatedCount}");
                EditorGUILayout.LabelField($"Single fence length: {fenceLength:F2} units");
            }
        }
        
        EditorGUILayout.Space();
        
        GUI.enabled = fencePrefab != null;
        if (GUILayout.Button("Place Fence Line", GUILayout.Height(30)))
        {
            PlaceFenceLine();
        }
        GUI.enabled = true;
        
        if (GUILayout.Button("Clear All Fences"))
        {
            ClearFences();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        // 시작점과 끝점을 씬 뷰에 표시
        Handles.color = Color.green;
        Handles.DrawWireCube(startPosition, Vector3.one * 0.5f);
        Handles.Label(startPosition + Vector3.up, "Start");
        
        Handles.color = Color.red;
        Handles.DrawWireCube(endPosition, Vector3.one * 0.5f);
        Handles.Label(endPosition + Vector3.up, "End");
        
        // 연결선 표시
        Handles.color = Color.yellow;
        Handles.DrawLine(startPosition, endPosition);
        
        // 마우스로 위치 조정 가능하게 핸들 추가
        EditorGUI.BeginChangeCheck();
        Vector3 newStartPos = Handles.PositionHandle(startPosition, Quaternion.identity);
        Vector3 newEndPos = Handles.PositionHandle(endPosition, Quaternion.identity);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Move Fence Positions");
            startPosition = newStartPos;
            endPosition = newEndPos;
            Repaint();
        }
    }
    
    private float GetFenceLength(GameObject prefab)
    {
        // 우선 BoxCollider가 있다면 콜리더 기준으로 계산
        BoxCollider collider = prefab.GetComponent<BoxCollider>();
        if (collider != null)
        {
            // 콜리더의 가장 긴 축을 길이로 사용
            Vector3 size = collider.size;
            return Mathf.Max(size.x, size.z);
        }
        
        // 콜리더가 없다면 전체 바운드 계산 (하위 오브젝트 포함)
        Bounds totalBounds = GetTotalBounds(prefab);
        if (totalBounds.size != Vector3.zero)
        {
            // 가장 긴 축을 길이로 사용 (보통 펜스는 X 또는 Z축이 길다)
            return Mathf.Max(totalBounds.size.x, totalBounds.size.z);
        }
        
        return 1f; // 기본값
    }
    
    private Bounds GetTotalBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds();
        
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        
        // 로컬 공간으로 변환
        Transform transform = obj.transform;
        Vector3 center = transform.InverseTransformPoint(bounds.center);
        Vector3 size = bounds.size;
        
        // 회전을 고려하여 실제 로컬 크기 계산
        size = new Vector3(
            size.x / transform.lossyScale.x,
            size.y / transform.lossyScale.y,
            size.z / transform.lossyScale.z
        );
        
        return new Bounds(center, size);
    }
    
    private void PlaceFenceLine()
    {
        if (fencePrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a fence prefab!", "OK");
            return;
        }
        
        // 이전 펜스들 제거
        ClearFences();
        
        // 거리와 펜스 길이 계산
        Vector3 direction = (endPosition - startPosition).normalized;
        float totalDistance = Vector3.Distance(startPosition, endPosition);
        float fenceLength = GetFenceLength(fencePrefab);

        Vector3 dir_pos = direction * fenceLength;
        
        if (fenceLength <= 0)
        {
            EditorUtility.DisplayDialog("Error", "Cannot determine fence length. Make sure the prefab has a BoxCollider!", "OK");
            return;
        }

        // 펜스 개수 계산
        int fenceCount = Mathf.RoundToInt(totalDistance / fenceLength);
        if (fenceCount == 0) fenceCount = 1;
        
        // 실제 간격 계산 (정확히 맞추기 위해)
        float actualSpacing = totalDistance / fenceCount;
        
        // 부모 오브젝트 생성
        GameObject parentObject = new GameObject("Fence Line");
        Undo.RegisterCreatedObjectUndo(parentObject, "Create Fence Line");
        
        //Vector3 pos_cur = startPosition;
        // 펜스 배치
        for (int i = 0; i <= fenceCount; i++)
        {
            // float t = fenceCount > 0 ? (float)i / fenceCount : 0;
            // Vector3 position = Vector3.Lerp(startPosition, endPosition, t);
            // Debug.Log($"Placing fence {i + 1}/{fenceCount + 1} at {position}");

            Vector3 position = startPosition + dir_pos * i;

            // 지형에 정렬
            if (alignToTerrain)
            {
                if (Physics.Raycast(position + Vector3.up * 100, Vector3.down, out RaycastHit hit, 200f, terrainLayerMask))
                {
                    position.y = hit.point.y;
                }
            }

            // 회전 계산 (Z축이 방향을 향하도록)
            Quaternion rotation = Quaternion.LookRotation(direction);

            // 펜스 생성
            GameObject fence = PrefabUtility.InstantiatePrefab(fencePrefab, parentObject.transform) as GameObject;
            if (fence != null)
            {
                fence.transform.position = position;
                fence.transform.rotation = rotation;

                Undo.RegisterCreatedObjectUndo(fence, "Place Fence");
            }
            else
            {
                EditorUtility.DisplayDialog("에러!!!", "프리펩 인스턴스 생성 안됨", "OK");
                return;
            }
        }
        
        // // 마지막 펜스가 정확히 끝점에 오도록 조정
        // if (parentObject.transform.childCount > 0)
        // {
        //     Transform lastFence = parentObject.transform.GetChild(parentObject.transform.childCount - 1);
        //     Vector3 adjustedEndPos = endPosition;
            
        //     if (alignToTerrain)
        //     {
        //         if (Physics.Raycast(endPosition + Vector3.up * 100, Vector3.down, out RaycastHit hit, 200f, terrainLayerMask))
        //         {
        //             adjustedEndPos.y = hit.point.y;
        //         }
        //     }
            
        //     lastFence.position = adjustedEndPos;
        // }
        
        EditorUtility.DisplayDialog("Success", $"Created fence line with {parentObject.transform.childCount} fence pieces!", "OK");
    }
    
    private void ClearFences()
    {
        GameObject[] fenceLines = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in fenceLines)
        {
            if (obj.name.StartsWith("Fence Line"))
            {
                Undo.DestroyObjectImmediate(obj);
            }
        }
        
        // 또는 특정 이름으로 찾기
        GameObject existingFenceLine = GameObject.Find("Fence Line");
        if (existingFenceLine != null)
        {
            Undo.DestroyObjectImmediate(existingFenceLine);
        }
    }
}