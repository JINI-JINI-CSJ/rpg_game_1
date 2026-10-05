using UnityEngine;
using System.Collections.Generic;

// 두려움 
// 행동 불가 , 가끔 도망? ,  개인 탈주?
public class StateSkill_Afraid : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_DISABLE_CMD_INPUT  , GTF_Define.BATTLE_CHR_ACTION_READY };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_DISABLE_CMD_INPUT )
        {
            return true;
        }

        if( tag == GTF_Define.BATTLE_CHR_ACTION_READY )
        {
            // 행동 시작시 그냥 넘기거나 도망?
            return true;
        }
        
        return false;
    }
}
