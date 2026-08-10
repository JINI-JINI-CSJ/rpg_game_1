using UnityEngine;

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

        CSV_Item csv_item = GTF_CSV.csv_ItemPage_ALL.Find_Int( csv_ID ) as CSV_Item;

        if( csv_item == null )
        {
            // 빈 상자 였다!
            SJ_UIDefaultShortMsg.SetMsg( SJ_Language.Str( "BASE" , "EMPTY_CHEST_BOX" ) );
            return;
        }

        // 숏 메세지 ,  "약초" 입수했습니다. 
        string msg = csv_item.GetName() + " " + SJ_Language.Str( "BASE" , "GET_MSG" );
        SJ_UIDefaultShortMsg.SetMsg( msg );

        // 
        Player.inventory.Add( csv_ID );
    }

}
