using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
//using UnityEngine.Rendering.Universal;

public class MaterialToURPConverter : EditorWindow
{
    [MenuItem("Tools/Material to URP Converter")]
    static void Init()
    {
        MaterialToURPConverter window = (MaterialToURPConverter)EditorWindow.GetWindow(typeof(MaterialToURPConverter));
        window.titleContent = new GUIContent("Material to URP Converter");
        window.Show();
    }

    private Material[] selectedMaterials;
    private Vector2 scrollPosition;
    private bool createBackup = true;
    private string backupFolderPath = "Assets/MaterialBackups";

    // 텍스처 매핑 딕셔너리 정의
    private static readonly Dictionary<string, string[]> texturePropertyMappings = new Dictionary<string, string[]>
    {
        // Base/Albedo/Diffuse Maps
        ["_BaseMap"] = new string[] { "_MainTex", "_BaseColorMap", "_AlbedoMap", "_DiffuseMap", "_Albedo", "_ColorMap", "_Color", "_Tex", "_Texture" , "Material_Texture2D_1" },
        
        // Normal Maps
        ["_BumpMap"] = new string[] { "_NormalMap", "_Normal", "_Bump", "_BumpTex", "_NormalTex", "_DetailNormalMap", "_NormalTexture" , "Material_Texture2D_0" },
        
        // Metallic Maps
        ["_MetallicGlossMap"] = new string[] { "_MetallicMap", "_Metallic", "_MetallicTex", "_MetallicTexture", "_MetallicGlossTexture", "_SpecGlossMap", "_SpecularMap" },
        
        // Occlusion Maps
        ["_OcclusionMap"] = new string[] { "_AOMap", "_AO", "_AmbientOcclusion", "_OcclusionTex", "_AOTex", "_LightMap", "_ShadowMap" },
        
        // Emission Maps
        ["_EmissionMap"] = new string[] { "_Emission", "_EmissionTex", "_EmissiveMap", "_Emissive", "_GlowMap", "_Glow", "_EmissionTexture" },
        
        // Detail Maps
        ["_DetailAlbedoMap"] = new string[] { "_DetailTex", "_Detail", "_DetailMap", "_DetailTexture", "_DetailAlbedo", "_DetailDiffuse" },
        ["_DetailNormalMap"] = new string[] { "_DetailNormal", "_DetailBump", "_DetailNormalTex", "_DetailBumpMap" },
        
        // Height/Parallax Maps
        ["_ParallaxMap"] = new string[] { "_HeightMap", "_Height", "_ParallaxTex", "_Parallax", "_DisplacementMap", "_Displacement" }
    };

    // 플로트 프로퍼티 매핑
    private static readonly Dictionary<string, string[]> floatPropertyMappings = new Dictionary<string, string[]>
    {
        ["_Metallic"] = new string[] { "_MetallicFactor", "_MetallicValue", "_Met" },
        ["_Smoothness"] = new string[] { "_Glossiness", "_Gloss", "_Roughness", "_Smooth", "_GlossMapScale" },
        ["_BumpScale"] = new string[] { "_NormalScale", "_NormalIntensity", "_BumpIntensity" },
        ["_OcclusionStrength"] = new string[] { "_AOIntensity", "_OcclusionIntensity", "_AmbientOcclusionStrength" },
        ["_Parallax"] = new string[] { "_ParallaxScale", "_HeightScale", "_DisplacementScale" },
        ["_DetailNormalMapScale"] = new string[] { "_DetailBumpScale", "_DetailNormalScale" },
        ["_Cutoff"] = new string[] { "_AlphaCutoff", "_Threshold", "_ClipThreshold" }
    };

    // 컬러 프로퍼티 매핑
    private static readonly Dictionary<string, string[]> colorPropertyMappings = new Dictionary<string, string[]>
    {
        ["_BaseColor"] = new string[] { "_Color", "_MainColor", "_AlbedoColor", "_DiffuseColor", "_Tint" },
        ["_EmissionColor"] = new string[] { "_Emission", "_EmissiveColor", "_GlowColor", "_EmissionTint" },
        ["_SpecColor"] = new string[] { "_SpecularColor", "_Specular" }
    };

