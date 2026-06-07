// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [System.Serializable]
// public class TerrainMaterial
// {
//     public Material material;
//     public float minHeight = 0f;
//     public float maxHeight = 1f;
// }

// [System.Serializable]
// public class BiomeObject
// {
//     public GameObject prefab;
//     public float minHeight = 0f;
//     public float maxHeight = 1f;
//     public float density = 0.1f; // 0~1 사이의 값
//     public float minScale = 0.8f;
//     public float maxScale = 1.2f;
//     public float slopeLimit = 45f; // 배치 가능한 최대 경사도
// }

// [System.Serializable]
// public class SceneryObject
// {
//     public GameObject prefab;
//     public float density = 0.05f;
//     public float minScale = 0.8f;
//     public float maxScale = 1.2f;
//     public float minDistanceFromOthers = 3f;
//     public float slopeLimit = 30f;
// }

// public class MeshTerrainGenerator : MonoBehaviour
// {
//     [Header("메쉬 설정")]
//     public int meshResolution = 128; // 메쉬의 세밀함 (버텍스 개수)
//     public float terrainWidth = 100f;
//     public float terrainHeight = 100f;
//     public float terrainScale = 20f;
//     public AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
//     [Header("노이즈 설정")]
//     public float noiseScale = 0.1f;
//     public int octaves = 4;
//     public float persistence = 0.5f;
//     public float lacunarity = 2f;
//     public int seed = 0;
    
//     [Header("추가 지형 특징")]
//     public bool addHills = true;
//     public int hillCount = 5;
//     public float hillRadius = 30f;
//     public float hillHeight = 10f;
    
//     public bool addValleys = true;
//     public int valleyCount = 3;
//     public float valleyRadius = 40f;
//     public float valleyDepth = 8f;
    
//     [Header("머티리얼 설정")]
//     public TerrainMaterial[] terrainMaterials;
//     public bool useVertexColors = true; // 버텍스 컬러 사용 여부
    
//     [Header("바이옴 오브젝트")]
//     public BiomeObject[] biomeObjects;
    
//     [Header("기타 오브젝트")]
//     public SceneryObject[] sceneryObjects;
    
//     [Header("디버그 정보")]
//     public bool showDebugInfo = false;
    
//     [Header("생성 옵션")]
//     public bool generateOnStart = true;
//     public bool clearExistingObjects = true;
//     public bool generateCollider = true;
    
//     private GameObject terrainObject;
//     private MeshFilter meshFilter;
//     private MeshRenderer meshRenderer;
//     private MeshCollider meshCollider;
//     private Mesh terrainMesh;
//     private float[,] heightMap;
//     private List<Vector3> occupiedPositions = new List<Vector3>();
    
//     void Start()
//     {
//         if (generateOnStart)
//         {
//             GenerateTerrain();
//         }
//     }
    
//     [ContextMenu("지형 생성")]
//     public void GenerateTerrain()
//     {
//         if (clearExistingObjects)
//         {
//             ClearExistingObjects();
//         }
        
//         CreateTerrainObject();
//         GenerateHeightMap();
//         GenerateMesh();
//         ApplyMaterials();
//         if (generateCollider)
//             GenerateCollider();
//         PlaceBiomeObjects();
//         PlaceSceneryObjects();
//     }
    
//     void CreateTerrainObject()
//     {
//         // 기존 지형 오브젝트 삭제
//         if (terrainObject != null)
//         {
//             DestroyImmediate(terrainObject);
//         }
        
//         // 새로운 지형 오브젝트 생성
//         terrainObject = new GameObject("Generated Terrain");
//         terrainObject.transform.parent = transform;
//         terrainObject.transform.localPosition = Vector3.zero;
        
//         // 컴포넌트 추가
//         meshFilter = terrainObject.AddComponent<MeshFilter>();
//         meshRenderer = terrainObject.AddComponent<MeshRenderer>();
        
//         if (generateCollider)
//         {
//             meshCollider = terrainObject.AddComponent<MeshCollider>();
//         }
//     }
    
