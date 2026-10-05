using System.Collections.Generic;
using UnityEngine;

// 암흑
// 무조건 빗나감 
// 그룹 공격 , 전체 공격은 예외?

public class StateSkill_Blind : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_CHECK_HIT_MISS };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_CHECK_HIT_MISS )
        {
            // 완전히 빗나감 , 
            // 또는 전체공격은 제외 , 스킬 타겟이 상대 전체 공격인지 체크해본다

            return true;
        }
        
        return false;
    }


}
