using System;
using System.Collections;
using UnityEngine;

public class SJ_AniCurveVal : MonoBehaviour
{
    public float MinVal = 0;
    public float MaxVal = 1.0f;
    public AnimationCurve ani_cur;
    public float Time_Play = 1.0f;
    float play_time;
    float startTime;
    public bool play;
    SJ_CallFunc sJ_CallFuncUpdate = new SJ_CallFunc();

    SJ_CallFunc sJ_CallFuncEnd = new SJ_CallFunc();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( play )
        {
            float elapse = Time.time - startTime;
            float r = elapse / play_time; 
            float val = Lerp( r );
            sJ_CallFuncUpdate.FuncOneArg(val);
        }
    }

    public float Lerp( float normal_time )
    {
        float f = ani_cur.Evaluate( normal_time );
        return Mathf.Lerp( MinVal , MaxVal , f );
    }

    public void Play( float _time = -1, object obj_func = null , string func_update = "" , string func_end = "" )
    {
        //gameObject.SetActive(true);

        StopAllCoroutines();

        play_time = Time_Play;
        if( _time >= 0 )
        {
            play_time = _time;
        }

        if( play_time <= 0 )
        {
            Debug.Log( " SJ_AniCurveVal Play 시간없음 : " + play_time );
            return;
        }

        startTime = Time.time;

        play = true;
        sJ_CallFuncUpdate.SetInst( obj_func , func_update );
        sJ_CallFuncEnd.SetInst( obj_func , func_end );

        Debug.Log( " SJ_AniCurveVal Play 시작 : " + play_time );
        StartCoroutine( CO_Wait(play_time) );
    }

    IEnumerator CO_Wait( float wait )
    {
        yield return new WaitForSeconds( wait );
        play = false;
        if( sJ_CallFuncEnd.CallAble() )sJ_CallFuncEnd.Func();
        //gameObject.SetActive(false);
    }

    

}
