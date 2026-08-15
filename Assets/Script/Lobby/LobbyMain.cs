using UnityEngine;

public class LobbyMain : MonoBehaviour
{
    static public LobbyMain G;

    public bool TEST_NO_EVENT;

    void Awake()
    {
        G = this;
        SJPool.InitMng();
        SJSound.Init();
        SJ_ResPoolSys.Init_Scene();
    }

    void Start()
    {
        SJ_CSV_Mng.LOAD_FILE_OR_URL = 1;
        SJ_CSV_Mng.Load( typeof( GTF_CSV ) , this , "OnLoadCSV" );
    }

    public void OnLoadCSV()
    {
        Player.LoadUserFile();

        //if( Player.saveFile.GetFirstPlay_Step() == 2 )
        {
            GameObject inst_IntroLobby = IntroLobby();
            SJ_SimpleSyncMono simpleSyncMono = inst_IntroLobby.GetComponent<SJ_SimpleSyncMono>();
            simpleSyncMono.StartPlay();
        }
    }

    public GameObject IntroLobby()
    {
        GameObject inst_Intro = GameObject.Find( "IntroLobby" );
        if( inst_Intro == null )
        {
            inst_Intro = SJ_ResPoolSys.Inst_Obj( "Intro/IntroLobby" );            
        }
        Player.FirstPlay();
        return inst_Intro;
    }




}
