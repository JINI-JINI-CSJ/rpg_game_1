
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_GodSelect : MonoBehaviour
{
    [System.Serializable]
    public class GOD_Image
    {
        public SJ_UITween_Color tween_Color;
        public int              ID;
        public Button           menu;
        public void PlayFWD()
        {
            tween_Color.PlayFwd();
        }
        public void Hide()
        {
            tween_Color.ForceMove(0);
            tween_Color.Stop();
            tween_Color.gameObject.SetActive(false);
        }
    }

    public List<GOD_Image> gOD_s;

    public Text text_name;
    public Text text_desc;
    public Text text_say;

    int sel_id = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPopup_StartAni()
    {
        foreach( var s in gOD_s ) s.Hide();
        SJ_UnityUI_Util.TextString( text_name );
        SJ_UnityUI_Util.TextString( text_desc );
        SJ_UnityUI_Util.TextString( text_say );
    }

    public void OpenPopup_StartAni_End()
    {
        SelectGod( 0 );
    }


    public void SelectGod( int idx )
    {
        sel_id = idx;
        for( int i = 0 ; i < gOD_s.Count ; i++ )
        {
            if( sel_id == i )   gOD_s[i].PlayFWD();
            else                gOD_s[i].Hide();
        }
        GOD_Image gOD = gOD_s[sel_id];

        // 선택 데네브
        SJ_UnityUI_Util.TextString( text_name , SJ_Language.Str( "BASE" ,"SELECT") + "  " + SJ_Language.Str( "GOD_SEL_NAME" , gOD.ID ) );
        SJ_UnityUI_Util.TextString( text_desc , SJ_Language.Str( "GOD_SEL_DESC" , gOD.ID ) );
        SJ_UnityUI_Util.TextString( text_say  , SJ_Language.Str( "GOD_SEL_SAY" , gOD.ID ) );
    }

    public void OnClickOK()
    {
        GOD_Image gOD = gOD_s[sel_id];
        string name_god = SJ_Language.Str( "GOD_SEL_NAME" , gOD.ID );
        string msg = name_god + "\n" + SJ_Language.Str( "GOD_SEL_UI" , "QS_SEL_OK" );
        SJ_UnityUI_CommonPopup.OpenCommonMsg_Curve( msg , this , null , "OnYES_OnClickOK" , true );
    }

    public void OnYES_OnClickOK()
    {
        SJ_UnityUIMng_Curve.CloseOne( OnCloseAniEnd );

        Player.saveFile.SetFirstPlay_Step(1);
        Player.SaveUserFile();
    }

    public void OnCloseAniEnd()
    {
        SJ_SimpleSyncMono.NextPlaySelf();   
    }

}
