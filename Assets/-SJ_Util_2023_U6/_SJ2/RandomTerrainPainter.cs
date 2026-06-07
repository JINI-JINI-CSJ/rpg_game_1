using UnityEngine;
using UnityEditor;

public class RandomTerrainPainter : MonoBehaviour
{
    [System.Serializable]
    public struct TexturePaintSettings
    {
        [Header("텍스처 인덱스 설정")]
        public int textureIndex;
        
        [Header("브러시 텍스처 설정")]
        public Texture2D[] brushTextures;
        
        [Header("페인팅 설정")]
        //[Range(1, 100)]
        public int paintCount;
        
        [Header("랜덤 범위 설정")]
        [Range(0.01f, 1f)]
        public float minStrength;
        [Range(0.01f, 1f)]
        public float maxStrength;
        
        [Range(1f, 50f)]
        public float minBrushSize;
        [Range(1f, 50f)]
        public float maxBrushSize;
        
        [Header("가중치 설정")]
        [Range(0f, 1f)]
        public float probability; // 이 텍스처가 선택될 확률
        
        public bool isEnabled; // 이 텍스처 설정 활성화 여부
    }
    
    [Header("터레인 설정")]
    public Terrain terrain;
    
    [Header("텍스처별 개별 설정")]
    public TexturePaintSettings[] textureSettings;
    
    [Header("페인팅 영역")]
    public Vector2 paintAreaMin = Vector2.zero;
    public Vector2 paintAreaMax = Vector2.one;
    
    [Header("자동 페인팅")]
    public bool autoStart = false;
    public float autoInterval = 1f;
    private float lastPaintTime;
    
    void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();
            
        if (terrain == null)
        {
            Debug.LogError("터레인을 찾을 수 없습니다!");
            return;
        }
        
        // 텍스처 설정 초기화
        InitializeTextureSettings();
        
