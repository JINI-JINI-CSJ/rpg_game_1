using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GTF_CSV : SJ_CSV_Mng
{
    static public CSV_Config                csv_Config = new();
    static public CSV_PercentInfPage        csv_PercentInfPage = new();
    static public CSV_TagDefinePage         csv_TagDefinePage = new();
    static public CSV_CharUniqueMakePage    csv_CharUniqueMakePage = new();
    static public CSV_ConteStoryPage        csv_ConteStoryPage = new();
    static public CSV_GODPage               csv_GODPage = new();
    static public CSV_EqItemDefinePage      csv_EqItemDefinePage = new();
    static public CSV_MagicPropDefinePage   csv_MagicPropDefinePage = new();


    static public CSV_SkillPage             csv_SkillPage_NORMAL = new();
    static public CSV_SkillPage             csv_SkillPage_ADD_EFF = new();
    static public CSV_SkillPage             csv_SkillPage_ALL = new();

    static public CSV_CharBaseStatPage      csv_CharPlayer = new(); // 아군 플레이어 정의
    static public CSV_CharBaseStatPage      csv_CharEnemy = new(); // 적군 정의

    static public CSV_DropRewardPerPage     csv_DropRewardPer = new();

    static public CSV_CharBaseStatPage      csv_Char_ALL = new();

    static public CSV_ItemPage              csv_ItemPage_Consume = new();
    static public CSV_ItemPage              csv_ItemPage_Equip = new();
    static public CSV_ItemPage              csv_ItemPage_Unique = new();
    static public CSV_ItemPage              csv_ItemPage_Collect = new();

    static public CSV_ItemPage              csv_ItemPage_ALL = new();

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
        Add_CSVName( csv_TagDefinePage          , "태그정의"        , false );
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
        Add_CSVName( csv_ItemPage_Collect       , "아이템-수집재료" , false );

        // 사무실 
        //..

        Add_CSVName( null      , "기본"     , false , true );   
        Add_CSVName( null      , "대화컷신"  , false , true );   
        Add_CSVName( null      , "아이템_스킬"  , false , true );   
    }

    public override void OnLoadAfter()
    {
        csv_Char_ALL.Add( csv_CharPlayer );
        csv_Char_ALL.Add( csv_CharEnemy );

        csv_SkillPage_ALL.Add( csv_SkillPage_NORMAL );
        csv_SkillPage_ALL.Add( csv_SkillPage_ADD_EFF );

        csv_ItemPage_ALL.Add( csv_ItemPage_Consume );
        csv_ItemPage_ALL.Add( csv_ItemPage_Equip );
        csv_ItemPage_ALL.Add( csv_ItemPage_Unique );
        csv_ItemPage_ALL.Add( csv_ItemPage_Collect );
    }

    // 전역 공통 수치 레벨링 계산
    // 인자 : 1레벨 기준 수치 , 현재 레벨
    static public int PrcValue_LEVEL( int src , int lv )
    {
        // 최대레벨 기준 비율
        float r_lv = (float)lv / (float)csv_Config.level_max_normal;
        float max_val = (float)src * csv_Config.level_MAX_Value_per;
        return (int)(max_val * r_lv);
    }

    // 레벨링 강함기준만 체크
    // 0~1 값을 강함 비율로 계산.
    static public float Calc_LEVEL_Pow( float ratio , float src_min )
    {
        float max = src_min * csv_Config.level_MAX_Value_per;
        return Mathf.Lerp( src_min , max , ratio );
    }

    // 레벨 대비 증가 비율
    // 자원
    static public float IncRatioLevel_Resource( int lv )
    {
        return Mathf.Pow( lv , csv_Config.level_ResourcesArgGlobal );
    }

    // 강함
    static public float IncRatioLevel_Pow( int lv )
    {
        return Mathf.Pow( lv , csv_Config.level_PowArgGlobal );
    }

    // 확률표

    static public int GetPerIdx( Mng_X128SS rd , string tag , ref int max_arg ){return csv_PercentInfPage.GetPerIdx( rd , tag , ref max_arg );}
    static public int GetPerIdx( Mng_X128SS rd , string tag)
    {
        int max_arg = 0;
        return GetPerIdx( rd , tag , ref max_arg );
    }

    // 희소 등급
    static public int GetPerIdx_UniqueGrade( Mng_X128SS rd ){return GetPerIdx( rd , "PER_UNIQUE_GRADE" );}

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

    // 10으로 나누고 +1
    // 레벨1 :  1 
    // 10 : 2
    // 20 : 3
    static public int LevelToGrade( int lv )
    {
        return lv / 10 + 1;
    }


    // 도시 등급 -> 시작 레벨 , 종료 레벨
    // 지금은 일단 클래스 등급이랑 같게
    // 현재 도시등급은 5가지 촌락,소,중,대,수도
    // 도시 등급 * 10

    // 도시 등급은 역순으로 수도 : 0  , 촌락 : 4
    // 
    static public void CityTier_To_Level( int city_tire , ref int level_start , ref int level_end )
    {
        switch( city_tire )
        {
            // 4 : 촌락 , 1~9
            case 4:
                {
                    level_start = 1;
                    level_end = 9;
                }
                break;
            case 3:
                {
                    level_start = 10;
                    level_end = 19;
                }
                break;

            case 2:
                {
                    level_start = 20;
                    level_end = 29;
                }
                break;
            case 1:
                {
                    level_start = 30;
                    level_end = 39;
                }
                break;
            case 0:
                {
                    level_start = 40;
                    level_end = 49;
                }
                break;
        }
    }

    // 위 함수의 랜덤 범위 
    static public int RandomLevel_CityTire( Mng_X128SS rd , int city_tire )
    {
        int min_lv = 0; 
        int max_lv = 0;
        GTF_CSV.CityTier_To_Level( city_tire , ref min_lv , ref max_lv );
        return rd.NextInt( min_lv , max_lv );
    }

    // 레벨 -> 가격 
    
    // 레벨 -> 적군 보상 가격
    static public int LevelTo_EnemyRewardGold( int lv , EnemyRarityGrade rarityGrade )
    {
        return 0;
    }
}
