using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// RPG 사각타일 던전 자동 생성기.
///
/// ▶ Inspector 설정 방법
///   1. Floor Prefabs / Wall Prefabs 배열에 원하는 프리펩을 여러 개 등록
///   2. 슬라이더로 방 개수·로비 비율·통로 폭·루프 비율 등 조정
///   3. Play 시 자동 생성, 또는 코드에서 Generate() 직접 호출
///
/// ▶ 생성 후 프로그래머가 사용할 주요 멤버
///   AllRooms            - 생성된 모든 방 목록
///   EntranceRoom        - 입구방
///   DeepestRoom         - 입구에서 가장 먼 방 (보스방 후보)
///   GetRoomsByType()    - 타입별 방 조회
///   GetRoomsSortedByDepth() - 깊이 순 정렬 목록
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    // ═══════════════════════════════════════════════════════════════
    // Inspector 노출 설정값
    // ═══════════════════════════════════════════════════════════════

    [Header("프리펩 — 여러 개 등록 시 랜덤 선택")]
    [Tooltip("바닥 타일 프리펩 목록 (1개 이상 필수)")]
    public GameObject[] FloorPrefabs;

    [Tooltip("벽/기둥 프리펩 목록 (1개 이상 필수)")]
    public GameObject[] WallPrefabs;

    [Header("던전 크기")]
    [Tooltip("그리드 전체 열 수")]
    [Range(40, 120)] public int GridWidth = 80;

    [Tooltip("그리드 전체 행 수")]
    [Range(30, 90)]  public int GridHeight = 60;

    [Tooltip("셀 1개가 Unity 월드에서 차지하는 크기 (m)")]
    public float CellSize = 1f;

    [Header("방 생성 파라미터")]
    [Range(6, 40)]   public int RoomCount = 18;
    [Range(0f, 0.4f)] public float LobbyRatio = 0.2f;
    [Range(0f, 0.5f)] public float ExtraConnectionRatio = 0.2f;
    [Range(1, 4)]    public int CorridorWidth = 2;

    [Header("방 크기 범위")]
    public Vector2Int NormalRoomMinSize = new Vector2Int(4, 4);
    public Vector2Int NormalRoomMaxSize = new Vector2Int(9, 8);
    public Vector2Int LobbyMinSize      = new Vector2Int(10, 8);
    public Vector2Int LobbyMaxSize      = new Vector2Int(16, 14);

    [Header("특수방 확률 (0~1)")]
    [Range(0f, 0.3f)] public float SecretRoomChance = 0.12f;
    [Range(0f, 0.3f)] public float ShopRoomChance   = 0.10f;
    [Range(0f, 0.3f)] public float TrapRoomChance   = 0.10f;

    [Header("씬 정리")]
    [Tooltip("Generate() 호출 시 기존 던전을 먼저 삭제할지 여부")]
    public bool ClearOnGenerate = true;

    // ═══════════════════════════════════════════════════════════════
    // 공개 결과 멤버 — 생성 후 외부에서 읽기
    // ═══════════════════════════════════════════════════════════════

    /// <summary>생성된 모든 방 목록 (ID 순)</summary>
    public List<DungeonRoom> AllRooms { get; private set; } = new List<DungeonRoom>();

    /// <summary>입구방 (Entrance 타입)</summary>
    public DungeonRoom EntranceRoom { get; private set; }

    /// <summary>입구에서 BFS 깊이가 가장 깊은 방</summary>
    public DungeonRoom DeepestRoom { get; private set; }

    /// <summary>보스방 목록</summary>
    public List<DungeonRoom> BossRooms { get; private set; } = new List<DungeonRoom>();

    /// <summary>비밀방 목록</summary>
    public List<DungeonRoom> SecretRooms { get; private set; } = new List<DungeonRoom>();

    /// <summary>로비방 목록</summary>
    public List<DungeonRoom> LobbyRooms { get; private set; } = new List<DungeonRoom>();

    /// <summary>생성 당시 사용한 랜덤 시드 (재현용)</summary>
    public int LastSeed { get; private set; }

    // ═══════════════════════════════════════════════════════════════
    // 내부 그리드 & 컨테이너
    // ═══════════════════════════════════════════════════════════════

    private enum CellType { Wall, Floor, Corridor }
    private CellType[,] _grid;
    private GameObject  _dungeonRoot;
    private int         _maxSecrets;

    // ═══════════════════════════════════════════════════════════════
    // Unity 진입점
    // ═══════════════════════════════════════════════════════════════

    private void Start() => Generate();

    // ═══════════════════════════════════════════════════════════════
    // 공개 API
    // ═══════════════════════════════════════════════════════════════

    /// <summary>씨드를 지정해 던전을 재현 가능하게 생성</summary>
    public void GenerateWithSeed(int seed)
    {
        Random.InitState(seed);
        LastSeed = seed;
        RunGeneration();
    }

    /// <summary>랜덤 씨드로 던전 생성</summary>
    public void Generate()
    {
        LastSeed = Random.Range(0, int.MaxValue);
        Random.InitState(LastSeed);
        RunGeneration();
    }

    /// <summary>특정 타입의 방 목록 반환</summary>
    public List<DungeonRoom> GetRoomsByType(RoomType type)
        => AllRooms.Where(r => r.Type == type).ToList();

    /// <summary>깊이(BFS) 오름차순으로 정렬된 방 목록</summary>
    public List<DungeonRoom> GetRoomsSortedByDepth()
        => AllRooms.OrderBy(r => r.DepthFromEntrance).ToList();

    /// <summary>입구에서 월드 거리 오름차순으로 정렬된 방 목록</summary>
    public List<DungeonRoom> GetRoomsSortedByDistance()
        => AllRooms.OrderBy(r => r.WorldDistanceFromEntrance).ToList();

    // ═══════════════════════════════════════════════════════════════
    // 생성 파이프라인
    // ═══════════════════════════════════════════════════════════════

    private void RunGeneration()
    {
        if (ClearOnGenerate) ClearDungeon();

        // 유효성 검사
        if (FloorPrefabs == null || FloorPrefabs.Length == 0)
        { Debug.LogError("[DungeonGenerator] FloorPrefabs가 비어있습니다."); return; }
        if (WallPrefabs == null || WallPrefabs.Length == 0)
        { Debug.LogError("[DungeonGenerator] WallPrefabs가 비어있습니다."); return; }

        _grid     = new CellType[GridWidth, GridHeight];
        AllRooms  = new List<DungeonRoom>();
        BossRooms = new List<DungeonRoom>();
        SecretRooms = new List<DungeonRoom>();
        LobbyRooms  = new List<DungeonRoom>();
        _maxSecrets = Mathf.Max(1, RoomCount / 6);

        _dungeonRoot = new GameObject("Dungeon");
        _dungeonRoot.transform.SetParent(transform);

        // ── Step 1: 방 배치 ──────────────────────────────────────
        List<DungeonRoom> placed = PlaceRooms();
        if (placed.Count < 2)
        { Debug.LogWarning("[DungeonGenerator] 방이 너무 적습니다. GridWidth/Height 또는 RoomCount를 조정하세요."); return; }

        // ── Step 2: 타입 결정 ─────────────────────────────────────
        AssignRoomTypes(placed);

        // ── Step 3: MST + 루프 연결 ───────────────────────────────
        ConnectRooms(placed);

        // ── Step 4: 통로 그리기 ───────────────────────────────────
        DrawCorridors(placed);

        // ── Step 5: 타일 스폰 ─────────────────────────────────────
        SpawnTiles(placed);

        // ── Step 6: BFS 깊이 계산 ─────────────────────────────────
        ComputeDepths();

        // ── Step 7: 분류 리스트 채우기 ────────────────────────────
        CategorizeRooms();

        // 결과 로그
        LogSummary();
    }

    // ───────────────────────────────────────────────────────────────
    // Step 1: 방 배치 (겹침 없이 랜덤 배치)
    // ───────────────────────────────────────────────────────────────
    private List<DungeonRoom> PlaceRooms()
    {
        var placed = new List<DungeonRoom>();
        int attempts = 0;
        const int maxAttempts = 1000;

        while (placed.Count < RoomCount && attempts < maxAttempts)
        {
            attempts++;
            bool isLobby = placed.Count > 0 && Random.value < LobbyRatio;

            Vector2Int minSz = isLobby ? LobbyMinSize    : NormalRoomMinSize;
            Vector2Int maxSz = isLobby ? LobbyMaxSize    : NormalRoomMaxSize;

            int rw = Random.Range(minSz.x, maxSz.x + 1);
            int rh = Random.Range(minSz.y, maxSz.y + 1);

            if (rw + 4 >= GridWidth || rh + 4 >= GridHeight) continue;

            int rx = Random.Range(2, GridWidth  - rw - 2);
            int ry = Random.Range(2, GridHeight - rh - 2);

            // 겹침 검사 (마진 2)
            bool overlap = placed.Any(r =>
                rx < r.GridPosition.x + r.GridSize.x + 2 && rx + rw + 2 > r.GridPosition.x &&
                ry < r.GridPosition.y + r.GridSize.y + 2 && ry + rh + 2 > r.GridPosition.y);
            if (overlap) continue;

            var room = new DungeonRoom
            {
                Id           = placed.Count,
                GridPosition = new Vector2Int(rx, ry),
                GridSize     = new Vector2Int(rw, rh),
                WorldCenter  = GridToWorld(rx + rw * 0.5f, ry + rh * 0.5f),
            };

            // 그리드에 바닥 셀 표시
            for (int x = rx; x < rx + rw; x++)
                for (int y = ry; y < ry + rh; y++)
                    _grid[x, y] = CellType.Floor;

            placed.Add(room);
        }

        return placed;
    }

    // ───────────────────────────────────────────────────────────────
    // Step 2: 방 타입 결정
    // ───────────────────────────────────────────────────────────────
    private void AssignRoomTypes(List<DungeonRoom> rooms)
    {
        int secretCount = 0;
        bool shopPlaced = false;
        int bossThreshold = Mathf.RoundToInt(rooms.Count * 0.7f);

        for (int i = 0; i < rooms.Count; i++)
        {
            var r = rooms[i];
            if (i == 0)
            {
                r.Type = RoomType.Entrance;
            }
            else if (r.IsLobby)
            {
                r.Type = RoomType.Lobby;
            }
            else if (i >= bossThreshold && i == rooms.Count - 1)
            {
                r.Type = RoomType.Boss;
            }
            else if (secretCount < _maxSecrets && Random.value < SecretRoomChance)
            {
                r.Type = RoomType.Secret;
                secretCount++;
            }
            else if (!shopPlaced && Random.value < ShopRoomChance)
            {
                r.Type = RoomType.Shop;
                shopPlaced = true;
            }
            else if (Random.value < TrapRoomChance)
            {
                r.Type = RoomType.Trap;
            }
            else
            {
                r.Type = RoomType.Normal;
            }
        }
    }

    // ───────────────────────────────────────────────────────────────
    // Step 3: MST + 추가 루프 연결
    // ───────────────────────────────────────────────────────────────
    private void ConnectRooms(List<DungeonRoom> rooms)
    {
        // 모든 방 쌍 거리 계산
        var edges = new List<(float dist, int a, int b)>();
        for (int i = 0; i < rooms.Count; i++)
            for (int j = i + 1; j < rooms.Count; j++)
            {
                float d = Vector2.Distance(rooms[i].GridCenter, rooms[j].GridCenter);
                edges.Add((d, i, j));
            }
        edges.Sort((a, b) => a.dist.CompareTo(b.dist));

        // Kruskal MST
        int[] parent = Enumerable.Range(0, rooms.Count).ToArray();
        System.Func<int, int> find = null;
        find = x => parent[x] == x ? x : parent[x] = find(parent[x]);

        var mst    = new List<(int a, int b)>();
        var nonMst = new List<(int a, int b)>();

        foreach (var (_, a, b) in edges)
        {
            if (find(a) != find(b))
            {
                parent[find(a)] = find(b);
                mst.Add((a, b));
            }
            else
            {
                nonMst.Add((a, b));
            }
        }

        // 루프 추가
        int extra = Mathf.RoundToInt(nonMst.Count * ExtraConnectionRatio);
        var connections = new List<(int a, int b)>(mst);
        for (int i = 0; i < extra && i < nonMst.Count; i++)
            connections.Add(nonMst[i]);

        // ConnectedRooms 채우기
        foreach (var (a, b) in connections)
        {
            if (!rooms[a].ConnectedRooms.Contains(rooms[b]))
                rooms[a].ConnectedRooms.Add(rooms[b]);
            if (!rooms[b].ConnectedRooms.Contains(rooms[a]))
                rooms[b].ConnectedRooms.Add(rooms[a]);
        }

        AllRooms.AddRange(rooms);
    }

    // ───────────────────────────────────────────────────────────────
    // Step 4: 통로 그리기 (L자 경로)
    // ───────────────────────────────────────────────────────────────
    private void DrawCorridors(List<DungeonRoom> rooms)
    {
        var drawn = new HashSet<(int, int)>();

        foreach (var room in rooms)
        {
            foreach (var neighbor in room.ConnectedRooms)
            {
                int key1 = room.Id * 1000 + neighbor.Id;
                int key2 = neighbor.Id * 1000 + room.Id;
                if (drawn.Contains((key1, key2))) continue;
                drawn.Add((key1, key2));
                drawn.Add((key2, key1));

                // 중심 셀 좌표
                int ax = Mathf.RoundToInt(room.GridCenter.x);
                int ay = Mathf.RoundToInt(room.GridCenter.y);
                int bx = Mathf.RoundToInt(neighbor.GridCenter.x);
                int by = Mathf.RoundToInt(neighbor.GridCenter.y);

                int half = CorridorWidth / 2;

                if (Random.value < 0.5f)
                {
                    // 수평 먼저
                    for (int x = Mathf.Min(ax, bx); x <= Mathf.Max(ax, bx); x++)
                        for (int d = -half; d <= half; d++)
                            SetCorridor(x, ay + d);
                    for (int y = Mathf.Min(ay, by); y <= Mathf.Max(ay, by); y++)
                        for (int d = -half; d <= half; d++)
                            SetCorridor(bx + d, y);
                }
                else
                {
                    // 수직 먼저
                    for (int y = Mathf.Min(ay, by); y <= Mathf.Max(ay, by); y++)
                        for (int d = -half; d <= half; d++)
                            SetCorridor(ax + d, y);
                    for (int x = Mathf.Min(ax, bx); x <= Mathf.Max(ax, bx); x++)
                        for (int d = -half; d <= half; d++)
                            SetCorridor(x, by + d);
                }
            }
        }
    }

    private void SetCorridor(int x, int y)
    {
        if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight) return;
        if (_grid[x, y] == CellType.Wall)
            _grid[x, y] = CellType.Corridor;
    }

    // ───────────────────────────────────────────────────────────────
    // Step 5: 타일 스폰
    // ───────────────────────────────────────────────────────────────
    private void SpawnTiles(List<DungeonRoom> rooms)
    {
        // 방 → 룸 맵 (빠른 역참조)
        var cellToRoom = new Dictionary<Vector2Int, DungeonRoom>();
        foreach (var r in rooms)
            for (int x = r.GridPosition.x; x < r.GridPosition.x + r.GridSize.x; x++)
                for (int y = r.GridPosition.y; y < r.GridPosition.y + r.GridSize.y; y++)
                    cellToRoom[new Vector2Int(x, y)] = r;

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var cell = _grid[x, y];
                if (cell == CellType.Wall) continue;

                Vector3 pos = GridToWorld(x, y);

                if (cell == CellType.Floor || cell == CellType.Corridor)
                {
                    // 바닥 타일 스폰
                    GameObject floorPrefab = FloorPrefabs[Random.Range(0, FloorPrefabs.Length)];
                    GameObject floorObj = Instantiate(floorPrefab, pos, Quaternion.identity, _dungeonRoot.transform);
                    floorObj.name = $"Floor_{x}_{y}";

                    // 방에 등록
                    if (cellToRoom.TryGetValue(new Vector2Int(x, y), out var ownerRoom))
                        ownerRoom.FloorTiles.Add(floorObj);

                    // 테두리 벽 체크 (4방향)
                    SpawnWallsAround(x, y, ownerRoom);
                }
            }
        }

        // Room GameObject 생성 & 연결
        foreach (var r in rooms)
        {
            r.RoomObject = new GameObject($"Room_{r.Id}_{r.Type}");
            r.RoomObject.transform.SetParent(_dungeonRoot.transform);
            r.RoomObject.transform.position = r.WorldCenter;

            foreach (var t in r.FloorTiles) t.transform.SetParent(r.RoomObject.transform);
            foreach (var t in r.WallTiles)  t.transform.SetParent(r.RoomObject.transform);
        }
    }

    private void SpawnWallsAround(int cx, int cy, DungeonRoom ownerRoom)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector3[] rotations =
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 270, 0),
            new Vector3(0, 90, 0),
        };

        for (int d = 0; d < dirs.Length; d++)
        {
            int nx = cx + dirs[d].x;
            int ny = cy + dirs[d].y;
            bool outOfBounds = (nx < 0 || nx >= GridWidth || ny < 0 || ny >= GridHeight);
            bool isWall = outOfBounds || _grid[nx, ny] == CellType.Wall;

            if (!isWall) continue;

            Vector3 wallPos = GridToWorld(cx, cy);
            // 벽을 셀 경계에 배치 (CellSize * 0.5 만큼 이동)
            wallPos += new Vector3(dirs[d].x, 0, dirs[d].y) * (CellSize * 0.5f);

            GameObject wallPrefab = WallPrefabs[Random.Range(0, WallPrefabs.Length)];
            GameObject wallObj = Instantiate(
                wallPrefab, wallPos,
                Quaternion.Euler(rotations[d]),
                _dungeonRoot.transform
            );
            wallObj.name = $"Wall_{cx}_{cy}_{dirs[d]}";

            if (ownerRoom != null)
                ownerRoom.WallTiles.Add(wallObj);
        }
    }

    // ───────────────────────────────────────────────────────────────
    // Step 6: BFS 깊이 계산
    // ───────────────────────────────────────────────────────────────
    private void ComputeDepths()
    {
        EntranceRoom = AllRooms.FirstOrDefault(r => r.Type == RoomType.Entrance);
        if (EntranceRoom == null)
        {
            Debug.LogWarning("[DungeonGenerator] 입구방을 찾지 못했습니다.");
            return;
        }

        // 모든 방 초기화
        foreach (var r in AllRooms)
        {
            r.DepthFromEntrance          = -1;
            r.WorldDistanceFromEntrance  = -1f;
        }

        // BFS
        var queue   = new Queue<DungeonRoom>();
        EntranceRoom.DepthFromEntrance         = 0;
        EntranceRoom.WorldDistanceFromEntrance = 0f;
        queue.Enqueue(EntranceRoom);

        int maxDepth = 0;
        DeepestRoom  = EntranceRoom;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var neighbor in current.ConnectedRooms)
            {
                if (neighbor.DepthFromEntrance != -1) continue;
                neighbor.DepthFromEntrance         = current.DepthFromEntrance + 1;
                neighbor.WorldDistanceFromEntrance = Vector3.Distance(
                    EntranceRoom.WorldCenter, neighbor.WorldCenter);

                if (neighbor.DepthFromEntrance > maxDepth)
                {
                    maxDepth    = neighbor.DepthFromEntrance;
                    DeepestRoom = neighbor;
                }
                queue.Enqueue(neighbor);
            }
        }
    }

    // ───────────────────────────────────────────────────────────────
    // Step 7: 분류 리스트 채우기
    // ───────────────────────────────────────────────────────────────
    private void CategorizeRooms()
    {
        BossRooms   = GetRoomsByType(RoomType.Boss);
        SecretRooms = GetRoomsByType(RoomType.Secret);
        LobbyRooms  = GetRoomsByType(RoomType.Lobby);
    }

    // ═══════════════════════════════════════════════════════════════
    // 유틸리티
    // ═══════════════════════════════════════════════════════════════

    /// <summary>그리드 좌표 → Unity 월드 좌표 (XZ 평면)</summary>
    public Vector3 GridToWorld(float col, float row)
        => new Vector3(col * CellSize, 0f, row * CellSize) + transform.position;

    public Vector2Int WorldToGrid(Vector3 world)
    {
        Vector3 local = world - transform.position;
        return new Vector2Int(Mathf.RoundToInt(local.x / CellSize),
                              Mathf.RoundToInt(local.z / CellSize));
    }

    /// <summary>기존 던전 오브젝트 전체 삭제</summary>
    public void ClearDungeon()
    {
        if (_dungeonRoot != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(_dungeonRoot);
#else
            Destroy(_dungeonRoot);
#endif
        }
        AllRooms.Clear();
        BossRooms.Clear();
        SecretRooms.Clear();
        LobbyRooms.Clear();
        EntranceRoom = null;
        DeepestRoom  = null;
    }

    // ═══════════════════════════════════════════════════════════════
    // 디버그
    // ═══════════════════════════════════════════════════════════════

    private void LogSummary()
    {
        Debug.Log($"[DungeonGenerator] 생성 완료 — Seed:{LastSeed} | 방:{AllRooms.Count} " +
                  $"| 입구:{EntranceRoom?.Id} | 최심방:{DeepestRoom?.Id}(depth={DeepestRoom?.DepthFromEntrance})");
        foreach (var r in AllRooms)
            Debug.Log(r.ToString());
    }

    private void OnDrawGizmosSelected()
    {
        if (AllRooms == null) return;

        foreach (var r in AllRooms)
        {
            // 방 AABB
            Gizmos.color = r.Type switch
            {
                RoomType.Entrance => Color.cyan,
                RoomType.Boss     => Color.red,
                RoomType.Secret   => Color.green,
                RoomType.Shop     => Color.yellow,
                RoomType.Lobby    => new Color(0.6f, 0.4f, 1f),
                RoomType.Trap     => new Color(1f, 0.4f, 0f),
                _                 => Color.white,
            };

            Vector3 center = r.WorldCenter + Vector3.up * 0.5f;
            Vector3 size   = new Vector3(r.GridSize.x * CellSize, 1f, r.GridSize.y * CellSize);
            Gizmos.DrawWireCube(center, size);

            // 연결선
            Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
            foreach (var nb in r.ConnectedRooms)
                Gizmos.DrawLine(r.WorldCenter + Vector3.up * 0.5f,
                                nb.WorldCenter + Vector3.up * 0.5f);
        }

        // 최심방 강조
        if (DeepestRoom != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(DeepestRoom.WorldCenter + Vector3.up, 1f);
        }
    }
}
