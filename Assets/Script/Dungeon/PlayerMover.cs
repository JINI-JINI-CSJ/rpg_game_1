using UnityEngine;
using TilemapTool;

public class PlayerMover : SJ_MapTileMover
{
    public SJ_MapTileViewer_claude mapView_Claude;

    // 일단 다음 요소들중에 하나만 하고 
    // 대신 우선순위 대로 한다.
    // 맵 만들때 밑에 사항들이 중복 되지 않게 한다.

    // 1. 트리거 레이어
    // 도착위치의 2번 레이어 트리거 
    // 던전 메이커의 특수 방( 보스 던전 등등 )

    // 2. 상호작용 : 제자리 객체 ( 바닥 포털등등 ) , 1번 레이어

    // 3. 상호작용 : 주인공 앞 객체 ( 보물 상자 ) , 1번 레이어

    // 4. 일반 전투 등등

    // 동시에 한다면  싱크 스택으로 하면 된다.

    void Awake()
    {
        sJ_MapTile = mapView_Claude;
    }

    public override void OnMoveStart()
    {
        PanelMENU_FieldObjInter.Clear_MENU();
    }

    public override void OnMoveEnd()
    {
        // 트리거 레이어 2 번 , 현재 타일
        ObjectPlacement placement = mapView_Claude.GetObjectPlacement( 2 , cur_pos );
        if( placement != null )
        {
            // 트리거 체크
            string trg_evt = placement.GetCustom<string>( "TRG_EVT" );
            if( string.IsNullOrEmpty( trg_evt ) == false )
            {
                // 트리거 실행
                if( MapEventPlayer.RunEventPlay( trg_evt ) == false )
                {
                }
                return;
            }
        }

        // 바닥 객체 
        SJ_TileCoordBase tile_obj_cur = mapView_Claude.GetTileCoordInst( cur_pos , 1 );
        if( tile_obj_cur != null && tile_obj_cur.OnAble_Interact() && tile_obj_cur.nowPosInterAble )
        {
            tile_obj_cur.OnPre_Interact();
            return;
        }

        // 앞 객체
        SJ_TileCoordBase tile_obj_front = mapView_Claude.GetTileCoordInst( GetPos_Front() , 1 );
        if( tile_obj_front != null && tile_obj_front.OnAble_Interact()  && tile_obj_front.frontInterAble)
        {
            tile_obj_front.OnPre_Interact();
            return;
        }

        // 일반 전투 체크

    }

}
