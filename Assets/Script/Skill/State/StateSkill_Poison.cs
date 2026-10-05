using System.Collections.Generic;
using UnityEngine;

public class StateSkill_Poison : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_TURN_END };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_TURN_END )
        {
            // 캐릭터 피해 

            // 캐릭터 이벤트 , 독 피해 받음 이펙트

            return true;
        }

        return false;
    }
}
