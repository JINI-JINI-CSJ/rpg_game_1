// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [System.Serializable]
// public class TerrainTextureLayer
// {
//     public Texture2D diffuseTexture;
//     public Texture2D normalMap;
//     public Vector2 tileSize = Vector2.one * 15f;
//     public Vector2 tileOffset = Vector2.zero;
//     [Range(0f, 1f)]
//     public float metallic = 0f;
//     [Range(0f, 1f)]
//     public float smoothness = 0f;
//     [Range(0f, 1f)]
//     public float heightBlend = 0.5f;
//     [Range(0f, 1f)]
//     public float minHeight = 0f;
//     [Range(0f, 1f)]
//     public float maxHeight = 1f;
//     [Range(0f, 90f)]
//     public float minSlope = 0f;
//     [Range(0f, 90f)]
//     public float maxSlope = 90f;
// }

// [System.Serializable]
// public class StageGeneratorSettings
// {
//     [Header("Terrain Settings")]
//     public int terrainWidth = 512;
//     public int terrainLength = 512;
//     public int terrainHeight = 50;
//     [Range(0.01f, 0.2f)]
//     public float noiseScale = 0.05f;
//     public AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
//     [Header("Terrain Textures")]
//     public TerrainTextureLayer[] terrainLayers;
//     public bool autoApplyTextures = true;
//     public float textureBlendStrength = 1f;
    
//     [Header("Obstacle Settings")]
//     public GameObject[] obstaclePrefabs;
//     public int minObstacles = 20;
//     public int maxObstacles = 40;
//     public float minObstacleDistance = 5f;
//     public LayerMask obstacleCheckLayer = -1;
    
//     [Header("Vegetation Settings")]
//     public GameObject[] treePrefabs;
//     public GameObject[] grassPrefabs;
//     public int minTrees = 30;
//     public int maxTrees = 60;
//     public int minGrass = 100;
//     public int maxGrass = 200;
//     public float vegetationDensity = 0.8f;
    
//     [Header("Important Objects")]
//     public GameObject startPointPrefab;
//     public GameObject[] exitPointPrefabs;
//     public int exitPointCount = 4;
//     public float safeZoneRadius = 20f;
    
//     [Header("Generation Settings")]
//     public bool generateOnStart = true;
//     public int randomSeed = 0;
//     [Range(0f, 1f)]
//     public float flatAreaRatio = 0.3f;
// }

// public class DBDStageGenerator : MonoBehaviour
// {
//     [SerializeField] private StageGeneratorSettings settings;
    
//     private Terrain currentTerrain;
//     private TerrainData currentTerrainData;
//     private List<Vector3> occupiedPositions = new List<Vector3>();
//     private List<GameObject> spawnedObjects = new List<GameObject>();
    
//     // 중요 오브젝트들의 위치 저장
//     private Vector3 startPoint;
//     private List<Vector3> exitPoints = new List<Vector3>();
    
//     void Start()
//     {
//         if (settings.generateOnStart)
//         {
//             GenerateStage();
//         }
//     }
    
//     [ContextMenu("Generate New Stage")]
//     public void GenerateStage()
//     {
//         ClearExistingStage();
        
//         if (settings.randomSeed == 0)
//             settings.randomSeed = Random.Range(1, 10000);
        
//         Random.InitState(settings.randomSeed);
        
//         StartCoroutine(GenerateStageCoroutine());
//     }
    
//     private void ClearExistingStage()
//     {
//         // 기존 오브젝트들 제거
//         foreach (GameObject obj in spawnedObjects)
//         {
//             if (obj != null)
//                 DestroyImmediate(obj);
//         }
//         spawnedObjects.Clear();
//         occupiedPositions.Clear();
//         exitPoints.Clear();
        
//         // 기존 터레인 제거
//         if (currentTerrain != null)
//             DestroyImmediate(currentTerrain.gameObject);
        
//         currentTerrain = null;
//         currentTerrainData = null;
//     }
    
//     private IEnumerator GenerateStageCoroutine()
//     {
//         Debug.Log("스테이지 생성 시작...");
        
//         // 1. 터레인 생성
//         yield return StartCoroutine(GenerateTerrain());
        
//         // 2. 터레인 텍스처 적용
//         if (settings.autoApplyTextures)
//             yield return StartCoroutine(ApplyTerrainTextures());
        
