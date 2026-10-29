using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무투회 , 토너먼트
/// 현 도시의 등급의 대전 상대를 만들고 대전
/// 대전 캐릭터만 메이킹에서 만들고 , 랜덤 대진표
/// 
/// 대전표 ui 보여주고 , 준비할 시간을 준다. 
/// 준비단계를 토너먼트 시작전 한번만 할지 , 매 대전마다 할지.. 일단 한번만.
/// 
/// </summary>

public class Gatherings_BattleTournament : GatheringsBase
{
    // 최대 파티 인원 , 1~6 , 일단 이것도 도시 규모로..
    public int chrNum_BattleParty;

    // 침여 파티 수 , 8~128 강 정도 , 도시 티어 0~4 순서대로 8,16,32,64,128 ( 2 의 n 승 , n 도시 티어 , 1 , 2 , 4 , 8 , 16  )
    public int total_BattleParty;

    // 메이킹 파티 , 대회 때 마다 랜덤으로 대진표 짠다.
    public  List<BattleParty> list_BattleParty = new();

    // 
    override public void OnMake()
    {
        // 파티 인원 도시 등급에 비례
        //chrNum_BattleParty = GTF_Random.rd_make_common.NextInt(1,7);
        chrNum_BattleParty = 6 - city.cityData.Tier;

        // 도시 등급에 따라 참여파티 수 : (1) 2 , (2) 4 , (3) 8 , (4) 16 , (5) 32 , (6) 64 , (7) 128
        int tire_num = (int)Mathf.Pow( 2 , 7 - city.cityData.Tier );
        total_BattleParty = 8 * tire_num;


        // 총파티에서 1 빼고 만들기 (주인공 파티)
        for( int i = 0 ; i < (total_BattleParty-1) ; i++ )
        {
            
        }


    }


}