//     void GenerateHeightMap()
//     {
//         heightMap = new float[meshResolution + 1, meshResolution + 1];
        
//         for (int x = 0; x <= meshResolution; x++)
//         {
//             for (int y = 0; y <= meshResolution; y++)
//             {
//                 float height = GeneratePerlinNoise(x, y);
//                 heightMap[x, y] = heightCurve.Evaluate(height);
//             }
//         }
        
//         // 언덕 추가
//         if (addHills)
//         {
//             AddHills();
//         }
        
//         // 계곡 추가
//         if (addValleys)
//         {
//             AddValleys();
//         }
//     }
    
//     float GeneratePerlinNoise(int x, int y)
//     {
//         float amplitude = 1f;
//         float frequency = noiseScale;
//         float noiseValue = 0f;
//         float maxValue = 0f;
        
//         for (int i = 0; i < octaves; i++)
//         {
//             float sampleX = (x + seed) * frequency / meshResolution;
//             float sampleY = (y + seed) * frequency / meshResolution;
            
//             float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
//             noiseValue += perlinValue * amplitude;
            
//             maxValue += amplitude;
//             amplitude *= persistence;
//             frequency *= lacunarity;
//         }
        
//         return noiseValue / maxValue;
//     }
    
//     void AddHills()
//     {
//         for (int i = 0; i < hillCount; i++)
//         {
//             Vector2 hillCenter = new Vector2(
//                 Random.Range(hillRadius / terrainWidth * meshResolution, meshResolution - (hillRadius / terrainWidth * meshResolution)),
//                 Random.Range(hillRadius / terrainHeight * meshResolution, meshResolution - (hillRadius / terrainHeight * meshResolution))
//             );
            
//             float hillStrength = Random.Range(0.7f, 1.3f);
//             float hillRadiusInMesh = hillRadius / terrainWidth * meshResolution;
            
//             for (int x = 0; x <= meshResolution; x++)
//             {
//                 for (int y = 0; y <= meshResolution; y++)
//                 {
//                     float distance = Vector2.Distance(new Vector2(x, y), hillCenter);
//                     if (distance < hillRadiusInMesh)
//                     {
//                         float hillInfluence = (1f - distance / hillRadiusInMesh) * hillStrength;
//                         hillInfluence = Mathf.Pow(hillInfluence, 2f); // 부드러운 곡선
//                         heightMap[x, y] += hillInfluence * (hillHeight / terrainScale);
//                         heightMap[x, y] = Mathf.Clamp01(heightMap[x, y]);
//                     }
//                 }
//             }
//         }
//     }
    
//     void AddValleys()
//     {
//         for (int i = 0; i < valleyCount; i++)
//         {
//             Vector2 valleyCenter = new Vector2(
//                 Random.Range(valleyRadius / terrainWidth * meshResolution, meshResolution - (valleyRadius / terrainWidth * meshResolution)),
//                 Random.Range(valleyRadius / terrainHeight * meshResolution, meshResolution - (valleyRadius / terrainHeight * meshResolution))
//             );
            
//             float valleyStrength = Random.Range(0.7f, 1.3f);
//             float valleyRadiusInMesh = valleyRadius / terrainWidth * meshResolution;
            
//             for (int x = 0; x <= meshResolution; x++)
//             {
//                 for (int y = 0; y <= meshResolution; y++)
//                 {
//                     float distance = Vector2.Distance(new Vector2(x, y), valleyCenter);
//                     if (distance < valleyRadiusInMesh)
//                     {
//                         float valleyInfluence = (1f - distance / valleyRadiusInMesh) * valleyStrength;
//                         valleyInfluence = Mathf.Pow(valleyInfluence, 2f);
//                         heightMap[x, y] -= valleyInfluence * (valleyDepth / terrainScale);
//                         heightMap[x, y] = Mathf.Max(0f, heightMap[x, y]);
//                     }
//                 }
//             }
//         }
//     }
    
//     void GenerateMesh()
//     {
//         if (terrainMesh != null)
//         {
//             DestroyImmediate(terrainMesh);
//         }
        
