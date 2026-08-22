using System.Collections.Generic;
using UnityEngine;


// 일반 범용 스킬 메이킹
// 단순하게 좋은거 확률적으로 더하기..
// 위력 % + 대상  + 소비 mp 감소 + 부가 효과 
public class Skill_MAKE_NormalGrade
{
    public const string GRADE_POW       = "GRADE_POW";
    public const string GRADE_TARGET    = "GRADE_TARGET";
    public const string GRADE_MP        = "GRADE_MP";
    public const string GRADE_ADD_EFF   = "GRADE_ADD_EFF";


    static public SkillBase Make( JOB_BASE jOB_type , string tag_job , int grade_bonus , Mng_X128SS rd )
    {
        Skill_MakeNormal skill = new();
        skill.skill_normal_inf = new();

        switch( tag_job )
        {
            case "FIGHTER":         DefaultMake( rd  , grade_bonus , skill , 0 , tag_job , GTF_CSV.csv_Config.makeSkill_BaseVal_FIGHTER ); break;
            case "WIZARD_ATK":      DefaultMake( rd  , grade_bonus , skill , 0 , tag_job , GTF_CSV.csv_Config.makeSkill_BaseVal_WIZARD_ATK ); break;
            case "WIZARD_DEBUFF":   DefaultMake( rd  , grade_bonus , skill , 0 , tag_job , GTF_CSV.csv_Config.makeSkill_BaseVal_WIZARD_ATK ); break;
            case "WIZARD_HEAL":     DefaultMake( rd  , grade_bonus , skill , 1 , tag_job , GTF_CSV.csv_Config.makeSkill_BaseVal_WIZARD_HEAL ); break;
            case "WIZARD_BUFF":     DefaultMake( rd  , grade_bonus , skill , 1 , tag_job , GTF_CSV.csv_Config.makeSkill_BaseVal_WIZARD_HEAL ); break;

        }


        // switch( jOB_type )
        // {
        //     case JOB_BASE.FIGHTER:
        //         {
        //             DefaultMake( rd  , grade_bonus , skill , 0 , tag_job , "FIGHTER" , GTF_CSV.csv_Config.makeSkill_BaseVal_FIGHTER );
        //         }
        //         break;

        //      case JOB_BASE.WIZARD:
        //         {
        //                 // 공격 , 회복 , 버프 , 디버프 등 
        //                 if( tag_job.Contains( "ATK" ) )
        //                 {
        //                     DefaultMake( rd  , grade_bonus , skill , 0 , tag_job , "WIZARD_ATK" , GTF_CSV.csv_Config.makeSkill_BaseVal_WIZARD_ATK );
        //                 }
        //                 if( tag_job.Contains( "DEBUFF" ) )
        //                 {
        //                     DefaultMake( rd  , grade_bonus , skill , 0 , tag_job , "WIZARD_DEBUFF" , GTF_CSV.csv_Config.makeSkill_BaseVal_WIZARD_ATK );
        //                 }

        //                 if( tag_job.Contains( "HEAL" ) )
        //                 {
        //                     DefaultMake( rd  , grade_bonus , skill , 1 , tag_job , "WIZARD_HEAL" , GTF_CSV.csv_Config.makeSkill_BaseVal_WIZARD_HEAL );
        //                 }
        //                 if( tag_job.Contains( "BUFF" ) )
        //                 {
        //                     DefaultMake( rd  , grade_bonus , skill , 1 , tag_job , "WIZARD_BUFF" , GTF_CSV.csv_Config.makeSkill_BaseVal_WIZARD_HEAL );
        //                 }
        //             }
        //         break;

        //     case JOB_BASE.SUPPORTER:
        //         {
        //             // 지원 전용 스킬들. 위력 
        //             // 지원 스킬 정의 후에 코딩
        //         }
        //         break;
        // }

        return skill;
    }