        if (autoStart)
        {
            InvokeRepeating("PaintRandomly", 0f, autoInterval);
        }
    }
    
    void Update()
    {
        // // 스페이스바를 누르면 랜덤 페인팅 실행
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     PaintRandomly();
        // }
        
        // // R키를 누르면 여러 번 페인팅
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     PaintMultipleRandom();
        // }
    }
    
    public void PaintRandomly()
    {
        if (terrain == null || textureSettings == null || textureSettings.Length == 0)
            return;
        
        // 활성화된 텍스처 설정 중에서 확률에 따라 선택
        TexturePaintSettings selectedSetting = SelectRandomTextureSetting();
        
        if (selectedSetting.textureIndex < 0)
        {
            Debug.LogWarning("유효한 텍스처 설정이 없습니다.");
            return;
        }
        
        // 선택된 설정에서 랜덤 브러시 선택
        Texture2D selectedBrush = GetRandomBrush(selectedSetting.brushTextures);
        
        // 랜덤 위치 생성
        Vector3 randomPosition = GetRandomPosition();
        
        // 선택된 설정의 범위에서 랜덤 강도 생성
        float randomStrength = Random.Range(selectedSetting.minStrength, selectedSetting.maxStrength);
        
        // 선택된 설정의 범위에서 랜덤 브러시 크기 생성
        float randomBrushSize = Random.Range(selectedSetting.minBrushSize, selectedSetting.maxBrushSize);
        
        // 터레인 페인팅 실행
        PaintTerrain(randomPosition, selectedSetting.textureIndex, selectedBrush, randomStrength, randomBrushSize);
        
        Debug.Log($"페인팅 완료 - 위치: {randomPosition}, 텍스처: {selectedSetting.textureIndex}, 강도: {randomStrength:F2}, 크기: {randomBrushSize:F1}");
    }
    
    public void PaintMultipleRandom()
    {
        if (textureSettings == null || textureSettings.Length == 0)
            return;
        
        // 각 텍스처 설정별로 paintCount만큼 페인팅
        foreach (var setting in textureSettings)
        {
            if (!setting.isEnabled) continue;
            
            for (int i = 0; i < setting.paintCount; i++)
            {
                PaintSpecificTexture(setting);
            }
        }
    }
    
    public void PaintSpecificTexture(TexturePaintSettings setting)
    {
        if (terrain == null || setting.textureIndex < 0)
            return;
        
        // 해당 설정의 랜덤 브러시 선택
        Texture2D selectedBrush = GetRandomBrush(setting.brushTextures);
        
        // 랜덤 위치 생성
        Vector3 randomPosition = GetRandomPosition();
        
        // 해당 설정의 범위에서 랜덤 강도 생성
        float randomStrength = Random.Range(setting.minStrength, setting.maxStrength);
        
        // 해당 설정의 범위에서 랜덤 브러시 크기 생성
        float randomBrushSize = Random.Range(setting.minBrushSize, setting.maxBrushSize);
        
        // 터레인 페인팅 실행
        PaintTerrain(randomPosition, setting.textureIndex, selectedBrush, randomStrength, randomBrushSize);
    }
    
    private void PaintTerrain(Vector3 worldPosition, int textureIndex, Texture2D brush, float strength, float brushSize)
    {
        TerrainData terrainData = terrain.terrainData;
        
        // 월드 좌표를 터레인 좌표로 변환
        Vector3 terrainPosition = worldPosition - terrain.transform.position;
        
        // 정규화된 좌표 계산
        float normalizedX = terrainPosition.x / terrainData.size.x;
        float normalizedZ = terrainPosition.z / terrainData.size.z;
        
        // 알파맵 좌표 계산
        int alphaMapWidth = terrainData.alphamapWidth;
        int alphaMapHeight = terrainData.alphamapHeight;
        
        int centerX = Mathf.RoundToInt(normalizedX * alphaMapWidth);
        int centerZ = Mathf.RoundToInt(normalizedZ * alphaMapHeight);
        
        // 브러시 크기를 알파맵 크기로 변환
        int brushPixelSize = Mathf.RoundToInt(brushSize * alphaMapWidth / terrainData.size.x);
        
        // 브러시 영역 계산
        int startX = Mathf.Max(0, centerX - brushPixelSize / 2);
        int startZ = Mathf.Max(0, centerZ - brushPixelSize / 2);
        int endX = Mathf.Min(alphaMapWidth, centerX + brushPixelSize / 2);
        int endZ = Mathf.Min(alphaMapHeight, centerZ + brushPixelSize / 2);
        
        int width = endX - startX;
        int height = endZ - startZ;
        
        if (width <= 0 || height <= 0) return;
        
        // 현재 알파맵 가져오기
        float[,,] alphaMap = terrainData.GetAlphamaps(startX, startZ, width, height);
        int textureCount = alphaMap.GetLength(2);
        
        // 유효한 텍스처 인덱스 확인
        if (textureIndex >= textureCount)
        {
            Debug.LogWarning($"텍스처 인덱스 {textureIndex}가 범위를 벗어났습니다. 최대 인덱스: {textureCount - 1}");
            return;
        }
        
        // 브러시 적용
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // 브러시 중심에서의 거리 계산
                float distanceX = (float)(x - width / 2) / (brushPixelSize / 2);
                float distanceZ = (float)(z - height / 2) / (brushPixelSize / 2);
                float distance = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);
                
                // 브러시 강도 계산 (거리에 따른 감소)
                float brushStrength = 1f - Mathf.Clamp01(distance);
                
                // 브러시 텍스처 적용
                if (brush != null)
                {
                    int brushX = Mathf.RoundToInt(((float)x / width) * brush.width);
                    int brushZ = Mathf.RoundToInt(((float)z / height) * brush.height);
                    brushX = Mathf.Clamp(brushX, 0, brush.width - 1);
                    brushZ = Mathf.Clamp(brushZ, 0, brush.height - 1);
                    
                    Color brushColor = brush.GetPixel(brushX, brushZ);
                    brushStrength *= brushColor.grayscale;
                }
                
                // 최종 강도 계산
                float finalStrength = brushStrength * strength;
                
                if (finalStrength > 0)
                {
                    // 현재 가중치 합계 계산
                    float totalWeight = 0f;
                    for (int t = 0; t < textureCount; t++)
                    {
                        totalWeight += alphaMap[z, x, t];
                    }
                    
                    // 타겟 텍스처 강도 증가
                    float targetIncrease = finalStrength;
                    alphaMap[z, x, textureIndex] = Mathf.Clamp01(alphaMap[z, x, textureIndex] + targetIncrease);
                    
                    // 다른 텍스처들의 가중치 조정
                    float newTotal = 0f;
                    for (int t = 0; t < textureCount; t++)
                    {
                        newTotal += alphaMap[z, x, t];
                    }
                    
                    // 정규화
                    if (newTotal > 0)
                    {
                        for (int t = 0; t < textureCount; t++)
                        {
                            alphaMap[z, x, t] /= newTotal;
                        }
                    }
                }
            }
        }
        
        // 수정된 알파맵 적용
        terrainData.SetAlphamaps(startX, startZ, alphaMap);
    }
    
    private TexturePaintSettings SelectRandomTextureSetting()
    {
        // 활성화된 설정들만 필터링
        var enabledSettings = new System.Collections.Generic.List<TexturePaintSettings>();
        var probabilities = new System.Collections.Generic.List<float>();
        
        foreach (var setting in textureSettings)
        {
            if (setting.isEnabled)
            {
                enabledSettings.Add(setting);
                probabilities.Add(setting.probability);
            }
        }
        
        if (enabledSettings.Count == 0)
        {
            return new TexturePaintSettings { textureIndex = -1 };
        }
        
        // 확률 기반 선택
        float totalProbability = 0f;
        foreach (float prob in probabilities)
        {
            totalProbability += prob;
        }
        
        if (totalProbability <= 0f)
        {
            // 확률이 모두 0이면 균등 선택
            return enabledSettings[Random.Range(0, enabledSettings.Count)];
        }
        
        float randomValue = Random.Range(0f, totalProbability);
        float currentSum = 0f;
        
        for (int i = 0; i < enabledSettings.Count; i++)
        {
            currentSum += probabilities[i];
            if (randomValue <= currentSum)
            {
                return enabledSettings[i];
            }
        }
        
        // fallback
        return enabledSettings[enabledSettings.Count - 1];
    }
    
    private Texture2D GetRandomBrush(Texture2D[] brushes)
    {
        if (brushes == null || brushes.Length == 0)
            return null;
            
        return brushes[Random.Range(0, brushes.Length)];
    }
    
    private void InitializeTextureSettings()
    {
        if (textureSettings == null || textureSettings.Length == 0)
        {
            // 기본 텍스처 설정 생성
            textureSettings = new TexturePaintSettings[4];
            
            for (int i = 0; i < textureSettings.Length; i++)
            {
                textureSettings[i] = new TexturePaintSettings
                {
                    textureIndex = i,
                    brushTextures = CreateDefaultBrushTextures(),
                    paintCount = 10,
                    minStrength = 0.1f,
                    maxStrength = 0.8f,
                    minBrushSize = 5f,
                    maxBrushSize = 20f,
                    probability = 0.25f, // 균등 확률
                    isEnabled = true
                };
            }
        }
        else
        {
            // 기존 설정에서 브러시 텍스처가 없는 경우 기본값 설정
            for (int i = 0; i < textureSettings.Length; i++)
            {
                if (textureSettings[i].brushTextures == null || textureSettings[i].brushTextures.Length == 0)
                {
                    textureSettings[i].brushTextures = CreateDefaultBrushTextures();
                }
            }
        }
    }
    
    private Vector3 GetRandomPosition()
    {
        TerrainData terrainData = terrain.terrainData;
        
        float randomX = Random.Range(
            paintAreaMin.x * terrainData.size.x,
            paintAreaMax.x * terrainData.size.x
        );
        
        float randomZ = Random.Range(
            paintAreaMin.y * terrainData.size.z,
            paintAreaMax.y * terrainData.size.z
        );
        
        // 터레인 높이 샘플링
        float normalizedX = randomX / terrainData.size.x;
        float normalizedZ = randomZ / terrainData.size.z;
        float height = terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
        
        return terrain.transform.position + new Vector3(randomX, height, randomZ);
    }
    
    // private void CreateDefaultBrushTextures()
    // {
    //     // 기본 원형 브러시 생성
    //     Texture2D circularBrush = CreateCircularBrush(64);
        
    //     // 기본 소프트 브러시 생성
    //     Texture2D softBrush = CreateSoftBrush(64);
        
    //     // 기본 노이즈 브러시 생성
    //     Texture2D noiseBrush = CreateNoiseBrush(64);
        
    //     // 첫 번째 텍스처 설정이 있으면 기본 브러시 할당
    //     if (textureSettings != null && textureSettings.Length > 0)
    //     {
    //         textureSettings[0].brushTextures = new Texture2D[] { circularBrush, softBrush, noiseBrush };
    //     }
    // }
    
    private Texture2D[] CreateDefaultBrushTextures()
    {
        // 기본 원형 브러시 생성
        Texture2D circularBrush = CreateCircularBrush(64);
        
        // 기본 소프트 브러시 생성
        Texture2D softBrush = CreateSoftBrush(64);
        
        // 기본 노이즈 브러시 생성
        Texture2D noiseBrush = CreateNoiseBrush(64);
        
        return new Texture2D[] { circularBrush, softBrush, noiseBrush };
    }
    
    private Texture2D CreateCircularBrush(int size)
    {
        Texture2D brush = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = 1f - Mathf.Clamp01(distance / radius);
                pixels[y * size + x] = new Color(alpha, alpha, alpha, alpha);
            }
        }
        
        brush.SetPixels(pixels);
        brush.Apply();
        return brush;
    }
    
    private Texture2D CreateSoftBrush(int size)
    {
        Texture2D brush = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float normalizedDistance = distance / radius;
                float alpha = Mathf.Pow(1f - Mathf.Clamp01(normalizedDistance), 2f);
                pixels[y * size + x] = new Color(alpha, alpha, alpha, alpha);
            }
        }
        
        brush.SetPixels(pixels);
        brush.Apply();
        return brush;
    }
    
    private Texture2D CreateNoiseBrush(int size)
    {
        Texture2D brush = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                float alpha = noise * 0.8f + 0.2f;
                pixels[y * size + x] = new Color(alpha, alpha, alpha, alpha);
            }
        }
        
        brush.SetPixels(pixels);
        brush.Apply();
        return brush;
    }
    
    // 에디터에서 사용할 수 있는 메서드들
    [System.Serializable]
    public class PaintSettings
    {
        public Vector3 position;
        public int textureIndex;
        public float strength;
        public float brushSize;
        public Texture2D brush;
    }
    
    public void SaveCurrentSettings()
    {
        // 현재 설정을 저장하는 기능 (필요시 구현)
        Debug.Log("현재 설정이 저장되었습니다.");
    }
    
    public void LoadSettings()
    {
        // 저장된 설정을 불러오는 기능 (필요시 구현)
        Debug.Log("설정이 로드되었습니다.");
    }
}

