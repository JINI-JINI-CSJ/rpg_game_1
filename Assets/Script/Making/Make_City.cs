using System.Collections.Generic;
using QuadTreeSystem;
using UnityEngine;
using WorldForge;

// 도파민 위주로 하자.
// 현재 소문은 이걸로만 할까?
// 알려지지 않은 도시 던전 , 특수 도시 던전
// 특수 도시 : 특정 종족 용병 동료 , 특정 아이템을 제한 수량( 예) 세계수의 잎등등  )
// 특수 던전 : 입장 조건? , 특수한 날에만 입장가능? , 갱신 될때 마다 새로운 던전 및 새로운 보상? 변형 던전


// 도시
// 스타팅 도시 , 등급 , 레벨 차등
// 월드 메이커에서 도시 등급이 정해져 있으니 , 대략 소도시 중 한개 랜덤해서 스타팅
// 이론상 모든 도시를 다 갈수 있으나 


// 1. 시작도시 , 시작 알려진 도시 타입 
// 2. 알져지지 않은 도시 , 특수 도시 
// 3. 알려지지 않은 던전 , 특수 던전
// ?3. 동료 아이템 퀘스트 등등 배분
// 



public class Make_City : MakeBase
{
    // 처음부터 보일 도시들의 깊이 제한
    public int depth_start_active = 2;

    public City city_Start;

    // 특수 도시 , 

    public override void OnMake()
    {
        WorldForgeManager worldForge = Make_WorldMap.G.worldForge;

        // 시작지점 , 소마을 티어중에 한개1
        QuadTree quadTree = Make_WorldMap.G.quadTree;
        List<QTPoint> qTs = quadTree.GetAllPoints( Make_WorldMap.TAG_HASH_CITY_Village() );
        QTPoint qt_s = GTF_Random.rd_make_world.RandomList( qTs );
        city_Start = (City)qt_s.Data;


        // 도시 이웃
        foreach (var (a, b) in worldForge.CurrentWorld.Roads)
        {
            City city_a = Make_WorldMap.GetCity_WorldIdx( a );
            City city_b = Make_WorldMap.GetCity_WorldIdx( b );
            city_a.AddNeighbor(city_b);
            city_b.AddNeighbor(city_a);
        }

        // 도시 활성 타입
        // 1. 시작점 및 수도권 지역 기본 활성
        foreach( var s in Make_WorldMap.G.GetCities_Tire(CityTier.Capital) )
        {
            s.SetDepthNeighbor_CITY_ACTIVE_TYPE( CITY_FIND_TYPE.FIND_FIRST , depth_start_active );
        }
        city_Start.SetDepthNeighbor_CITY_ACTIVE_TYPE( CITY_FIND_TYPE.FIND_FIRST , depth_start_active );


        // 2. 일반 도시 및 특수 발견 도시 나누기
        //   - 수도권에서 먼 순서로 정렬하고 , 그중에 비율로 특수 도시
        //   - 1. 계산 : 가장 가까운 수도건 찾아서 거리 저장
        //   - 2. 정렬 : 장거리 -> 근거리 정렬
        // 후보들 , CITY_ACTIVE_TYPE.FIND_FIRST 아닌 것들 , 소도시만 해당
        List<City> cities_no_FIND_FIRST = new();
        foreach( var s in Make_WorldMap.G.dic_world_idx_city.Values )
        {
            if( s.city_find_type != CITY_FIND_TYPE.FIND_FIRST && s.cityData.Tier == CityTier.Village )
                cities_no_FIND_FIRST.Add(s);
        }

        List<(City,float)> city_no_find = new();
        foreach( var s in cities_no_FIND_FIRST )
        {
            var near = s.FindNear( Make_WorldMap.G.GetCities_Tire(CityTier.Capital) );
            city_no_find.Add( new( s , near.Item2 ) );
        }

        // 가장 먼것이 앞으로
        city_no_find.Sort( (a,b) => -a.Item2.CompareTo( b.Item2 ) );

        // 1. 비율 분배해서 남은거 특수 도시
        // 2. 그 외에 특수 퀘스트 및 특수 아이템 등
        int total_spc_city = (int)((float)city_no_find.Count * Make_Global.G.BiasCityNormalSpc());

        // 총 특수 도시를 다 하거나 , 특수 태그가 다 없어질때까지.
        // 여기에 해당하는 도시들은 CITY_FIND_TYPE.NO_FIND 이다
        for( int i = 0 ; i < total_spc_city; i++ )
        {
            if( city_no_find.Count < 1 ) break;
            if( Make_Global.G.stock_CitySpcTag.Count() < 1 ) break;

            City city = city_no_find[0].Item1;
            city.city_find_type = CITY_FIND_TYPE.NO_FIND;
            city.tag_SpcCity = Make_Global.G.stock_CitySpcTag.RandomPop_Str( GTF_Random.rd_make_world );
            city_no_find.RemoveAt(0);
        }

        MakingMain.NextMake();
    }
}