//         // 3. 시작점 배치
//         yield return StartCoroutine(PlaceStartPoint());
        
//         // 4. 탈출점 배치
//         yield return StartCoroutine(PlaceExitPoints());
        
//         // 5. 장애물 배치
//         yield return StartCoroutine(PlaceObstacles());
        
//         // 6. 나무 배치
//         yield return StartCoroutine(PlaceTrees());
        
//         // 7. 풀 배치
//         yield return StartCoroutine(PlaceGrass());
        
//         Debug.Log("스테이지 생성 완료!");
//     }
    
//     private IEnumerator GenerateTerrain()
//     {
//         Debug.Log("터레인 생성 중...");
        
//         // 터레인 데이터 생성
//         currentTerrainData = new TerrainData();
//         currentTerrainData.size = new Vector3(settings.terrainWidth, settings.terrainHeight, settings.terrainLength);
        
//         // Unity 6에서는 해상도를 2^n + 1로 설정
//         int resolution = 513; // 기본값
//         if (settings.terrainWidth <= 256) resolution = 257;
//         else if (settings.terrainWidth <= 512) resolution = 513;
//         else resolution = 1025;
        
//         currentTerrainData.heightmapResolution = resolution;
        
//         // 높이맵 생성
//         float[,] heights = GenerateHeights(resolution, resolution);
//         currentTerrainData.SetHeights(0, 0, heights);
        
//         // 터레인 오브젝트 생성
//         GameObject terrainObject = Terrain.CreateTerrainGameObject(currentTerrainData);
//         terrainObject.name = "Generated Terrain";
//         terrainObject.transform.position = Vector3.zero;
        
//         currentTerrain = terrainObject.GetComponent<Terrain>();
//         spawnedObjects.Add(terrainObject);
        
//         yield return null;
//     }
    
//     private float[,] GenerateHeights(int width, int height)
//     {
//         float[,] heights = new float[width, height];
        
//         // 시드 기반 오프셋 추가
//         float offsetX = Random.Range(0f, 1000f);
//         float offsetY = Random.Range(0f, 1000f);
        
//         for (int x = 0; x < width; x++)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 // 정규화된 좌표 계산 (0~1)
//                 float normalizedX = (float)x / (width - 1);
//                 float normalizedY = (float)y / (height - 1);
                
//                 // 노이즈 좌표 계산 (시드 오프셋 적용)
//                 float xCoord = (normalizedX + offsetX) * settings.noiseScale * 10f;
//                 float yCoord = (normalizedY + offsetY) * settings.noiseScale * 10f;
                
//                 // 여러 옥타브의 노이즈를 합성
//                 float noiseValue = 0f;
//                 float amplitude = 1f;
//                 float frequency = 1f;
//                 float maxValue = 0f;
                
//                 for (int i = 0; i < 4; i++)
//                 {
//                     noiseValue += Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * amplitude;
//                     maxValue += amplitude;
//                     amplitude *= 0.5f;
//                     frequency *= 2f;
//                 }
                
//                 noiseValue /= maxValue; // 정규화
                
//                 // 가장자리 페이드 효과를 더 자연스럽게
//                 float distanceFromCenter = Vector2.Distance(
//                     new Vector2(normalizedX, normalizedY), 
//                     new Vector2(0.5f, 0.5f)
//                 );
//                 float maxDistance = 0.7f; // 더 넓은 범위
//                 float falloff = Mathf.Clamp01(1f - (distanceFromCenter / maxDistance));
//                 falloff = Mathf.SmoothStep(0f, 1f, falloff); // 더 부드러운 전환
                
//                 // 평탄한 영역 비율 적용
//                 if (Random.value < settings.flatAreaRatio)
//                 {
//                     noiseValue *= 0.3f; // 평탄한 영역
//                 }
                
//                 noiseValue *= falloff;
//                 noiseValue = settings.heightCurve.Evaluate(Mathf.Clamp01(noiseValue));
                
//                 heights[x, y] = noiseValue;
//             }
//         }
        
//         return heights;
//     }
    
//     private IEnumerator ApplyTerrainTextures()
//     {
//         if (settings.terrainLayers == null || settings.terrainLayers.Length == 0)
//             yield break;
        
//         Debug.Log("터레인 텍스처 적용 중...");
        
