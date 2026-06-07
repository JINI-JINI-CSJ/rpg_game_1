using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class TexturePaintLayer
{
    public enum NoiseType { Perlin, Simplex, Cellular }

    [Header("레이어 기본 설정")]
    public NoiseType noiseType = NoiseType.Perlin;
    [Tooltip("터레인의 'Paint Texture'에 등록된 텍스처의 인덱스입니다. (0부터 시작)")]
    public int textureIndex = 0;

    [Header("노이즈 파라미터")]
    [Tooltip("노이즈의 크기를 조절합니다. 값이 작을수록 패턴이 커집니다.")]
    public float noiseScale = 25.0f;
    [Tooltip("랜덤한 결과를 얻기 위한 노이즈 시드(오프셋) 값입니다.")]
    public Vector2 noiseSeed = Vector2.zero;
    [Tooltip("이 값보다 노이즈 값이 높아야 텍스처가 칠해집니다. (0~1 사이)")]
    [Range(0f, 1f)]
    public float paintThreshold = 0.5f;
    [Tooltip("더 세부적인 디테일을 추가하기 위한 옥타브 수입니다.")]
    [Range(1, 8)]
    public int octaves = 4;
    [Tooltip("각 옥타브마다 노이즈 스케일을 얼마나 곱할지 결정합니다.")]
    public float lacunarity = 2.0f;
    [Tooltip("각 옥타브마다 노이즈 강도를 얼마나 줄일지 결정합니다.")]
    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Header("브러시 마스크 설정 (옵션)")]
    [Tooltip("활성화하면 위의 노이즈 패턴에 랜덤 브러시 모양을 마스크처럼 겹칩니다.")]
    public bool useBrushMask = false;
    [Tooltip("브러시 텍스처의 밝기가 이 값보다 높아야 텍스처가 칠해집니다. (0~1)")]
    [Range(0f, 1f)]
    public float brushThreshold = 0.5f;
    [Tooltip("브러시 마스크를 터레인에 몇 번 타일링할지 결정합니다.")]
    public float brushScale = 1.0f;

    // --- 추가된 기능 1: 랜덤 브러시 알파 ---
    [Tooltip("브러시로 칠할 때 적용할 최소 알파(강도) 값입니다.")]
    [Range(0f, 1f)]
    public float minBrushAlpha = 0.5f;
    [Tooltip("브러시로 칠할 때 적용할 최대 알파(강도) 값입니다.")]
    [Range(0f, 1f)]
    public float maxBrushAlpha = 1.0f;
}

[AddComponentMenu("Terrain/Terrain Texture Painter")]
public class TerrainTexturePainter : MonoBehaviour
{
    [Header("전역 설정")]
    [Tooltip("텍스처를 칠할 대상 터레인")]
    public Terrain targetTerrain;
    
    // --- 추가된 기능 2: 자동 시드 변경 토글 ---
    [Tooltip("활성화하면 생성 버튼을 누를 때마다 모든 레이어의 노이즈 시드가 자동으로 변경됩니다.")]
    public bool autoRandomizeSeedOnGenerate = true;
    
    [Header("브러시 설정")]
    [Tooltip("사용자가 직접 브러시 텍스처를 등록합니다. 이 리스트가 비어있으면 내장 브러시를 사용합니다.")]
    public List<Texture2D> customBrushes;

    [Header("A. 페인트 레이어 목록")]
    public List<TexturePaintLayer> paintLayers;
    
    [Header("B. 편의 기능")]
    [Tooltip("랜덤 전체 채우기 기능에서 사용할 '터레인 레이어'의 인덱스 목록입니다.")]
    public List<int> fillWithRandomLayerIndices;
    
    [ContextMenu("1. [A] 노이즈만으로 생성")]
    public void GenerateWithNoiseOnly()
    {
        Generate(false);
    }

    [ContextMenu("2. [A+C] 노이즈 + 랜덤 브러시로 생성")]
    public void GenerateWithNoiseAndBrush()
    {
        Generate(true);
    }
    