// 에디터 확장 (선택사항)
#if UNITY_EDITOR
[CustomEditor(typeof(RandomTerrainPainter))]
public class RandomTerrainPainterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RandomTerrainPainter painter = (RandomTerrainPainter)target;
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("랜덤 페인팅 (1회)"))
        {
            painter.PaintRandomly();
        }
        
        if (GUILayout.Button("랜덤 페인팅 (여러 번)"))
        {
            painter.PaintMultipleRandom();
        }
        
        if (GUILayout.Button("특정 텍스처 페인팅"))
        {
            // 활성화된 첫 번째 설정으로 페인팅
            var enabledSetting = System.Array.Find(painter.textureSettings, s => s.isEnabled);
            if (enabledSetting.textureIndex >= 0)
            {
                painter.PaintSpecificTexture(enabledSetting);
            }
        }
        
        GUILayout.Space(10);
        
        // 각 텍스처 설정별 개별 버튼
        if (painter.textureSettings != null)
        {
            for (int i = 0; i < painter.textureSettings.Length; i++)
            {
                var setting = painter.textureSettings[i];
                if (setting.isEnabled)
                {
                    if (GUILayout.Button($"텍스처 {setting.textureIndex} 페인팅"))
                    {
                        painter.PaintSpecificTexture(setting);
                    }
                }
            }
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("설정 저장"))
        {
            painter.SaveCurrentSettings();
        }
        
        if (GUILayout.Button("설정 로드"))
        {
            painter.LoadSettings();
        }
    }
}
#endif