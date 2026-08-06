using UnityEngine;

public class EffectManager
{
    static public void InstEff( string path_eff , Vector3 pos )
    {
        GameObject inst = SJ_ResPoolSys.Inst_Obj( path_eff );
        inst.transform.position = pos;
    }

    static public void InstEff( string path_eff , Transform tr_pos )
    {
        GameObject inst = SJ_ResPoolSys.Inst_Obj( path_eff );
        inst.transform.position = tr_pos.position;
    }

    static public void InstEff_ATK( string eff_atk , Transform tr_pos )
    {
        InstEff( "VFX/ATK/" + eff_atk , tr_pos );
    }

    
}
