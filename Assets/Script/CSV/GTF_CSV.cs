using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GTF_CSV : SJ_CSV_Mng
{

    static public CSV_PercentInfPage csv_PercentInfPage = new();

    static public CSV_MagicPropDefinePage csv_MagicPropDefinePage = new();

    static public List<CSV_Skill> GetSkill_JobMake( JOB_BASE job )
    {
        List<CSV_Skill> cSVs = new List<CSV_Skill>();
        return cSVs;
    }

    static public List<CSV_CharBaseStat> GetCharStat_JobMake( JOB_BASE job )
    {
        List<CSV_CharBaseStat> cSVs = new();
        return cSVs;
    }

    // 스킬 보너스 갯수
    static public int PerBONUS_SkillNum( Mng_X128SS rd )
    {
        return 0;
    }

    // 스탯 보너스 갯수
    static public int PerBONUS_StatNum( Mng_X128SS rd )
    {
        return 0;
    }
}
