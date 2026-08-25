using System.Linq;
using UnityEngine;


// 제한 갯수등을 고정하자.(도시 갯수 ,유니크 갯수 등등)

public class Make_Global : MakeBase
{
    static public Make_Global G;

    public int total_city; // 

    public int total_city_Nation;
    public int total_city_Major;
    public int total_city_Minor;
    public int total_city_Village;

    // 도시 특수 태그
    public StockMakeInf stock_CitySpcTag = new();


    //===========================================
    // 유니크 캐릭터 , 총 갯수 등급별 갯수
    public int uniqueStep_GradeChr;


    //===========================================
    // 유니크 아이템 , 총 갯수 등급별 갯수
    public int uniqueStep_GradeItem;


    void Awake()
    {
        G = this;
    }

    override public void OnMake()
    {
        stock_CitySpcTag.AddRange( GTF_CSV.csv_TagDefinePage.GetTagPart_Str( "TAG_TRIBE" ) );

        // 도시 갯수
        // 수도 + 대중소 
        total_city_Nation = GTF_CSV.csv_Config.Random_WorldNation( GTF_Random.rd_make_world );
        total_city_Major = GTF_CSV.csv_Config.Random_WorldCityMajor( GTF_Random.rd_make_world );
        total_city_Minor = GTF_CSV.csv_Config.Random_WorldCityMinor( GTF_Random.rd_make_world );
        total_city_Village = GTF_CSV.csv_Config.Random_WorldCityVillage( GTF_Random.rd_make_world );

        total_city = total_city_Nation + total_city_Major + total_city_Minor + total_city_Village;

        // 유니크 객체들
        // 다음 항목으로 나눈다.

        // 총갯수 2가지 : 일단 임의로 랜덤 비율
        // 각 등급별 갯수 = 각 총 갯수 / 레벨링 스텝        

        // 일단 총갯수
        int total_unique_all = (int)((float)total_city * GTF_CSV.csv_Config.Random_UniqueObjPer( GTF_Random.rd_make_world ));

        SJ_RANDOM_AverageStep.Clear();

        SJ_RANDOM_AverageStep.Add( 1 ); // 캐릭터 
        SJ_RANDOM_AverageStep.Add( 2 ); // 아이템
        SJ_RANDOM_AverageStep.CalcAverage( GTF_Random.rd_make_world );

        int total_unique_chr = (int)((float)total_unique_all * SJ_RANDOM_AverageStep.ResultObj( 1 ));
        int total_unique_item = (int)((float)total_unique_all * SJ_RANDOM_AverageStep.ResultObj( 2 ));

        // 각 단계 평균 유니크 객체
        uniqueStep_GradeChr = total_unique_chr / GTF_CSV.csv_Config.LevelStepUnique_Total();
        uniqueStep_GradeItem = total_unique_item / GTF_CSV.csv_Config.LevelStepUnique_Total();
    }   

    public float BiasCityNormalSpc()
    {
        return GTF_CSV.csv_Config.Random_WorldCitySpcPer( GTF_Random.rd_make_world );
    }
}
