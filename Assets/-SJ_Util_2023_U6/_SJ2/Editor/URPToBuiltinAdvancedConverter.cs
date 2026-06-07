using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class URPToBuiltinAdvancedConverter : EditorWindow
{
    private class Entry
    {
        public Material material;
        public Shader from;
        public Shader to;
        public bool enabled = true;
    }

    private readonly List<Entry> entries = new();
    private readonly HashSet<Material> collected = new();
    private Vector2 scroll;

    [MenuItem("Tools/Render Pipeline/URP → Built-in Converter (Advanced)")]
    public static void Open()
    {
        GetWindow<URPToBuiltinAdvancedConverter>("URP → Built-in");
    }

    private void OnEnable()
    {
        RefreshSelection();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("선택 항목 새로고침 (Project + Scene)", GUILayout.Height(30)))
            RefreshSelection();

        if (entries.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "변환 가능한 URP / Shader Graph Material이 없습니다.",
                MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("변환 미리보기", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var e in entries)
        {
            EditorGUILayout.BeginHorizontal("box");
            e.enabled = EditorGUILayout.Toggle(e.enabled, GUILayout.Width(20));
            EditorGUILayout.ObjectField(e.material, typeof(Material), false);
            EditorGUILayout.LabelField(e.from.name, GUILayout.Width(280));
            EditorGUILayout.LabelField("→", GUILayout.Width(18));
            EditorGUILayout.LabelField(e.to.name);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("변환 실행", GUILayout.Height(40)))
            ConvertMaterials();
    }

    // =========================================================
    // Collect
    // =========================================================

    private void RefreshSelection()
    {
        entries.Clear();
        collected.Clear();

        CollectFromProjectSelection();
        CollectFromSceneSelection();
    }

    private void CollectFromProjectSelection()
    {
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            if (obj is Material m)
                TryAdd(m);
            else if (AssetDatabase.IsValidFolder(path))
            {
                foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { path }))
                {
                    var mat = AssetDatabase.LoadAssetAtPath<Material>(
                        AssetDatabase.GUIDToAssetPath(guid));
                    TryAdd(mat);
                }
            }
        }
    }

    private void CollectFromSceneSelection()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            if (!go) continue;

            foreach (Renderer r in go.GetComponentsInChildren<Renderer>(true))
                foreach (Material m in r.sharedMaterials)
                    TryAdd(m);

            foreach (Graphic g in go.GetComponentsInChildren<Graphic>(true))
                TryAdd(g.material);
        }
    }

    private void TryAdd(Material mat)
    {
        if (!mat || !mat.shader)
            return;

        if (collected.Contains(mat))
            return;

        bool isURP =
            mat.shader.name.Contains("Universal Render Pipeline");

        bool isShaderGraph =
            IsShaderGraphShader(mat.shader);

        if (!isURP && !isShaderGraph)
            return;

        Shader target = ResolveTargetShader(mat);
        if (!target)
            return;

        collected.Add(mat);
        entries.Add(new Entry
        {
            material = mat,
            from = mat.shader,
            to = target
        });
    }

    // =========================================================
    // Shader 판단
    // =========================================================

    private bool IsShaderGraphShader(Shader shader)
    {
        return shader.name.Contains("Shader Graphs");
    }

    private Shader ResolveTargetShader(Material mat)
    {
        string name = mat.shader.name;

        if (name.Contains("Unlit"))
            return Shader.Find("Unlit/Texture");

        if (LooksLikePBR(mat))
            return Shader.Find("Standard");

        return Shader.Find("Legacy Shaders/Diffuse");
    }

    private bool LooksLikePBR(Material m)
    {
        return
            m.HasProperty("_Metallic") ||
            m.HasProperty("_Smoothness") ||
            m.HasProperty("_MetallicGlossMap") ||
            m.HasProperty("_BumpMap") ||
            m.HasProperty("Normal");
    }

    // =========================================================
    // Convert
    // =========================================================

    private void ConvertMaterials()
    {
        Undo.RecordObjects(GetEnabledMaterials(), "URP To Built-in Convert");

        foreach (var e in entries)
        {
            if (!e.enabled)
                continue;

            Material m = e.material;

            // Base Color / Texture
            CopyTextureFromAny(m,
                new[] { "_BaseMap", "_MainTex", "ColorTexture" },
                "_MainTex");

            CopyColorIfExists(m, "_BaseColor", "_Color");
            CopyColorIfExists(m, "_Color", "_Color");

            // Metallic / Smoothness
            CopyFloatIfExists(m, "_Metallic", "_Metallic");
            CopyFloatIfExists(m, "_Smoothness", "_Glossiness");
            CopyTextureIfExists(m, "_MetallicGlossMap", "_MetallicGlossMap");

            // Normal
            CopyTextureFromAny(m,
                new[] { "_BumpMap", "_NormalMap", "Normal" },
                "_BumpMap");

            if (m.GetTexture("_BumpMap"))
                m.EnableKeyword("_NORMALMAP");

            // Emission
            bool emission = false;

            if (m.HasProperty("_EmissionMap") && m.GetTexture("_EmissionMap"))
                emission = true;

            if (m.HasProperty("_EmissionColor") &&
                m.GetColor("_EmissionColor").maxColorComponent > 0f)
                emission = true;

            if (emission)
                m.EnableKeyword("_EMISSION");

            // Alpha Mode
            ApplyAlphaMode(m);

            m.shader = e.to;
            EditorUtility.SetDirty(m);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ApplyAlphaMode(Material m)
    {
        if (!m.HasProperty("_Surface"))
            return;

        bool transparent = m.GetFloat("_Surface") == 1f;
        bool cutout =
            m.HasProperty("_AlphaClip") && m.GetFloat("_AlphaClip") == 1f;

        if (cutout)
        {
            m.SetOverrideTag("RenderType", "TransparentCutout");
            m.SetInt("_ZWrite", 1);
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
        }
        else if (transparent)
        {
            m.SetOverrideTag("RenderType", "Transparent");
            m.SetInt("_ZWrite", 0);
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        else
        {
            m.SetOverrideTag("RenderType", "Opaque");
            m.SetInt("_ZWrite", 1);
            m.renderQueue = -1;
        }
    }

    // =========================================================
    // Helpers
    // =========================================================

    private Object[] GetEnabledMaterials()
    {
        List<Object> list = new();
        foreach (var e in entries)
            if (e.enabled)
                list.Add(e.material);
        return list.ToArray();
    }

    private void CopyTextureFromAny(Material m, string[] sources, string target)
    {
        if (!m.HasProperty(target))
            return;

        foreach (string src in sources)
        {
            if (!m.HasProperty(src))
                continue;

            Texture tex = m.GetTexture(src);
            if (tex)
            {
                m.SetTexture(target, tex);
                return;
            }
        }
    }

    private void CopyTextureIfExists(Material m, string from, string to)
    {
        if (m.HasProperty(from) && m.HasProperty(to))
        {
            Texture t = m.GetTexture(from);
            if (t) m.SetTexture(to, t);
        }
    }

    private void CopyColorIfExists(Material m, string from, string to)
    {
        if (m.HasProperty(from) && m.HasProperty(to))
            m.SetColor(to, m.GetColor(from));
    }

    private void CopyFloatIfExists(Material m, string from, string to)
    {
        if (m.HasProperty(from) && m.HasProperty(to))
            m.SetFloat(to, m.GetFloat(from));
    }
}
