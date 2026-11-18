using UnityEngine;
using TilemapTool;

/// <summary>
/// 클로드 타일맵 메타 데이터
/// - CSV_ID
/// - GOLD
/// </summary>

public class TileObj_ChestBox : SJ_TileCoordBase
{
    public Animator animator;

    public float    time_openAni = 0.5f;

    // 저장된 아이템 정보
    public int csv_ID;

    // 아이템 저장된 상태 
    public bool fillItem = true;

    override public void OnLoadAfter()
    {
        // 이미 열린 상태라면 상자 오픈된 애니
        if( fillItem == false )
        {
            animator.Play( "OpenEnd" );
        }
    }

    public override bool OnAble_Interact()
    {
        return fillItem;
    }

    public override void OnPre_Interact()
    {
        // 열기
        PanelMENU_FieldObjInter.ADD_MENU( this , "BASE" , "OPEN" , 1 );
    }

    public override void OnInteract(int ID)
    {
        switch( ID )
        {
            case 1:
                {
                    GTF_Global.PlayerInputAble( false , false );

                    animator.Play( "OpenStart" );
                    WaitFunc( time_openAni , OnEnd_OpenAni );

                    fillItem = false;
                }
                break;
        }
    }

    public void OnEnd_OpenAni()
    {
        GTF_Global.PlayerInputAble( true , true );

        int gold = 0;
        CSV_Item csv_item = null;

        // 메타 데이터에서 한번 가져와 본다.
        ObjectPlacement placement = GTF_TileMap.CurTileMap().GetObjectPlacement( 1 , pos_layer.pos );
        if( placement != null )
        {
            string meta_val = placement.GetCustom<string>( "CSV_ID" );
            int meta_csv_id = SJ_CSV_Common.TryParseInt( meta_val );


            csv_ID = meta_csv_id;
            meta_val = placement.GetCustom<string>( "GOLD" );
            gold = SJ_CSV_Common.TryParseInt( meta_val );

            Debug.Log( "아이템 메타 : " + meta_csv_id + "  : " + gold );
        }

        csv_item = GTF_CSV.csv_ItemPage_ALL.Find_Int( csv_ID ) as CSV_Item;

        if( csv_item == null && gold == 0 )
        {
            // 빈 상자 였다!
            SJ_UIDefaultShortMsg.SetMsg( SJ_Language.Str( "BASE" , "EMPTY_CHEST_BOX" ) );
            return;
        }

        string msg = "";
        if( csv_item != null )
        {
            // 숏 메세지 ,  "약초" 입수했습니다. 
            msg = csv_item.GetName() + " " + SJ_Language.Str( "BASE" , "GET_MSG" );
        }
        else if( gold > 0 )
        {
            msg = gold + "G " + SJ_Language.Str( "BASE" , "GET_MSG" );
        }

        SJ_UIDefaultShortMsg.SetMsg( msg );

        // 
        Player.inventory.Add( csv_ID );
    }

}
