using UnityEngine;


// 일반 범용 스킬 메이킹
// 단순하게 좋은거 확률적으로 더하기..
// 위력 % + 대상수  + 소비 mp 감소 + 부가 효과 
public class Skill_MAKE_NormalGrade
{
    public const string GRADE_POW       = "GRADE_POW";
    public const string GRADE_TARGET    = "GRADE_TARGET";
    public const string GRADE_MP        = "GRADE_MP";
    public const string GRADE_ADD_EFF   = "GRADE_ADD_EFF";

    static public SkillBase Make( JOB_BASE jOB_type , int grade_bonus , Mng_X128SS rd )
    {
        SkillBase skill = new();

        rd.Clear_RandomDivision();
        rd.Add_RandomDivision( GRADE_POW );
        rd.Add_RandomDivision( GRADE_TARGET );
        rd.Add_RandomDivision( GRADE_MP );
        rd.Add_RandomDivision( GRADE_ADD_EFF );
        rd.Random_RandomDivision( grade_bonus );




        return skill;
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
