using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SJ_UI_Gage_Ani 
{
    public Scrollbar    scrollbar;
    public int          max;
    public  int         target_cur;
    public  int         start_cur;    
    public  float       time_ani = 0.3f;
    public  AnimationCurve  curve;
    public float            time_ani_play_start = -1;

    public bool         play;

    public void     InitValue( int _max , int _cur = -1 )
    {
        max = _max;
        target_cur = max;
        if( _cur != -1 )
        target_cur = _cur;
        UpdateScrollBar( target_cur );
        play = false;
    }

    public void     UpdateScrollBar( int cur )
    {
        if( scrollbar == null ) return;

        if( max == 0 )
        {
            scrollbar.size = 0;
            return;
        }

        scrollbar.size = (float)cur / (float)max;
    }

    public void     StartAni( int _cur )
    {
        start_cur = target_cur;
        target_cur = _cur;
        time_ani_play_start = Time.time;
        play = true;
    }

    public void     Update()
    {
        if( play == false ) return;
        float ratio = Time.time - time_ani_play_start;
        if( ratio >= time_ani )
        {
            ratio = time_ani;
            play = false;
        }
        ratio /= time_ani;
        float curve_ratio = curve.Evaluate( ratio );
        int cur = (int)Mathf.Lerp( (float)start_cur , (float)target_cur , curve_ratio );
        UpdateScrollBar(cur);
    }
}
