using UnityEngine;
using System.Collections.Generic;

// 맹독 , 독이랑 똑같은데 설정 값만 다르게 가져오자.
public class StateSkill_Poison_Deadly : StateSkillBase
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
