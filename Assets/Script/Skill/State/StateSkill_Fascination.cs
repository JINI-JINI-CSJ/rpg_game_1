using UnityEngine;
using System.Collections.Generic;

// 매혹
public class StateSkill_Fascination : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_DISABLE_CMD_INPUT  , GTF_Define.BATTLE_TURN_END };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        
        return false;
    }
}