//         // 터레인 레이어 생성
//         List<TerrainLayer> terrainLayers = new List<TerrainLayer>();
        
//         foreach (var layerSettings in settings.terrainLayers)
//         {
//             if (layerSettings.diffuseTexture == null) continue;
            
//             TerrainLayer terrainLayer = new TerrainLayer();
//             terrainLayer.diffuseTexture = layerSettings.diffuseTexture;
//             terrainLayer.normalMapTexture = layerSettings.normalMap;
//             terrainLayer.tileSize = layerSettings.tileSize;
//             terrainLayer.tileOffset = layerSettings.tileOffset;
//             terrainLayer.metallic = layerSettings.metallic;
//             terrainLayer.smoothness = layerSettings.smoothness;
            
//             terrainLayers.Add(terrainLayer);
//         }
        
//         currentTerrainData.terrainLayers = terrainLayers.ToArray();
        
//         yield return null;
        
//         // 알파맵 생성 (텍스처 블렌딩)
//         if (terrainLayers.Count > 0)
//         {
//             int alphamapWidth = currentTerrainData.alphamapWidth;
//             int alphamapHeight = currentTerrainData.alphamapHeight;
//             float[,,] alphamaps = new float[alphamapWidth, alphamapHeight, terrainLayers.Count];
            
//             // 노이즈 오프셋 (더 자연스러운 텍스처 분포를 위해)
//             float noiseOffsetX = Random.Range(0f, 100f);
//             float noiseOffsetY = Random.Range(0f, 100f);
            
//             for (int x = 0; x < alphamapWidth; x++)
//             {
//                 for (int y = 0; y < alphamapHeight; y++)
//                 {
//                     // 정규화된 좌표 계산
//                     float normalizedX = (float)x / (alphamapWidth - 1);
//                     float normalizedY = (float)y / (alphamapHeight - 1);
                    
//                     // 높이 정보 가져오기
//                     float height = currentTerrainData.GetInterpolatedHeight(normalizedX, normalizedY) / settings.terrainHeight;
                    
//                     // 경사도 계산
//                     Vector3 normal = currentTerrainData.GetInterpolatedNormal(normalizedX, normalizedY);
//                     float slope = Vector3.Angle(normal, Vector3.up);
                    
//                     // 노이즈를 이용한 자연스러운 변화
//                     float noiseX = normalizedX * 5f + noiseOffsetX;
//                     float noiseY = normalizedY * 5f + noiseOffsetY;
//                     float textureNoise = Mathf.PerlinNoise(noiseX, noiseY);
                    
//                     // 각 레이어별 가중치 계산
//                     float[] weights = new float[terrainLayers.Count];
//                     float totalWeight = 0f;
                    
//                     for (int i = 0; i < settings.terrainLayers.Length && i < terrainLayers.Count; i++)
//                     {
//                         var layerSettings = settings.terrainLayers[i];
                        
//                         // 높이 기반 가중치
//                         float heightWeight = 1f;
//                         if (height >= layerSettings.minHeight && height <= layerSettings.maxHeight)
//                         {
//                             // 높이 범위 내에서 블렌딩
//                             float heightCenter = (layerSettings.minHeight + layerSettings.maxHeight) * 0.5f;
//                             float heightRange = layerSettings.maxHeight - layerSettings.minHeight;
//                             if (heightRange > 0)
//                             {
//                                 float distFromCenter = Mathf.Abs(height - heightCenter) / (heightRange * 0.5f);
//                                 heightWeight = Mathf.Lerp(1f, layerSettings.heightBlend, distFromCenter);
//                             }
//                         }
//                         else if (height < layerSettings.minHeight)
//                         {
//                             float fadeDistance = 0.1f;
//                             float dist = layerSettings.minHeight - height;
//                             heightWeight = Mathf.Clamp01(1f - (dist / fadeDistance));
//                         }
//                         else if (height > layerSettings.maxHeight)
//                         {
//                             float fadeDistance = 0.1f;
//                             float dist = height - layerSettings.maxHeight;
//                             heightWeight = Mathf.Clamp01(1f - (dist / fadeDistance));
//                         }
//                         else
//                         {
//                             heightWeight = 0f;
//                         }
                        
