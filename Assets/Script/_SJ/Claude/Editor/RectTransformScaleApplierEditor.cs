using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RectTransformScaleApplier))]
public class RectTransformScaleApplierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var applier = (RectTransformScaleApplier)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("스케일 적용", GUILayout.Height(30)))
        {
            Undo.RegisterFullObjectHierarchyUndo(applier.gameObject, "Apply RectTransform Scale");
            applier.Apply();
            EditorUtility.SetDirty(applier);
        }
    }
}
