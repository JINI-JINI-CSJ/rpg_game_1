using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SJ_StepMode_Mono : MonoBehaviour
{
    [System.Serializable]
    public  class _MODE
    {
        public  string      Name;

        [System.Serializable]
        public  class       _UNITY_EVENT
        {
            public  UnityEvent  evt_start;
            public  UnityEvent  evt_update;
            public  UnityEvent  evt_end;            
        }
        public  _UNITY_EVENT _unity_event;

        public  GameObject  go_active;
    
        public  SJ_StepMode_Obj stepMode_Obj;

        public  float       endTime = -1;
        public  void Play()
        {
            if( _unity_event.evt_start != null ) _unity_event.evt_start.Invoke();
            if( go_active != null ){go_active.SetActive(true);}
            if( stepMode_Obj != null ) stepMode_Obj.OnStart_Mode();
        }

        public  void    Update()
        {
            if( _unity_event.evt_update != null ) _unity_event.evt_update.Invoke();
            if( stepMode_Obj != null ) stepMode_Obj.OnUpdate_Mode();
        }

        public  void    End()
        {
            if( _unity_event.evt_end != null ) _unity_event.evt_end.Invoke();
            if( stepMode_Obj != null ) stepMode_Obj.OnEnd_Mode();
        }
    }
    public  List<_MODE> lt_MODE = new List<_MODE>();

    public    _MODE   mode_cur;

    public  bool    list_mode;

    public  bool    LoopPlay;

    public  int     list_mode_idx;

    public  bool    play = false;

    public  bool    noUpdate_prc = true;

    public  UnityEvent  evt_End_ListMode;

    public  void    Start_ListMode()
    {
        enabled = true;
        list_mode = true;
        list_mode_idx = 0;
        mode_cur = null;

        foreach( _MODE s in lt_MODE )
        {
            if(s.stepMode_Obj != null)
            {
                s.stepMode_Obj.par_mng = this;
                s.stepMode_Obj.step_mode = s;
            }
        }
        play = true;

        OnStartPlay();
        Next_ListModePlay();
    }

    virtual public  void    OnStartPlay(){}

    public  void    Next_ListModePlay()
    {
        if( mode_cur != null )
        {
            mode_cur.End();
        }

        if( list_mode_idx >= lt_MODE.Count )
        {
            mode_cur = null;

            if(LoopPlay)
            {
                list_mode_idx = 0;
                mode_cur = null;
                Next_ListModePlay();
                return;
            }

            if( evt_End_ListMode != null )evt_End_ListMode.Invoke();
            StopPlay();
            return;
        }
        mode_cur = lt_MODE[list_mode_idx];
        mode_cur.Play();

        if( mode_cur.endTime > 0 )
        {
            StartCoroutine( CO_EndMode(mode_cur.endTime , mode_cur ) );
        }
        list_mode_idx++;

        //Debug.Log( "Next_ListModePlay : " + list_mode_idx );
    }

    public  void StopPlay()
    {
        enabled = false;
        play = false;
        OnStop();
    }

    virtual public  void    OnStop(){}

    IEnumerator CO_EndMode( float wait , _MODE obj )
    {
        yield return new WaitForSeconds(wait);
        if( mode_cur != obj ) yield return null;
        if( list_mode )
        {
            Next_ListModePlay();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if( noUpdate_prc ) return;
        if( play == false) return;
        if( mode_cur != null ) mode_cur.Update();
    }

    virtual public  void    Update_Func()
    {
        if( play == false) return;
        if( mode_cur != null ) mode_cur.Update();
    }
}
