using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SJ_RawTileVal
{
    public int tile_val;
    public SJ_COMMON.SJ_NEWS_DIR news_dir;

}

// Vec2 기준으로 데이터
public class SJ_RawMap2D
{
    public string layerName;

    public int width;
    public int height;
    public Dictionary<Vector2Int,SJ_RawTileVal> dic_Tile = new();
    public bool CheckAlloc()
    {
        return true;
    }

    public void AddRawTile( int x , int y , int tile_val , SJ_COMMON.SJ_NEWS_DIR news_dir )
    {
        AddRawTile( new Vector2Int(x,y) , tile_val , news_dir );
    }

    public void AddRawTile( Vector2Int pos , int tile_val , SJ_COMMON.SJ_NEWS_DIR news_dir )
    {
        SJ_RawTileVal rawTileVal = new();
        rawTileVal.tile_val = tile_val;
        rawTileVal.news_dir = news_dir;
        dic_Tile[pos] = rawTileVal;
    }

    public SJ_RawTileVal GetTileVal( int x , int y )
    {
        return GetTileVal( new Vector2Int(x,y) );
    }

    public SJ_RawTileVal GetTileVal( Vector2Int pos )
    {
        //return GetTileVal( pos.x , pos.y );
        SJ_RawTileVal rawTileVal = null;
        dic_Tile.TryGetValue( pos , out rawTileVal );
        return rawTileVal;
    }
}

// 타일 객체 아이디
// 타일 레이어 번호 
// 위치
public class SJ_MAP_LAYER_TILE_COORD : IEquatable<SJ_MAP_LAYER_TILE_COORD>
{
    public int layer;
    public Vector2Int pos;

    public bool Equals(SJ_MAP_LAYER_TILE_COORD other)
    {
        throw new NotImplementedException();
    }

    public void Save( BinaryWriter bw )
    {
        bw.Write( layer );
        bw.Write( pos.x );
        bw.Write( pos.y );
    }

     public void Load( BinaryReader br )
    {
        layer = br.ReadInt32();
        pos.x = br.ReadInt32();
        pos.y = br.ReadInt32();
    }
}

// 배열의 값을 각각 대응하는 프리펩으로 생성
// 벽은 4방향 벽 등록
// 1. 배열대로 프리펩 생성
// 2. 벽체크하여 만들기

// 인게임 기능 : 이동 가능한 방향 체크

public class SJ_MapTileViewer : MonoBehaviour
{
    [System.Serializable]
    public class PREFAB_TILE
    {
        public List<GameObject> objects = new();
        public GameObject GetRandom( Mng_X128SS rd = null )
        {
            if( objects.Count < 1 ) return null;
            if( objects.Count == 1 ) return objects[0];
            if( rd == null )
            {
                return SJ_Unity.GetRandomItem( objects ) as GameObject;
            }
            return rd.RandomList( objects );
        }

        public void RegPrefab( GameObject go )
        {
            objects.Add(go);
        }
    }

    [System.Serializable]
    public class Palette_PREFAB_TILE
    {
        public List<PREFAB_TILE> prfList = new();

        public void Add_Prefab( GameObject prf )
        {
            PREFAB_TILE prf_tile = new();
            prf_tile.RegPrefab(prf);
            prfList.Add(prf_tile);
        }

        public GameObject GetRandom( int idx , Mng_X128SS rd = null )
        {
            if( idx < 0 || idx >= prfList.Count ) return null;
            return prfList[idx].GetRandom(rd);
        }
    }
    public List<Palette_PREFAB_TILE> palette_s;



    // 벽, 북쪽 방향이 막힌 기준
    public PREFAB_TILE prf_Wall;
    public int prefab_size = 1; // 프레팹 크기
    public Transform tr_Inst;
    public bool NO_WALL;
    public int  noWall_TileID = 1;      // 1번 타일은 벽생성 안함
    public List<SJ_RawMap2D> mapData = new();
    Mng_X128SS rd_main;
    public Dictionary<SJ_MAP_LAYER_TILE_COORD,SJ_TileCoordBase> dic_tileObj = new();

    // 타일 객체 생성 안하는 레이어
    // 예) 이벤트 트리거 레이어
    public List<string> noInstTileLayerName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void CreateMap( Mng_X128SS rd = null )
    {
        if( tr_Inst == null )tr_Inst = transform;
        SJ_Unity.Delete_Child( tr_Inst );
        rd_main = rd;
        MakeTilePrefab();
        if( NO_WALL == false ) MakeWallPrefab();

        AlignTileCoord();
    }

    public void Clear_PrefabPalette()
    {
        //prfList.Clear();

        palette_s.Clear();
    }

    public Palette_PREFAB_TILE Add_Palette()
    {
        Palette_PREFAB_TILE palette = new();
        palette_s.Add( palette );
        return palette;
    }

    public void Clear_MapLayer()
    {
        mapData.Clear();
    }

    public SJ_RawMap2D NewLayer()
    {
        SJ_RawMap2D new_layer = new();
        mapData.Add(new_layer);
        return new_layer;
    }

    public SJ_RawMap2D GetBaseMap()
    {
        if( mapData.Count < 1 ) return null;
        return mapData[0];
    }

    public SJ_RawMap2D GetMapLayer(int layer)
    {
        if( mapData.Count < 1 ) return null;
        return mapData[layer];
    }

