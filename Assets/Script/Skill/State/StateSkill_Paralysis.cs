using System.Collections.Generic;
using UnityEngine;

// 마비
// 명령어 입력은 되지만 , 행동 실행시 안될수 있음
public class StateSkill_Paralysis : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_DISABLE_EXEC_ACTION_START };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_DISABLE_EXEC_ACTION_START )
        {
            // 확률적으로 행동 불가
            return true;
        }

        return false;
    }

}
