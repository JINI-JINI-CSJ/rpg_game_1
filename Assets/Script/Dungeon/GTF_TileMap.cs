using System.Collections.Generic;
using UnityEditor.SettingsManagement;
using UnityEngine;

/// <summary>
/// 맵 한장 기본 단위
/// 리소스는 이 객체에 등록하자. 던전 정보의 res 
/// 맵을 불러오거나 , 자동으로 만들기
/// </summary>

// 타일맵 
// 기존 타일 뷰어에서 레이어 1이상 데이터를 가져온다.
// 레이어 0 : 지형 : 타일 0 만 사용
// 레이어 1 : 포털 등등 중요 객체 , 타일 1~10
// 레이어 2 : 이벤트 11~20

public class GTF_TileMap : SJ_MapTileViewer_claude
{
    static public GTF_TileMap G;
    public MapEventPlayer mapEvent;
    public DungeonInfo dungeonInfo = new();
    public Mng_X128SS random_dungeon;

    public Material mat_skyBox;

    void Awake()
    {
        G = this;
    }

    static public GTF_TileMap CurTileMap()
    {
        return G;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void LoadMap()
    {
        G?._LoadMap();
    }

    public void _LoadMap()
    {
        random_dungeon = new( (uint)dungeonInfo.randomSeed );
        MENU_Load();

        if( mat_skyBox != null )
        {
            RenderSettings.skybox = mat_skyBox;
        }
    }

    static public Mng_X128SS Random()
    {
        return G.random_dungeon;
    }

    public Vector2Int PlayerStartPos()
    {
        // 레이어 1번의 0번 인덱스 찾기
        List<TilemapTool.ObjectPlacement> lt = GetObjectPlacement_ByTileIdx( 1 , 0 );
        if( lt.Count < 1 )
        {
            // 입구가 없다.
            Debug.LogError( "입구 타일 없다!!!" );
        }

        return lt[0].Vector2Int();
    }
}