    public SJ_RawTileVal GetTileVal( int x , int y , int layer_num = 0  )
    {
        SJ_RawMap2D layer = GetMapLayer(layer_num);
        if( layer == null )return null;
        return layer.GetTileVal( x , y );
    }

    public bool CheckGetTileVal( int x , int y )
    {
        SJ_RawTileVal rawTileVal = null;
        rawTileVal = GetTileVal( x , y );
        if( rawTileVal == null || rawTileVal.tile_val < 0 ) return false;
        return true;
    }

    public bool CheckGetTileVal( Vector2Int pos )
    {
        return CheckGetTileVal( pos.x , pos.y );
    }

    virtual public bool OnMoveAble( Vector2Int pos )
    {
        return CheckGetTileVal(pos);
    }

    public GameObject InstPrfTile( int idx , int x , int y , int palette_idx )
    {
        if( palette_idx < 0 || palette_idx >= palette_s.Count )
            return null;

        GameObject prf = palette_s[palette_idx].GetRandom( idx , rd_main);
        if( prf == null ) return null;

        return InstPrfPos( prf , x, y );
    }

    public Vector3 GetPos( Vector2Int pos)
    {
        return new Vector3( pos.x * prefab_size , 0 , pos.y * prefab_size );
    }

    public Vector3 GetPos( int x , int y )
    {
        return new Vector3( x * prefab_size , 0 , y * prefab_size );
    }

    public GameObject InstPrfPos( GameObject prf , int x , int y )
    {
        Vector3 pos = GetPos( x , y );

        GameObject inst = GameObject.Instantiate( prf );
        inst.transform.SetParent( tr_Inst );
        inst.transform.localPosition = pos;
        inst.SetActive(true);
        return inst;
    }

    public void MakeTilePrefab()
    {
        // 팔레트 갯수 체크
        if( mapData.Count > palette_s.Count )
        {
            Debug.LogError( "팔레트 갯수 모자람 : " + mapData.Count + "  : " + palette_s.Count );
            return;
        }

        for( int i = 0 ; i < mapData.Count ; i++ )
        {
            var map_layer = mapData[i];

            // 인스턴스 생성 안함 레이어 이름
            if( noInstTileLayerName.Contains( map_layer.layerName ) )
            {
                Debug.Log( "생성 안함 레이어 ---->>> " + map_layer.layerName );
                continue;
            }

            foreach( var s in map_layer.dic_Tile )
            {
                Vector2Int pos = s.Key;
                SJ_RawTileVal rawTileVal = s.Value;

                GameObject inst = InstPrfTile( rawTileVal.tile_val , pos.x , pos.y , i );
                if( inst != null )
                {
                    SJ_TileCoordBase tileCoordBase = inst.GetComponent<SJ_TileCoordBase>();
                    if( tileCoordBase != null )
                    {
                        tileCoordBase.SetPosLayer( pos , i );
                    }
                }
            }              
        }
    }

    public void MakeWallPrefab()
    {
        SJ_RawMap2D layer_base = GetBaseMap();

        if( layer_base == null ) return;

        foreach( var s in layer_base.dic_Tile )
        {
            Vector2Int pos = s.Key;
            WallTileInst( pos.x , pos.y , 0,  1 , 0 );
            WallTileInst( pos.x , pos.y , 0, -1 , 180 );
            WallTileInst( pos.x , pos.y , 1,  0 , 90 );
            WallTileInst( pos.x , pos.y ,-1,  0 , 270 );
        }
    }

    void WallTileInst( int x , int y , int off_x , int off_y , float rot )
    {
        SJ_RawTileVal self_tile = GetTileVal( x , y );
        
        if( self_tile == null || self_tile.tile_val < 0 )  return;
        if( self_tile.tile_val == noWall_TileID ) return;

        SJ_RawTileVal off_tile = GetTileVal( x + off_x , y + off_y );
        if(  off_tile == null || off_tile.tile_val < 0 ) //  -1 이면 벽
        {
            GameObject prf = prf_Wall.GetRandom(rd_main);
            GameObject inst = InstPrfPos( prf , x , y );
            inst.transform.localRotation = Quaternion.Euler( 0 , rot , 0 );
        }
    }

    [ContextMenu("지우기")]
    public void ClearTileInst()
    {
        if( tr_Inst == null )tr_Inst = transform;
        SJ_Unity.Delete_Child( tr_Inst );
    }

    public Vector2Int RandomAblePos()
    {
        // 일단 기본 타일맵에서 랜덤 1개
        SJ_RawMap2D layer_base = GetBaseMap();

        if( layer_base == null )
        {
            return Vector2Int.zero;
        }

        Vector2Int pos_random = (Vector2Int)SJ_Unity.GetRandomItem<Vector2Int>( new List<Vector2Int>(layer_base.dic_Tile.Keys) );
        return pos_random;
    }

    public SJ_TileCoordBase GetTileCoordInst( Vector2Int pos , int layer )
    {
        SJ_MAP_LAYER_TILE_COORD key = new();
        key.pos = pos;
        key.layer = layer;

        SJ_TileCoordBase tile = null;
        dic_tileObj.TryGetValue( key , out tile );
        return tile;
    }

    public void AlignTileCoord()
    {
        SJ_TileCoordBase[] coordBases = tr_Inst.GetComponentsInChildren<SJ_TileCoordBase>();
        foreach( var s in coordBases )
        {
            dic_tileObj[ s.pos_layer ]  = s;
        }
    }
}
