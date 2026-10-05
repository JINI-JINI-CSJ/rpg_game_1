using UnityEngine;

// 각종 정의들...
public class GTF_Define
{
    //=================================
    // 일반 
    //=================================

    // 월드 턴 시작

    // 월드 턴 종료

    // 마을 진입


    //=================================
    // 던전
    //=================================

    // 던전 입장
    public const string DUNGEON_FIRST_ENTER = "DUNGEON_FIRST_ENTER";
    
    // 던전 1칸 이동
    public const string DUNGEON_MOVE_ONE = "DUNGEON_MOVE_ONE";


    
    //=================================
    // 전투 
    //=================================

    // 전투 인트로
    public const string BATTLE_INTRO = "BATTLE_INTRO";

    // 전투 첫턴 처음 진입
    public const string BATTLE_START_FIRST_TURN = "BATTLE_START_FIRST_TURN";

    // 배틀 입력 가능 체크
    public const string BATTLE_DISABLE_CMD_INPUT = "BATTLE_DISABLE_CMD_INPUT";

    // 전사 특기 가능 여부 , 전용태그 
    public const string BATTLE_DISABLE_MENU_COMBAT_ARTS = "BATTLE_DISABLE_MENU_COMBAT_ARTS";

    // 마법사 마법 가능 여부
    public const string BATTLE_DISABLE_MENU_MAGIC = "BATTLE_DISABLE_MENU_MAGIC";

    // 행동 실행 여부 , 마비
    public const string BATTLE_DISABLE_EXEC_ACTION_START = "BATTLE_DISABLE_EXEC_ACTION_START";

    // 캐릭터 턴 행동 시작 , 밑에보다 더 이전 단계
    public const string BATTLE_CHR_ACTION_READY = "BATTLE_CHR_ACTION_READY";

    // 캐릭터 공격 시작
    public const string BATTLE_CHR_ACTION_START = "BATTLE_CHR_ACTION_START";

    // 캐릭터 피격 시작
    public const string BATTLE_CHR_DAMAGE_START = "BATTLE_CHR_DAMAGE_START";

    // 무조건 빗나가는 지
    public const string BATTLE_CHECK_HIT_MISS = "BATTLE_CHECK_HIT_MISS";
    
    // 전투 턴 종료
    public const string BATTLE_TURN_END = "BATTLE_TURN_END";
}
