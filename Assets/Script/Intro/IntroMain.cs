using UnityEngine;

public class IntroMain : MonoBehaviour
{
    public SJ_SimpleSyncMono syncMono;

    void Awake()
    {
        SJPool.InitMng();
        SJSound.Init();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SJ_CSV_Mng.Load( typeof( GTF_CSV ) , this , "OnLoadCSV" , true );
    }

    public void OnLoadCSV()
    {
        syncMono.StartPlay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
