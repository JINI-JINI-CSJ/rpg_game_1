using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SJ_GageBarText : MonoBehaviour
{
    public Scrollbar    scrollbar;
    public Slider       slider;
    public Text         text;
    public TMP_Text     tmp_Text;


    public void SetValue( int cur , int max )
    {
        float val = (float)cur / (float)max;
        if( scrollbar != null ) scrollbar.size = val;
        if( slider != null )    slider.value = val;

        string str = cur + "/" + max;
        SJ_UnityUI_Util.TextString( text , str );
        SJ_UnityUI_Util.TextStringTMP( tmp_Text , str );
    }
}
