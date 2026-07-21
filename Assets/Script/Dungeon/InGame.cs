using UnityEngine;

public class InGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SJ_CSV_Mng.LOAD_FILE_OR_URL = 1;
        SJ_CSV_Mng.Load( typeof( GTF_CSV ) , this , "OnLoadCSV" );
    }

    public void OnLoadCSV()
    {
        Player.LoadUserFile();

        // 인트로 끝나고 후원자 선택후
        if( Player.saveFile.GetFirstPlay_Step() == 1 )
        {
            IntroFirst();
        }
    }

    public void IntroFirst()
    {
        GameObject inst_Intro = SJ_ResPoolSys.Inst_Obj( "Intro/IntroFirst" );
        SJ_SimpleSyncMono sJ_SimpleSyncMono = inst_Intro.GetComponentInChildren<SJ_SimpleSyncMono>();
        sJ_SimpleSyncMono.StartPlay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
