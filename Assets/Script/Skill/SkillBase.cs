using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬 베이스
/// </summary>

public class SkillBase 
{
    public CharBase charBase;

    public CSV_Skill csv;

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
        return BATTLE_ACTION_TARGET.One_Opp_Front;
    }

    virtual public void SelectTarget()
    {
        List<BATTLE_SEL_GROUP> lt = BattleTargetSelector.MakeSelectGroup( charBase.armyForce , GetTargetType() );
        OnSelectTargetDefault( lt );

        BattleTargetSelector.Show( true );
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