    private void Generate(bool useBrushMasking)
    {
        if (!IsReady()) return;

        // 자동 시드 변경 기능
        if (autoRandomizeSeedOnGenerate)
        {
            foreach (var layer in paintLayers)
            {
                layer.noiseSeed = new Vector2(Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            }
            Debug.Log("모든 레이어의 노이즈 시드를 자동으로 변경했습니다.");
        }
        
        Texture2D randomBrush = null;
        if (useBrushMasking)
        {
            List<Texture2D> activeBrushes = (customBrushes != null && customBrushes.Count > 0) ? customBrushes : FindBuiltinBrushes();
            if (activeBrushes == null || activeBrushes.Count == 0)
            {
                Debug.LogError("사용할 브러시가 없습니다. 'Custom Brushes' 리스트에 텍스처를 추가하거나, 'Terrain Tools' 패키지를 설치해주세요.");
                return;
            }
            randomBrush = activeBrushes[Random.Range(0, activeBrushes.Count)];
            Debug.Log($"선택된 브러시: {randomBrush.name}");
#if UNITY_EDITOR
            SetTextureIsReadable(randomBrush, true);
#endif
        }

        TerrainData terrainData = targetTerrain.terrainData;
        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;
        int textureLayerCount = terrainData.alphamapLayers;
        float[,,] splatmapData = new float[alphamapHeight, alphamapWidth, textureLayerCount];

        for (int y = 0; y < alphamapHeight; y++)
        {
            for (int x = 0; x < alphamapWidth; x++)
            {
                float[] splatStrengths = new float[textureLayerCount];
                float totalStrength = 0f;

                foreach (var layer in paintLayers)
                {
                    if (layer.textureIndex < 0 || layer.textureIndex >= textureLayerCount) continue;

                    float noiseValue = GetNoiseValue(x, y, alphamapWidth, alphamapHeight, layer);
                    bool noiseCondition = (noiseValue >= layer.paintThreshold);
                    bool finalCondition = noiseCondition;

                    if (useBrushMasking && layer.useBrushMask && randomBrush != null)
                    {
                        float u = ((float)x / alphamapWidth) * layer.brushScale;
                        float v = ((float)y / alphamapHeight) * layer.brushScale;
                        float brushValue = randomBrush.GetPixelBilinear(u, v).grayscale;
                        bool brushCondition = (brushValue >= layer.brushThreshold);
                        finalCondition = noiseCondition && brushCondition;
                    }

                    if (finalCondition)
                    {
                        float finalStrength = noiseValue;
                        // 브러시 마스킹 모드일 때만 랜덤 알파 적용
                        if (useBrushMasking && layer.useBrushMask)
                        {
                            float randomAlpha = Random.Range(layer.minBrushAlpha, layer.maxBrushAlpha);
                            finalStrength *= randomAlpha; // 노이즈 강도에 랜덤 알파를 곱하여 자연스러움 유지
                        }
                        
                        splatStrengths[layer.textureIndex] = finalStrength;
                        totalStrength += finalStrength;
                    }
                }

                if (totalStrength > 0)
                {
                    for (int i = 0; i < textureLayerCount; i++)
                    {
                        splatmapData[y, x, i] = splatStrengths[i] / totalStrength;
                    }
                }
                else
                {
                    splatmapData[y, x, 0] = 1;
                }
            }
        }
        
        terrainData.SetAlphamaps(0, 0, splatmapData);
        Debug.Log("터레인 텍스처 생성이 완료되었습니다!");

#if UNITY_EDITOR
        if (randomBrush != null) SetTextureIsReadable(randomBrush, false);
#endif
    }
    
    [ContextMenu("3. [B] 랜덤 레이어로 전체 채우기")]
    private void FillWithRandomLayer()
    {
        if (!IsReady()) return;
        if (fillWithRandomLayerIndices == null || fillWithRandomLayerIndices.Count == 0)
        {
            Debug.LogError("'랜덤 채우기 레이어 인덱스' 목록이 비어있습니다.");
            return;
        }

        int chosenIndex = fillWithRandomLayerIndices[Random.Range(0, fillWithRandomLayerIndices.Count)];
        TerrainData terrainData = targetTerrain.terrainData;
        int textureLayerCount = terrainData.alphamapLayers;

        if (chosenIndex < 0 || chosenIndex >= textureLayerCount)
        {
            Debug.LogError($"선택된 터레인 레이어 인덱스({chosenIndex})가 유효하지 않습니다.");
            return;
        }

        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;
        float[,,] splatmapData = new float[alphamapHeight, alphamapWidth, textureLayerCount];

        for (int y = 0; y < alphamapHeight; y++)
        {
            for (int x = 0; x < alphamapWidth; x++)
            {
                for (int i = 0; i < textureLayerCount; i++)
                {
                    splatmapData[y, x, i] = (i == chosenIndex) ? 1.0f : 0.0f;
                }
            }
        }
        
        terrainData.SetAlphamaps(0, 0, splatmapData);
        Debug.Log($"터레인 전체를 '터레인 레이어' 인덱스 {chosenIndex} (으)로 채웠습니다.");
    }
    
    [ContextMenu("4. 추천 설정으로 페인트 레이어 추가")]
    private void AddRecommendedPaintLayer()
    {
        var newLayer = new TexturePaintLayer();

        int nextIndex = 1;
        if (paintLayers.Count > 0)
        {
            int maxIndex = 0;
            foreach (var layer in paintLayers)
            {
                if (layer.textureIndex > maxIndex) maxIndex = layer.textureIndex;
            }
            nextIndex = maxIndex + 1;
        }
        newLayer.textureIndex = nextIndex;

        newLayer.noiseScale = Random.Range(20f, 70f);
        newLayer.noiseSeed = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));
        newLayer.paintThreshold = Random.Range(0.45f, 0.65f);
        newLayer.octaves = Random.Range(3, 6);
        newLayer.lacunarity = 2.0f;
        newLayer.persistence = 0.5f;

