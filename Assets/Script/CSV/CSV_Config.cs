using System.Collections.Generic;
using UnityEngine;

public class CSV_Config : SJ_CSV_BasePage
{
    // 월드 메이킹 

    // 캐릭터 메이킹

    // 스킬 메이킹
    public int makeSkill_BaseVal_WARRIOR;       // 메이킹 스킬 기본 공격력
    public int makeSkill_BaseVal_WIZARD_ATK;    // 공격 마법 기본 공격력
    public int makeSkill_BaseVal_WIZARD_HEAL;   // 회복 마법 기본 수치
    public int makeSkill_BaseVal_SUPPORTER;     // 지원

    // 위력 강도 목록
    // 일단 다 같게..
    public List<float> makeSkill_addPow;
    public List<int>   makeSkill_mp;

    public override void Read()
    {
        
    }

    public float GetMakeSkill_addPow( int grade ){return SJ_CSharpUtil.GetList_IndexSafe( makeSkill_addPow , grade );}
    public int GetMakeSkill_MP( int grade ){return SJ_CSharpUtil.GetList_IndexSafe( makeSkill_mp , grade );}
}
