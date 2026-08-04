using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GTF_CSV : SJ_CSV_Mng
{
    static public CSV_Config                csv_Config = new();
    static public CSV_PercentInfPage        csv_PercentInfPage = new();
    static public CSV_ConteStoryPage        csv_ConteStoryPage = new();
    static public CSV_GODPage               csv_GODPage = new();
    static public CSV_EqItemDefinePage      csv_EqItemDefinePage = new();
    static public CSV_MagicPropDefinePage   csv_MagicPropDefinePage = new();
    static public CSV_SkillPage             csv_SkillPage_NORMAL = new();
    static public CSV_SkillPage             csv_SkillPage_ADD_EFF = new();
    static public CSV_CharBaseStatPage      csv_CharPlayer = new(); // 아군 플레이어 정의
    static public CSV_CharBaseStatPage      csv_CharEnemy = new(); // 적군 정의

    static public CSV_ItemPage              csv_ItemPage_Consume = new();
    static public CSV_ItemPage              csv_ItemPage_Equip = new();
    static public CSV_OfficeUpgradePage     csv_OfficeUpgradePage = new();
    static public CSV_OfficeDepartmentPage  csv_OfficeDepartmentPage = new();

    public GTF_CSV()
    {
        url_Base        = "https://docs.google.com/spreadsheets/d/1rgsuacZzfhN3i95GJvW_8Wwy7QAzajXbk-FRrXjFEyE/gviz/tq?tqx=out:csv&sheet=";
        url_Base_Lang   = "https://docs.google.com/spreadsheets/d/1JCiUQIezvUO4c3ycRROHDkYDeCi7JyzOO_JJYeI_4ls/gviz/tq?tqx=out:csv&sheet=";
    }

    override	public void OnAdd_CSVUrlList()
    {
        Add_CSVName( csv_Config                 , "전역"            , false );
        Add_CSVName( csv_PercentInfPage         , "확률표"          , false );
        Add_CSVName( csv_ConteStoryPage         , "다중퀘스트성향"  , false );
        Add_CSVName( csv_GODPage                , "후원자 신"       , false );
        Add_CSVName( csv_EqItemDefinePage       , "장비아이템정의"  , false );
        Add_CSVName( csv_MagicPropDefinePage    , "마법속성정의"    , false );
        Add_CSVName( csv_SkillPage_NORMAL       , "스킬일반"        , false );
        Add_CSVName( csv_SkillPage_ADD_EFF      , "스킬추가효과"    , false );    
        Add_CSVName( csv_CharPlayer             , "캐릭터정의"      , false );   
        Add_CSVName( csv_CharEnemy              , "적군정의"        , false );   
        Add_CSVName( csv_ItemPage_Consume       , "아이템-소비"     , false );   
        Add_CSVName( csv_ItemPage_Equip         , "아이템-장비"     , false );   

        // 사무실 


        Add_CSVName( null      , "기본"     , false , true );   
        Add_CSVName( null      , "대화컷신"  , false , true );   
    }

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

    // 전투 결과 
    static public void ResultBattle( List<CharBase> charBases , out int gold , out int exp )
    {
        // 레벨 * 강함 보정
        // 나중에 전역설정
        int csv_gold = 100;
        int csv_exp = 20;

        gold = 0;
        exp = 0;
        foreach( var s in charBases )
        {
            gold += ResultBattle_Calc( csv_gold , s.LEVEL , s.csv.pow_ratio );
            exp += ResultBattle_Calc( csv_exp , s.LEVEL , s.csv.pow_ratio );
        }
    }

    static public int ResultBattle_Calc( int base_val , int level , float pw_ratio )
    {
        float val = base_val * level * pw_ratio;
        return (int)val;
    }
}
