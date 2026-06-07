using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MaterialCollectorWindow : EditorWindow
{
    private GameObject targetRoot;
    private Vector2 scroll;

    private Dictionary<Material, List<Renderer>> materialUsage = new Dictionary<Material, List<Renderer>>();

    [MenuItem("Tools/Material Collector")]
    static void Open()
    {
        GetWindow<MaterialCollectorWindow>("Material Collector");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Target Root Object", EditorStyles.boldLabel);
        targetRoot = (GameObject)EditorGUILayout.ObjectField(targetRoot, typeof(GameObject), true);

        if (GUILayout.Button("Collect Materials"))
        {
            Collect();
        }

        EditorGUILayout.Space();

        if (materialUsage.Count == 0)
        {
            EditorGUILayout.HelpBox("No materials collected.", MessageType.Info);
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var pair in materialUsage)
        {
            Material mat = pair.Key;
            List<Renderer> renderers = pair.Value;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(mat, typeof(Material), false);

            GUILayout.Label($"Used by {renderers.Count} renderer(s)", GUILayout.Width(160));
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            foreach (var r in renderers)
            {
                EditorGUILayout.ObjectField(r.gameObject, typeof(GameObject), true);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    private void Collect()
    {
        materialUsage.Clear();

        if (targetRoot == null)
            return;

        Renderer[] renderers = targetRoot.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.sharedMaterials;

            foreach (Material m in mats)
            {
                if (m == null)
                    continue;

                if (!materialUsage.TryGetValue(m, out var list))
                {
                    list = new List<Renderer>();
                    materialUsage.Add(m, list);
                }

                if (!list.Contains(r))
                    list.Add(r);
            }
        }
    }
}
