using UnityEngine;

// 필드 던전
public class InGame : MonoBehaviour
{
    void Awake()
    {
        SJPool.InitMng();
        SJSound.Init();
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

        // 인트로 씬 끝나고 후원자 선택후
        //if( Player.saveFile.GetFirstPlay_Step() == 1 )
        {
            IntroFirst();
        }

        //MapEventPlayer.StartEventPlay();
    }

    public void IntroFirst()
    {
        GameObject inst_Intro = GameObject.Find( "MapIntro" );
        if( inst_Intro == null )
        {
            SJ_ResPoolSys.Inst_Obj( "Intro/MapIntro" );            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
