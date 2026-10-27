using UnityEngine;
using QuadTreeSystem;
using WorldForge;
using System.Collections.Generic;


/// <summary>
/// WorldForgeManager 클래스로 만들자.
/// 그 후 포인트들을 전부 쿼드트리에 넣자.
/// </summary>

public class Make_WorldMap : MakeBase
{
    public static Make_WorldMap G;
    // 인자들...
    public WorldForgeManager worldForge;

    public QuadTree quadTree;


    // 일반 스폿 , 월드 메이커에서는 타입이 있지만 여기서는 통합
    // 일단 던전으로 통합

    // 쿼드트리 해쉬 태그 , 기본 수치
    public const int HS_TAG_SPOT = 1000;

    // 인덱스 , 도시
    public Dictionary<int, City>            dic_world_idx_city = new();
    // 도시 등급별 정리
    public Dictionary<int,List<City>>       dic_tire_City = new();

    // 인덱스 , 외부 던전
    public Dictionary<int, DungeonInfo>     dic_world_idx_dungeon = new();
    // 던전 형태별 정리
    public Dictionary<DUNGEON_TYPE,List<DungeonInfo>>       dic_type_dungeon = new();

    Make_WorldMap()
    {
        G = this;
    }

    public override void OnMake()
    {
        // 계산된 총 개수로 만들기
        // 유저 초기 메이킹 세팅
        WorldGenSettings settings =  worldForge.Settings;

        // 월드 맵 사이즈
        // 일단 고정..
        settings.MapWidth = GTF_CSV.csv_Config.size_world;
        settings.MapHeight = GTF_CSV.csv_Config.size_world;


        // 국가 수
        settings.NumNations = GTF_MakeSetting.NumNations;

        // 등급별 도시
        // 대도시 -> 소도시 순으로. 수도는 0 개 , 클로드 월드 규칙이다.
        settings.CityTierDefs.Clear();
        
        // 수도 
        settings.CityTierDefs.Add( new CityTierDef( "NationCity"  , 0 , 255,255,255, 2 ) );

        // 도시 티어들
        List<int> total_city_tier = Make_Global.G.total_city_tier;
        for( int i = 0 ; i < total_city_tier.Count ; i++ )
        {
            string tier = (i+1).ToString();
            settings.CityTierDefs.Add( new CityTierDef( "City_" + tier , total_city_tier[0] , 255,255,255, 1 ) );
        }

        // 던전 
        settings.SpotTypeDefs.Clear();
        List<int> total_dungeon_type = Make_Global.G.total_dungeon_type;
        for( int i = 0 ; i < total_dungeon_type.Count ; i++ )
        {
            settings.SpotTypeDefs.Add( new SpotTypeDef( "Spot_" + i , total_dungeon_type[0] , 255,255,255 ) );
        }

        worldForge.OnWorldGenerated += OnAfterMakeWorld;
        worldForge.Generate();

        // 월드 이미지 저장
    }
    private void OnAfterMakeWorld(WorldData w)
    {
        InitQuadTree();
        MakingMain.NextMake();
    }

    public override void OnSave(){}
    public override void OnLoad()
    {
        InitQuadTree();
    }

    public void InitQuadTree()
    {
        Debug.Log( "월드 : 시티 : " + worldForge.CurrentWorld.Cities.Count + "     던전 스폿 : " + worldForge.CurrentWorld.Spots.Count );


        // 월드 메이커는 0 위치 시작 , 넓이 인자로 되 있다.
        quadTree = new QuadTree( Vector2.zero , new Vector2( worldForge.CurrentWorld.Width , worldForge.CurrentWorld.Height ) );

        // 도시
        for( int i = 0 ; i < worldForge.CurrentWorld.Cities.Count ; i++ )
        {
            CityData city = worldForge.CurrentWorld.Cities[i];
            InsertQuad_City( city , i );
        } 

        // 던전
        for( int i = 0 ; i < worldForge.CurrentWorld.Spots.Count ; i++ )
        {
            SpotData spot = worldForge.CurrentWorld.Spots[i];
            InsertQuad_Spot( spot , i );
        }
    }

    // 해쉬 태그는 등급으로 넣는다
    void InsertQuad_City( CityData cityData , int idx_world )
    {
        Vector2 pos = new Vector2( cityData.X , cityData.Y );
        City city = new();
        city.ID = MakingMain.Make_UID( typeof(City) );
        city.idx_world_data = idx_world;
        city.cityData = cityData;
        List<City> lt = null;
        if( dic_tire_City.TryGetValue( cityData.Tier , out lt ) == false )
        {
            lt = new();
            dic_tire_City[cityData.Tier] = lt;
        }
        lt.Add( city );
        dic_world_idx_city[idx_world] = city;
        quadTree.Insert( pos , cityData.Tier , city );
    }


    static public City GetCity_WorldIdx(int idx)
    {
        City city = null;
        G.dic_world_idx_city.TryGetValue( idx , out city );
        return city;
    }

    static public List<City> GetCities_Tire( int tire )
    {
        List<City> cities = null;
        G.dic_tire_City.TryGetValue( tire , out cities );
        return cities;
    }

    void InsertQuad_Spot( SpotData spotData , int idx_dungeon )
    {
        Vector2 pos = new Vector2( spotData.X , spotData.Y );

        DungeonInfo dungeon = new();
        dungeon.ID =  MakingMain.Make_UID( typeof(DungeonInfo) );
        dungeon.randomSeed = GTF_Random.rd_make_common.NextUInt();
        dungeon.spotData = spotData;

        // 해시 태그는 기본 수치 + SpotTypeId
        quadTree.Insert( pos , spotData.SpotTypeId + HS_TAG_SPOT , dungeon );

        dic_world_idx_dungeon[idx_dungeon] = dungeon;
        List<DungeonInfo> dungeonInfos = null;
        DUNGEON_TYPE dUNGEON_TYPE = DUNGEON_TYPE.None;
        dUNGEON_TYPE = (DUNGEON_TYPE)spotData.SpotTypeId;
        dungeon.dUNGEON_TYPE = dUNGEON_TYPE;
        if( dic_type_dungeon.TryGetValue( dUNGEON_TYPE , out dungeonInfos ) == false )
        {
            dungeonInfos = new();
            dic_type_dungeon[dUNGEON_TYPE] = dungeonInfos;
        }
        dungeonInfos.Add(dungeon);
    }

    static public int Get_HashTag_Spot( DUNGEON_TYPE t )
    {
        return HS_TAG_SPOT + (int)t;
    }
}
