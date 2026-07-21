using System;
using System.Collections.Generic;
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
    public int width;
    public int height;
    //public int[] mapTile;

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
        public List<GameObject> objects;
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
    // 프리펩
    public List<PREFAB_TILE> prfList;
    // 벽, 북쪽 방향이 막힌 기준
    public PREFAB_TILE prf_Wall;
    public int prefab_size = 1; // 프레팹 크기
    public Transform tr_Inst;
    public bool NO_WALL;
    public int  noWall_TileID = 1;      // 1번 타일은 벽생성 안함
    public List<SJ_RawMap2D> mapData = new();
    Mng_X128SS rd_main;
    public Dictionary<Vector2Int,TileResBottom> dic_tileResBottoms = new();

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
    }

    public void Clear_PrefabPalette()
    {
        prfList.Clear();
    }

    public void Add_PrefabPalette( GameObject prf )
    {
        PREFAB_TILE prf_tile = new();
        prf_tile.RegPrefab(prf);
        prfList.Add(prf_tile);
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

    public GameObject InstPrfTile( int idx , int x , int y )
    {
        if( idx < 0 ) return null;
        if( idx >= prfList.Count )
        {
            Debug.LogError( "에러 프리펩 : " + prfList.Count + "      idx : " + idx );
            return null;
        }
        return InstPrfPos( prfList[idx].GetRandom(rd_main) , x, y );
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
        foreach( var map_layer in mapData )
        {
            foreach( var s in map_layer.dic_Tile )
            {
                Vector2Int pos = s.Key;
                SJ_RawTileVal rawTileVal = s.Value;

                GameObject inst = InstPrfTile( rawTileVal.tile_val , pos.x , pos.y );
                if( inst != null )
                {
                    TileResBottom bottom = inst.GetComponent<TileResBottom>();
                    if( bottom != null )
                    {
                        bottom.SetPos( pos.x , pos.y );
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

    // 타일에디트 클로드 버전용 
    public TextAsset binaryFile_TileMap;

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

    public TileResBottom GetBottomInst( Vector2Int pos )
    {
        TileResBottom bottom = null;
        dic_tileResBottoms.TryGetValue( pos , out bottom );
        return bottom;
    }
}
