using UnityEngine;

// 잠정 보류



// 스킬 먼저 만들고 결과에 따라 직업을 만들자.
public class Skill_Make : SkillBase
{
    public SkillUnit_Target     skillUnit_Target = new();
    public SkillUnit_ACTNum     skillUnit_ACTNum = new();
    public SkillUnit_ATKPow     skillUnit_ATKPow = new();
    public SkillUnit_AddEffect  skillUnit_AddEffect = new();

    // 마법일경우 속성
    public CSV_MagicPropDefine magic_prop = null;


    // 일단 스킬 카테는 도시의 성향으로 하자.
    public void MakeSkill( Mng_X128SS rd , _SKILL_MAIN_TYPE skill_cate )
    {
        skillUnit_Target.PerMake( rd , skill_cate );
        skillUnit_ACTNum.PerMake( rd , skill_cate );
        skillUnit_ATKPow.PerMake( rd , skill_cate );
        skillUnit_AddEffect.PerMake( rd , skill_cate );

        // 공격 마법일때만
        if( skill_cate ==  _SKILL_MAIN_TYPE.ATK_M )
        {
            magic_prop = GTF_CSV.csv_MagicPropDefinePage.GetRandom(rd);
        }

        skillUnit_ATKPow.AfterWork( this );
    }
}
