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

// BATTLE_TOURNAMENT

public class Gatherings_BattleTournament : GatheringsBase
{
    // 최대 파티 인원 , 1~6 , 일단 이것도 도시 규모로..
    //public int chrNum_BattleParty;

    // 침여 파티 수 , 8~128 강 정도 , 도시 티어 0~4 순서대로 8,16,32,64,128 ( 2 의 n 승 , n 도시 티어 , 1 , 2 , 4 , 8 , 16  )
    public int total_BattleParty;

    // 메이킹 파티 , 대회 때 마다 랜덤으로 대진표 짠다.
    public  List<BattleParty> list_BattleParty = new();


    // 토너먼트 페어 구조체
    public class Tournament_Pair
    {
        public Tournament_Pair node_par; // 상위 노드 , 없으면 결승전
        public int RoundOfNum;  // 16 강 , 8 강 , 4 강 등등

        public int teamLeft = -1;    //  list_BattleParty 의 인덱스
        public int teamRight = -1;   

        public List<Tournament_Pair> node_child = new();

        public int result_win;  // 이긴팀 ,  0 : 미 결정 , 1 : 왼쪽 , 2 : 오른쪽

        public void Clear()
        {
            teamLeft = -1;
            teamRight = -1;
            result_win = 0;
        }

        // 

        public void CreateNode(  int max_RoundOfNum , Tournament_Pair _par = null , int cur_RoundOfNum = 2 )
        {
            node_par = _par;
            RoundOfNum = cur_RoundOfNum;
            int next_RoundOfNum = RoundOfNum * 2;
            if( next_RoundOfNum > max_RoundOfNum ) return;
            for( int i = 0 ; i < 2 ; i++ )
            {
                Tournament_Pair tp = new();
                tp.CreateNode( max_RoundOfNum , this , next_RoundOfNum );
                node_child.Add( tp );
            }
        }

        // 마지막 노드 ( 시작 라운드 강 ) 가져오기
        public void List_LastNode( List<Tournament_Pair> lt )
        {
            if( node_child.Count < 1 ) lt.Add(this);
            foreach( var s in node_child )
            {
                s.List_LastNode( lt );
            }
        }
    }

    public Tournament_Pair tournament_Pair_root;

    // 
    override public void OnMake()
    {
        // 파티 인원 도시 등급에 비례 : 보류
        //chrNum_BattleParty = 6 - city.cityData.Tier;

        // 도시 등급에 따라 참여파티 수 : (1) 2 , (2) 4 , (3) 8 , (4) 16 , (5) 32 , (6) 64 , (7) 128
        // 촌락 : 2 ~~~~ 수도 : 7 
        int pow_val = GTF_CSV.CityTier_To_Grade( city.cityData.Tier ) + 2;
        total_BattleParty = (int)Mathf.Pow( 2 , pow_val );

        
        int boss_num = 0;   // 보스 개수
        int elite_num = 0;  // 엘리트

        
        // 중도시 까지 없음
        // 대도시 : 1
        // 수도 : 2 
        if( city.cityData.Tier == GTF_Define.CITY_TIER_CAPITAL )
        {
            boss_num = 4;
            elite_num = 20;
        }
         
        if( city.cityData.Tier == GTF_Define.CITY_TIER_BIG )
        {
            boss_num = 1;
            elite_num = 10;
        } 

        if( city.cityData.Tier == GTF_Define.CITY_TIER_MID )
        {
            elite_num = 5;
        } 

        if( city.cityData.Tier == GTF_Define.CITY_TIER_SMALL )
        {
            elite_num = 2;
        } 

        // 촌락은 대회가 없는 걸로?



        // 총파티에서 1 빼고 만들기 (주인공 파티)
        // 인간형 , 엘리트 또는 보스 1~2 정도 , 보스는 단기전 용으로 만들자. 무투회는 전투 시간이 너무 길면 안될꺼 같다.
        // 회복 : 각 경기 대략 20 ~ 50퍼 정도? , 결승은 완전 회복

        // 전역 태그는 "토너먼트"
        string tagTheme = "BATTLE_TOURNAMENT";
        for( int i = 0 ; i < (total_BattleParty-1) ; i++ )
        {
            BattleParty battleParty = new();
            if( boss_num > 0 )
            {
                boss_num--;
                battleParty.MakeEnemyList_Simple( rd_self , city.RandomLevel_CityTire( rd_self ) , tagTheme , EnemyRarityGrade.Boss );
            }
            else if( elite_num > 0 )
            {
                elite_num--;
                battleParty.MakeEnemyList_Simple( rd_self , city.RandomLevel_CityTire( rd_self ) , tagTheme , EnemyRarityGrade.Elite );
            }
            else
            {
                battleParty.MakeEnemyList_Simple( rd_self , city.RandomLevel_CityTire( rd_self ) , tagTheme , EnemyRarityGrade.Normal );
            }
            list_BattleParty.Add( battleParty );
        }

        MakeRoundOf();
    }

    void MakeRoundOf()
    {
        if( tournament_Pair_root == null )
        {
            tournament_Pair_root = new();
            tournament_Pair_root.CreateNode( total_BattleParty );
        }
    }


}