    static public void DefaultMake( Mng_X128SS rd , int grade_bonus , SkillBase skill_self , int atk_def , string tag_addEff , 
                                    int base_val )
    {
        SKILL_NORMAL_INF sni = skill_self.skill_normal_inf;

        // 일단 공통
        // 점점 세부적으로 할경우 다 따로 설정할수도 있다.
        rd.Clear_RandomDivision();
        rd.Add_RandomDivision( GRADE_POW );
        rd.Add_RandomDivision( GRADE_TARGET );
        rd.Add_RandomDivision( GRADE_MP );
        rd.Add_RandomDivision( GRADE_ADD_EFF );
        rd.Random_RandomDivision( grade_bonus );

        SJ_RangeStep rangeStep = new();

        sni.base_val = base_val;
        sni.add_pow = GTF_CSV.csv_Config.GetMakeSkill_addPow( rd.Result_RandomDivision( GRADE_POW ) );
        sni.mp = GTF_CSV.csv_Config.GetMakeSkill_MP( rd.Result_RandomDivision( GRADE_MP ) );          

        AddEffSkill( rd , skill_self ,  tag_addEff , 3 , (int)rd.Result_RandomDivision( GRADE_ADD_EFF ) );

        switch( atk_def )
        {
            // 공격형 
            case 0:
                {
                    rangeStep.Add( 0 , 1 , 1 );     // 1 단계 : 전열 1 , 후열 1
                    rangeStep.Add( 2 , 5 , 2 );     // 2 단계 : 전후 1
                    rangeStep.Add( 6 , 8 , 3 );     // 3 단계 : 전열 라인 , 후열 라인
                    rangeStep.Add( 9 , 12 , 4 );    // 4 단계 : 전후열 라인
                    rangeStep.Add( 13 , 9999 , 5 ); // 5 단계 : 전체
                    int grade_target = (int)rangeStep.Result( rd.Result_RandomDivision( GRADE_MP ) );
                    switch( grade_target )
                    {
                        case 1:sni.target = rd.RandomList( BATTLE_ACTION_TARGET.One_Opp_Front , BATTLE_ACTION_TARGET.One_Opp_Back );
                                break;
                        case 2:sni.target = BATTLE_ACTION_TARGET.One_Opp_ALL;
                            break;
                        case 3:sni.target = rd.RandomList( BATTLE_ACTION_TARGET.Line_Opp_Front , BATTLE_ACTION_TARGET.Line_Opp_Back );
                            break;
                        case 4:sni.target = BATTLE_ACTION_TARGET.Line_Opp_ALL;
                            break;
                        case 5:sni.target = BATTLE_ACTION_TARGET.ALL_Opp;
                            break;
                    }                       
                }
                break;

            // 방어형
            // 3 단계 정도 : 1 체 선택 , 1라인 선택 , 전체
            case 1:
                {
                    
                    rangeStep.Add( 0 , 5 , 1 );     // 1 단계 :
                    rangeStep.Add( 6 , 12 , 2 );    // 4 단계 : 
                    rangeStep.Add( 13 , 9999 , 3 ); // 5 단계 : 
                    int grade_target = (int)rangeStep.Result( rd.Result_RandomDivision( GRADE_MP ) );
                    switch( grade_target )
                    {
                        case 1:sni.target = BATTLE_ACTION_TARGET.One_Self_ALL;
                                break;
                        case 2:sni.target = BATTLE_ACTION_TARGET.Line_Self_ALL;
                            break;
                        case 3:sni.target = BATTLE_ACTION_TARGET.ALL_Self;
                            break;
                    }     
                }
                break;
         
        }


    }

    // 추가 효과
    // 물리스킬용 추가 이펙트 중에서 1개 가져오기 , 물리 공격에 맞는것만.
    // 최소값 이상일때만 
    static public void AddEffSkill( Mng_X128SS rd , SkillBase skill_self , string tag , int min_lv , int lv_score )
    {
        if( lv_score >= min_lv )
        {
            List<CSV_Skill> skills_addEff = GTF_CSV.csv_SkillPage_ADD_EFF.GetTag_Contain( tag );
            CSV_Skill csv_addEff = rd.RandomList( skills_addEff );     
            int lv_addEff = lv_score - min_lv;
            skill_self.skill_addEff = SkillBase.InstSkill( csv_addEff , lv_addEff );
        }
    }

}


// 잠정 보류~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// 스킬 정의 + 부가 효과


