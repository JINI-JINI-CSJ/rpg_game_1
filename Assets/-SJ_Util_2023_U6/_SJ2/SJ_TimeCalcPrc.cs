using System;
using Unity.VisualScripting;
using UnityEngine;


public class SJ_TimeCalcPrc
{
    static public DateTime date_init;    
    static public DateTime date_start;



    static public void Start( string msg = "" )
    {
        date_start = DateTime.Now;
        date_init = DateTime.Now;

        Debug.Log( "시작 타이머----> " + date_start.ToString() + " " + msg );
    }

    static public void CalcNow(  string msg = "" , bool running = true)
    {
        TimeSpan ts = DateTime.Now - date_start;
        Debug.Log( ts.ToString() + " " + msg );
        if( running )
        {
            date_start = DateTime.Now;
        }
    }

    static public void EndTimer( string msg = "" )
    {
        TimeSpan ts = DateTime.Now - date_init;
        Debug.Log( "종료 타이머<---- " + ts.ToString() + " " + msg );
    }
}
