using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SJ_UnityUI_Text_Lang : MonoBehaviour
{
    public string   LANG_PART;
    public string   LANG_WORD;
    public int      LANG_ID;

    public string   Default_str = "";

    public bool     debug;

    Text text;
    TMP_Text    text_tm;

    TextMesh    textMesh;

    private void OnEnable() {
        Update_Text_Lang();
    }

    public void Update_Text_Lang()
    {
        if( text == null || text_tm == null || textMesh == null )
        {
            text = GetComponent<Text>();
            text_tm = GetComponent<TMP_Text>();
            textMesh = GetComponent<TextMesh>();
        }

        if( text == null && text_tm == null && textMesh == null )
        {
            Debug.LogError( "텍스트 필드 없음!!! : " + gameObject.name );
            return;
        }

        string str = "";
        if( string.IsNullOrEmpty( LANG_WORD ) == false )
        {
            str = SJ_Language.Str( LANG_PART , LANG_WORD );
        }
        else if(LANG_ID > 0  ) 
        {
            str = SJ_Language.Str( LANG_PART , LANG_ID );
        }

        if( debug )
        {
            Debug.Log( "UI_Text_Lang : " + LANG_PART + " : " + LANG_WORD + " : " + LANG_ID + " -> " + str );
        }

        if (string.IsNullOrEmpty(str)) return;

        if (text != null) text.text = str;
        if( text_tm != null ) text_tm.text = str;
        if( textMesh != null ) textMesh.text = str;
    }

}
