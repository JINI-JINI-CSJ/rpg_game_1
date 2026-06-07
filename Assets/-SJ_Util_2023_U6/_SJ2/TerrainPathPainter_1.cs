using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[ExecuteInEditMode]
public class TerrainPathPainter_1 : MonoBehaviour
{
    [Header("Target Terrain")]
    [Tooltip("길을 그릴 터레인 객체를 여기에 연결하세요.")]
    public Terrain targetTerrain;

    [Header("Terrain Paint Settings")]
    [Tooltip("길을 그릴 때 사용할 터레인 레이어의 인덱스입니다.")]
    public int terrainLayerIndex = 1;
    [Tooltip("길 가장자리의 부드러운 정도 (클수록 경계가 흐려짐)")]
    [Range(0.01f, 1f)]
    public float blendStrength = 0.5f;

    [Header("Path Generation")]
    [Tooltip("생성할 주요 길의 개수")]
    public int numberOfMainPaths = 3;
    [Tooltip("하나의 길을 구성하는 경유지의 수 (많을수록 복잡)")]
    [Range(2, 20)] public int pointsPerPath = 5;
    [Tooltip("길의 굵기 (터레인 유닛 단위)")]
    [Range(1, 50)] public float pathThickness = 5f;

    [Header("Natural Wiggle (Perlin Noise)")]
    [Tooltip("구불거림의 빈도 (작을수록 완만한 커브)")]
    public float noiseScale = 0.05f;
    [Tooltip("구불거림의 강도 (클수록 심하게 구불거림)")]
    public float noiseStrength = 15f;
    
    [Header("Advanced Drawing")]
    [Tooltip("경로의 점들 사이를 선으로 연결하여 끊김 현상을 방지합니다.")]
    public bool connectTheDots = true;

    [Header("Image Output (Optional)")]
    [Tooltip("흑백 이미지 파일로도 저장할지 여부")]
    public bool saveAsImageFile = true;
    [Tooltip("저장할 파일 이름 (확장자 제외)")]
    public string outputFileName = "GeneratedPath";
    [Tooltip("생성할 이미지의 해상도")]
    public int imageResolution = 512;
    

    [ContextMenu("1. Generate and Paint Path on Terrain")]
    public void GenerateAndPaintPath()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("오류: Target Terrain이 설정되지 않았습니다!");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;
        
        if(terrainLayerIndex < 0 || terrainLayerIndex >= terrainData.alphamapLayers)
        {
            Debug.LogError($"오류: 유효하지 않은 Terrain Layer Index ({terrainLayerIndex}) 입니다. 0과 {terrainData.alphamapLayers - 1} 사이의 값을 사용해주세요.");
            return;
        }

        List<List<Vector2>> allPaths = GeneratePathData();
        PaintPathsOnTerrain(allPaths, terrainData);

        if (saveAsImageFile)
        {
            Texture2D texture = GenerateImageFromPaths(allPaths);
            SaveTextureToFile(texture, outputFileName);
            if (Application.isEditor) DestroyImmediate(texture);
            else Destroy(texture);
        }
        
