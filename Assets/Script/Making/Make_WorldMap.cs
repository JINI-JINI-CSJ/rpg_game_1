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
    //public const string SPOT_BASE = "SPOT_BASE";

    // 쿼드트리 해쉬 태그
    public const int HS_TAG_SPOT = 10000;

    // 아이디 , 도시
    public Dictionary<int, City>            dic_world_idx_city = new();
    // 등급별 정리
    public Dictionary<int,List<City>>       dic_grade_City = new();

    public List<DungeonInfo>                dungeonInfos = new();
    


    void Awake()
    {
        G = this;
    }

    public override void OnMake()
    {
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

        //int hs_SPOT_BASE    = TAG_HASH_SPOT_BASE();

        // 도시
        for( int i = 0 ; i < worldForge.CurrentWorld.Cities.Count ; i++ )
        {
            CityData city = worldForge.CurrentWorld.Cities[i];
            InsertQuad_City( city , i );
        } 

        // 던전
        foreach( var s in worldForge.CurrentWorld.Spots )
        {
            InsertQuad_Spot( s , HS_TAG_SPOT );
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
        if( dic_grade_City.TryGetValue( cityData.Tier , out lt ) == false )
        {
            lt = new();
            dic_grade_City[cityData.Tier] = lt;
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

    void InsertQuad_Spot( SpotData spotData , int tag_hash )
    {
        Vector2 pos = new Vector2( spotData.X , spotData.Y );

        DungeonInfo dungeon = new();
        dungeon.ID =  MakingMain.Make_UID( typeof(DungeonInfo) );
        dungeon.spotData = spotData;

        quadTree.Insert( pos , tag_hash , dungeon );
    }

}
