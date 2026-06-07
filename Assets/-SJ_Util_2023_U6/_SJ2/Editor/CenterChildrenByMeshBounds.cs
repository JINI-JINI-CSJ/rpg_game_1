using UnityEngine;
using UnityEditor;

public class CenterChildrenByMeshBounds : EditorWindow
{
    private GameObject targetObject;
    private GameObject rendererSourceObject;
    private bool alignToGround = false;

    [MenuItem("Tools/Center Children By Mesh Bounds")]
    static void Init()
    {
        GetWindow<CenterChildrenByMeshBounds>("Center Children");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("하위 MeshRenderer 기준 중심 정렬", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        targetObject = (GameObject)EditorGUILayout.ObjectField("대상 객체 (이동할 부모)", targetObject, typeof(GameObject), true);
        rendererSourceObject = (GameObject)EditorGUILayout.ObjectField("렌더러 기준 객체 (선택 안하면 대상 객체)", rendererSourceObject, typeof(GameObject), true);
        alignToGround = EditorGUILayout.Toggle("Y=0 기준 정렬 (바닥)", alignToGround);

        EditorGUILayout.Space(15);

        if (GUILayout.Button("중심으로 정렬하기", GUILayout.Height(30)))
        {
            if (targetObject == null)
            {
                EditorUtility.DisplayDialog("오류", "대상 객체를 지정하세요.", "확인");
                return;
            }

            GameObject source = rendererSourceObject != null ? rendererSourceObject : targetObject;
            CenterByMeshBounds(targetObject, source, alignToGround);
        }
    }

    static void CenterByMeshBounds(GameObject parent, GameObject source, bool alignToGround)
    {
        MeshRenderer[] renderers = source.GetComponentsInChildren<MeshRenderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"[{source.name}] 하위에서 MeshRenderer를 찾을 수 없습니다.");
            return;
        }

        // 전체 Bounds 계산
        Bounds totalBounds = renderers[0].bounds;
        foreach (var rend in renderers)
            totalBounds.Encapsulate(rend.bounds);

        Vector3 worldCenter = totalBounds.center;
        float minY = totalBounds.min.y;

        // 부모 기준 로컬 오프셋 계산
        Vector3 localOffset = parent.transform.InverseTransformPoint(worldCenter);

        if (alignToGround)
        {
            // 바닥면 기준으로 조정
            float worldYOffset = worldCenter.y - minY;
            Vector3 worldGroundCenter = worldCenter;
            worldGroundCenter.y -= worldYOffset * 0.5f;
            localOffset = parent.transform.InverseTransformPoint(worldGroundCenter);
        }

        Undo.RecordObject(parent.transform, "Center Children By Bounds");

        // 하위 객체 이동
        foreach (Transform child in parent.transform)
            child.localPosition -= localOffset;

        Debug.Log($"[{parent.name}] 중심 정렬 완료 (Renderer 기준: {source.name}, alignToGround: {alignToGround})");
    }
}
