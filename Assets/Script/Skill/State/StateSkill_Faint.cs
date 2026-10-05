using System.Collections.Generic;
using UnityEngine;


// 기절 , 스턴
public class StateSkill_Faint : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_DISABLE_CMD_INPUT , GTF_Define.BATTLE_TURN_END };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_DISABLE_CMD_INPUT )
        {
            // 입력 불가
            return true;
        }

        if( tag == GTF_Define.BATTLE_TURN_END )
        {
            // 턴 종료 시 확률적으로 기절 해재
            return true;
        }

        
        // if( tag == GTF_Define.BATTLE_CHR_DAMAGE_START )
        // {
        //     // 피격시 해제
        //     return true;
        // }


        return false;
    }
}
