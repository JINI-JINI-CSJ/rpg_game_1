using UnityEngine;

public class WORLD_POS
{
    public Vector2 pos;
    public object data;
}


// 캐릭터 스탯
public enum CHAR_STAT
{
    None = -1 ,
    HP ,
    MP , 
    ACTION_SPEED ,      // 행동속도   
    ATK_P ,
    DEF_P ,
    HIT_RATE_P ,        // 물리 명중률
    EVASION_RATE_P ,    // 물리 회피율
    ATK_M ,
    DEF_M , 
    // 마법 명중 회피는 일단 제외 , 무조건 맞는다.

    MAX ,

}

// 직업 큰 분류
// 공통(보통 적군) , 전사 , 마법사 , 지원가
public enum JOB_BASE
{
    Common = 0,
    FIGHTER , 
    WIZARD , 
    SUPPORTER ,
    MAX 
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

// 스킬 활성 타입
// 액티브 - 전투
// 액티브 - 탐험
// 패시브
public enum SKILL_ACTIVE_TYPE
{
    None = -1 ,     
    Active_BATTLE ,
    Active_NORMAL , 
    Passive ,
    ALL ,
}

public class GTF_Common 
{
    // 추가 점수 분배
    static public void MakeAddScore_ChrVal_EffSk( Mng_X128SS rd , int total , ref int sc_chrVal , ref int sc_effSk )
    {
        sc_chrVal = rd.NextInt( 0 , total+1 );
        sc_effSk = total - sc_chrVal;
    }

    static public CHAR_STAT Random_Char_Stat( Mng_X128SS rd )
    {
        return (CHAR_STAT)rd.NextInt( 0 , (int)CHAR_STAT.MAX );
    }

    // 복수 대상 별 위력
    // 인자 
    // 0 : 1인
    // 1 : 1라인
    // 2 : 전체
    static public int CountNum_SelectTarget(int type)
    {
        switch( type )
        {
            case 0: return 1;
            case 1: return 3;
            case 2: return 6;
        }
        return -1;
    }

    static public float PowRatio_SelectTarget( int type ){return 1.0f / CountNum_SelectTarget( type );}

    // static public int CountNum_SelectTarget_ToIdx( BATTLE_ACTION_TARGET btr )
    // {
    //     switch( btr )
    //     {
    //         case 
    //     }
    // }
}
