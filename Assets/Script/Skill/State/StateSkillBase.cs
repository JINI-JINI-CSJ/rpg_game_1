using System.Collections.Generic;
using UnityEngine;

// 버프 , 디버프 , 상태이상 타입들 정의 

public enum STATE_SKILL_TYPE
{
    None , 
    Buff ,
    Debuff,
    // 기절 / 스턴	일정 턴 동안 행동 불가
    Faint ,
    // 수면	행동 불가, 공격받으면 깨어날 수 있음
    Sleep , 
    // 마비	행동할 수 없거나 일정 확률로 행동 실패
    Paralysis ,
    // 석화	행동 불가, 사실상 전투에서 제외
    // 빙결	행동 불가, 공격받으면 해제될 수 있음

    // 매혹	일정 턴 동안 아군을 공격하거나 행동 통제
    Fascination , 
    // 혼란	무작위 행동
    Confusing ,
    // 공포	공격력 감소 + 도망치거나 행동 실패
    // 망각	스킬/마법 사용 불가
    // 봉인	특정 종류의 행동만 사용 불가
    // 특기 불가 , 전사 계열 , CombatArts , 그냥 Skill 은 전체 통합 , TP 
    Disable_CombatArts ,
    // 마법 불가 , MP
    Disable_Magic , 
    // 암흑 , 지정 목표 공격 무조건 빗나감 , 전체 공격은 가능?
    Blind , 
    // 분노 , 무조건 공격
    Anger , 
    // 두려움 , 기절 같은 행동 불가
    Afraid ,


    // 독	턴 종료 시 HP 감소
    Poison , 
    // 맹독	일반 독보다 강한 지속 피해
    Poison_Deadly , 
    // 출혈	행동할 때마다 HP 감소
    Bleeding , 
    // 화상	턴마다 화염 피해 + 공격력 감소
    Burn , 
    // 저주	턴마다 HP 또는 능력치 감소

    // 부식	방어력/장비 내구도 감소
    // 중독	HP뿐 아니라 MP/자원까지 지속 감소

    // 흡혈 저주	행동할 때 HP가 추가로 감소

}


public class StateSkillBase
{
    public CharBase charBase;

    public int TURN_MAX;

    public int turn_cur;
    
    // 관련 태그
    virtual public List<string> Tag()
    {
        return null;
    }

    public bool CheckTag( string tag )
    {
        List<string> tags = Tag();
        if( tags == null ) return false;
        foreach( var s in tags )
        {
            if( tag == s ) return true;
        }
        return false;
    }

    virtual public void OnAdd(){}
    virtual public void OnRemove(){}
    virtual public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        return false;
    }

    public void TurnAdd_Battle()
    {
        turn_cur++;
        if( turn_cur >= TURN_MAX )
        {
            // charBase 리브무 스킬 스테이트
        }
    }
}
