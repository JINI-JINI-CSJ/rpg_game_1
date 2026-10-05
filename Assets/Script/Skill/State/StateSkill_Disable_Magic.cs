using System.Collections.Generic;
using UnityEngine;

public class StateSkill_Disable_Magic : StateSkillBase
{
    public override List<string> Tag()
    {
        return new List<string>() { GTF_Define.BATTLE_DISABLE_MENU_MAGIC };
    }

    override public bool OnQuery( string tag , SJ_DIC<string> dic = null )
    {
        if( tag == GTF_Define.BATTLE_DISABLE_MENU_MAGIC )
        {
            // 메뉴 비활성화
            return true;
        }
        return false;
    }
}
