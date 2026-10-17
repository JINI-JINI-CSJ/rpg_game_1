using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// 제한 갯수등을 고정하자.(도시 갯수 ,유니크 갯수 등등)

public class Make_Global : MakeBase
{
    static public Make_Global G;

    public int total_city; // 

    // 티어 별로 총 도시 계산 , 5단계
    public List<int> total_city_tier = new();

    // 미리 계산 정규화
    public List<float> normalize_total_city_ratio = new();

    // 도시 특수 태그
    public StockMakeInf stock_CitySpcTag = new();


    // 던전 타입별 개수
    public List<int> total_dungeon_type = new();

    // 정규화
    public List<float> normalize_total_dungeon_type = new();

    //===========================================
    // 유니크 캐릭터 , 총 갯수 등급별 갯수
    public int uniqueStep_GradeChr;


    //===========================================
    // 유니크 아이템 , 총 갯수 등급별 갯수
    public int uniqueStep_GradeItem;


    Make_Global()
    {
        G = this;
    }

    override public void OnMake()
    {
        stock_CitySpcTag.AddRange( GTF_CSV.csv_TagDefinePage.GetTagPart_Str( "TAG_TRIBE" ) );

        // 도시 갯수
        // 수도 + 대중소 
        // total_city_Nation = GTF_CSV.csv_Config.Random_WorldNation( GTF_Random.rd_make_common );
        // total_city_Major = GTF_CSV.csv_Config.Random_WorldCityMajor( GTF_Random.rd_make_common );
        // total_city_Minor = GTF_CSV.csv_Config.Random_WorldCityMinor( GTF_Random.rd_make_common );
        // total_city_Village = GTF_CSV.csv_Config.Random_WorldCityVillage( GTF_Random.rd_make_common );
        //total_city = total_city_Nation + total_city_Major + total_city_Minor + total_city_Village;

        // 총 도시 개수 , 유저 세팅 
        total_city = GTF_CSV.csv_Config.TotalCity( GTF_MakeSetting.world_size );

        // 비율 정규화 및 총 갯수
        // 도시 티어 등급
        // 수도 티어 : 0
        // 대도시 : 1
        // ...
        float cityGrade_ratio_total = 0;
        for( int i = 0 ; i < GTF_CSV.csv_Config.cityGrade_ratio.Count ; i++ ) cityGrade_ratio_total += GTF_CSV.csv_Config.cityGrade_ratio[i];
        for( int i = 0 ; i < GTF_CSV.csv_Config.cityGrade_ratio.Count ; i++ )
        {
            float src = GTF_CSV.csv_Config.cityGrade_ratio[i];
            float nor_ratio = src / cityGrade_ratio_total;
            normalize_total_city_ratio.Add( nor_ratio );
            int total_city_tier_cur = (int)((float)total_city * nor_ratio);
            total_city_tier.Add(total_city_tier_cur);
        }
        
        // 외부 던전 
        float dungeon_ratio_total = 0;
        for( int i = 0 ; i < GTF_CSV.csv_Config.dungeon_BaseType_ratio.Count ; i++ ) dungeon_ratio_total += GTF_CSV.csv_Config.dungeon_BaseType_ratio[i];
        for( int i = 0 ; i < GTF_CSV.csv_Config.dungeon_BaseType_ratio.Count ; i++ )
        {
            float src = GTF_CSV.csv_Config.dungeon_BaseType_ratio[i];
            float nor_ratio = src / dungeon_ratio_total;
            normalize_total_dungeon_type.Add( nor_ratio );
            int total_dungeon_type_cur = (int)((float)total_city * nor_ratio);
            total_dungeon_type.Add(total_dungeon_type_cur);
        }



        // 유니크 객체들
        // 다음 항목으로 나눈다.

        // 총갯수 2가지 : 일단 임의로 랜덤 비율
        // 각 등급별 갯수 = 각 총 갯수 / 레벨링 스텝        

        // 일단 총갯수
        int total_unique_all = (int)((float)total_city * GTF_CSV.csv_Config.Random_UniqueObjPer( GTF_Random.rd_make_common ));

        SJ_RANDOM_AverageStep.Clear();

        SJ_RANDOM_AverageStep.Add( 1 ); // 캐릭터 
        SJ_RANDOM_AverageStep.Add( 2 ); // 아이템
        SJ_RANDOM_AverageStep.CalcAverage( GTF_Random.rd_make_common );

        int total_unique_chr = (int)((float)total_unique_all * SJ_RANDOM_AverageStep.ResultObj( 1 ));
        int total_unique_item = (int)((float)total_unique_all * SJ_RANDOM_AverageStep.ResultObj( 2 ));

        // 각 단계 평균 유니크 객체
        uniqueStep_GradeChr = total_unique_chr / GTF_CSV.csv_Config.LevelStepUnique_Total();
        uniqueStep_GradeItem = total_unique_item / GTF_CSV.csv_Config.LevelStepUnique_Total();
    }   

    public float BiasCityNormalSpc()
    {
        return GTF_CSV.csv_Config.Random_WorldCitySpcPer( GTF_Random.rd_make_common );
    }
}