//                         // 경사 기반 가중치
//                         float slopeWeight = 1f;
//                         if (slope >= layerSettings.minSlope && slope <= layerSettings.maxSlope)
//                         {
//                             float slopeCenter = (layerSettings.minSlope + layerSettings.maxSlope) * 0.5f;
//                             float slopeRange = layerSettings.maxSlope - layerSettings.minSlope;
//                             if (slopeRange > 0)
//                             {
//                                 float distFromCenter = Mathf.Abs(slope - slopeCenter) / (slopeRange * 0.5f);
//                                 slopeWeight = Mathf.Lerp(1f, 0.5f, distFromCenter);
//                             }
//                         }
//                         else
//                         {
//                             slopeWeight = 0f;
//                         }
                        
//                         // 노이즈 기반 자연스러운 변화
//                         float noiseInfluence = Mathf.Lerp(0.7f, 1.3f, textureNoise);
                        
//                         weights[i] = heightWeight * slopeWeight * noiseInfluence;
//                         totalWeight += weights[i];
//                     }
                    
//                     // 정규화 및 적용
//                     if (totalWeight > 0)
//                     {
//                         for (int i = 0; i < weights.Length; i++)
//                         {
//                             alphamaps[x, y, i] = weights[i] / totalWeight;
//                         }
//                     }
//                     else
//                     {
//                         // 기본 레이어 적용
//                         if (terrainLayers.Count > 0)
//                             alphamaps[x, y, 0] = 1f;
//                     }
//                 }
                
//                 if (x % 16 == 0) yield return null; // 주기적으로 yield
//             }
            
//             currentTerrainData.SetAlphamaps(0, 0, alphamaps);
//         }
        
//         yield return null;
//     }
    
//     private IEnumerator PlaceStartPoint()
//     {
//         if (settings.startPointPrefab == null) yield break;
        
//         Debug.Log("시작점 배치 중...");
        
//         Vector3 startPos = GetRandomValidPosition(settings.safeZoneRadius);
//         startPoint = startPos;
        
//         GameObject startObj = Instantiate(settings.startPointPrefab, startPos, Quaternion.identity);
//         startObj.name = "Start Point";
//         spawnedObjects.Add(startObj);
        
//         AddOccupiedPosition(startPos, settings.safeZoneRadius);
        
//         yield return null;
//     }
    
//     private IEnumerator PlaceExitPoints()
//     {
//         if (settings.exitPointPrefabs == null || settings.exitPointPrefabs.Length == 0) yield break;
        
//         Debug.Log("탈출점 배치 중...");
        
//         for (int i = 0; i < settings.exitPointCount; i++)
//         {
//             int attempts = 0;
//             Vector3 exitPos;
            
//             do
//             {
//                 exitPos = GetRandomValidPosition(settings.safeZoneRadius);
//                 attempts++;
//             }
//             while (Vector3.Distance(exitPos, startPoint) < settings.safeZoneRadius * 2f && attempts < 50);
            
//             if (attempts >= 50)
//             {
//                 Debug.LogWarning($"탈출점 {i + 1} 배치 실패 - 적절한 위치를 찾을 수 없습니다.");
//                 continue;
//             }
            
//             GameObject exitPrefab = settings.exitPointPrefabs[Random.Range(0, settings.exitPointPrefabs.Length)];
//             GameObject exitObj = Instantiate(exitPrefab, exitPos, Quaternion.identity);
//             exitObj.name = $"Exit Point {i + 1}";
//             spawnedObjects.Add(exitObj);
            
//             exitPoints.Add(exitPos);
//             AddOccupiedPosition(exitPos, settings.safeZoneRadius);
            
//             yield return null;
//         }
//     }
    
//     private IEnumerator PlaceObstacles()
//     {
//         if (settings.obstaclePrefabs == null || settings.obstaclePrefabs.Length == 0) yield break;
        
//         Debug.Log("장애물 배치 중...");
        
//         int obstacleCount = Random.Range(settings.minObstacles, settings.maxObstacles + 1);
//         int placed = 0;
//         int attempts = 0;
//         const int maxAttempts = 1000;
        
//         while (placed < obstacleCount && attempts < maxAttempts)
//         {
//             attempts++;
            
//             Vector3 position = GetRandomValidPosition(settings.minObstacleDistance);
//             if (position == Vector3.zero) continue;
            
//             GameObject obstaclePrefab = settings.obstaclePrefabs[Random.Range(0, settings.obstaclePrefabs.Length)];
//             GameObject obstacle = Instantiate(obstaclePrefab, position, GetRandomRotation());
//             obstacle.name = $"Obstacle {placed + 1}";
//             spawnedObjects.Add(obstacle);
            
