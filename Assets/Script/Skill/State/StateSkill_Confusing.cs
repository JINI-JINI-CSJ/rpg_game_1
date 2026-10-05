using System.Collections.Generic;
using UnityEngine;

// 혼란 
// 입력 불가
// 아무 행동
public class StateSkill_Confusing : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_DISABLE_CMD_INPUT , GTF_Define.BATTLE_DISABLE_EXEC_ACTION_START };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_DISABLE_CMD_INPUT )
        {
            // 입력 불가
            return true;
        }

        if( tag == GTF_Define.BATTLE_DISABLE_EXEC_ACTION_START )
        {
            // 아무 행동이나 하기
            // 무작위로 적 아군 공격
            return true;
        }
        
        return false;
    }
}