// 스킬 메이킹 요소
// (0) [스킬 대 분류 ] -> 공격 (물,마) , 적군 디법 , 아군 버프 , 지원 보조 스킬
// (1) [대상] 
// (2) [횟수] 
// (3) [공격력] 
// (4) [부가 효과] 

// 옵션 [ 속성(마법일때만) ]

// 0. 물리 클래스 , 마법 클래스
// 전부 확률로 한다.


//[스킬 대 분류]
// 클래스 별 기능
// 공격 물리 1  
// 공격 마법 1
// 디버프 적군 다수
// 버프 아군 다수 (힐링같은것도 포함)
// 지원가 다수

// 스킬 일반 csv 를 참조한다.
public enum _SKILL_MAIN_TYPE
{
    None = 0 ,
    ATK_P , 
    ATK_M ,
    DEBUFF_ENEMY , 
    BUFF_ALLIES , 
    SUPPORT ,
}


// 요소 베이스 클래스
public class SkillUnitBase
{
    virtual public void PerMake( Mng_X128SS rd , _SKILL_MAIN_TYPE skill_cate ){}

    public int GetPerCSV( string str , Mng_X128SS rd )
    {
        int max = 0;
        return GTF_CSV.csv_PercentInfPage.GetPerIdx( rd , str , ref max );
    }
}

// [대상]
public enum _SKILL_TARGET_TYPE
{
    None = 0,
    One , 
    Random ,     
    ALL , 
}



public class SkillUnit_Target : SkillUnitBase
{
    // 기본 : 선택이 안되더라도 기본 값 -> 단일 대상
    public _SKILL_TARGET_TYPE target = _SKILL_TARGET_TYPE.One;

    override public void PerMake( Mng_X128SS rd ,  _SKILL_MAIN_TYPE skill_cate )
    {
        int per_idx = GetPerCSV( "SKILL_MAKE_TARGET" , rd );
        switch( per_idx )
        {
            case 0: target = _SKILL_TARGET_TYPE.One; break;
            case 1: target = _SKILL_TARGET_TYPE.Random; break;
            case 2: target = _SKILL_TARGET_TYPE.ALL;break;
        }
    }

}

// [횟수]
public class SkillUnit_ACTNum : SkillUnitBase
{
    public int actNum = 1;
    override public void PerMake( Mng_X128SS rd , _SKILL_MAIN_TYPE skill_cate )
    {
        int per_idx = GetPerCSV( "SKILL_MAKE_TARGET" , rd );
        actNum = per_idx + 1;
    }
}

// [공격력] , 퍼센트
// 진여신 처럼 소중대 같은 정해진 등급으로 할까?
// 최종 정리시 재보정 , 대신 너무 낮추지는 말자. 
public class SkillUnit_ATKPow : SkillUnitBase
{
    public float total_per;

    // 스킬 시작 공격력
    public float start_per = 1.3f;

    // 점수당 증가
    public float add_per = 0.1f;

    // 최종 정리시에 보정

    // 공격횟수  보정값
    public float dec_per_OneTarget = 0.9f;

    // 다중 대상 보정값
    public float dec_per_AllTarget = 0.7f;

    override public void PerMake( Mng_X128SS rd , _SKILL_MAIN_TYPE skill_cate )
    {
        int per_idx = GetPerCSV( "SKILL_MAKE_ATKPOW" , rd );
        total_per = add_per * per_idx + start_per;
    }

    public void AfterWork( Skill_Make skill )
    {
        if( skill.skillUnit_ACTNum.actNum >= 2 )
        {
            if( skill.skillUnit_Target.target != _SKILL_TARGET_TYPE.ALL )
            {
                total_per *= ( dec_per_OneTarget * skill.skillUnit_ACTNum.actNum  );
            }
            else
            {
                total_per *= ( dec_per_AllTarget * skill.skillUnit_ACTNum.actNum  );
            }
        }
    }
}

// [ 추가 효과 ]
public class SkillUnit_AddEffect : SkillUnitBase
{
    public int add_effect;
    override public void PerMake( Mng_X128SS rd , _SKILL_MAIN_TYPE skill_cate)
    {
        add_effect = GetPerCSV( "SKILL_MAKE_ADD_EFFECT" , rd );

        // 갯수만큼 csv 스킬 추가 효과 에서 가져온다.
    }
}