        Debug.Log("<color=cyan>터레인에 길 그리기를 완료했습니다.</color>");
    }

    [ContextMenu("2. Clear All Painted Paths (Reset to Base)")]
    public void ClearPaths()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("오류: Target Terrain이 설정되지 않았습니다!");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;
        int alphaMapWidth = terrainData.alphamapWidth;
        int alphaMapHeight = terrainData.alphamapHeight;
        
        float[,,] splatmapData = new float[alphaMapWidth, alphaMapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < alphaMapHeight; y++)
        {
            for (int x = 0; x < alphaMapWidth; x++)
            {
                splatmapData[x, y, 0] = 1;
                for (int i = 1; i < terrainData.alphamapLayers; i++)
                {
                    splatmapData[x, y, i] = 0;
                }
            }
        }
        
        terrainData.SetAlphamaps(0, 0, splatmapData);
        Debug.Log("<color=orange>터레인의 모든 경로를 기본 텍스처로 초기화했습니다.</color>");
    }


    private List<List<Vector2>> GeneratePathData()
    {
        List<List<Vector2>> allPaths = new List<List<Vector2>>();
        Vector2 terrainSize = new Vector2(targetTerrain.terrainData.size.x, targetTerrain.terrainData.size.z);

        for (int p = 0; p < numberOfMainPaths; p++)
        {
            List<Vector2> currentPathPoints = new List<Vector2>();
            List<Vector2> waypoints = new List<Vector2>();
            for (int i = 0; i < pointsPerPath; i++)
            {
                waypoints.Add(new Vector2(Random.Range(0, terrainSize.x), Random.Range(0, terrainSize.y)));
            }

            waypoints = waypoints.OrderBy(point => point.x).ToList();

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Vector2 p0 = (i > 0) ? waypoints[i - 1] : waypoints[i];
                Vector2 p1 = waypoints[i];
                Vector2 p2 = waypoints[i + 1];
                Vector2 p3 = (i < waypoints.Count - 2) ? waypoints[i + 2] : waypoints[i + 1];
                
                float segmentLength = Vector2.Distance(p1, p2);
                int steps = Mathf.Max(2, (int)(segmentLength / (pathThickness * 0.5f)));

                for (int j = 0; j < steps; j++)
                {
                    float t = (steps == 1) ? 0f : (float)j / (steps - 1);
                    Vector2 splinePoint = GetPointOnCatmullRomSpline(p0, p1, p2, p3, t);
                    float noise = (Mathf.PerlinNoise(splinePoint.x * noiseScale, splinePoint.y * noiseScale) - 0.5f) * 2f;
                    Vector2 direction = (p2 - p1).normalized;
                    Vector2 perpendicular = new Vector2(direction.y, -direction.x);
                    Vector2 finalPoint = splinePoint + perpendicular * noise * noiseStrength;
                    currentPathPoints.Add(finalPoint);
                }
            }
            allPaths.Add(currentPathPoints);
        }
        return allPaths;
    }
    
    private void PaintPathsOnTerrain(List<List<Vector2>> allPaths, TerrainData terrainData)
    {
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        foreach (var path in allPaths)
        {
            Vector2? previousPoint = null; 

            foreach (var point in path)
            {
                if (connectTheDots && previousPoint.HasValue)
                {
                    DrawLineOnSplatmap(previousPoint.Value, point, splatmapData, terrainData);
                }
                else
                {
                    DrawCircleOnSplatmap(point, splatmapData, terrainData);
                }
                previousPoint = point;
            }
        }
        
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
    
    private void DrawCircleOnSplatmap(Vector2 worldPos, float[,,] splatmapData, TerrainData terrainData)
    {
        int alphaX = (int)((worldPos.x / terrainData.size.x) * terrainData.alphamapWidth);
        int alphaY = (int)((worldPos.y / terrainData.size.z) * terrainData.alphamapHeight);
        int radius = (int)((pathThickness / terrainData.size.x) * terrainData.alphamapWidth);

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    PaintSplatmapPixel(alphaX + x, alphaY + y, distance, radius, splatmapData, terrainData);
                }
            }
        }
    }

    private void DrawLineOnSplatmap(Vector2 worldPosStart, Vector2 worldPosEnd, float[,,] splatmapData, TerrainData terrainData)
    {
        int x0 = (int)((worldPosStart.x / terrainData.size.x) * terrainData.alphamapWidth);
        int y0 = (int)((worldPosStart.y / terrainData.size.z) * terrainData.alphamapHeight);
        int x1 = (int)((worldPosEnd.x / terrainData.size.x) * terrainData.alphamapWidth);
        int y1 = (int)((worldPosEnd.y / terrainData.size.z) * terrainData.alphamapHeight);

        int dx = Mathf.Abs(x1 - x0);
        int dy = -Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            DrawCircleOnSplatmap(new Vector2(
                (float)x0 / terrainData.alphamapWidth * terrainData.size.x, 
                (float)y0 / terrainData.alphamapHeight * terrainData.size.z),
                splatmapData, terrainData
            );

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }
    
    private void PaintSplatmapPixel(int x, int y, float distance, int radius, float[,,] splatmapData, TerrainData terrainData)
    {
        if (x < 0 || x >= terrainData.alphamapWidth || y < 0 || y >= terrainData.alphamapHeight) return;

        float falloff = radius > 0 ? Mathf.Clamp01(1 - (distance / radius)) : 1f;
        float blendValue = Mathf.Pow(falloff, 1f / blendStrength);

        float currentWeight = splatmapData[y, x, terrainLayerIndex];
        float newWeight = Mathf.Max(currentWeight, blendValue);

        splatmapData[y, x, terrainLayerIndex] = newWeight;

        float totalWeight = 0;
        for (int i = 0; i < terrainData.alphamapLayers; i++)
        {
            totalWeight += splatmapData[y, x, i];
        }

        if (totalWeight > 0)
        {
            for (int i = 0; i < terrainData.alphamapLayers; i++)
            {
                splatmapData[y, x, i] /= totalWeight;
            }
        }
    }

    private Texture2D GenerateImageFromPaths(List<List<Vector2>> allPaths)
    {
        Texture2D texture = new Texture2D(imageResolution, imageResolution);
        Color[] pixels = new Color[imageResolution * imageResolution];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.black;

        Vector2 terrainSize = new Vector2(targetTerrain.terrainData.size.x, targetTerrain.terrainData.size.z);

        foreach (var path in allPaths)
        {
            Vector2? previousPoint = null;
            foreach (var point in path)
            {
                if (connectTheDots && previousPoint.HasValue)
                {
                    DrawLineOnImage(previousPoint.Value, point, pixels, terrainSize);
                }
                else
                {
                    DrawCircleOnImage(point, pixels, terrainSize);
                }
                previousPoint = point;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void DrawCircleOnImage(Vector2 worldPos, Color[] pixels, Vector2 terrainSize)
    {
        int imgX = (int)((worldPos.x / terrainSize.x) * imageResolution);
        int imgY = (int)((worldPos.y / terrainSize.y) * imageResolution);
        int radius = (int)((pathThickness / terrainSize.x) * imageResolution);
        
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int drawX = imgX + x;
                    int drawY = imgY + y;
                    if (drawX >= 0 && drawX < imageResolution && drawY >= 0 && drawY < imageResolution)
                    {
                        pixels[drawY * imageResolution + drawX] = Color.white;
                    }
                }
            }
        }
    }
    
    private void DrawLineOnImage(Vector2 worldPosStart, Vector2 worldPosEnd, Color[] pixels, Vector2 terrainSize)
    {
        int x0 = (int)((worldPosStart.x / terrainSize.x) * imageResolution);
        int y0 = (int)((worldPosStart.y / terrainSize.y) * imageResolution);
        int x1 = (int)((worldPosEnd.x / terrainSize.x) * imageResolution);
        int y1 = (int)((worldPosEnd.y / terrainSize.y) * imageResolution);

        int dx = Mathf.Abs(x1 - x0);
        int dy = -Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            DrawCircleOnImage(new Vector2(
                (float)x0 / imageResolution * terrainSize.x, 
                (float)y0 / imageResolution * terrainSize.y),
                pixels, terrainSize
            );

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }

    private Vector2 GetPointOnCatmullRomSpline(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }
    
    private void SaveTextureToFile(Texture2D texture, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, fileName + ".png");
        File.WriteAllBytes(path, bytes);
        Debug.Log($"<color=green>성공적으로 이미지를 저장했습니다: {path}</color>");
        
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}