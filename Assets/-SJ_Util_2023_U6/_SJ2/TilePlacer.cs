using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float tileSize = 1f;

    [Header("Ratios (0..1)")]
    [Range(0f, 1f)] public float roadAndWaterRatio = 0.15f; // 전체 타일 중 도로+수로 비중
    [Range(0f, 1f)] public float waterShare = 0.3f; // 도로+수로 중 수로가 차지하는 비율 (0 => 전부 도로)

    [Header("Road/Water generation")]
    public bool generatePaths = true; // true면 선형(랜덤 워크)으로 일부 생성, false면 단순 랜덤 분포
    public int numPathSeeds = 6; // 생성할 랜덤 워크 시작점 개수
    public int maxPathLength = 40; // 각 랜덤 워크 최대 길이
    [Range(0f,1f)] public float straightness = 0.7f; // 0-1, 1이면 직선에 가깝게 (확률로 방향 유지)

    [Header("Prefabs")]
    public GameObject roadPrefab;
    public GameObject waterPrefab;
    public List<GameObject> objectPrefabs; // 빈 블럭에 랜덤으로 배치할 오브젝트들

    [Header("Misc")]
    public Transform parent; // 생성된 타일의 부모 (optional)
    public int randomSeed = 0; // 0이면 시드 자동, 아닌경우 고정 시드

    // internal
    enum TileType { Empty, Road, Water, Object }
    TileType[,] map;

    // Clear previous children
    public void ClearAll()
    {
        if (parent == null)
        {
            Transform t = transform;
            var children = new List<GameObject>();
            for (int i = 0; i < t.childCount; i++) children.Add(t.GetChild(i).gameObject);
            foreach (var c in children) DestroyImmediate(c);
        }
        else
        {
            var t = parent;
            var children = new List<GameObject>();
            for (int i = 0; i < t.childCount; i++) children.Add(t.GetChild(i).gameObject);
            foreach (var c in children) DestroyImmediate(c);
        }
    }

    // Inspector에서 바로 실행하려면 ContextMenu 사용 가능
    [ContextMenu("Generate")]
    public void Generate()
    {
        if (randomSeed != 0) Random.InitState(randomSeed);
        InitializeMap();
        PlaceRoadsAndWater();
        FillObjects();
        InstantiateMap();
    }

    void InitializeMap()
    {
        map = new TileType[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
            for (int z = 0; z < gridHeight; z++)
                map[x, z] = TileType.Empty;
    }

    void PlaceRoadsAndWater()
    {
        int total = gridWidth * gridHeight;
        int targetCount = Mathf.RoundToInt(total * roadAndWaterRatio);
        if (targetCount <= 0) return;

        int waterTarget = Mathf.RoundToInt(targetCount * waterShare);
        int roadTarget = targetCount - waterTarget;

        // If using path-based generation, create random-walk paths until quotas reached.
        if (generatePaths)
        {
            int createdRoad = 0;
            int createdWater = 0;

            // Alternate creating paths of road or water for variety
            int seeds = Mathf.Max(1, numPathSeeds);
            for (int s = 0; s < seeds && (createdRoad + createdWater) < targetCount; s++)
            {
                // choose whether this path is water or road (prefer the one with remaining quota)
                bool makeWater = (createdWater < waterTarget) && (Random.value < 0.5f || createdRoad >= roadTarget);
                // pick a random start
                int x = Random.Range(0, gridWidth);
                int z = Random.Range(0, gridHeight);

                int length = Random.Range(3, Mathf.Max(3, maxPathLength));
                Vector2Int dir = RandomDirection();

                for (int i = 0; i < length && (createdRoad + createdWater) < targetCount; i++)
                {
                    if (!InBounds(x, z)) break;

                    if (map[x, z] == TileType.Empty)
                    {
                        map[x, z] = makeWater ? TileType.Water : TileType.Road;
                        if (makeWater) createdWater++; else createdRoad++;
                    }

                    // decide next direction: with probability straightness keep same dir, else random turn
                    if (Random.value > straightness) dir = RandomDirection();

                    x += dir.x;
                    z += dir.y;

                    // small chance to flip to other type if quotas require
                    if (Random.value < 0.02f)
                    {
                        if (createdWater < waterTarget && createdRoad >= roadTarget) makeWater = true;
                        if (createdRoad < roadTarget && createdWater >= waterTarget) makeWater = false;
                    }
                }
            }

            // If quotas not fulfilled yet (e.g., due to overlaps/out of bounds), fill remaining randomly
            int attempts = 0;
            while ((createdRoad + createdWater) < targetCount && attempts < total * 3)
            {
                attempts++;
                int rx = Random.Range(0, gridWidth);
                int rz = Random.Range(0, gridHeight);
                if (map[rx, rz] != TileType.Empty) continue;

                if (createdWater < waterTarget)
                {
                    map[rx, rz] = TileType.Water; createdWater++;
                }
                else if (createdRoad < roadTarget)
                {
                    map[rx, rz] = TileType.Road; createdRoad++;
                }
            }
        }
        else
        {
            // simple random placement
            int placed = 0;
            List<int> indices = new List<int>(total);
            for (int i = 0; i < total; i++) indices.Add(i);
            // shuffle
            for (int i = 0; i < total; i++)
            {
                int j = Random.Range(i, total);
                int tmp = indices[i]; indices[i] = indices[j]; indices[j] = tmp;
            }

            int waterPlaced = 0;
            int roadPlaced = 0;

            for (int k = 0; k < total && placed < targetCount; k++)
            {
                int idx = indices[k];
                int x = idx % gridWidth;
                int z = idx / gridWidth;
                if (waterPlaced < waterTarget)
                {
                    map[x, z] = TileType.Water; waterPlaced++; placed++;
                }
                else
                {
                    map[x, z] = TileType.Road; roadPlaced++; placed++;
                }
            }
        }
    }

    void FillObjects()
    {
        if (objectPrefabs == null || objectPrefabs.Count == 0) return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                if (map[x, z] == TileType.Empty)
                {
                    map[x, z] = TileType.Object; // marker, instantiate later
                }
            }
        }
    }

    void InstantiateMap()
    {
        // clear previous
        ClearAll();

        Transform root = parent != null ? parent : transform;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 pos = transform.position + new Vector3(x * tileSize, 0f, z * tileSize);
                GameObject toSpawn = null;

                switch (map[x, z])
                {
                    case TileType.Road:
                        toSpawn = roadPrefab;
                        break;
                    case TileType.Water:
                        toSpawn = waterPrefab;
                        break;
                    case TileType.Object:
                        // choose random prefab from objectPrefabs
                        if (objectPrefabs.Count > 0)
                        {
                            int i = Random.Range(0, objectPrefabs.Count);
                            toSpawn = objectPrefabs[i];
                        }
                        break;
                    case TileType.Empty:
                    default:
                        toSpawn = null;
                        break;
                }

                if (toSpawn != null)
                {
                    GameObject go = Instantiate(toSpawn, pos, Quaternion.identity, root);
                    // optional: adjust scale to tileSize
                    // go.transform.localScale = new Vector3(tileSize, go.transform.localScale.y, tileSize);
                    // slight random rotation for objects (avoid rotating road/water)
                    if (map[x, z] == TileType.Object)
                    {
                        go.transform.Rotate(0f, Random.Range(0, 4) * 90f, 0f);
                    }
                }
            }
        }
    }

    bool InBounds(int x, int z) => x >= 0 && z >= 0 && x < gridWidth && z < gridHeight;

    Vector2Int RandomDirection()
    {
        int r = Random.Range(0, 4);
        switch (r)
        {
            case 0: return new Vector2Int(1, 0);
            case 1: return new Vector2Int(-1, 0);
            case 2: return new Vector2Int(0, 1);
            default: return new Vector2Int(0, -1);
        }
    }
}
