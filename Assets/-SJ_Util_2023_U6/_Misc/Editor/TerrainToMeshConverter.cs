using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class TerrainToMeshConverter : EditorWindow
{
    [Header("Terrain Settings")]
    public Terrain targetTerrain;
    
    [Header("Mesh Quality Settings")]
    [Range(1, 10)]
    public int meshQualityLevel = 5; // 1 = 낮은 품질, 10 = 최고 품질
    
    [Header("Texture Quality Settings")]
    [Range(256, 4096)]
    public int textureResolution = 1024;
    
    [Header("Output Settings")]
    public string meshFileName = "TerrainMesh";
    public string textureFileName = "TerrainTexture";
    public string outputPath = "Assets/TerrainExport/";
    
    [MenuItem("Tools/Terrain to Mesh Converter")]
    public static void ShowWindow()
    {
        GetWindow<TerrainToMeshConverter>("Terrain to Mesh Converter");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Terrain to Mesh Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Terrain 선택
        EditorGUILayout.LabelField("Terrain Settings", EditorStyles.boldLabel);
        targetTerrain = (Terrain)EditorGUILayout.ObjectField("Target Terrain", targetTerrain, typeof(Terrain), true);
        
        EditorGUILayout.Space();
        
        // 품질 설정
        EditorGUILayout.LabelField("Quality Settings", EditorStyles.boldLabel);
        meshQualityLevel = EditorGUILayout.IntSlider("Mesh Quality (1-10)", meshQualityLevel, 1, 10);
        textureResolution = EditorGUILayout.IntSlider("Texture Resolution", textureResolution, 256, 4096);
        
        EditorGUILayout.Space();
        
        // 출력 설정
        EditorGUILayout.LabelField("Output Settings", EditorStyles.boldLabel);
        meshFileName = EditorGUILayout.TextField("Mesh File Name", meshFileName);
        textureFileName = EditorGUILayout.TextField("Texture File Name", textureFileName);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);
        
        EditorGUILayout.Space();
        
        // 변환 버튼
        if (GUILayout.Button("Convert Terrain to Mesh", GUILayout.Height(30)))
        {
            ConvertTerrainToMesh();
        }
        
        EditorGUILayout.Space();
        
        // 도움말
        EditorGUILayout.HelpBox("이 도구는 터레인의 지형 메쉬와 텍스처를 추출하여 일반 메쉬 파일로 변환합니다. Grass와 Tree는 무시됩니다.", MessageType.Info);
    }
    
    void ConvertTerrainToMesh()
    {
        if (targetTerrain == null)
        {
            EditorUtility.DisplayDialog("Error", "타겟 터레인을 선택해주세요.", "OK");
            return;
        }
        
        // 출력 디렉토리 생성
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        
        try
        {
            EditorUtility.DisplayProgressBar("Converting Terrain", "Extracting mesh data...", 0.2f);
            
            // 메쉬 생성
            Mesh terrainMesh = GenerateTerrainMesh();
            
            EditorUtility.DisplayProgressBar("Converting Terrain", "Creating texture...", 0.6f);
            
            // 텍스처 생성
            Texture2D combinedTexture = GenerateTerrainTexture();
            
            EditorUtility.DisplayProgressBar("Converting Terrain", "Saving files...", 0.8f);
            
            // 파일 저장
            SaveMeshAsset(terrainMesh);
            SaveTextureAsset(combinedTexture);
            
            // 머티리얼 생성 및 적용
            CreateAndAssignMaterial(terrainMesh, combinedTexture);
            
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Success", "터레인이 성공적으로 메쉬로 변환되었습니다!", "OK");
            
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error", "변환 중 오류가 발생했습니다: " + e.Message, "OK");
        }
    }
    
    Mesh GenerateTerrainMesh()
    {
        TerrainData terrainData = targetTerrain.terrainData;
        
        // 품질에 따른 해상도 계산
        int baseResolution = terrainData.heightmapResolution;
        int targetResolution = Mathf.RoundToInt(baseResolution * (meshQualityLevel / 10f));
        targetResolution = Mathf.Clamp(targetResolution, 33, baseResolution); // 최소 33x33
        
        // 높이맵 데이터 가져오기
        float[,] heights = terrainData.GetHeights(0, 0, baseResolution, baseResolution);
        
        // 다운샘플링 (품질 조절)
        float[,] sampledHeights = DownsampleHeights(heights, baseResolution, targetResolution);
        
        // 메쉬 데이터 생성
        Vector3[] vertices = new Vector3[targetResolution * targetResolution];
        Vector2[] uvs = new Vector2[targetResolution * targetResolution];
        int[] triangles = new int[(targetResolution - 1) * (targetResolution - 1) * 6];
        
        Vector3 terrainSize = terrainData.size;
        
        // 버텍스와 UV 생성
        for (int y = 0; y < targetResolution; y++)
        {
            for (int x = 0; x < targetResolution; x++)
            {
                int index = y * targetResolution + x;
                
                float normalizedX = (float)x / (targetResolution - 1);
                float normalizedY = (float)y / (targetResolution - 1);
                
                vertices[index] = new Vector3(
                    normalizedX * terrainSize.x,
                    sampledHeights[y, x] * terrainSize.y,
                    normalizedY * terrainSize.z
                );
                
                uvs[index] = new Vector2(normalizedX, normalizedY);
            }
        }
        
        // 삼각형 생성
        int triangleIndex = 0;
        for (int y = 0; y < targetResolution - 1; y++)
        {
            for (int x = 0; x < targetResolution - 1; x++)
            {
                int bottomLeft = y * targetResolution + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = (y + 1) * targetResolution + x;
                int topRight = topLeft + 1;
                
                // 첫 번째 삼각형
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomRight;
                
                // 두 번째 삼각형
                triangles[triangleIndex++] = bottomRight;
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = topRight;
            }
        }
        
        // 메쉬 생성
        Mesh mesh = new Mesh();
        mesh.name = meshFileName;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }
    
    float[,] DownsampleHeights(float[,] heights, int originalSize, int targetSize)
    {
        if (originalSize == targetSize)
            return heights;
            
        float[,] sampledHeights = new float[targetSize, targetSize];
        float ratio = (float)(originalSize - 1) / (targetSize - 1);
        
        for (int y = 0; y < targetSize; y++)
        {
            for (int x = 0; x < targetSize; x++)
            {
                float srcX = x * ratio;
                float srcY = y * ratio;
                
                int x1 = Mathf.FloorToInt(srcX);
                int y1 = Mathf.FloorToInt(srcY);
                int x2 = Mathf.Min(x1 + 1, originalSize - 1);
                int y2 = Mathf.Min(y1 + 1, originalSize - 1);
                
                float fx = srcX - x1;
                float fy = srcY - y1;
                
                // 바이리니어 보간
                float h1 = Mathf.Lerp(heights[y1, x1], heights[y1, x2], fx);
                float h2 = Mathf.Lerp(heights[y2, x1], heights[y2, x2], fx);
                sampledHeights[y, x] = Mathf.Lerp(h1, h2, fy);
            }
        }
        
        return sampledHeights;
    }
    
    Texture2D GenerateTerrainTexture()
    {
        TerrainData terrainData = targetTerrain.terrainData;
        TerrainLayer[] terrainLayers = terrainData.terrainLayers;
        
        if (terrainLayers == null || terrainLayers.Length == 0)
        {
            // 기본 텍스처 생성
            Texture2D defaultTexture = new Texture2D(textureResolution, textureResolution);
            Color[] colors = new Color[textureResolution * textureResolution];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.gray;
            }
            defaultTexture.SetPixels(colors);
            defaultTexture.Apply();
            return defaultTexture;
        }
        
        // 알파맵 가져오기
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        
        // 결합된 텍스처 생성
        Texture2D combinedTexture = new Texture2D(textureResolution, textureResolution);
        Color[] pixels = new Color[textureResolution * textureResolution];
        
        for (int y = 0; y < textureResolution; y++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                // 알파맵 좌표로 변환
                float alphaX = (float)x / textureResolution * terrainData.alphamapWidth;
                float alphaY = (float)y / textureResolution * terrainData.alphamapHeight;
                
                int alphaXInt = Mathf.FloorToInt(alphaX);
                int alphaYInt = Mathf.FloorToInt(alphaY);
                
                alphaXInt = Mathf.Clamp(alphaXInt, 0, terrainData.alphamapWidth - 1);
                alphaYInt = Mathf.Clamp(alphaYInt, 0, terrainData.alphamapHeight - 1);
                
                Color finalColor = Color.black;
                
                // 각 레이어의 기여도 계산
                for (int layer = 0; layer < terrainLayers.Length; layer++)
                {
                    if (terrainLayers[layer] != null && terrainLayers[layer].diffuseTexture != null)
                    {
                        float alpha = alphaMaps[alphaYInt, alphaXInt, layer];
                        
                        // 텍스처에서 색상 샘플링
                        Texture2D layerTexture = terrainLayers[layer].diffuseTexture;
                        Vector2 tileSize = terrainLayers[layer].tileSize;
                        
                        float texX = (x * tileSize.x / textureResolution) % 1f;
                        float texY = (y * tileSize.y / textureResolution) % 1f;
                        
                        Color layerColor = layerTexture.GetPixelBilinear(texX, texY);
                        finalColor += layerColor * alpha;
                    }
                }
                
                pixels[y * textureResolution + x] = finalColor;
            }
        }
        
        combinedTexture.SetPixels(pixels);
        combinedTexture.Apply();
        
        return combinedTexture;
    }
    
    void SaveMeshAsset(Mesh mesh)
    {
        string meshPath = outputPath + meshFileName + ".asset";
        AssetDatabase.CreateAsset(mesh, meshPath);
        AssetDatabase.SaveAssets();
    }
    
    void SaveTextureAsset(Texture2D texture)
    {
        string texturePath = outputPath + textureFileName + ".png";
        byte[] textureBytes = texture.EncodeToPNG();
        File.WriteAllBytes(texturePath, textureBytes);
        AssetDatabase.ImportAsset(texturePath);
    }
    
    void CreateAndAssignMaterial(Mesh mesh, Texture2D texture)
    {
        // 머티리얼 생성
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = texture;
        material.name = meshFileName + "_Material";
        
        string materialPath = outputPath + material.name + ".mat";
        AssetDatabase.CreateAsset(material, materialPath);
        
        // 게임오브젝트 생성 및 메쉬 적용
        GameObject terrainMeshObject = new GameObject(meshFileName);
        MeshFilter meshFilter = terrainMeshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = terrainMeshObject.AddComponent<MeshRenderer>();
        
        meshFilter.mesh = mesh;
        meshRenderer.material = material;
        
        // 원래 터레인과 같은 위치에 배치
        terrainMeshObject.transform.position = targetTerrain.transform.position;
        
        AssetDatabase.SaveAssets();
    }
}