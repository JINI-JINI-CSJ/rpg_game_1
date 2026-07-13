using UnityEngine;
using UnityEditor;

/// <summary>
/// DistanceActivator의 각 단계(Level) 반경을 Scene 뷰에서 LODGroup처럼
/// 직접 드래그하여 조절할 수 있게 해주는 커스텀 에디터.
/// 반드시 프로젝트 내 "Editor" 폴더 안에 위치해야 한다.
/// (예: Assets/Editor/DistanceActivatorEditor.cs)
/// </summary>
[CustomEditor(typeof(DistanceActivator))]
public class DistanceActivatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var da = (DistanceActivator)target;

        if (da.levels != null && da.levels.Count > 0)
        {
            EditorGUILayout.Space();
            string current = (da.currentLevelIndex >= 0 && da.currentLevelIndex < da.levels.Count)
                ? da.levels[da.currentLevelIndex].label
                : "-";
            EditorGUILayout.LabelField("현재 거리", $"{da.lastDistance:F1} m");
            EditorGUILayout.LabelField("현재 활성 단계", current);
        }

        if (GUILayout.Button("씬에서 즉시 갱신 (Evaluate)"))
        {
            da.Evaluate();
            SceneView.RepaintAll();
        }

        EditorGUILayout.HelpBox(
            "Scene 뷰에서 각 단계의 구를 직접 드래그하면 maxDistance 값을 바로 조절할 수 있습니다.\n" +
            "마지막 단계는 항상 무제한 거리로 취급되어 구가 표시되지 않습니다.",
            MessageType.Info);
    }

    void OnSceneGUI()
    {
        var da = (DistanceActivator)target;
        if (da.levels == null || da.levels.Count == 0) return;

        Vector3 pos = da.transform.position;

        for (int i = 0; i < da.levels.Count; i++)
        {
            var lvl = da.levels[i];
            bool isLast = (i == da.levels.Count - 1);
            if (isLast) continue; // 마지막 단계는 무제한이라 핸들 없음

            EditorGUI.BeginChangeCheck();

            Handles.color = lvl.gizmoColor;
            float newDist = Handles.RadiusHandle(Quaternion.identity, pos, lvl.maxDistance);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(da, "Change Distance Activator Level Distance");
                lvl.maxDistance = Mathf.Max(0f, newDist);
                EditorUtility.SetDirty(da);
            }
        }
    }
}
