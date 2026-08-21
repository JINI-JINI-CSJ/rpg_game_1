using System.Collections.Generic;
using UnityEngine;

public class SKILL_NORMAL_INF
{
    public BATTLE_ACTION_TARGET  target;
    public int      base_val;
    public int      mp;
    public float    add_pow;
}

/// <summary>
/// 스킬 베이스
/// </summary>

public class SkillBase 
{
    public CSV_Skill csv;    

    // 기본 공격력 이외에 기타 지원 스킬도 참조 할수 있다.
    // 탐문 , 함정 해체 등등  메이킹으로 만들었을 경우 참조하자.
    public SKILL_NORMAL_INF skill_normal_inf;


    public CharBase charHave;
    public int LEVEL;

    static public SkillBase InstSkill( CSV_Skill csv )
    {
        SkillBase inst_skill = null;
        if( string.IsNullOrEmpty( csv.class_name ) == false )
        {
            inst_skill = SJ_CSharpUtil.NewClass_Str( csv.class_name ) as SkillBase;
        }
        else
        {
            inst_skill = new();
        }
        inst_skill.SetCSV(csv);
        return inst_skill;
    }

    public void SetCSV( CSV_Skill _csv )
    {
        csv = _csv;
    }

    virtual public BATTLE_ACTION_TARGET GetTargetType()
    {
        if( skill_normal_inf != null ) return skill_normal_inf.target;
        return BATTLE_ACTION_TARGET.One_Opp_Front;
    }

    virtual public void SelectTarget( SJ_COMMON.Func_Arg func_ok = null , SJ_COMMON.Func_VOID func_cancel = null )
    {
        List<BATTLE_SEL_GROUP> lt = BattleTargetSelector.MakeSelectGroup( charHave.armyForce , GetTargetType() );
        OnSelectTargetDefault( lt );

        BattleTargetSelector.Show( true , func_ok , func_cancel );
    }

    virtual public void OnSelectTargetDefault( List<BATTLE_SEL_GROUP> lt )
    {
        if( lt.Count > 0 )
        {
            BattleTargetSelector.SetCursor( lt[0] );
        }
    }

    virtual public void Action( BATTLE_SEL_GROUP sel_group )
    {
        OnAction( sel_group );
        foreach( var s in sel_group.chars )
        {
            OnActionChar(s);
            s.Call_RecvSkill( this );
        }
    }
    virtual public void OnAction( BATTLE_SEL_GROUP sel_group ){}
    virtual public void OnActionChar( CharBase chr ){}



    // 플레이어 (바닥 유아이) 에  스킬 효과 
    virtual public void OnViewEffect_Player( GameObject go ){}

    // 적군에  스킬 효과 
    virtual public void OnViewEffect_Enemy( GameObject go ){}
}
