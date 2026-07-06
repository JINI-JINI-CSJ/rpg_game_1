using UnityEngine;

public class WORLD_POS
{
    public Vector2 pos;
    public object data;
}


// 캐릭터 스탯
public enum CHAR_STAT
{
    None = 0 ,
    HP ,
    ACTION_SPEED ,      // 행동속도   
    ATK_P ,
    DEF_P ,
    HIT_RATE_P ,        // 물리 명중률
    EVASION_RATE_P ,    // 물리 회피율
    ATK_M ,
    DEF_M , 
    // 마법 명중 회피는 일단 제외 , 무조건 맞는다.
}

// 직업 큰 분류
// 공통(보통 적군) , 전사 , 마법사 , 지원가
public enum JOB_BASE
{
    Common = 0,
    WARRIOR , 
    WIZARD , 
    SUPPORTER
}

// PARTNER


// 장비 아이템 큰 분류
public enum EQ_ITEM_BASE
{
    None = 0, 
    WEAPON , 
    ARMOR , 
    ACCESSORIES , 
}

// 스킬 대 분류
// 자주 사용하는 타입
// -적군 공격
// -적군 디버프
// -
public enum SKILL_TYPE
{
    None = 0 ,      // 전용 클래스가 있는 경우
    ATK ,           // 
    DEBUFF_ENEMY , 
    BUFF_ALLIES ,   
    SUPPORT ,       
}

public class GTF_Common 
{

}