    void OnGUI()
    {
        GUILayout.Label("Material to URP Lit Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        createBackup = EditorGUILayout.Toggle("Create Backup", createBackup);
        
        if (createBackup)
        {
            EditorGUILayout.LabelField("Backup Folder Path:");
            backupFolderPath = EditorGUILayout.TextField(backupFolderPath);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Get Selected Materials"))
        {
            GetSelectedMaterials();
        }

        if (selectedMaterials != null && selectedMaterials.Length > 0)
        {
            EditorGUILayout.LabelField($"Selected Materials ({selectedMaterials.Length}):");
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            foreach (Material mat in selectedMaterials)
            {
                if (mat != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(mat, typeof(Material), false);
                    EditorGUILayout.LabelField($"Shader: {mat.shader.name}");
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Convert to URP Lit", GUILayout.Height(30)))
            {
                ConvertMaterialsToURPLit();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Select materials in Project view and click 'Get Selected Materials'", MessageType.Info);
        }
    }

    void GetSelectedMaterials()
    {
        Object[] selection = Selection.objects;
        List<Material> materials = new List<Material>();

        foreach (Object obj in selection)
        {
            if (obj is Material)
            {
                materials.Add((Material)obj);
            }
        }

        selectedMaterials = materials.ToArray();
    }

    void ConvertMaterialsToURPLit()
    {
        if (selectedMaterials == null || selectedMaterials.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No materials selected!", "OK");
            return;
        }

        // 백업 폴더 생성
        if (createBackup && !AssetDatabase.IsValidFolder(backupFolderPath))
        {
            string[] pathParts = backupFolderPath.Split('/');
            string currentPath = pathParts[0];
            
            for (int i = 1; i < pathParts.Length; i++)
            {
                string newPath = currentPath + "/" + pathParts[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                }
                currentPath = newPath;
            }
        }

        int convertedCount = 0;
        int totalCount = selectedMaterials.Length;

        for (int i = 0; i < totalCount; i++)
        {
            Material material = selectedMaterials[i];
            if (material == null) continue;

            EditorUtility.DisplayProgressBar("Converting Materials", $"Converting {material.name}...", (float)i / totalCount);

            try
            {
                ConvertSingleMaterial(material);
                convertedCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to convert material {material.name}: {e.Message}");
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Conversion Complete", 
            $"Successfully converted {convertedCount} out of {totalCount} materials to URP Lit shader.", "OK");
    }

    void ConvertSingleMaterial(Material material)
    {
        // 백업 생성
        if (createBackup)
        {
            CreateMaterialBackup(material);
        }

        // 기존 프로퍼티 저장
        Dictionary<string, object> oldProperties = new Dictionary<string, object>();
        Shader oldShader = material.shader;

        // 모든 프로퍼티 수집
        for (int i = 0; i < ShaderUtil.GetPropertyCount(oldShader); i++)
        {
            string propName = ShaderUtil.GetPropertyName(oldShader, i);
            ShaderUtil.ShaderPropertyType propType = ShaderUtil.GetPropertyType(oldShader, i);

            try
            {
                switch (propType)
                {
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        if (material.HasProperty(propName))
                            oldProperties[propName] = material.GetTexture(propName);
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                    case ShaderUtil.ShaderPropertyType.Range:
                        if (material.HasProperty(propName))
                            oldProperties[propName] = material.GetFloat(propName);
                        break;
                    case ShaderUtil.ShaderPropertyType.Color:
                        if (material.HasProperty(propName))
                            oldProperties[propName] = material.GetColor(propName);
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        if (material.HasProperty(propName))
                            oldProperties[propName] = material.GetVector(propName);
                        break;
                }
            }
            catch { /* 프로퍼티 접근 실패 시 무시 */ }
        }

        // URP Lit 셰이더로 변경
        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null)
        {
            Debug.LogError("URP Lit shader not found! Make sure URP is installed.");
            return;
        }

        material.shader = urpLitShader;

        // 텍스처 매핑
        MapTextures(material, oldProperties);
        
        // 플로트 값 매핑
        MapFloatProperties(material, oldProperties);
        
        // 컬러 값 매핑
        MapColorProperties(material, oldProperties);

        // 특수 처리
        HandleSpecialProperties(material, oldProperties);

        EditorUtility.SetDirty(material);
    }

    void CreateMaterialBackup(Material material)
    {
        string originalPath = AssetDatabase.GetAssetPath(material);
        string fileName = Path.GetFileNameWithoutExtension(originalPath);
        string extension = Path.GetExtension(originalPath);
        string backupFileName = $"{fileName}_backup_{System.DateTime.Now:yyyyMMdd_HHmmss}{extension}";
        string backupPath = Path.Combine(backupFolderPath, backupFileName);

        AssetDatabase.CopyAsset(originalPath, backupPath);
        Debug.Log($"Backup created: {backupPath}");
    }

    void MapTextures(Material material, Dictionary<string, object> oldProperties)
    {
        foreach (var mapping in texturePropertyMappings)
        {
            string urpProperty = mapping.Key;
            string[] oldPropertyNames = mapping.Value;

            foreach (string oldProp in oldPropertyNames)
            {
                if (oldProperties.ContainsKey(oldProp) && oldProperties[oldProp] is Texture)
                {
                    Texture texture = (Texture)oldProperties[oldProp];
                    if (texture != null && material.HasProperty(urpProperty))
                    {
                        material.SetTexture(urpProperty, texture);
                        Debug.Log($"Mapped texture {oldProp} -> {urpProperty} for material {material.name}");
                        break; // 첫 번째 매칭되는 텍스처만 사용
                    }
                }
            }
        }
    }

    void MapFloatProperties(Material material, Dictionary<string, object> oldProperties)
    {
        foreach (var mapping in floatPropertyMappings)
        {
            string urpProperty = mapping.Key;
            string[] oldPropertyNames = mapping.Value;

            foreach (string oldProp in oldPropertyNames)
            {
                if (oldProperties.ContainsKey(oldProp) && oldProperties[oldProp] is float)
                {
                    float value = (float)oldProperties[oldProp];
                    if (material.HasProperty(urpProperty))
                    {
                        // Roughness to Smoothness 변환
                        if (oldProp.ToLower().Contains("roughness") && urpProperty == "_Smoothness")
                        {
                            value = 1.0f - value;
                        }
                        
                        material.SetFloat(urpProperty, value);
                        Debug.Log($"Mapped float {oldProp} -> {urpProperty} = {value} for material {material.name}");
                        break;
                    }
                }
            }
        }
    }

    void MapColorProperties(Material material, Dictionary<string, object> oldProperties)
    {
        foreach (var mapping in colorPropertyMappings)
        {
            string urpProperty = mapping.Key;
            string[] oldPropertyNames = mapping.Value;

            foreach (string oldProp in oldPropertyNames)
            {
                if (oldProperties.ContainsKey(oldProp) && oldProperties[oldProp] is Color)
                {
                    Color color = (Color)oldProperties[oldProp];
                    if (material.HasProperty(urpProperty))
                    {
                        material.SetColor(urpProperty, color);
                        Debug.Log($"Mapped color {oldProp} -> {urpProperty} for material {material.name}");
                        break;
                    }
                }
            }
        }
    }

    void HandleSpecialProperties(Material material, Dictionary<string, object> oldProperties)
    {
        // Alpha 모드 설정 (투명도가 있는 경우)
        if (oldProperties.ContainsKey("_Color") && oldProperties["_Color"] is Color)
        {
            Color mainColor = (Color)oldProperties["_Color"];
            if (mainColor.a < 1.0f)
            {
                // Transparent 모드로 설정
                material.SetFloat("_Surface", 1); // Transparent
                material.SetFloat("_Blend", 0); // Alpha
            }
        }

        // Cutoff 모드 확인
        if (oldProperties.ContainsKey("_Mode") && oldProperties["_Mode"] is float)
        {
            float mode = (float)oldProperties["_Mode"];
            if (mode == 1) // Cutout mode
            {
                material.SetFloat("_Surface", 0); // Opaque
                material.SetFloat("_AlphaClip", 1); // Enable alpha clipping
            }
        }

        // 메탈릭 워크플로우 설정
        if (material.HasProperty("_WorkflowMode"))
        {
            material.SetFloat("_WorkflowMode", 1); // Metallic workflow
        }

        // 기본값 설정
        if (!material.HasProperty("_Smoothness") || material.GetFloat("_Smoothness") == 0)
        {
            material.SetFloat("_Smoothness", 0.5f);
        }
    }
}