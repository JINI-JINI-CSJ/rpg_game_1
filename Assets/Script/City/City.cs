using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using WorldForge;

public enum CITY_FIND_TYPE
{
    None , 
    FIND_FIRST ,    // 처음부터 알려짐
    FIND_NEAR ,     // 근처 도착하면 알려짐 , 이웃 도시
    NO_FIND ,       // 힌트등이 없으면 알수 없음
}

public class City 
{
    public uint ID;
    public int idx_world_data;  // 월드 매니저의 인덱스
    public CityData cityData;

    // 이웃 발견가능이 디폴트
    // 처음부터 알려짐 , 완전 숨겨짐은 따로 계산한다.
    public CITY_FIND_TYPE city_find_type = CITY_FIND_TYPE.FIND_NEAR;
    // 연결된 도시
    public HashSet<City> cities_Neighbor = new();

    // 특수도시 일 경우 해당 태그
    public string tag_SpcCity;      

    // 도시 규모에 따른 시설들
    public List<CityPartBase> cityParts = new();

    // 의뢰
    public _BIAS_MISSION bias_mission;

    // 용병 : 직업 경향  , 마법경향 
    // 더 세부적이라면 각 직업들 가중치
    public Making_Char  making_Char;


    // 도시 전용 퀘스트
    // 인물 , 아이템 등등 보상
    public List<MissionBase> mission_Unique;


    // 상태
    public bool find_cur;
    
    public void AddNeighbor( City city ){cities_Neighbor.Add(city);}

    public void SetDepthNeighbor_CITY_ACTIVE_TYPE( CITY_FIND_TYPE type , int max_depth , int cur_depth = 0 )
    {
        city_find_type = type;
        cur_depth++;
        if( cur_depth >= max_depth ) return;
        foreach( var s in cities_Neighbor )
        {
            SetDepthNeighbor_CITY_ACTIVE_TYPE( type , max_depth , cur_depth );
        }
    }

    public Vector2 GetPos()
    {
        return new Vector2( cityData.X , cityData.Y );
    }

    public float DistSqCity( City city )
    {
        Vector2 v = GetPos() - city.GetPos();
        return v.sqrMagnitude;
    }

    public (City,float) FindNear( List<City> cities )
    {
        if( cities.Count < 1 ) return default;
        List<(City,float)> lt = new();
        foreach( var s in cities )
        {
            float sq = DistSqCity( s );
            lt.Add( (s , sq) );
        }
        lt.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        return lt[0];
    }


}
