using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DungeonGenerator 사용 예시 — 실제 게임 로직에서 던전 정보를 읽는 방법을 보여줍니다.
///
/// 같은 GameObject에 DungeonGenerator와 함께 붙여서 사용하세요.
/// </summary>
[RequireComponent(typeof(DungeonGenerator))]
public class DungeonGeneratorExample : MonoBehaviour
{
    private DungeonGenerator _gen;

    private void Awake()
    {
        _gen = GetComponent<DungeonGenerator>();
    }

    /// <summary>
    /// DungeonGenerator.Start() 이후에 호출해야 합니다.
    /// 실제 프로젝트에서는 Start() 대신 이벤트/콜백으로 연결하세요.
    /// </summary>
    private void Start()
    {
        // DungeonGenerator.Start()보다 늦게 실행되도록 Script Execution Order를
        // DungeonGenerator보다 높은 숫자로 설정하거나, Invoke를 사용하세요.
        Invoke(nameof(ReadDungeonInfo), 0.1f);
    }

    private void ReadDungeonInfo()
    {
        // ─────────────────────────────────────────
        // 1. 기본 통계
        // ─────────────────────────────────────────
        Debug.Log($"=== 던전 생성 결과 (Seed: {_gen.LastSeed}) ===");
        Debug.Log($"전체 방 수: {_gen.AllRooms.Count}");

        // ─────────────────────────────────────────
        // 2. 입구방 정보
        // ─────────────────────────────────────────
        DungeonRoom entrance = _gen.EntranceRoom;
        if (entrance != null)
        {
            Debug.Log($"[입구방] ID={entrance.Id} " +
                      $"위치={entrance.GridPosition} 크기={entrance.GridSize} " +
                      $"월드좌표={entrance.WorldCenter}");
        }

        // ─────────────────────────────────────────
        // 3. 가장 깊은 방 (보스방 배치 등에 활용)
        // ─────────────────────────────────────────
        DungeonRoom deepest = _gen.DeepestRoom;
        if (deepest != null)
        {
            Debug.Log($"[최심방] ID={deepest.Id} Type={deepest.Type} " +
                      $"BFS깊이={deepest.DepthFromEntrance} " +
                      $"월드거리={deepest.WorldDistanceFromEntrance:F1}m");
        }

        // ─────────────────────────────────────────
        // 4. 타입별 방 조회
        // ─────────────────────────────────────────
        List<DungeonRoom> bossRooms   = _gen.BossRooms;
        List<DungeonRoom> secretRooms = _gen.SecretRooms;
        List<DungeonRoom> lobbyRooms  = _gen.LobbyRooms;

        Debug.Log($"보스방 {bossRooms.Count}개 / 비밀방 {secretRooms.Count}개 / 로비 {lobbyRooms.Count}개");

        foreach (var boss in bossRooms)
            Debug.Log($"  ▶ 보스방 ID={boss.Id} 깊이={boss.DepthFromEntrance}");

        // ─────────────────────────────────────────
        // 5. 전체 방 깊이 순 순회
        // ─────────────────────────────────────────
        Debug.Log("--- 깊이 순 방 목록 ---");
        foreach (var room in _gen.GetRoomsSortedByDepth())
        {
            Debug.Log($"  Depth {room.DepthFromEntrance,2} | ID {room.Id,2} | {room.Type,-8} " +
                      $"| 연결={room.ConnectedRooms.Count} | 면적={room.Area}셀 " +
                      $"| 거리={room.WorldDistanceFromEntrance:F1}m");
        }

        // ─────────────────────────────────────────
        // 6. 플레이어 스폰 — 입구 월드 좌표 사용
        // ─────────────────────────────────────────
        if (entrance != null)
        {
            Vector3 spawnPos = entrance.WorldCenter + Vector3.up; // 바닥 위 1m
            Debug.Log($"플레이어 스폰 위치: {spawnPos}");
            // Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        }

        // ─────────────────────────────────────────
        // 7. 비밀방 — 연결된 방 없이 입장 루트 숨기기 예시
        // ─────────────────────────────────────────
        foreach (var secret in secretRooms)
        {
            Debug.Log($"비밀방 ID={secret.Id} | 연결방: " +
                      string.Join(", ", secret.ConnectedRooms.ConvertAll(r => r.Id.ToString())));
            // 여기서 비밀방 RoomObject에 커스텀 컴포넌트 추가 등 처리 가능
        }

        // ─────────────────────────────────────────
        // 8. 씬 오브젝트 참조 — 방별 바닥/벽 타일 접근
        // ─────────────────────────────────────────
        foreach (var room in _gen.AllRooms)
        {
            // 예: 보스방 바닥 전체를 다른 머티리얼로 교체
            if (room.Type == RoomType.Boss)
            {
                foreach (var tile in room.FloorTiles)
                {
                    // Renderer rend = tile.GetComponent<Renderer>();
                    // if (rend != null) rend.material = bossMaterial;
                }
            }
        }

        // ─────────────────────────────────────────
        // 9. 특정 방 인접 정보
        // ─────────────────────────────────────────
        if (_gen.AllRooms.Count > 0)
        {
            var sampleRoom = _gen.AllRooms[0];
            Debug.Log($"방 {sampleRoom.Id}의 인접 방: " +
                      string.Join(", ", sampleRoom.ConnectedRooms.ConvertAll(r =>
                          $"ID{r.Id}({r.Type})")));
        }
    }

    // ─────────────────────────────────────────
    // 런타임 재생성 예시 (UI 버튼 연결용)
    // ─────────────────────────────────────────
    public void OnRegenerateButtonClicked()
    {
        _gen.Generate();
        ReadDungeonInfo();
    }

    public void OnRegenerateWithSeedClicked(int seed)
    {
        _gen.GenerateWithSeed(seed);
        ReadDungeonInfo();
    }
}
