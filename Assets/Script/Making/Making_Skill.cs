using UnityEngine;

// 스킬 메이킹 요소
// (1) [대상] -> 최고점수 제한 , 정해지면 제외
// (2) [횟수] -> 정해지면 제외
// (3) [공격력] -> 최종 정리 단계에서 대상 , 횟수를 고려 하여 재설정 , 제외 안함
// (4) [부가 효과] -> 갯수 제한 넘으면 제외

// [ 속성(마법일때만) ]

// 0. 물리 클래스 , 마법 클래스
// 1. 총 점수 할당
// 2. 점수 1~ 총점수중 랜덤 
// 3. 요소에다 더함 , 최고점수제한 요소는 제외
// 4. 총 점수 다 없을때까지 반복

// 요소 베이스 클래스
public class SkillUnitBase
{
    // 반환값 : 점수 다 쓰고 남으면 반환
    // 예 ) [대상] 의 최대 점수는 3 정도인데 4이상이면 3만사용하고 1 반환
    virtual public int AddScore( Mng_X128SS rd , int lv )
    {
        return 0;
    }
    virtual public bool Check_Remove(){return false;}
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

    override public int AddScore( Mng_X128SS rd , int lv )
    {

        // 각 항목당 최소 점수
        rd.Step_Start_Add( 1 , _SKILL_TARGET_TYPE.One ); // 단일은 기본
        if( lv >= 1 ) rd.Step_Add( 0.7f , _SKILL_TARGET_TYPE.Random ); // 
        if( lv >= 2 ) rd.Step_Add( 0.5f , _SKILL_TARGET_TYPE.ALL ); // 

        target = (_SKILL_TARGET_TYPE)rd.Step_Random();

        // 점수 계산
        switch( target )
        {
            case _SKILL_TARGET_TYPE.Random: lv -= 1; break;
            case _SKILL_TARGET_TYPE.ALL:    lv -= 2; break;
        }
        return lv;
    }

    // 한번 정해지면 무조건 제외
    override public bool Check_Remove()
    {
        return true;
    }
}

// [횟수]
public class SkillUnit_ACTNum : SkillUnitBase
{
    public int actNum = 1;
    override public int AddScore( Mng_X128SS rd , int lv )
    {
        // 1 회 기본
        // 2 회 2 점
        // 3 회 4 점
        rd.Step_Start_Add( 1 , 1); // 단일은 기본
        if( lv >= 1 ) rd.Step_Add( 0.7f , 2 ); // 
        if( lv >= 2 ) rd.Step_Add( 0.5f , 3 ); // 

        actNum = (int)rd.Step_Random();

        switch( actNum )
        {
            case 2: lv -= 2; break;
            case 3: lv -= 4; break;
        }

        return lv;
    }

    override public bool Check_Remove()
    {
        return true;
    }
}

// [공격력] , 퍼센트
// 점수당 일정 퍼센트 올린다.
// 최종 정리시 재보정 , 대신 너무 낮추지는 말자. 



public class Making_Skill
{
    
}
