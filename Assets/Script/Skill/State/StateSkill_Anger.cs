using UnityEngine;
using System.Collections.Generic;

// 분노 
// 입력 불가 , 무조건 일반 공격
public class StateSkill_Anger : StateSkillBase
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
            // 행동 시작시 무조건 기본 공격만
            // 캐릭터 명령어를 기본공격 으로 바꾼다.

            return true;
        }
        
        return false;
    }
}
