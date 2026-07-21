using System.Collections.Generic;
using UnityEngine;

public class SJ_MapTileViewer_claude : SJ_MapTileViewer
{
    public TextAsset textAsset;
    public TilemapTool.TileMapSettings settings = new();
    public List<TilemapTool.TileLayer> layers = new();
    public TilemapTool.TilePalette tilePalette;
    //public List<SJ_RawMap2D> rawMap2D_layer = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("로드 클로드 맵")]
    public void MENU_Load()
    {
        LoadClaude();
    }

    public void LoadClaude()
    {
        if( textAsset == null )
        {
            Debug.Log( " textAsset == null " );
            return;
        }
        TilemapTool.TilemapBinaryIO.LoadData_TextAsset( textAsset , settings , out layers );

        if( layers.Count < 1 )
        {
            Debug.Log( " layers.Count < 1 " );
            return;   
        }

        // 일단 등록 순서대로 아이디
        // useValue 는 무조건 0--->
        Clear_PrefabPalette();
        foreach( var s in tilePalette.entries )
        {
            Add_PrefabPalette( s.prefab );
        }

        Clear_MapLayer();

        foreach( var s in layers )
        {
            SJ_RawMap2D map2D = NewLayer();
            Load_SJ_RawMap2D( map2D , s );
        }

        ClearTileInst();
        MakeTilePrefab();
        MakeWallPrefab();
    }



    public void Load_SJ_RawMap2D( SJ_RawMap2D rawMap2D , TilemapTool.TileLayer tileLayer_cl )
    {
        foreach( var s in tileLayer_cl.placements )
        {
            TilemapTool.ObjectPlacement placement = s.Value;

            SJ_COMMON.SJ_NEWS_DIR news_dir = SJ_COMMON.SJ_NEWS_DIR.None;
            switch( placement.direction )
            {
                case TilemapTool.TileDirection.North:   news_dir = SJ_COMMON.SJ_NEWS_DIR.N;break;
                case TilemapTool.TileDirection.East:    news_dir = SJ_COMMON.SJ_NEWS_DIR.E;break;
                case TilemapTool.TileDirection.South:   news_dir = SJ_COMMON.SJ_NEWS_DIR.S;break;
                case TilemapTool.TileDirection.West:    news_dir = SJ_COMMON.SJ_NEWS_DIR.W;break;
            }

            rawMap2D.AddRawTile( placement.x , placement.z , placement.userValue , news_dir );
        }
    }
}