//         terrainMesh = new Mesh();
//         terrainMesh.name = "Generated Terrain Mesh";
        
//         // 버텍스 생성
//         List<Vector3> vertices = new List<Vector3>();
//         List<Vector2> uvs = new List<Vector2>();
//         List<Color> colors = new List<Color>();
        
//         for (int x = 0; x <= meshResolution; x++)
//         {
//             for (int y = 0; y <= meshResolution; y++)
//             {
//                 float xPos = (float)x / meshResolution * terrainWidth - terrainWidth * 0.5f;
//                 float yPos = (float)y / meshResolution * terrainHeight - terrainHeight * 0.5f;
//                 float height = heightMap[x, y] * terrainScale;
                
//                 vertices.Add(new Vector3(xPos, height, yPos));
//                 uvs.Add(new Vector2((float)x / meshResolution, (float)y / meshResolution));
                
//                 // 버텍스 컬러 (높이 기반)
//                 if (useVertexColors)
//                 {
//                     colors.Add(GetColorForHeight(heightMap[x, y]));
//                 }
//             }
//         }
        
//         // 삼각형 인덱스 생성 (시계 반대 방향으로 와인딩)
//         List<int> triangles = new List<int>();
        
//         for (int x = 0; x < meshResolution; x++)
//         {
//             for (int y = 0; y < meshResolution; y++)
//             {
//                 int i = x * (meshResolution + 1) + y;
                
//                 // 첫 번째 삼각형 (시계 반대 방향)
//                 triangles.Add(i);
//                 triangles.Add(i + 1);
//                 triangles.Add(i + meshResolution + 1);
                
//                 // 두 번째 삼각형 (시계 반대 방향)
//                 triangles.Add(i + 1);
//                 triangles.Add(i + meshResolution + 2);
//                 triangles.Add(i + meshResolution + 1);
//             }
//         }
        
//         // 메쉬 설정
//         terrainMesh.vertices = vertices.ToArray();
//         terrainMesh.triangles = triangles.ToArray();
//         terrainMesh.uv = uvs.ToArray();
        
//         if (useVertexColors && colors.Count > 0)
//         {
//             terrainMesh.colors = colors.ToArray();
//         }
        
//         terrainMesh.RecalculateNormals();
//         terrainMesh.RecalculateBounds();
        
//         meshFilter.mesh = terrainMesh;
//     }
    
//     Color GetColorForHeight(float normalizedHeight)
//     {
//         // 높이에 따른 컬러 그라디언트
//         if (normalizedHeight < 0.3f)
//             return Color.Lerp(new Color(0.2f, 0.4f, 0.1f), new Color(0.3f, 0.6f, 0.2f), normalizedHeight / 0.3f); // 어두운 녹색 -> 녹색
//         else if (normalizedHeight < 0.6f)
//             return Color.Lerp(new Color(0.3f, 0.6f, 0.2f), new Color(0.6f, 0.5f, 0.3f), (normalizedHeight - 0.3f) / 0.3f); // 녹색 -> 갈색
//         else if (normalizedHeight < 0.8f)
//             return Color.Lerp(new Color(0.6f, 0.5f, 0.3f), new Color(0.5f, 0.5f, 0.5f), (normalizedHeight - 0.6f) / 0.2f); // 갈색 -> 회색
//         else
//             return Color.Lerp(new Color(0.5f, 0.5f, 0.5f), Color.white, (normalizedHeight - 0.8f) / 0.2f); // 회색 -> 흰색 (눈)
//     }
    
//     void ApplyMaterials()
//     {
//         if (terrainMaterials != null && terrainMaterials.Length > 0)
//         {
//             // 첫 번째 머티리얼 적용 (추후 높이별 머티리얼 블렌딩 구현 가능)
//             meshRenderer.material = terrainMaterials[0].material;
//         }
//         else
//         {
//             // 기본 머티리얼 생성
//             Material defaultMaterial = new Material(Shader.Find("Standard"));
//             defaultMaterial.color = Color.green;
//             meshRenderer.material = defaultMaterial;
//         }
//     }
    
