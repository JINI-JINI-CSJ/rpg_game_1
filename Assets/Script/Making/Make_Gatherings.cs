using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모임 , 행사 
/// 도시에 배정
/// - 올림픽 :  수도 국가 전용
/// - 전쟁 : 대도시 이상 , 랜덤
/// - 무투회 :  중도시 이상
/// - 인공던전: 대도시 이상
/// - 경매장 :  대도시 이상
/// - 암시장 :  중도시 이상
/// 
/// 기타 행사는 계속 아이디어 내자.
/// 
/// 
/// </summary>

public class Make_Gatherings : MakeBase
{
    static public Make_Gatherings G;

    Make_Gatherings()
    {
        G = this;
    }

    public override void OnMake()
    {
        // 수도 : 거의 모든 행사 다 만들기
        
        // 수도권 주요 행사들의 턴 배치를 너무 겹치지 않게 하자.
        // 행사종류마다 턴 간격을 정하고 배치 , 큰 행사일수록 턴이 길다.

        // 원하는 턴 계산 : 도시별 턴 = 원하는 턴 * 도시 개수
        // 남은 턴 = 원하는 턴 * 도시 인덱스

        // csv 는 나중에 정하자.

        // 일정턴 국가 배분 아주 큰 행사들
        Make_Common_Regular( typeof( Gatherings_Olympics )          , 0 , 500 , 10 , true );  // 국가 올림픽
        Make_Common_Regular( typeof( Gatherings_BattleTournament )  , 0 , 400 , 7  , true );  // 국가 무투회
        Make_Common_Regular( typeof( Gatherings_AuctionHouse )      , 0 , 200 , 5  , true );  // 국가 경매장 , 일반 경매장은 나중에

        // 그외 행사들 , 각각에 특성에 맞게 생성 및 배치

        // 수도 도시 미만급 
        // 턴 관련 도시 규모로 일정하게 하는 행사들
        // 일괄적으로 암시장 , 전쟁같은 비정규를 제외하고 만듬

        // 일반 무투회 : 대 ,중
        Make_Common_Regular_CSV_List( typeof( Gatherings_BattleTournament ) , 1 , 2 );

        // 일반 경매장 : 대 ,중
        Make_Common_Regular_CSV_List( typeof( Gatherings_AuctionHouse ) , 1 , 2 );

        // 지방 음식 행사 : 소 , 촌
        Make_Common_Regular_CSV_List( typeof( Gatherings_FoodEvent ) , 3 , 4 );


        // 암시장 , 완전 랜덤 



    }

    // 완전 랜덤
    // 
    void Make_Common_Irregular(System.Type type_gathering , int wait_min , int wait_max  , int keep )
    {
        
    }

    // 도시 등급 범위로 목록 만들기 , csv 참고하여 만들기
    void Make_Common_Regular_CSV_List( System.Type type_gathering , int city_tier_start , int city_tier_end )
    {
        for( int i = city_tier_start ; i < city_tier_end + 1 ; i++ )
        {
            float city_per = GTF_CSV.csv_Config.Random_gatherings_ListTier_CityPer( GTF_Random.rd_make_common , i );
            Make_Common_Regular( type_gathering , i , 0 , 0 , false , city_per );
        }
    }

    // 정기 행사 고정 턴
    // turn_want , turn_keep : 값이 0 이하이면 csv 랜덤
    // turn_align 
    // -> true : 각 도시가 일정한 턴 배분
    // -> false : 완전 랜덤

    // city_ratio : 전체 도시중 행사 도시 비율 , 1이면 전부 (100%)
    void Make_Common_Regular( System.Type type_gathering , int city_tier , int turn_want , int turn_keep , bool turn_align = true , float city_ratio = 1.0f )
    {
        Mng_X128SS rd = GTF_Random.rd_make_common;        

        if( turn_want < 1 )
        {
            turn_want = GTF_CSV.csv_Config.Random_gatherings_ListTier_waitTurn( rd , city_tier );
        }

        if( turn_keep < 1 )
        {
            turn_keep = GTF_CSV.csv_Config.Random_gatherings_ListTier_KeepUpTurn( rd , city_tier );
        }
  
        // 원하는 턴 
        int start_wait_turn = rd.NextInt( 1 , turn_want );
        List<City> cities_cp = Make_WorldMap.GetCities_Tire_Copy( city_tier );
        int city_turn = turn_want * cities_cp.Count;

        int total_city = cities_cp.Count;

        int idx = 0;
        while( true )
        {
            if( cities_cp.Count < 1 )
            {
                break;
            }

            City city = rd.RandomList( cities_cp , true );
            GatheringsBase gatherings = SJ_CSharpUtil.NewClass_Type( type_gathering ) as GatheringsBase;
            gatherings.Make( rd.NextUInt() , city );
            gatherings.waitTurn = city_turn;
            gatherings.keepUpTurn = turn_keep;
            if( turn_align )
                gatherings.cur_waitTurn = idx * turn_want + start_wait_turn;        // 전체 도시 별로 적당히 분배
            else    
                gatherings.cur_waitTurn = rd.NextInt( 1 , turn_want );              // 모든 도시 랜덤
            idx++;

            // 행사 / 도시 비율 넘어가면 중지
            if( (int)city_ratio != 1 )
            {
                float cur_per = (float)idx / (float)total_city;
                if( cur_per >= city_ratio ) break;
            }
        }
    }


    
}
