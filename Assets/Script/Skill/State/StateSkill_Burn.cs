using System.Collections.Generic;
using UnityEngine;

// 화상
// 독이랑 비슷
// 피격 때 얼음 물 계열이면 해제
public class StateSkill_Burn : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_TURN_END , GTF_Define.BATTLE_CHR_DAMAGE_START };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_TURN_END )
        {
            // 캐릭터 피해 
            // 캐릭터 이벤트 

            return true;
        }

        if( tag == GTF_Define.BATTLE_CHR_DAMAGE_START )
        {
            // 맞은것이 얼음 계열이면 해제

            return true;
        }

        return false;
    }
}
