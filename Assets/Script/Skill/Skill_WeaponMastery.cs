using UnityEngine;

/// <summary>
/// 무기 장착 가능
/// 스킬 기본 공격
/// </summary>

public class Skill_WeaponMastery : SkillBase
{

    public override void OnActionChar(CharBase chr)
    {
        chr.GetDamage( charBase.csv.charPrcValue.ATK_P );
    }

    override public void OnViewEffect_Enemy( GameObject go )
    {
        string eff_res = csv.res3d;
        if( string.IsNullOrEmpty( eff_res ) )
        {
            eff_res = "EFF_Sword Slash 1";
        }
        EffectManager.InstEff_ATK( eff_res , go.transform );
    }
}
