using UnityEngine;
using QuadTreeSystem;
using WorldForge;


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

    // 월드 스폿 태그 
    // 도시 
    public const string CITY_Village = "CITY_Village";
    public const string CITY_Minor = "CITY_Minor";
    public const string CITY_Major = "CITY_Major";
    public const string CITY_Capital= "CITY_Capital";

    // 일반 스폿 , 월드 메이커에서는 타입이 있지만 여기서는 통합
    public const string SPOT_BASE = "SPOT_BASE";


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

    public override void OnSave()
    {
        
    }

    public override void OnLoad()
    {
        InitQuadTree();
    }

    static public int TAG_HASH_CITY_Village(){return CITY_Village.GetHashCode();}
    static public int TAG_HASH_CITY_Minor(){return CITY_Minor.GetHashCode();}
    static public int TAG_HASH_CITY_Major(){return CITY_Major.GetHashCode();}
    static public int TAG_HASH_CITY_Capital(){return CITY_Capital.GetHashCode();}
    static public int TAG_HASH_SPOT_BASE(){return SPOT_BASE.GetHashCode();}

    public void InitQuadTree()
    {
        Debug.Log( "월드 시티 : " + worldForge.CurrentWorld.Cities.Count );

        // 월드 메이커는 0 위치 시작 , 넓이 인자로 되 있다.
        quadTree = new QuadTree( Vector2.zero , new Vector2( worldForge.CurrentWorld.Width , worldForge.CurrentWorld.Height ) );

        int hs_CITY_Village = TAG_HASH_CITY_Village();
        int hs_CITY_Minor   = TAG_HASH_CITY_Minor();
        int hs_CITY_Major   = TAG_HASH_CITY_Major();
        int hs_CITY_Capital = TAG_HASH_CITY_Capital();
        int hs_SPOT_BASE    = TAG_HASH_SPOT_BASE();

        foreach( var s in worldForge.CurrentWorld.Cities )
        {
            switch( s.Tier )
            {
                case CityTier.Village:InsertQuad_City( s , hs_CITY_Village );break;
                case CityTier.Minor:    InsertQuad_City( s , hs_CITY_Minor );break;
                case CityTier.Major:    InsertQuad_City( s , hs_CITY_Major );break;
                case CityTier.Capital:  InsertQuad_City( s , hs_CITY_Capital );break;
            }
        } 

        foreach( var s in worldForge.CurrentWorld.Spots )
        {
            InsertQuad_Spot( s , hs_SPOT_BASE );
        }
    }

    void InsertQuad_City( CityData cityData , int tag_hash )
    {
        Vector2 pos = new Vector2( cityData.X , cityData.Y );
        quadTree.Insert( pos , tag_hash , cityData );
    }

    void InsertQuad_Spot( SpotData spotData , int tag_hash )
    {
        Vector2 pos = new Vector2( spotData.X , spotData.Y );
        quadTree.Insert( pos , tag_hash , spotData );
    }

}
