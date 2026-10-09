using System.Collections.Generic;
using System.IO;
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

// 위력 , 타겟 , 추가 스킬
// 스킬 , 아이템 등의 기본 데이터
public class COMMON_MAKE_ACT_DATA
{
    //=======================
    // 저장 데이터
    // 처음 생성할때의 타입
    public int RANDOM_CNT_TYPE;
    public int CSV_ID; // 기본 csv , 스킬이나 아이템 , 실제 객체(SkillBase , ItemBase ) 에서 처리하자.
    public int DEF_ATK_TYPE;

    // 점수 
    // 점수만 기록하고 로딩때 계산
    public int SCORE_POW_RATIO;
    public int SCORE_ADD_SKILL;
    public int SCORE_MP;

    // 추가 스킬 아이디를 일단 여기에 놓자.
    public List<int> CSV_ID_SKILL_EFF = new();

    //=======================

    // 점수 세팅 및 랜덤 추가 스킬
    public void SetScore_RandomAddEff( Mng_X128SS rd , int score_pow_ratio , int score_add_skill , int score_mp , string tag_add_eff )
    {
        CSV_ID_SKILL_EFF.Clear();

        SCORE_POW_RATIO = score_pow_ratio;
        SCORE_ADD_SKILL = score_add_skill;
        SCORE_MP = score_mp;

        if( string.IsNullOrEmpty(tag_add_eff) == false )
        {
            // 태그 리스트 미리 가져와서 한개씩 팝업
            List<CSV_Skill> skills = GTF_CSV.csv_SkillPage_ADD_EFF.GetTag_Contain( tag_add_eff );

            // 스킬 태그 csv 추가 스킬
            for( int i = 0 ; i < score_add_skill ; i++ )
            {
                if( skills.Count < 1 )
                {
                    Debug.LogError( "추가 스킬 모자람 : " + tag_add_eff + " : " + score_add_skill );
                    break;
                }
                CSV_Skill csv = rd.RandomList( skills , true );
                CSV_ID_SKILL_EFF.Add( csv.ID_int );
            }
        }
    }

    public BATTLE_ACTION_TARGET GetBATTLE_ACTION_TARGET()
    {
        if( DEF_ATK_TYPE == 0 )
        {
            // 방어 , 아군
            switch( RANDOM_CNT_TYPE )
            {
                case 0: return BATTLE_ACTION_TARGET.One_Self_ALL;
                case 1: return BATTLE_ACTION_TARGET.Line_Self_ALL;
                case 2: return BATTLE_ACTION_TARGET.ALL_Self;
            }
        }
        else if(DEF_ATK_TYPE == 1)
        {
            switch( RANDOM_CNT_TYPE )
            {
                case 0: return BATTLE_ACTION_TARGET.One_Opp_ALL;
                case 1: return BATTLE_ACTION_TARGET.Line_Opp_ALL;
                case 2: return BATTLE_ACTION_TARGET.ALL_Opp;
            }
        }
        return BATTLE_ACTION_TARGET.None;
    }



    public void Write( BinaryWriter bw )
    {
        
    }

    public void Read( BinaryReader br )
    {
        
    }

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


    // 무작위로 타겟 및 위력
    // 랜덤 스킬의 기초 
    // 인자 0 : 0 방어 , 1 공격

    // 반환 : 위력 비례
    static public int Random_TargetRatioPow( Mng_X128SS rd , int def_atk , ref BATTLE_ACTION_TARGET bat , ref float pow_ratio )
    {
        // 먼저 CountNum_SelectTarget 실행
        int cnt_type = rd.NextInt(0,3);
        pow_ratio = PowRatio_SelectTarget(cnt_type);

        // // 셀렉트 타입에 따라서..
        // // 라인은 무조건 라인 선택
        // if( def_atk == 0 )
        // {
        //     // 방어 , 아군
        //     switch( cnt_type )
        //     {
        //         case 0: bat = BATTLE_ACTION_TARGET.One_Self_ALL;break;
        //         case 1: bat = BATTLE_ACTION_TARGET.Line_Self_ALL;break;
        //         case 2: bat = BATTLE_ACTION_TARGET.ALL_Self;break;
        //     }
        // }
        // else if(def_atk == 1)
        // {
        //     switch( cnt_type )
        //     {
        //         case 0: bat = BATTLE_ACTION_TARGET.One_Opp_ALL;break;
        //         case 1: bat = BATTLE_ACTION_TARGET.Line_Opp_ALL;break;
        //         case 2: bat = BATTLE_ACTION_TARGET.ALL_Opp;break;
        //     }
        // }

        return cnt_type;
    }

    // 위 함수를 MAKE_TARGET_POW 구조체로 반환
    static public COMMON_MAKE_ACT_DATA Random_TargetRatioPow( Mng_X128SS rd , int def_atk )
    {
        COMMON_MAKE_ACT_DATA s = new();
        BATTLE_ACTION_TARGET bat = BATTLE_ACTION_TARGET.None;
        float pow_ratio = 0;
        s.RANDOM_CNT_TYPE = Random_TargetRatioPow( rd , def_atk , ref bat , ref pow_ratio );
        //s.bat = bat;
        //s.pow_ratio = pow_ratio;
        return s;
    }
}
