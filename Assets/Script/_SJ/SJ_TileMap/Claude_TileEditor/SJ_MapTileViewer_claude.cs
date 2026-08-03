using System.Collections.Generic;
using System.Linq;
using UnityEngine;





public class SJ_MapTileViewer_claude : SJ_MapTileViewer
{
    // 사용하려는 바이너리 파일의 확장자를 기존 형태에서 .bytes로 변경합니다 (예: data.bin -> data.bytes).
    public TextAsset textAsset;
    public TilemapTool.TileMapSettings settings = new();
    public List<TilemapTool.TileLayer> layers = new();

    public List<TilemapTool.TilePalette> claudePalette_s;

    // 타일 인덱스 -> 위치 찾기 위한 클래스 
    public class _LAYER_TILE_IDX_TO_POS
    {
        public Dictionary<int,HashSet<Vector2Int> > dic_TileIndex_Pos = new();
        public void Clear()
        {
            dic_TileIndex_Pos.Clear();
        }

        public void Add( int tile_idx , Vector2Int pos )
        {
            HashSet<Vector2Int> hs = null;
            if( dic_TileIndex_Pos.TryGetValue( tile_idx , out hs ) == false)
            {
                hs = new();
                dic_TileIndex_Pos[tile_idx] = hs;
            }
            hs.Add( pos );
        }

        public HashSet<Vector2Int> Find( int tile_idx )
        {
            HashSet<Vector2Int> hs = null;
            dic_TileIndex_Pos.TryGetValue( tile_idx , out hs );
            return hs;
        }

        public Vector2Int FindOne( int tile_idx )
        {
            HashSet<Vector2Int> hs = Find( tile_idx );
            if( hs != null && hs.Count > 0 )
            {
                Vector2Int[] arr = hs.ToArray();
                return arr[0];
            }
            return new Vector2Int( -1 , -1 );
        }
    }

    public List<_LAYER_TILE_IDX_TO_POS> Layer_TileIdxToPos = new();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if( mapData.Count < 1 )
        {
            LoadClaudeFile();
            AlignTileCoord();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("로드 클로드 맵")]
    public void MENU_Load()
    {
        LoadClaudeFile();
        ClaudeInstMap();
        AlignTileCoord();
    }

    public void LoadClaudeFile()
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
        Clear_MapLayer();
        Layer_TileIdxToPos.Clear();
        foreach( var s in layers )
        {
            SJ_RawMap2D map2D = NewLayer();
            _LAYER_TILE_IDX_TO_POS layer_tile_idx = new();
            Layer_TileIdxToPos.Add( layer_tile_idx );
            map2D.layerName = s.layerName;
            Load_SJ_RawMap2D( map2D , s , layer_tile_idx );
        }
    }

    public void ClaudeInstMap()
    {
        // 일단 등록 순서대로 아이디
        // useValue 는 무조건 0---> 증가
        Clear_PrefabPalette();
        foreach( var s in claudePalette_s )
        {
            Palette_PREFAB_TILE palette = Add_Palette();
            foreach( var tile in s.entries )
            {
                palette.Add_Prefab( tile.prefab );
            }
        }

        ClearTileInst();
        MakeTilePrefab();
        MakeWallPrefab();
    }


    public void Load_SJ_RawMap2D( SJ_RawMap2D rawMap2D , TilemapTool.TileLayer tileLayer_cl , _LAYER_TILE_IDX_TO_POS layer_tile )
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

            layer_tile.Add( placement.userValue , new Vector2Int( placement.x , placement.z ) );
        }
    }

    public TilemapTool.ObjectPlacement GetObjectPlacement( int layer , Vector2Int pos )
    {
        if( layers.Count <= layer ) return null;
        TilemapTool.TileLayer tileLayer = layers[layer];
        return tileLayer.Get( pos.x , pos.y );
    }

     public List<TilemapTool.ObjectPlacement> GetObjectPlacement_ByTileIdx( int layer , int tile_idx )
    {
        List<TilemapTool.ObjectPlacement> lt = new();
        if( Layer_TileIdxToPos.Count <= layer ) return lt;

        HashSet<Vector2Int> hs = Layer_TileIdxToPos[layer].Find( tile_idx );
        if( hs != null )
        {
            foreach( var s in hs )
            {
                TilemapTool.ObjectPlacement tile_obj_pl = GetObjectPlacement( layer , s );
                if( tile_obj_pl != null )
                {
                    lt.Add( tile_obj_pl );
                }
            }
        }
        return lt;
    }
}
