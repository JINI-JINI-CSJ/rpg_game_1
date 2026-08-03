using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        G = this;
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
        if( G == null )
        {
            Debug.LogError( "주 타일맵 없음" );
            return;
        }
        G._LoadMap();
    }

    public void _LoadMap()
    {
        MENU_Load();
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