//     void GenerateCollider()
//     {
//         if (meshCollider != null)
//         {
//             meshCollider.sharedMesh = terrainMesh;
//         }
//     }
    
//     void PlaceBiomeObjects()
//     {
//         if (biomeObjects == null || biomeObjects.Length == 0) return;
        
//         occupiedPositions.Clear();
        
//         foreach (BiomeObject biomeObj in biomeObjects)
//         {
//             if (biomeObj.prefab == null) continue;
            
//             GameObject parent = new GameObject(biomeObj.prefab.name + "_Group");
//             parent.transform.parent = transform;
            
//             int objectCount = Mathf.RoundToInt(terrainWidth * terrainHeight * biomeObj.density / 10000f);
            
//             for (int i = 0; i < objectCount; i++)
//             {
//                 Vector3 position = GetRandomValidPosition(biomeObj.minHeight, biomeObj.maxHeight, biomeObj.slopeLimit);
                
//                 if (position != Vector3.zero)
//                 {
//                     GameObject instance = Instantiate(biomeObj.prefab, position, Quaternion.identity, parent.transform);
                    
//                     // 랜덤 회전
//                     instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    
//                     // 랜덤 스케일
//                     float scale = Random.Range(biomeObj.minScale, biomeObj.maxScale);
//                     instance.transform.localScale = Vector3.one * scale;
                    
//                     occupiedPositions.Add(position);
//                 }
//             }
//         }
//     }
    
//     void PlaceSceneryObjects()
//     {
//         if (sceneryObjects == null || sceneryObjects.Length == 0) return;
        
//         foreach (SceneryObject sceneryObj in sceneryObjects)
//         {
//             if (sceneryObj.prefab == null) continue;
            
//             GameObject parent = new GameObject(sceneryObj.prefab.name + "_Group");
//             parent.transform.parent = transform;
            
//             int objectCount = Mathf.RoundToInt(terrainWidth * terrainHeight * sceneryObj.density / 10000f);
            
//             for (int i = 0; i < objectCount; i++)
//             {
//                 Vector3 position = GetRandomValidPositionWithDistance(sceneryObj.minDistanceFromOthers, sceneryObj.slopeLimit);
                
//                 if (position != Vector3.zero)
//                 {
//                     GameObject instance = Instantiate(sceneryObj.prefab, position, Quaternion.identity, parent.transform);
                    
//                     // 랜덤 회전
//                     instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    
//                     // 랜덤 스케일
//                     float scale = Random.Range(sceneryObj.minScale, sceneryObj.maxScale);
//                     instance.transform.localScale = Vector3.one * scale;
                    
//                     occupiedPositions.Add(position);
//                 }
//             }
//         }
//     }
    
//     Vector3 GetRandomValidPosition(float minHeight, float maxHeight, float maxSlope)
//     {
//         int attempts = 0;
//         int successfulPlacements = 0;
        
//         while (attempts < 100)
//         {
//             float x = Random.Range(-terrainWidth * 0.5f, terrainWidth * 0.5f);
//             float z = Random.Range(-terrainHeight * 0.5f, terrainHeight * 0.5f);
            
//             float height = GetHeightAtPosition(x, z);
//             float normalizedHeight = height / terrainScale;
            
//             // 디버그 정보 출력
//             if (showDebugInfo && attempts < 5)
//             {
//                 Debug.Log($"시도 {attempts}: 높이={normalizedHeight:F3} (범위: {minHeight:F2}-{maxHeight:F2}), 경사={GetSlope(x, z):F1}도 (최대: {maxSlope}도)");
//             }
            
//             // 높이 체크
//             if (normalizedHeight < minHeight || normalizedHeight > maxHeight)
//             {
//                 attempts++;
//                 continue;
//             }
            
//             // 경사도 체크
//             Vector3 worldPos = new Vector3(x, height, z) + transform.position;
//             float currentSlope = GetSlope(x, z);
//             if (currentSlope > maxSlope)
//             {
//                 attempts++;
//                 continue;
//             }
            
//             successfulPlacements++;
//             return worldPos;
//         }
        
