using System;
using System.Collections.Generic;
using UnityEngine;

// 배열의 값을 각각 대응하는 프리펩으로 생성
// 벽은 4방향 벽 등록
// 1. 배열대로 프리펩 생성
// 2. 벽체크하여 만들기

// 인게임 기능 : 이동 가능한 방향 체크

public class SJ_MapTileViewer : MonoBehaviour
{
    [System.Serializable]
    public class PREFAB_ARR
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
    }

    // 프리펩
    public List<PREFAB_ARR> prfList;

    // 벽, 북쪽 방향이 막힌 기준
    public PREFAB_ARR prf_Wall;

    public int prefab_size = 1; // 프레팹 크기

    public Transform tr_Inst;

    int width;
    int height;
    int[] mapTile;

    Mng_X128SS rd_main;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMap( int w , int h , int[] arr , Mng_X128SS rd = null )
    {
        if( tr_Inst == null )tr_Inst = transform;
        SJ_Unity.Delete_Child( tr_Inst );

        width = w;
        height = h;
        mapTile = new int[arr.Length];
        Array.Copy( arr , mapTile , arr.Length );
        rd_main = rd;

        MakeTilePrefab();
        MakeWallPrefab();
    }

    public int GetTileVal( int x , int y )
    {
        if( x < 0 || x >= width || y < 0 || y >= height ) return -1;
        return mapTile[ y * width + x ];
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

    public GameObject InstPrfPos( GameObject prf , int x , int y )
    {
        Vector3 pos = new Vector3( x * prefab_size , 0 , y * prefab_size );

        GameObject inst = GameObject.Instantiate( prf );
        inst.transform.SetParent( tr_Inst );
        inst.transform.localPosition = pos;
        inst.SetActive(true);
        return inst;
    }



    void MakeTilePrefab()
    {
        for( int y = 0 ; y < height ; y++ )
        {
            for( int x = 0 ; x < width ; x++ )
            {
                int idx_p = GetTileVal(x,y);
                InstPrfTile( idx_p , x , y );
            }
        }
    }

    void MakeWallPrefab()
    {
        for( int y = 0 ; y < height ; y++ )
        {
            for( int x = 0 ; x < width ; x++ )
            {
                WallTileInst( x , y , 0,  1 , 0 );
                WallTileInst( x , y , 0, -1 , 180 );
                WallTileInst( x , y , 1,  0 , 90 );
                WallTileInst( x , y ,-1,  0 , 270 );
            }
        }
    }

    void WallTileInst( int x , int y , int off_x , int off_y , float rot )
    {
        int self_tile = GetTileVal( x , y );
        if( self_tile < 0 )  return;

        int off_tile = GetTileVal( x + off_x , y + off_y );
        if( off_tile < 0 ) //  -1 이면 벽
        {
            GameObject prf = prf_Wall.GetRandom(rd_main);
            GameObject inst = InstPrfPos( prf , x , y );
            inst.transform.localRotation = Quaternion.Euler( 0 , rot , 0 );
        }
    }

    // 타일에디트 클로드 버전용 
    public TextAsset binaryFile_TileMap;

    TileEditor.Core.TileMapData tileMapData;

    [ContextMenu("생성 등록 파일")]
    public void Load_TileEditClaude_RD()
    {
        if( binaryFile_TileMap == null )
        {
            Debug.LogError( "파일 세팅 없음" );
            return;
        }
        tileMapData = TileEditor.Core.TileMapBinaryIO.Load( binaryFile_TileMap );

        // 0번째 레이어로만 지형타일 구성
        if( tileMapData == null || tileMapData.Layers.Count < 1 )
        {
            Debug.LogError( "파일 클로드 " );
            return;
        }
        TileEditor.Core.TileLayer tileLayer_0 = tileMapData.Layers[0];
        CreateMap( tileLayer_0.Width , tileLayer_0.Height , tileLayer_0.RawTiles );
    }

    [ContextMenu("지우기")]
    public void ClearTileInst()
    {
        if( tr_Inst == null )tr_Inst = transform;
        SJ_Unity.Delete_Child( tr_Inst );
    }
}
