using UnityEngine;

// 필드 던전
public class InGame : MonoBehaviour
{
    static public InGame G;

    public int Turn_Dungeon = 1;

    void Awake()
    {
        G = this;
        SJPool.InitMng();
        SJSound.Init();
        SJ_ResPoolSys.Init_Scene();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SJ_CSV_Mng.LOAD_FILE_OR_URL = 1;
        SJ_CSV_Mng.Load( typeof( GTF_CSV ) , this , "OnLoadCSV" );
    }

    public void OnLoadCSV()
    {
        Player.LoadUserFile();

        GameObject inst_TileMap = null;

        // 인트로 씬 끝나고 후원자 선택후
        //if( Player.saveFile.GetFirstPlay_Step() == 1 )
        {
            inst_TileMap = IntroFirst();
        }
        // else
        // {
            // 일반 던전 맵
        // }

        if( inst_TileMap == null )
        {
            Debug.LogError( "맵 타일 없음!!! " );
            return;
        }

        GTF_TileMap gTF_TileMap = inst_TileMap.GetComponent<GTF_TileMap>();
        PlayerMover.G.SetMap( gTF_TileMap );
        GTF_TileMap.LoadMap();
        PlayerMover.G.Ready();

        BattlePartyView_Player.Update_Player();

        DungeonBattleInCounter.G.incount_inf = gTF_TileMap.dungeonInfo.iNCOUNT_INF;

        Panel_DungeonMain.OnEnd_PlayerMove();

        // 맵 시작 이벤트
        MapEventPlayer.StartEventPlay();
    }

    public GameObject IntroFirst()
    {
        GameObject inst_Intro = GameObject.Find( "MapIntro" );
        if( inst_Intro == null )
        {
            inst_Intro = SJ_ResPoolSys.Inst_Obj( "Intro/MapIntro" );            
        }
        Player.FirstPlay();
        return inst_Intro;
    }

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


}
