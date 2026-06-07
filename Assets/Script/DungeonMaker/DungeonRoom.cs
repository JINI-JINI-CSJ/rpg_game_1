using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 던전 방 타입
/// </summary>
public enum RoomType
{
    Entrance,   // 입구 (시작점)
    Normal,     // 일반 전투방
    Lobby,      // 넓은 로비
    Boss,       // 보스방
    Secret,     // 비밀방
    Shop,       // 상점
    Trap,       // 함정방
}

/// <summary>
/// 던전 내 하나의 방(Room)에 대한 모든 정보를 담는 데이터 클래스.
/// DungeonGenerator가 생성 후 채워주며, 프로그래머는 이 클래스를 통해
/// 방의 위치/크기/타입/연결/깊이 등을 조회할 수 있습니다.
/// </summary>
public class DungeonRoom
{
    // ─────────────────────────────────────────
    // 기본 정보
    // ─────────────────────────────────────────

    /// <summary>방 고유 ID (0부터 순서대로)</summary>
    public int Id;

    /// <summary>방 타입</summary>
    public RoomType Type;

    /// <summary>그리드 상 좌상단 셀 좌표 (열, 행)</summary>
    public Vector2Int GridPosition;

    /// <summary>그리드 상 방 크기 (너비, 높이) — 셀 단위</summary>
    public Vector2Int GridSize;

    /// <summary>방 중심의 그리드 좌표 (소수점 포함)</summary>
    public Vector2 GridCenter => new Vector2(GridPosition.x + GridSize.x * 0.5f,
                                             GridPosition.y + GridSize.y * 0.5f);

    /// <summary>월드 스페이스 중심 위치</summary>
    public Vector3 WorldCenter;

    /// <summary>방 면적 (셀 수)</summary>
    public int Area => GridSize.x * GridSize.y;

    /// <summary>로비 여부 (크기가 큰 방)</summary>
    public bool IsLobby => Type == RoomType.Lobby;

    // ─────────────────────────────────────────
    // 연결 / 그래프 정보
    // ─────────────────────────────────────────

    /// <summary>이 방에 직접 연결된 인접 방 목록</summary>
    public List<DungeonRoom> ConnectedRooms = new List<DungeonRoom>();

    /// <summary>
    /// 입구(Entrance)로부터 BFS 최단 거리 (방 단위).
    /// 입구 자신은 0. 도달 불가 시 -1.
    /// </summary>
    public int DepthFromEntrance;

    /// <summary>
    /// 입구로부터 월드 유클리드 거리 (Unity 단위).
    /// 정확한 물리적 거리가 필요할 때 사용.
    /// </summary>
    public float WorldDistanceFromEntrance;

    // ─────────────────────────────────────────
    // 씬 오브젝트 참조
    // ─────────────────────────────────────────

    /// <summary>이 방의 루트 GameObject</summary>
    public GameObject RoomObject;

    /// <summary>배치된 바닥 타일 오브젝트 목록</summary>
    public List<GameObject> FloorTiles = new List<GameObject>();

    /// <summary>배치된 벽 오브젝트 목록</summary>
    public List<GameObject> WallTiles = new List<GameObject>();

    // ─────────────────────────────────────────
    // 편의 메서드
    // ─────────────────────────────────────────

    /// <summary>그리드 기준 AABB 내 셀인지 확인</summary>
    public bool ContainsCell(int col, int row)
    {
        return col >= GridPosition.x && col < GridPosition.x + GridSize.x &&
               row >= GridPosition.y && row < GridPosition.y + GridSize.y;
    }

    public override string ToString()
    {
        return $"[Room {Id}] Type={Type} Pos={GridPosition} Size={GridSize} " +
               $"Depth={DepthFromEntrance} WorldDist={WorldDistanceFromEntrance:F1} " +
               $"Connections={ConnectedRooms.Count}";
    }
}