        paintLayers.Add(newLayer);
        Debug.Log($"추천 설정 값을 가진 새 페인트 레이어(텍스처 인덱스: {newLayer.textureIndex})가 추가되었습니다.");
    }
    
    private bool IsReady()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("Target Terrain이 설정되지 않았습니다!");
            return false;
        }
        return true;
    }

    private float GetNoiseValue(int x, int y, int width, int height, TexturePaintLayer layer)
    {
        float xCoord = (float)x / width * layer.noiseScale + layer.noiseSeed.x;
        float yCoord = (float)y / height * layer.noiseScale + layer.noiseSeed.y;
        return GenerateFBM(xCoord, yCoord, layer.octaves, layer.lacunarity, layer.persistence);
    }

    private float GenerateFBM(float x, float y, int octaves, float lacunarity, float persistence)
    {
        float total = 0, frequency = 1, amplitude = 1, maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return maxValue > 0 ? total / maxValue : 0;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (paintLayers == null) return;
        foreach (var layer in paintLayers)
        {
            if (layer.maxBrushAlpha < layer.minBrushAlpha)
            {
                layer.maxBrushAlpha = layer.minBrushAlpha;
            }
        }
    }

    private List<Texture2D> FindBuiltinBrushes()
    {
        var brushes = new List<Texture2D>();
        string[] searchPaths = { "Packages/com.unity.terrain-tools/Editor/Brushes" };
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", searchPaths);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D brush = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (brush != null) brushes.Add(brush);
        }
        return brushes;
    }
    
    private void SetTextureIsReadable(Texture2D texture, bool isReadable)
    {
        if (texture == null) return;
        string path = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(path)) return;

        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter != null && textureImporter.isReadable != isReadable)
        {
            textureImporter.isReadable = isReadable;
            AssetDatabase.ImportAsset(path);
        }
    }
#endif
}