//             // 콜리더 크기 기반으로 점유 영역 계산
//             float obstacleRadius = GetObjectRadius(obstacle);
//             AddOccupiedPosition(position, obstacleRadius + settings.minObstacleDistance);
            
//             placed++;
            
//             if (placed % 5 == 0) yield return null;
//         }
        
//         Debug.Log($"장애물 {placed}개 배치 완료");
//     }
    
//     private IEnumerator PlaceTrees()
//     {
//         if (settings.treePrefabs == null || settings.treePrefabs.Length == 0) yield break;
        
//         Debug.Log("나무 배치 중...");
        
//         int treeCount = Random.Range(settings.minTrees, settings.maxTrees + 1);
//         int placed = 0;
//         int attempts = 0;
//         const int maxAttempts = 2000;
        
//         while (placed < treeCount && attempts < maxAttempts)
//         {
//             attempts++;
            
//             Vector3 position = GetRandomValidPosition(3f);
//             if (position == Vector3.zero) continue;
            
//             // 경사도 체크 (나무는 평평한 곳에 배치)
//             if (GetTerrainSlope(position) > 30f) continue;
            
//             GameObject treePrefab = settings.treePrefabs[Random.Range(0, settings.treePrefabs.Length)];
//             GameObject tree = Instantiate(treePrefab, position, GetRandomRotation());
//             tree.name = $"Tree {placed + 1}";
            
//             // 크기 약간 랜덤화
//             float scale = Random.Range(0.8f, 1.2f);
//             tree.transform.localScale = Vector3.one * scale;
            
//             spawnedObjects.Add(tree);
            
//             float treeRadius = GetObjectRadius(tree);
//             AddOccupiedPosition(position, treeRadius + 2f);
            
//             placed++;
            
//             if (placed % 10 == 0) yield return null;
//         }
        
//         Debug.Log($"나무 {placed}개 배치 완료");
//     }
    
//     private IEnumerator PlaceGrass()
//     {
//         if (settings.grassPrefabs == null || settings.grassPrefabs.Length == 0) yield break;
        
//         Debug.Log("풀 배치 중...");
        
//         int grassCount = Random.Range(settings.minGrass, settings.maxGrass + 1);
//         int placed = 0;
        
//         for (int i = 0; i < grassCount; i++)
//         {
//             Vector3 position = GetRandomValidPosition(1f);
//             if (position == Vector3.zero) continue;
            
//             // 풀은 더 자유롭게 배치
//             if (Random.value > settings.vegetationDensity) continue;
            
//             GameObject grassPrefab = settings.grassPrefabs[Random.Range(0, settings.grassPrefabs.Length)];
//             GameObject grass = Instantiate(grassPrefab, position, GetRandomRotation());
//             grass.name = $"Grass {placed + 1}";
            
//             // 크기와 회전 랜덤화
//             float scale = Random.Range(0.5f, 1.5f);
//             grass.transform.localScale = Vector3.one * scale;
            
//             spawnedObjects.Add(grass);
//             placed++;
            
//             if (placed % 20 == 0) yield return null;
//         }
        
//         Debug.Log($"풀 {placed}개 배치 완료");
//     }
    
//     private Vector3 GetRandomValidPosition(float minDistance)
//     {
//         const int maxAttempts = 100;
        
//         for (int i = 0; i < maxAttempts; i++)
//         {
//             // 터레인 전체 영역에서 균등하게 분포되도록 수정
//             float x = Random.Range(minDistance * 2f, settings.terrainWidth - minDistance * 2f);
//             float z = Random.Range(minDistance * 2f, settings.terrainLength - minDistance * 2f);
            
//             Vector3 position = new Vector3(x, 0, z);
            
//             // 터레인 높이 정확히 계산
//             if (currentTerrain != null)
//             {
//                 position.y = currentTerrain.SampleHeight(position);
//             }
            
//             bool isValid = true;
            
//             // 다른 오브젝트와의 거리 체크
//             foreach (Vector3 occupied in occupiedPositions)
//             {
//                 float distance = Vector3.Distance(new Vector3(position.x, 0, position.z), new Vector3(occupied.x, 0, occupied.z));
//                 if (distance < minDistance)
//                 {
//                     isValid = false;
//                     break;
//                 }
//             }
            
