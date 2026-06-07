// PrefabBrushPainter.cs
// Place inside an "Editor" folder.
// Fix: use Layer index (int) for EditorGUILayout.LayerField to avoid "Layer index out of bounds".
// Includes: Layer selection (single layer), Parent Container, Brush Density, and Undo Group Naming.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabBrushPainter : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private Vector2 prefabListScroll;

    private float radius = 2f;
    private float spacing = 1f;
    private int attemptsPerStamp = 30;
    private bool alignToNormal = true;
    private bool randomYRotation = true;
    private Vector2 randomScale = new Vector2(1f, 1f);
    private bool uniformScale = true;
    private int seed = 12345;

    private bool brushEnabled = false;
    private bool eraseMode = false;

    // Use single layer index (EditorGUILayout.LayerField requires an int layer index)
    private int paintLayer = 0; // layer index (0..31)
    private Transform parentContainer;

    // New options
    private float brushDensity = 1.0f; // 0.0~1.0 multiplier for attempts
    private string undoGroupName = "Prefab Brush Stroke";

    private const string kMarkerName = "_PrefabBrushPainterSpawn";

    [MenuItem("Window/Prefab Brush Painter")]
    public static void ShowWindow()
    {
        var w = GetWindow<PrefabBrushPainter>("Prefab Brush Painter");
        w.minSize = new Vector2(360, 320);
    }

    void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        brushEnabled = false;
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab Brush Settings", EditorStyles.boldLabel);

        radius = EditorGUILayout.Slider(new GUIContent("Radius"), radius, 0.1f, 50f);
        spacing = EditorGUILayout.Slider(new GUIContent("Spacing"), spacing, 0.1f, 10f);
        attemptsPerStamp = EditorGUILayout.IntSlider(new GUIContent("Base Attempts"), attemptsPerStamp, 5, 200);
        brushDensity = EditorGUILayout.Slider(new GUIContent("Brush Density", "Multiplier for placement density (0~1)"), brushDensity, 0f, 1f);

        alignToNormal = EditorGUILayout.Toggle(new GUIContent("Align To Normal"), alignToNormal);
        randomYRotation = EditorGUILayout.Toggle(new GUIContent("Random Y Rotation"), randomYRotation);

        EditorGUILayout.BeginHorizontal();
        uniformScale = EditorGUILayout.ToggleLeft("Uniform Scale", uniformScale, GUILayout.Width(110));
        if (uniformScale)
        {
            float s = EditorGUILayout.FloatField(randomScale.x, GUILayout.Width(60));
            randomScale.x = Mathf.Max(0.01f, s);
            randomScale.y = randomScale.x;
        }
        else
        {
            float minS = EditorGUILayout.FloatField(randomScale.x, GUILayout.Width(60));
            float maxS = EditorGUILayout.FloatField(randomScale.y, GUILayout.Width(60));
            randomScale.x = Mathf.Max(0.01f, Mathf.Min(minS, maxS));
            randomScale.y = Mathf.Max(randomScale.x, maxS);
        }
        EditorGUILayout.EndHorizontal();

        seed = EditorGUILayout.IntField(new GUIContent("Random Seed"), seed);

        // Correct usage: LayerField expects an int layer index (0..31)
        paintLayer = EditorGUILayout.LayerField("Paint Layer", paintLayer);
        parentContainer = (Transform)EditorGUILayout.ObjectField("Parent Container", parentContainer, typeof(Transform), true);

        EditorGUILayout.Space();
        GUILayout.Label("Undo / Naming", EditorStyles.boldLabel);
        undoGroupName = EditorGUILayout.TextField("Undo Group Name", undoGroupName);

        EditorGUILayout.Space();
        GUILayout.Label("Prefabs", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Selected Prefabs"))
        {
            foreach (var obj in Selection.gameObjects)
            {
                if (PrefabUtility.IsPartOfAnyPrefab(obj))
                {
                    if (!prefabs.Contains(obj)) prefabs.Add(obj);
                }
            }
        }

        prefabListScroll = EditorGUILayout.BeginScrollView(prefabListScroll, GUILayout.Height(120));
        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField(prefabs[i], typeof(GameObject), false);
            if (GUILayout.Button("X", GUILayout.Width(20))) { prefabs.RemoveAt(i); i--; }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (!brushEnabled)
        {
            if (GUILayout.Button("Enable Brush", GUILayout.Height(32))) { brushEnabled = true; SceneView.RepaintAll(); }
        }
        else
        {
            if (GUILayout.Button("Disable Brush", GUILayout.Height(32))) { brushEnabled = false; SceneView.RepaintAll(); }
        }

        eraseMode = GUILayout.Toggle(eraseMode, "Erase Mode", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Paint onto colliders on the selected layer. Density and Undo naming supported.", MessageType.Info);
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!brushEnabled) return;

        Event e = Event.current;
        Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        int layerMask = 1 << paintLayer;

        if (Physics.Raycast(worldRay, out RaycastHit hitInfo, 1000f, layerMask))
        {
            Handles.color = new Color(0.2f, 0.6f, 1f, 0.15f);
            Handles.DrawSolidDisc(hitInfo.point, hitInfo.normal, radius);
            Handles.color = new Color(0.2f, 0.6f, 1f, 1f);
            Handles.DrawWireDisc(hitInfo.point, hitInfo.normal, radius);

            if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0 && !e.alt)
            {
                int attempts = Mathf.CeilToInt(attemptsPerStamp * brushDensity);
                if (eraseMode) EraseAt(hitInfo.point, layerMask);
                else PaintAt(hitInfo.point, hitInfo.normal, attempts, layerMask);
                e.Use();
            }
        }

        sceneView.Repaint();
    }

    void PaintAt(Vector3 center, Vector3 normal, int attempts, int layerMask)
    {
        if (prefabs.Count == 0 || attempts <= 0) return;

        Undo.SetCurrentGroupName(undoGroupName);
        int group = Undo.GetCurrentGroup();

        System.Random rng = new System.Random(seed + (int)(center.x * 100f) + (int)(center.z * 10f));
        List<Vector3> placedPositions = new List<Vector3>();

        for (int i = 0; i < attempts; i++)
        {
            float r = (float)rng.NextDouble() * radius;
            float ang = (float)rng.NextDouble() * Mathf.PI * 2f;
            Vector3 offset = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * r;
            Vector3 samplePos = center + offset + Vector3.up * 5f;

            if (Physics.Raycast(samplePos, Vector3.down, out RaycastHit hit, 20f, layerMask))
            {
                bool tooClose = false;
                foreach (var p in placedPositions)
                {
                    if (Vector3.Distance(p, hit.point) < spacing) { tooClose = true; break; }
                }
                if (tooClose) continue;

                Collider[] nearby = Physics.OverlapSphere(hit.point, spacing * 0.45f, layerMask);
                bool collides = false;
                foreach (var c in nearby)
                {
                    if (c.gameObject.name.EndsWith(kMarkerName)) { collides = true; break; }
                }
                if (collides) continue;

                GameObject prefab = prefabs[rng.Next(prefabs.Count)];
                if (prefab == null) continue;

                GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(inst, undoGroupName);
                inst.name = inst.name + kMarkerName;

                inst.transform.position = hit.point;
                inst.transform.rotation = alignToNormal ? Quaternion.FromToRotation(Vector3.up, hit.normal) : Quaternion.identity;
                if (randomYRotation)
                    inst.transform.Rotate(Vector3.up, (float)(rng.NextDouble() * 360.0));

                float s = 1f;
                if (uniformScale)
                {
                    float min = randomScale.x, max = randomScale.y;
                    s = Mathf.Lerp(min, max, (float)rng.NextDouble());
                    inst.transform.localScale = new Vector3(s, s, s);
                }
                else
                {
                    float sx = Mathf.Lerp(randomScale.x, randomScale.y, (float)rng.NextDouble());
                    float sy = Mathf.Lerp(randomScale.x, randomScale.y, (float)rng.NextDouble());
                    float sz = Mathf.Lerp(randomScale.x, randomScale.y, (float)rng.NextDouble());
                    inst.transform.localScale = new Vector3(sx, sy, sz);
                }

                if (parentContainer != null)
                    inst.transform.SetParent(parentContainer);

                placedPositions.Add(hit.point);
            }
        }

        Undo.CollapseUndoOperations(group);
        seed += 1;
    }

    void EraseAt(Vector3 center, int layerMask)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, layerMask);
        Undo.SetCurrentGroupName(undoGroupName + " (Erase)");
        int group = Undo.GetCurrentGroup();
        foreach (var c in hits)
        {
            GameObject go = c.gameObject;
            if (go.name.EndsWith(kMarkerName))
                Undo.DestroyObjectImmediate(go);
        }
        Undo.CollapseUndoOperations(group);
    }
}
