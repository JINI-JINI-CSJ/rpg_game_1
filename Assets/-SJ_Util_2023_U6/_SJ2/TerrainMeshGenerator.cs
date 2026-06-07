using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[System.Serializable]
public class TerrainTexture
{
    public Texture2D diffuse;
    public Texture2D normal;
    public float minHeight = 0f;
    public float maxHeight = 1f;
    public float minSlope = 0f;
    public float maxSlope = 90f;
    public Color tint = Color.white;
}

[System.Serializable]
public class BiomeObject
{
    public GameObject prefab;
    public float density = 0.1f;
    public float minHeight = 0f;
    public float maxHeight = 1f;
    public float minSlope = 0f;
    public float maxSlope = 30f;
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);
}

[System.Serializable]
public class GeneralObject
{
    public GameObject prefab;
    public int count = 10;
    public float minHeight = 0f;
    public float maxHeight = 1f;
    public float minSlope = 0f;
    public float maxSlope = 45f;
    public Vector2 scaleRange = new Vector2(1f, 1f);
}

public class TerrainMeshGenerator : MonoBehaviour
{
    [Header("지형 설정")]
    public int width = 256;
    public int height = 256;
    public float scale = 10f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    [Header("언덕 설정")]
    public int hillCount = 5;
    public float hillRadius = 30f;
    public float hillHeight = 5f;
    
    [Header("구덩이 설정")]
    public int craterCount = 3;
    public float craterRadius = 20f;
    public float craterDepth = 3f;
    
    [Header("텍스처 설정")]
    public TerrainTexture[] terrainTextures = new TerrainTexture[4];
    public int blendTextureSize = 512;
    
    [Header("바이옴 오브젝트")]
    public BiomeObject[] biomeObjects = new BiomeObject[0];
    
    [Header("기타 오브젝트")]
    public GeneralObject[] generalObjects = new GeneralObject[0];
    
    [Header("생성 설정")]
    public bool generateOnStart = false;
    public string saveAssetPath = "Assets/GeneratedTerrain/";
    
    private float[,] heightMap;
    private Vector3[,] normalMap;
    private List<Bounds> placedObjectBounds = new List<Bounds>();
    
    void Start()
    {
        if (generateOnStart)
        {
            GenerateTerrain();
        }
    }
    
    [ContextMenu("지형 생성")]
    public void GenerateTerrain()
    {
        ClearExistingTerrain();
        GenerateHeightMap();
        GenerateNormals();
        CreateMesh();
        GenerateBlendTexture();
        PlaceObjects();
    }
    
    void ClearExistingTerrain()
    {
        // 기존 자식 오브젝트들 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        placedObjectBounds.Clear();
    }
    
