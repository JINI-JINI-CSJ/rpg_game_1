using UnityEngine;

public class TileObj_ChestBox : SJ_TileCoordBase
{
    public Animator animator;

    public float    time_openAni = 0.5f;

    // 저장된 아이템 정보

    // 아이템 저장된 상태 
    public bool fillItem;

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

        // 숏 메세지 ,  "약초" 입수했습니다. 
        

    }

}
