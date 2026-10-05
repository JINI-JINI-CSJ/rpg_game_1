using UnityEngine;
using System.Collections.Generic;

public class StateSkill_Disable_CombatArts : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_DISABLE_MENU_COMBAT_ARTS };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_DISABLE_MENU_COMBAT_ARTS )
        {
            // 메뉴 비활성화
            return true;
        }
        return false;
    }

    
}
