using UnityEngine;

// 필드 던전
public class InGame : MonoBehaviour
{
    static public InGame G;
    public int Turn_Dungeon = 1;
    public int Dungeon_LAYER_cur = 1;   // 현재 던전 계층
    public bool TEST_NO_EVENT;

    void Awake()
    {
        G = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SJPool.InitMng();
        SJSound.Init();
        SJ_ResPoolSys.Init_Scene();        

        SJ_CSV_Mng.LOAD_FILE_OR_URL = 1;
        SJ_CSV_Mng.Load( typeof( GTF_CSV ) , this , "OnLoadCSV" );
    }

    public void OnLoadCSV()
    {
        Player.LoadUserFile();
        Player.FirstPlay();
        
        BattlePartyView_Player.Update_Player();
        Panel_DungeonMain.OnEnd_PlayerMove();        

        if( GTF_EventCheckStart.CheckSceneStart() )
        {
            // 특수 이벤트 진행
            return;
        }

        // 일반 던전 진행
        // 플레이어에 던전 번호 있다.
        

        //GameObject inst_TileMap = null;
        // 인트로 씬 끝나고 후원자 선택후
        //if( Player.saveFile.GetFirstPlay_Step() == 1 )
        // {
        //     //inst_TileMap = IntroFirst();
        // }
        // else
        // {
            // 일반 던전 맵
        // }

        // if( inst_TileMap == null )
        // {
        //     Debug.LogError( "맵 타일 없음!!! " );
        //     return;
        // }

        // GTF_TileMap gTF_TileMap = inst_TileMap.GetComponent<GTF_TileMap>();
        // PlayerMover.G.SetMap( gTF_TileMap );
        // GTF_TileMap.LoadMap();
        // PlayerMover.G.Ready();



        // 맵 시작 이벤트
        // if( TEST_NO_EVENT == false )
        //     MapEventPlayer.StartEventPlay();
    }

    // public void LoadMapDungeon( string str )
    // {
    //     GameObject inst_TileMap = null;

    //     inst_TileMap = SJ_ResPoolSys.Inst_Obj( str );  
    //     if( inst_TileMap == null )
    //     {
    //         Debug.LogError( "던전 맵 타일 없음!!! " + str );
    //         return;
    //     }

    //     GTF_TileMap gTF_TileMap = inst_TileMap.GetComponent<GTF_TileMap>();
    //     PlayerMover.G.SetMap( gTF_TileMap );
    //     GTF_TileMap.LoadMap();
    //     PlayerMover.G.Ready();
    // }


    static public void ShowUIDungeon( bool b )
    {
        Panel_DungeonMain.Show( b );
        BattlePartyView_Player.Show(b);
    }

    static public void OnEnd_PlayerOneMove()
    {
        G._OnEnd_PlayerOneMove();
    }

    public void _OnEnd_PlayerOneMove()
    {
        Turn_Dungeon++;

        // 던전턴 월드 1턴 체크
        // ....


        Panel_DungeonMain.OnEnd_PlayerMove();
    }


    [ContextMenu("전투 테스트")]
    public void BattleTEST()
    {
        BattleMain.BattleTEST();
    }

}
