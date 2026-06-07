using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GridRoadGenerator : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float tileSize = 1f;

    [Range(0f, 1f)]
    public float roadDensity = 0.12f; // 전체 타일 중 도로가 될 비율(대략)
    public int randomWalkLengthMin = 8;
    public int randomWalkLengthMax = 40;
    public int randomWalkSeeds = 10; // roadDensity 기반으로 자동조정도 가능

    [Header("Road Prefabs (base orientations)")]
    public GameObject prefabRoadStraight; // 직선(베이스: X방향)
    public GameObject prefabRoadCorner;   // 코너(베이스: 연결 N->E 예)
    public GameObject prefabRoadT;        // T자
    public GameObject prefabRoadCross;    // + 교차
    public GameObject prefabRoadEnd;      // 막다른 길(끝) - 베이스: 연결 X쪽

    [Header("Other objects")]
    public GameObject[] otherPrefabs; // 빈칸에 배치할 오브젝트들
    [Range(0f, 1f)]
    public float otherFillRatio = 0.6f; // 빈칸 중 몇%를 오브젝트로 채울지

    [Header("Runtime / Cleanup")]
    public bool clearChildrenBeforeGenerate = true;
    public int seed = 0; // 0이면 랜덤

    public int road_gap_size_min = 4;
    public int road_gap_size_max = 9;


    // internal grid
    private bool[,] isRoad;
    private System.Random rng;

    // Directions: N(0,1), E(1,0), S(0,-1), W(-1,0)
    private readonly Vector2Int[] dirs = new Vector2Int[]
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0)
    };

    void Start()
    {
        // 자동 실행 원하면 여기서 호출
        // Generate();
    }

    // Public call to (re)generate the map
    [ContextMenu("생성")]
    public void Generate()
    {
        if (seed == 0) rng = new System.Random();
        else rng = new System.Random(seed);

        if (clearChildrenBeforeGenerate) ClearChildren();

        // 새로운 격자형 도로 생성
        GenerateGridStyleRoads();

        // Instantiate 타일
        for (int x=0; x<gridWidth; x++)
        {
            for (int z=0; z<gridHeight; z++)
            {
                Vector3 worldPos = GridToWorld(x,z);

                if (isRoad[x,z])
                {
                    int mask = NeighborMask(x,z);
                    InstantiateRoadByMask(mask, worldPos);
                }
                else
                {
                    // 빈 공간엔 오브젝트
                    if (otherPrefabs != null && otherPrefabs.Length > 0 && rng.NextDouble() < otherFillRatio)
                    {
                        GameObject prefab = otherPrefabs[rng.Next(0, otherPrefabs.Length)];
                        Instantiate(prefab, worldPos, Quaternion.identity, transform);
                    }
                }
            }
        }
    }


    private void CreateSomeRandomConnections(int count)
    {
        // simple: carve straight lines between random road tiles to make more intersections
        for (int i = 0; i < count; i++)
        {
            int ax = rng.Next(0, gridWidth), az = rng.Next(0, gridHeight);
            int bx = rng.Next(0, gridWidth), bz = rng.Next(0, gridHeight);
            // carve an L-shaped corridor between (ax,az) and (bx,bz)
            int x = ax, z = az;
            while (x != bx)
            {
                if (!isRoad[x, z]) isRoad[x, z] = true;
                x += (bx > x) ? 1 : -1;
            }
            while (z != bz)
            {
                if (!isRoad[x, z]) isRoad[x, z] = true;
                z += (bz > z) ? 1 : -1;
            }
        }
    }

    private Vector3 GridToWorld(int x, int z)
    {
        float wx = x * tileSize;
        float wz = z * tileSize;
        return new Vector3(wx, 0f, wz) + transform.position;
    }

    private int NeighborMask(int x, int z)
    {
        // bitmask: N=1, E=2, S=4, W=8
        int mask = 0;
        if (IsRoadAt(x, z + 1)) mask |= 1;
        if (IsRoadAt(x + 1, z)) mask |= 2;
        if (IsRoadAt(x, z - 1)) mask |= 4;
        if (IsRoadAt(x - 1, z)) mask |= 8;
        return mask;
    }

    private bool IsRoadAt(int x, int z)
    {
        if (x < 0 || x >= gridWidth || z < 0 || z >= gridHeight) return false;
        return isRoad[x, z];
    }

    private void InstantiateRoadByMask(int mask, Vector3 pos)
    {
        // Decide which base prefab to use and its rotation (y-angle)
        GameObject toSpawn = null;
        Quaternion rot = Quaternion.identity;

        // count connections
        int connections = CountBits(mask);

        if (connections == 4)
        {
            toSpawn = prefabRoadCross;
            rot = Quaternion.identity;
        }
        else if (connections == 3)
        {
            toSpawn = prefabRoadT;
            // T rotation: we want the missing direction to face 'back' of T
            // missing bit -> rotation
            // mask bits: N=1, E=2, S=4, W=8
            if ((mask & 1) == 0) rot = Quaternion.Euler(0, 180f, 0); // no N -> T opens S,E,W -> rotate 180
            else if ((mask & 2) == 0) rot = Quaternion.Euler(0, -90f, 0);
            else if ((mask & 4) == 0) rot = Quaternion.Euler(0, 0f, 0);
            else if ((mask & 8) == 0) rot = Quaternion.Euler(0, 90f, 0);
        }
        else if (connections == 2)
        {
            // Could be straight or corner
            bool n = (mask & 1) != 0;
            bool e = (mask & 2) != 0;
            bool s = (mask & 4) != 0;
            bool w = (mask & 8) != 0;

            if ((n && s) && !e && !w)
            {
                // straight along Z (north-south)
                toSpawn = prefabRoadStraight;
                rot = Quaternion.Euler(0, 90f, 0); // base straight assumed X-direction; rotate 90 for Z
            }
            else if ((e && w) && !n && !s)
            {
                // straight along X (east-west)
                toSpawn = prefabRoadStraight;
                rot = Quaternion.Euler(0, 0f, 0);
            }
            else
            {
                // corner (two adjacent directions)
                toSpawn = prefabRoadCorner;
                // determine rotation: assume base corner connects N->E (north and east) at rotation 0
                if (n && e) rot = Quaternion.Euler(0, 0f, 0);
                else if (e && s) rot = Quaternion.Euler(0, 90f, 0);
                else if (s && w) rot = Quaternion.Euler(0, 180f, 0);
                else if (w && n) rot = Quaternion.Euler(0, -90f, 0);
                else
                {
                    // fallback: if diagonal-ish (shouldn't happen), treat as straight
                    toSpawn = prefabRoadStraight;
                    rot = Quaternion.identity;
                }
            }
        }
        else if (connections == 1)
        {
            // end piece (dead end)
            toSpawn = prefabRoadEnd;
            if ((mask & 1) != 0) rot = Quaternion.Euler(0, 0f, 0);    // connected north -> end faces north: rotate so connector points north
            else if ((mask & 2) != 0) rot = Quaternion.Euler(0, 90f, 0);
            else if ((mask & 4) != 0) rot = Quaternion.Euler(0, 180f, 0);
            else if ((mask & 8) != 0) rot = Quaternion.Euler(0, -90f, 0);
        }
        else // connections == 0
        {
            // isolated single tile -> treat as end or small block; place end with random rotation
            toSpawn = prefabRoadEnd ?? prefabRoadStraight ?? prefabRoadCorner;
            rot = Quaternion.Euler(0, 90f * rng.Next(0, 4), 0);
        }

        if (toSpawn != null)
        {
            Instantiate(toSpawn, pos, rot, transform);
        }
    }

    private int CountBits(int mask)
    {
        int c = 0;
        for (int i = 0; i < 4; i++) if (((mask >> i) & 1) == 1) c++;
        return c;
    }

    [ContextMenu("삭제")]
    public void ClearChildren()
    {
        // Destroy children in editor and play mode
        List<GameObject> children = new List<GameObject>();
        foreach (Transform t in transform)
        {
            children.Add(t.gameObject);
        }
        for (int i = 0; i < children.Count; i++)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(children[i]);
            else Destroy(children[i]);
#else
            Destroy(children[i]);
#endif
        }
    }

    // For quick debugging: draw grid in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 p = GridToWorld(x, z);
                Gizmos.DrawWireCube(p + new Vector3(tileSize / 2f, 0, tileSize / 2f), new Vector3(tileSize, 0.01f, tileSize));
            }
        }
    }
        
    private void GenerateGridStyleRoads()
    {
        isRoad = new bool[gridWidth, gridHeight];

        // ----------------------------
        // X축 도로 위치를 랜덤 간격으로 선택
        // ----------------------------
        List<int> xRoads = new List<int>();
        int x = 0;
        while (x < gridWidth)
        {
            xRoads.Add(x);
            int step = rng.Next(road_gap_size_min, road_gap_size_max); // X 도로 간격 [4~8] 사이
            x += step;
        }

        // ----------------------------
        // Z축 도로 위치를 랜덤 간격으로 선택
        // ----------------------------
        List<int> zRoads = new List<int>();
        int z = 0;
        while (z < gridHeight)
        {
            zRoads.Add(z);
            int step = rng.Next(road_gap_size_min, road_gap_size_max); // Z 도로 간격 [4~8] 사이
            z += step;
        }

        // ----------------------------
        // 도로 타일 표시
        // ----------------------------
        foreach (int rx in xRoads)
        {
            for (int zz = 0; zz < gridHeight; zz++)
            {
                isRoad[rx, zz] = true;
            }
        }
        foreach (int rz in zRoads)
        {
            for (int xx = 0; xx < gridWidth; xx++)
            {
                isRoad[xx, rz] = true;
            }
        }

        // ----------------------------
        // 변형(랜덤하게 끊기 or 코너 만들기)
        // ----------------------------
        int modifications = (gridWidth * gridHeight) / 20;
        for (int i = 0; i < modifications; i++)
        {
            int cx = rng.Next(1, gridWidth - 1);
            int cz = rng.Next(1, gridHeight - 1);

            if (isRoad[cx, cz])
            {
                double roll = rng.NextDouble();
                if (roll < 0.4)
                {
                    // 코너 유도: 임의의 방향 연결 끊기
                    int dir = rng.Next(0, 4);
                    Vector2Int d = dirs[dir];
                    int nx = cx + d.x, nz = cz + d.y;
                    if (nx >= 0 && nx < gridWidth && nz >= 0 && nz < gridHeight)
                    {
                        isRoad[nx, nz] = false;
                    }
                }
                else if (roll < 0.7)
                {
                    // 교차로 → T자로
                    int dir = rng.Next(0, 4);
                    Vector2Int d = dirs[dir];
                    int nx = cx + d.x, nz = cz + d.y;
                    if (nx >= 0 && nx < gridWidth && nz >= 0 && nz < gridHeight)
                    {
                        isRoad[nx, nz] = false;
                    }
                }
            }
        }
    }


}