    void GenerateHeightMap()
    {
        heightMap = new float[width, height];
        
        // 베이스 노이즈 생성
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;
                
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;
                    
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                
                heightMap[x, y] = heightCurve.Evaluate(Mathf.InverseLerp(-1, 1, noiseHeight));
            }
        }
        
        // 언덕 추가
        for (int i = 0; i < hillCount; i++)
        {
            Vector2 hillCenter = new Vector2(
                Random.Range(hillRadius, width - hillRadius),
                Random.Range(hillRadius, height - hillRadius)
            );
            
            AddHill(hillCenter, hillRadius, hillHeight);
        }
        
        // 구덩이 추가
        for (int i = 0; i < craterCount; i++)
        {
            Vector2 craterCenter = new Vector2(
                Random.Range(craterRadius, width - craterRadius),
                Random.Range(craterRadius, height - craterRadius)
            );
            
            AddCrater(craterCenter, craterRadius, craterDepth);
        }
    }
    
    void AddHill(Vector2 center, float radius, float maxHeight)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance < radius)
                {
                    float falloff = 1f - (distance / radius);
                    falloff = Mathf.SmoothStep(0f, 1f, falloff);
                    heightMap[x, y] += falloff * maxHeight;
                }
            }
        }
    }
    
    void AddCrater(Vector2 center, float radius, float depth)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance < radius)
                {
                    float falloff = 1f - (distance / radius);
                    falloff = Mathf.SmoothStep(0f, 1f, falloff);
                    heightMap[x, y] -= falloff * depth;
                }
            }
        }
    }
    
    void GenerateNormals()
    {
        normalMap = new Vector3[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightL = GetHeight(x - 1, y);
                float heightR = GetHeight(x + 1, y);
                float heightD = GetHeight(x, y - 1);
                float heightU = GetHeight(x, y + 1);
                
                Vector3 normal = new Vector3(heightL - heightR, 2f, heightD - heightU);
                normalMap[x, y] = normal.normalized;
            }
        }
    }
    
    float GetHeight(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return 0f;
        return heightMap[x, y];
    }
    
    void CreateMesh()
    {
        GameObject terrainObject = new GameObject("Generated Terrain");
        terrainObject.transform.parent = transform;
        
        MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = terrainObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = terrainObject.AddComponent<MeshCollider>();
        
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        
        // 버텍스 생성
        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];
        Vector3[] normals = new Vector3[width * height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int i = y * width + x;
                vertices[i] = new Vector3(x, heightMap[x, y] * scale, y);
                uvs[i] = new Vector2((float)x / width, (float)y / height);
                normals[i] = normalMap[x, y];
            }
        }
        
        // 삼각형 생성
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        int triangleIndex = 0;
        
        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                int i = y * width + x;
                
                triangles[triangleIndex] = i;
                triangles[triangleIndex + 1] = i + width;
                triangles[triangleIndex + 2] = i + 1;
                
                triangles[triangleIndex + 3] = i + 1;
                triangles[triangleIndex + 4] = i + width;
                triangles[triangleIndex + 5] = i + width + 1;
                
                triangleIndex += 6;
            }
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        
        // 기본 머테리얼 설정
        Material material = new Material(Shader.Find("Standard"));
        meshRenderer.material = material;
        
        SaveMeshAsset(mesh, "GeneratedTerrainMesh");
    }
    
    void GenerateBlendTexture()
    {
        if (terrainTextures.Length == 0) return;
        
        Texture2D blendTexture = new Texture2D(blendTextureSize, blendTextureSize, TextureFormat.RGBA32, false);
        
        for (int x = 0; x < blendTextureSize; x++)
        {
            for (int y = 0; y < blendTextureSize; y++)
            {
                float worldX = (float)x / blendTextureSize * this.width;
                float worldY = (float)y / blendTextureSize * this.height;
                
                float height = GetInterpolatedHeight(worldX, worldY);
                float slope = GetSlope(worldX, worldY);
                
                Color blendColor = CalculateBlendColor(height, slope);
                blendTexture.SetPixel(x, y, blendColor);
            }
        }
        
        blendTexture.Apply();
        SaveTextureAsset(blendTexture, "TerrainBlendTexture");
        
        // 머테리얼에 블렌드 텍스처 적용
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = blendTexture;
        }
    }
    
    float GetInterpolatedHeight(float x, float y)
    {
        int x1 = Mathf.FloorToInt(x);
        int y1 = Mathf.FloorToInt(y);
        int x2 = Mathf.Min(x1 + 1, width - 1);
        int y2 = Mathf.Min(y1 + 1, height - 1);
        
        float fx = x - x1;
        float fy = y - y1;
        
        float h1 = heightMap[x1, y1];
        float h2 = heightMap[x2, y1];
        float h3 = heightMap[x1, y2];
        float h4 = heightMap[x2, y2];
        
        float i1 = Mathf.Lerp(h1, h2, fx);
        float i2 = Mathf.Lerp(h3, h4, fx);
        
        return Mathf.Lerp(i1, i2, fy);
    }
    
    float GetSlope(float x, float y)
    {
        int ix = Mathf.Clamp(Mathf.FloorToInt(x), 0, width - 1);
        int iy = Mathf.Clamp(Mathf.FloorToInt(y), 0, height - 1);
        
        Vector3 normal = normalMap[ix, iy];
        return Vector3.Angle(normal, Vector3.up);
    }
    
    Color CalculateBlendColor(float height, float slope)
    {
        Color result = Color.black;
        float totalWeight = 0f;
        
        foreach (TerrainTexture tex in terrainTextures)
        {
            if (tex.diffuse == null) continue;
            
            float heightWeight = (height >= tex.minHeight && height <= tex.maxHeight) ? 1f : 0f;
            float slopeWeight = (slope >= tex.minSlope && slope <= tex.maxSlope) ? 1f : 0f;
            
            float weight = heightWeight * slopeWeight;
            
            if (weight > 0f)
            {
                result += tex.tint * weight;
                totalWeight += weight;
            }
        }
        
        if (totalWeight > 0f)
        {
            result /= totalWeight;
        }
        else
        {
            result = Color.gray;
        }
        
        result.a = 1f;
        return result;
    }
    
    void PlaceObjects()
    {
        // 먼저 기타 오브젝트들 배치
        PlaceGeneralObjects();
        
        // 그 다음 바이옴 오브젝트들 배치
        PlaceBiomeObjects();
    }
    
    void PlaceGeneralObjects()
    {
        foreach (GeneralObject obj in generalObjects)
        {
            if (obj.prefab == null) continue;
            
            int placed = 0;
            int attempts = 0;
            int maxAttempts = obj.count * 10;
            
            while (placed < obj.count && attempts < maxAttempts)
            {
                attempts++;
                
                Vector3 position = GetRandomValidPosition(obj.minHeight, obj.maxHeight, obj.minSlope, obj.maxSlope);
                
                if (position == Vector3.zero) continue;
                
                Bounds newBounds = GetPrefabBounds(obj.prefab, position, obj.scaleRange);
                
                if (!IsOverlapping(newBounds))
                {
                    GameObject instance = Instantiate(obj.prefab, position, GetRandomRotation(), transform);
                    
                    float scale = Random.Range(obj.scaleRange.x, obj.scaleRange.y);
                    instance.transform.localScale = Vector3.one * scale;
                    
                    placedObjectBounds.Add(newBounds);
                    placed++;
                }
            }
        }
    }
    
    void PlaceBiomeObjects()
    {
        foreach (BiomeObject biome in biomeObjects)
        {
            if (biome.prefab == null) continue;
            
            int targetCount = Mathf.RoundToInt(width * height * biome.density / 10000f);
            int placed = 0;
            int attempts = 0;
            int maxAttempts = targetCount * 10;
            
            while (placed < targetCount && attempts < maxAttempts)
            {
                attempts++;
                
                Vector3 position = GetRandomValidPosition(biome.minHeight, biome.maxHeight, biome.minSlope, biome.maxSlope);
                
                if (position == Vector3.zero) continue;
                
                Bounds newBounds = GetPrefabBounds(biome.prefab, position, biome.scaleRange);
                
                if (!IsOverlapping(newBounds))
                {
                    GameObject instance = Instantiate(biome.prefab, position, GetRandomRotation(), transform);
                    
                    float scale = Random.Range(biome.scaleRange.x, biome.scaleRange.y);
                    instance.transform.localScale = Vector3.one * scale;
                    
                    placedObjectBounds.Add(newBounds);
                    placed++;
                }
            }
        }
    }
    
    Vector3 GetRandomValidPosition(float minHeight, float maxHeight, float minSlope, float maxSlope)
    {
        int attempts = 0;
        while (attempts < 100)
        {
            attempts++;
            
            int x = Random.Range(0, this.width);
            int y = Random.Range(0, this.height);
            
            float height = heightMap[x, y];
            float slope = Vector3.Angle(normalMap[x, y], Vector3.up);
            
            if (height >= minHeight && height <= maxHeight && 
                slope >= minSlope && slope <= maxSlope)
            {
                return new Vector3(x, height * scale, y);
            }
        }
        
        return Vector3.zero;
    }
    
    Bounds GetPrefabBounds(GameObject prefab, Vector3 position, Vector2 scaleRange)
    {
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            float avgScale = (scaleRange.x + scaleRange.y) * 0.5f;
            bounds.size *= avgScale;
            bounds.center = position;
            return bounds;
        }
        
        // 기본 바운드
        return new Bounds(position, Vector3.one * 2f);
    }
    
    bool IsOverlapping(Bounds newBounds)
    {
        foreach (Bounds existing in placedObjectBounds)
        {
            if (newBounds.Intersects(existing))
                return true;
        }
        return false;
    }
    
    Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(0, Random.Range(0, 360), 0);
    }
    
    void SaveMeshAsset(Mesh mesh, string fileName)
    {
#if UNITY_EDITOR
        if (!System.IO.Directory.Exists(saveAssetPath))
        {
            System.IO.Directory.CreateDirectory(saveAssetPath);
        }
        
        AssetDatabase.CreateAsset(mesh, saveAssetPath + fileName + ".asset");
        AssetDatabase.SaveAssets();
#endif
    }
    
    void SaveTextureAsset(Texture2D texture, string fileName)
    {
#if UNITY_EDITOR
        if (!System.IO.Directory.Exists(saveAssetPath))
        {
            System.IO.Directory.CreateDirectory(saveAssetPath);
        }
        
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(saveAssetPath + fileName + ".png", bytes);
        AssetDatabase.Refresh();
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TerrainMeshGenerator))]
public class TerrainMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        TerrainMeshGenerator generator = (TerrainMeshGenerator)target;
        
        if (GUILayout.Button("지형 생성", GUILayout.Height(30)))
        {
            generator.GenerateTerrain();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "사용법:\n" +
            "1. 지형 설정에서 크기와 노이즈 값 조정\n" +
            "2. 언덕과 구덩이 개수 설정\n" +
            "3. 텍스처 배열에 텍스처 할당\n" +
            "4. 바이옴과 기타 오브젝트 프리팹 할당\n" +
            "5. '지형 생성' 버튼 클릭",
            MessageType.Info
        );
    }
}
#endif