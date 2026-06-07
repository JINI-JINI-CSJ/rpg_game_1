using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UITweenBase : MonoBehaviour
{
    public  float   PlayTime = 1.0f;
    public  bool    PingPong;
    public bool     play_once;
    public  bool    play;    
    [HideInInspector]
    public  float   time_cur;
    [HideInInspector]
    public  bool    reverse_cur;
    [HideInInspector]
    public  float   ratio_cur;
    public  AnimationCurve  curve;

    public bool StartEnable;

    public _SJ_GO_FUNC endFunc_Once = new _SJ_GO_FUNC();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //FrameMove( Time.deltaTime );
    }

    void OnEnable()
    {
        if( StartEnable ) Play();
    }

    virtual public  void    Play()
    {
        if( play ) return;
        enabled = true;
        play = true;
        time_cur = 0;
        ratio_cur = 0;
        gameObject.SetActive(true);

        reverse_cur = false;
    }

    virtual public void     PlayFwd()
    {
        Debug.Log( "PlayFwd" );
        Play();
    }

    virtual public void     PlayBack()
    {
        Debug.Log( "PlayBack" );
        Play();
        reverse_cur = true;
    }

    virtual public  void    Stop()
    {
        play = false;
        enabled = false;
        time_cur = 0;
        ratio_cur = 0;
        reverse_cur = false;
        OnFrameMove();
    }

    public  void    FrameMove( float t )
    {
        if(play == false) return;
        time_cur += t;
        if( time_cur >= PlayTime )
        {
            time_cur -= PlayTime;
            if(PingPong) reverse_cur = !reverse_cur;

            if( play_once )
            {
                play = false;
                time_cur = PlayTime;
                OnEndOnce();

                endFunc_Once.Func();
            }
        }
        float r = time_cur / PlayTime;
        if( reverse_cur ) r = 1.0f - r;
        ratio_cur = curve.Evaluate(r);
        OnFrameMove();
    }

    public void SetRatio( float ratio )
    {
        ratio_cur = curve.Evaluate(ratio);
        OnFrameMove();
    }

    virtual public  void    OnFrameMove(){}
    virtual public  void    OnEndOnce(){}


    // public void START_PLAY()
    // {
    //     enabled = true;
    //     Play();
    // }

    // public void STOP_PLAY()
    // {
    //     enabled = false;
    //     Stop();
    // }
}