//         if (showDebugInfo)
//         {
//             Debug.LogWarning($"오브젝트 배치 실패: 100번 시도 후 적합한 위치를 찾지 못했습니다. 성공한 배치: {successfulPlacements}");
//         }
        
//         return Vector3.zero;
//     }
    
//     Vector3 GetRandomValidPositionWithDistance(float minDistance, float maxSlope)
//     {
//         int attempts = 0;
//         while (attempts < 100)
//         {
//             float x = Random.Range(-terrainWidth * 0.5f, terrainWidth * 0.5f);
//             float z = Random.Range(-terrainHeight * 0.5f, terrainHeight * 0.5f);
            
//             float height = GetHeightAtPosition(x, z);
//             Vector3 worldPos = new Vector3(x, height, z) + transform.position;
            
//             // 경사도 체크
//             if (GetSlope(x, z) > maxSlope)
//             {
//                 attempts++;
//                 continue;
//             }
            
//             // 다른 오브젝트와의 거리 체크
//             bool tooClose = false;
//             foreach (Vector3 occupied in occupiedPositions)
//             {
//                 if (Vector3.Distance(worldPos, occupied) < minDistance)
//                 {
//                     tooClose = true;
//                     break;
//                 }
//             }
            
//             if (!tooClose)
//             {
//                 return worldPos;
//             }
            
//             attempts++;
//         }
        
//         return Vector3.zero;
//     }
    
//     float GetHeightAtPosition(float x, float z)
//     {
//         // 로컬 좌표를 헤이트맵 인덱스로 변환
//         float normalizedX = (x + terrainWidth * 0.5f) / terrainWidth;
//         float normalizedZ = (z + terrainHeight * 0.5f) / terrainHeight;
        
//         normalizedX = Mathf.Clamp01(normalizedX);
//         normalizedZ = Mathf.Clamp01(normalizedZ);
        
//         int indexX = Mathf.FloorToInt(normalizedX * meshResolution);
//         int indexZ = Mathf.FloorToInt(normalizedZ * meshResolution);
        
//         indexX = Mathf.Clamp(indexX, 0, meshResolution);
//         indexZ = Mathf.Clamp(indexZ, 0, meshResolution);
        
//         return heightMap[indexX, indexZ] * terrainScale;
//     }
    
//     float GetSlope(float x, float z)
//     {
//         // 샘플링 거리를 지형 크기에 맞게 조정
//         float sampleDistance = Mathf.Max(terrainWidth, terrainHeight) / meshResolution;
        
//         float height = GetHeightAtPosition(x, z);
//         float heightRight = GetHeightAtPosition(x + sampleDistance, z);
//         float heightUp = GetHeightAtPosition(x, z + sampleDistance);
        
//         // 높이 차이를 거리로 나누어 기울기 계산
//         float slopeX = Mathf.Abs(heightRight - height) / sampleDistance;
//         float slopeZ = Mathf.Abs(heightUp - height) / sampleDistance;
        
//         // 최대 기울기를 각도로 변환
//         float maxSlope = Mathf.Max(slopeX, slopeZ);
//         float slopeAngle = Mathf.Atan(maxSlope) * Mathf.Rad2Deg;
        
//         return slopeAngle;
//     }
    
//     void ClearExistingObjects()
//     {
//         // 기존에 생성된 오브젝트들 삭제
//         for (int i = transform.childCount - 1; i >= 0; i--)
//         {
//             DestroyImmediate(transform.GetChild(i).gameObject);
//         }
//     }
    
//     void OnValidate()
//     {
//         // 값 범위 제한
//         meshResolution = Mathf.Max(1, meshResolution);
//         terrainWidth = Mathf.Max(0.1f, terrainWidth);
//         terrainHeight = Mathf.Max(0.1f, terrainHeight);
//         terrainScale = Mathf.Max(0.1f, terrainScale);
//         noiseScale = Mathf.Max(0.001f, noiseScale);
//         octaves = Mathf.Max(1, octaves);
//         persistence = Mathf.Clamp01(persistence);
//         lacunarity = Mathf.Max(1f, lacunarity);
//     }
// }