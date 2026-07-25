using UnityEngine;

public class TileObj_Portal : SJ_TileCoordBase
{
    // 플레이어 시작점
    public bool startPlayerPos;

    // 연결된 타일 맵 
    // 이전 맵이든가 다음맵


    // 던전 탈출 여부
    // 처음 진입점 , 또는 종료 지점의 탈출
    public bool exitWorldMap;
    

    public override bool OnAble_Interact()
    {
        // 연결맵 , 탈출 기능이 있다
        if( exitWorldMap )
        {
            return true;
        }

        return false;
    }

    public override void OnPre_Interact()
    {
        // 
        //PanelMENU_FieldObjInter.ADD_MENU( this , "BASE" , "OPEN" , 1 );

        // 연결맵 : n 층  이동하기

        // 나가기 
        if( exitWorldMap )
        {
            PanelMENU_FieldObjInter.ADD_MENU( this , "BASE" , "EXIT" , 1 );
        }
    }

    public override void OnInteract(int ID)
    {
        switch( ID )
        {
            case 1:
                {
                    // 로비 씬 , 월드맵

                }
                break;
        }
    }
}