//             // 터레인 경계에서 너무 가깝지 않은지 체크
//             if (position.x < minDistance || position.x > settings.terrainWidth - minDistance ||
//                 position.z < minDistance || position.z > settings.terrainLength - minDistance)
//             {
//                 isValid = false;
//             }
            
//             if (isValid)
//                 return position;
//         }
        
//         return Vector3.zero; // 실패
//     }
    
//     private float GetTerrainHeight(Vector3 worldPosition)
//     {
//         if (currentTerrain == null) return 0f;
        
//         return currentTerrain.SampleHeight(worldPosition);
//     }
    
//     private float GetTerrainHeightNormalized(Vector3 worldPosition)
//     {
//         if (currentTerrain == null) return 0f;
        
//         float height = currentTerrain.SampleHeight(worldPosition);
//         return height / settings.terrainHeight;
//     }
    
//     private float GetTerrainSlope(Vector3 worldPosition)
//     {
//         if (currentTerrain == null || currentTerrainData == null) return 0f;
        
//         // 월드 좌표를 터레인 로컬 좌표로 변환
//         Vector3 terrainPos = worldPosition - currentTerrain.transform.position;
        
//         // 정규화된 좌표로 변환
//         float normalizedX = Mathf.Clamp01(terrainPos.x / currentTerrainData.size.x);
//         float normalizedZ = Mathf.Clamp01(terrainPos.z / currentTerrainData.size.z);
        
//         return GetTerrainSlopeAtPosition(normalizedX, normalizedZ);
//     }
    
//     private float GetTerrainSlopeAtPosition(float normalizedX, float normalizedZ)
//     {
//         if (currentTerrainData == null) return 0f;
        
//         Vector3 normal = currentTerrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
//         float slope = Vector3.Angle(normal, Vector3.up);
        
//         return slope;
//     }
    
//     private float GetObjectRadius(GameObject obj)
//     {
//         Collider collider = obj.GetComponent<Collider>();
//         if (collider != null)
//         {
//             return Mathf.Max(collider.bounds.size.x, collider.bounds.size.z) * 0.5f;
//         }
        
//         Renderer renderer = obj.GetComponent<Renderer>();
//         if (renderer != null)
//         {
//             return Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.z) * 0.5f;
//         }
        
//         return 2f; // 기본값
//     }
    
//     private void AddOccupiedPosition(Vector3 position, float radius)
//     {
//         occupiedPositions.Add(position);
//     }
    
//     private Quaternion GetRandomRotation()
//     {
//         return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
//     }
    
//     // 퍼블릭 메서드들
//     public void SetSeed(int seed)
//     {
//         settings.randomSeed = seed;
//     }
    
//     public void SetTerrainSize(int width, int length)
//     {
//         settings.terrainWidth = width;
//         settings.terrainLength = length;
//     }
    
//     public void SetObstacleCount(int min, int max)
//     {
//         settings.minObstacles = min;
//         settings.maxObstacles = max;
//     }
    
//     public void SetVegetationCount(int minTrees, int maxTrees, int minGrass, int maxGrass)
//     {
//         settings.minTrees = minTrees;
//         settings.maxTrees = maxTrees;
//         settings.minGrass = minGrass;
//         settings.maxGrass = maxGrass;
//     }
    
//     public Vector3 GetStartPoint()
//     {
//         return startPoint;
//     }
    
//     public List<Vector3> GetExitPoints()
//     {
//         return new List<Vector3>(exitPoints);
//     }
    
//     void OnDrawGizmos()
//     {
//         if (Application.isPlaying && currentTerrain != null)
//         {
//             // 터레인 경계 표시
//             Gizmos.color = Color.white;
//             Vector3 terrainSize = new Vector3(settings.terrainWidth, 0, settings.terrainLength);
//             Gizmos.DrawWireCube(terrainSize * 0.5f, terrainSize);
            
//             // 시작점 표시
//             Gizmos.color = Color.green;
//             Gizmos.DrawWireSphere(startPoint, 3f);
            
//             // 탈출점들 표시
//             Gizmos.color = Color.red;
//             foreach (Vector3 exit in exitPoints)
//             {
//                 Gizmos.DrawWireSphere(exit, 3f);
//             }
//         }
//     }
// }