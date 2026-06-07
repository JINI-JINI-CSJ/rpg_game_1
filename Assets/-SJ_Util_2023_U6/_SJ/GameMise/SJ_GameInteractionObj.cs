using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 게임 객체들의 상호작용 이벤트 
// 문이나 기타 기믹들.
// 접근 했을때 현재 상태에 따른 상호작용 가능한 이벤트 보여주기 ( 문열기 닫기 , 열쇠 넣기 등등 )
// 상태 구조 : 현재 상태모드 , 가능한 변경 상태모드 , 애니메이션 플레이 시간
// 상태 변형중이면 조작 불가능 인터페이스도 지원

public class SJ_GameInteractionObj : MonoBehaviour 
{
    [System.Serializable]
    public class _STATE_MODE
    {
        public string Name;
        public List<string> lt_AbleState;
        public float aniTime = 1.0f;

        public List<SJ_CallFunc_Mono> lt_func_start;
        public List<SJ_CallFunc_Mono> lt_func_end;

        public SJ_CallFunc_Mono func_end = new SJ_CallFunc_Mono();


        public bool Check_AbleState( string str )
        {
            return lt_AbleState.Contains(str);
        }

        public void FuncStart()
        {
            foreach( var s in lt_func_start ) s.Func();
        }

        public void FuncEnd()
        {
            foreach( var s in lt_func_end ) s.Func();
            func_end.Func();
        }

    }
    public List<_STATE_MODE> lt_STATE_MODE;

    public SJ_CallFunc_Mono  call_TrgEnter = new SJ_CallFunc_Mono();
    public SJ_CallFunc_Mono  call_TrgExit = new SJ_CallFunc_Mono();

    _STATE_MODE state_cur;
    _STATE_MODE state_next;
    bool playing_ani = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        if( lt_STATE_MODE.Count == 0 ) return;
        state_cur = lt_STATE_MODE[0];
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log( "트리거 엔터 : " + other.name );
        Call_TrgEnter();
    }

    void OnTriggerExit(Collider other)
    {
        Call_TrgExit();
    }

    public void Call_TrgEnter()
    {
        call_TrgEnter.Func( this );
    }

    public void Call_TrgExit()
    {
        call_TrgExit.Func( this );
    }

    public _STATE_MODE Find( string str )
    {
        foreach( var s in lt_STATE_MODE )if( s.Name == str ) return s;
        return null;
    }

    public _STATE_MODE CurState(){return state_cur;}

    public bool PlayStart( string str , MonoBehaviour mono_end_func_user = null , string end_func_user = "" )
    {
        if( lt_STATE_MODE.Count == 0 )
        {
            Debug.LogError( "lt_STATE_MODE.Count == 0 없음!!!!" );
            return false;
        }


        if( state_cur == null )
        {
            Debug.LogError( "시작 상태 없음!!!!" );
            return false;
        }

        if( playing_ani )
        {
            Debug.Log( "플레이중.... " + gameObject.name );
            return false;
        }

        if( state_cur.Check_AbleState(str) == false )
        {
            Debug.Log( "state_cur.Check_AbleState(str) == false" );
            return false;
        }
        _STATE_MODE find_mode = Find( str );
        if( find_mode == null )
        {
            Debug.Log( "find_mode == null" + str );
            return false;
        }
        StopAllCoroutines();
        state_next = find_mode;

        if( mono_end_func_user != null )
        {
            //state_next.sJ_CallFunc.SetInst( mono_end_func_user , end_func_user );
        }

        Debug.Log( "PlaySTART ===>>> " + str );

        StartCoroutine( CO_NextPlay() );
        return true;
    }

    IEnumerator CO_NextPlay()
    {
        playing_ani = true;
        state_cur.FuncStart();
        yield return new WaitForSeconds( state_next.aniTime );
        state_next.FuncEnd();
        state_cur = state_next;
        state_next = null;
        playing_ani = false;

        Debug.Log( "END ===>>> " + state_cur.Name );

    }
}
