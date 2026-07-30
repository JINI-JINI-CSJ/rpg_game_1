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

    virtual public BATTLE_ACTION_TARGET GetTargetType()
    {
        return BATTLE_ACTION_TARGET.One_Opp_Front;
    }

    virtual public void SelectTarget()
    {
        List<BATTLE_SEL_GROUP> lt = BattleTargetSelector.MakeSelectGroup( charBase.armyForce , GetTargetType() );
        OnSelectTargetDefault( lt );
    }

    virtual public void OnSelectTargetDefault( List<BATTLE_SEL_GROUP> lt )
    {
        if( lt.Count > 0 )
        {
            BattleTargetSelector.SetCursor( lt[0] );
        }
    }

    virtual public void Action( BATTLE_SEL_GROUP sel_group ){}
}
