using UnityEngine;

public class Panel_Title : MonoBehaviour
{
    public GameObject   go_LoadActive;
    public IntroMain    introMain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        go_LoadActive.SetActive(false);

        SJ_CSV_Mng.LOAD_FILE_OR_URL = 1;
        SJ_CSV_Mng.Load( typeof( GTF_CSV ) , this , "OnLoadCSV" );
    }

    public void OnLoadCSV()
    {
        go_LoadActive.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMenu_StartFirst()
    {

    }

    public void OnFadeIn_Load()
    {
        introMain.gameObject.SetActive(true);
    }

    public void OnFadeOut_End()
    {
        gameObject.SetActive(false);
        introMain.PlayIntro();
    }

    public void OnMenu_Load()
    {
        
    }

    public void OnMenu_Option()
    {
        
    }

    public void OnMenu_Exit()
    {
        
    }

}
