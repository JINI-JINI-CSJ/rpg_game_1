using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 각 씬마다 객체를 둔다.
/// 생성시에 세팅을 하자.
/// </summary>

public class GTF_LangMetaFunc : MonoBehaviour
{
    static public GTF_LangMetaFunc G;

    public SJ_SyncFuncLangPart funcLangPart = new();

    //  미리 등록해놓자.
    public SJ_GameSync_UIFade       uiFade;
    public SJ_GameSync_TalkMsgBox   talkMsgBox;

    // 토크 박스 창에 포함되 있다.
    public SJ_UIGameTalkChr         talkChrImg;


    void Awake()
    {
        G = this;
        SJ_SyncFuncLangPart.G.obj_call_default = this;
        SJ_SyncFuncLangPart.RegFunc_MsgBase( Func_LangStr );

        // SJ_SyncFuncLangPart.RegFunc( "PLAYER_INPUT" , PLAYER_INPUT );
        // SJ_SyncFuncLangPart.RegFunc( "TALK_BOX" , TALK_BOX );
        // SJ_SyncFuncLangPart.RegFunc( "LOAD_PREFAB" , LOAD_PREFAB );
        // SJ_SyncFuncLangPart.RegFunc( "LOAD_DUNGEON" , LOAD_DUNGEON );
        // SJ_SyncFuncLangPart.RegFunc( "DELETE_PREFAB" , DELETE_PREFAB );
        // SJ_SyncFuncLangPart.RegFunc( "FADE" , FADE );
        // SJ_SyncFuncLangPart.RegFunc( "SHOW_CHR_IMG" , SHOW_CHR_IMG );
        // SJ_SyncFuncLangPart.RegFunc( "ENTER_CITY_PLACE" , ENTER_CITY_PLACE );
        // SJ_SyncFuncLangPart.RegFunc( "ACTIVE_POPUP" , ACTIVE_POPUP );
        // SJ_SyncFuncLangPart.RegFunc( "SAVE_FILE" , SAVE_FILE );
        // SJ_SyncFuncLangPart.RegFunc( "SYNC_EVT_END" , SYNC_EVT_END );

        uiFade.func_endStep = OnEndFunc;
        talkMsgBox.func_endStep = OnEndFunc;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEndFunc()
    {
        SJ_SyncFuncLangPart.NextExec();
    }

    //=============================================================================================
    // meta 함수들 

    public bool STR_TRUE_FALSE( string str )
    {
        if( str == "TRUE" ) return true;
        return false;
    }

    // 기본 메세지
    public void Func_LangStr( string msg )
    {
        talkMsgBox.StartText_OneStep( msg );
    }

    // s2 : 현재는 "CLOSE" 한개만 있다. 오픈은 위 함수로 대체
    public void TALK_BOX( string s2 , string s3 )
    {
        // 
        if( s2 == "CLOSE" )
        {
            talkMsgBox.CloseMsgBoxAwait();
        }

        //SJ_SyncFuncLangPart.NextExec();
    }

    // s2 : true , false
    // 전체 게임 입력
    public void PLAYER_INPUT( string s2 , string s3 )
    {
        bool b = STR_TRUE_FALSE(s2);
        GTF_Global.PlayerInputAble( b , true );

        // 유아이도 여기서 처리하자.
        GTF_Global.ShowMainUI( b );

        SJ_SyncFuncLangPart.NextExec();
    }

    // 특정 리소스를 로딩 
    public void LOAD_PREFAB( string s2 , string s3 )
    {
        SJ_ResPoolSys.Inst_Obj( "MetaRes/" + s2 );
        SJ_SyncFuncLangPart.NextExec();
    }

    // 화면에 있는 객체 삭제
    public void DELETE_PREFAB( string s2 , string s3 )
    {
        GameObject go = GameObject.Find( s2 );
        if( go != null )
        {
            GameObject.DestroyImmediate( go );
        }
        SJ_SyncFuncLangPart.NextExec();
    }

    public void FADE( string s2 , string s3 )
    {
        uiFade.StartFadeAni( STR_TRUE_FALSE(s2) );
    }

    public void SHOW_CHR_IMG( string s2 , string s3 )
    {
        // s2: 이미지 이름
        // 경로는 여기서 하자.
        string path_full = "2D/FullSize/" + s2;
        talkChrImg.SetImage( path_full );
        //SJ_SyncFuncLangPart.NextExec(); // 캐릭터 일러스트 함수 제외
    }


    // 현재는 인트로 던전만 있다.
    public void LOAD_DUNGEON( string s2 , string s3 )
    {
        string dungeon_name = s2;

        GameObject inst_TileMap = null;

        // 일단 "MetaRes" 에서 다하자
        inst_TileMap = SJ_ResPoolSys.Inst_Obj( "MetaRes/" + dungeon_name );  
        if( inst_TileMap == null )
        {
            Debug.LogError( "던전 맵 타일 없음!!! " + dungeon_name );
            return;
        }

        GTF_TileMap gTF_TileMap = inst_TileMap.GetComponent<GTF_TileMap>();
        PlayerMover.G.SetMap( gTF_TileMap );
        GTF_TileMap.LoadMap();
        PlayerMover.G.Ready();

        SJ_SyncFuncLangPart.NextExec();
    }

    public void ACTIVE_POPUP( string s2 , string s3 )
    {
        bool b = STR_TRUE_FALSE(s2);
        SJ_UnityUIMng_Curve.ActivePanel( b , s3 );
        SJ_SyncFuncLangPart.NextExec();
    }

    public void SAVE_FILE( string s2 , string s3 )
    {
        if( s2 == "FIRST_STATE" )
        {
            int val = SJ_CSV_Common.TryParseInt( s3 );
            PlayerSaveFile.SetFirstPlay_Step( val );
            PlayerSaveFile.Save();
        }
        
        SJ_SyncFuncLangPart.NextExec();
    }



    public void ENTER_CITY_PLACE( string s2 , string s3 )
    {
        
        SJ_SyncFuncLangPart.NextExec();
    }

    public void SYNC_EVT_END( string s2 , string s3 )
    {
        
        SJ_SyncFuncLangPart.NextExec();
    }
}
