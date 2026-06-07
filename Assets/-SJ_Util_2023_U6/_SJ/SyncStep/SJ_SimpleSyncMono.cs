using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 하위 모노들은 다음 함수를 구현
// PlayStep , 본인 종료시  SJ_SimpleSyncMono.NextStep
// EndStep , 위 호출 후 이거 실행
// 

public class SJ_SimpleSyncMono : MonoBehaviour
{
    static public SJ_SimpleSyncMono instance = null;
    public List<GameObject> lt_step = new List<GameObject>();
    public MonoBehaviour func_end_mono = null;
    public string func_end_name = "";

    int cur_idx = 0;
    public bool is_play = false;

    public GameObject TEST_STEP;

    [HideInInspector]
    public GameObject go_curPlay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AllCallFunc( string func , System.Type type = null )
    {
        if( type == null )
        {
            foreach( var s in lt_step )
            {
                if( s != null )
                    SJ_CSharpUtil.CallStrFunc_NoArg( s , func );
            }
            return;
        }

        // 타입이 지정되면 해당 타입의 컴포넌트만 호출
        foreach( var s in lt_step )
        {
            if( s != null )
            {
                var c = s.GetComponent(type);
                if( c != null )
                    SJ_CSharpUtil.CallStrFunc_NoArg( c , func );
            }
        }
    }


    public void StartPlay()
    {
        instance = this;
        cur_idx = 0;

        if( lt_step.Count < 1 )
        {
            for( int i = 0 ; i < transform.childCount ; i++ )
            {
                GameObject go = transform.GetChild( i ).gameObject;
                if( go.activeSelf )
                {
                    lt_step.Add( go );
                }
            }            
        }

        if( lt_step.Count < 1 )
        {
            Debug.Log( "이벤트 객체 목록 없음!!!!" );
            return;
        }
        is_play = true;

        if( TEST_STEP != null )
        {
            for( int i = 0 ; i < lt_step.Count ; i++ )
            {
                if( lt_step[i] == TEST_STEP )
                {
                    cur_idx = i;
                    Debug.Log( "테스트 스텝으로 교체 : " + TEST_STEP.name + " : " + cur_idx );
                    break;
                }
            }
        }

        Play();
    }

    public void Play()
    {
 //       GF_PlayerMachine.DEBUG_LOG_POS("SJ_SimpleSyncMono 1======================================================");
        if( cur_idx >= lt_step.Count )
        {
            Debug.Log( "이벤트 객체 목록 끝!!!!" );
            is_play = false;
            SJ_Unity.SendMsg( func_end_mono , func_end_name );
            return;
        }
        GameObject s = lt_step[cur_idx];
        if (s != null)
        {
//            GF_PlayerMachine.DEBUG_LOG_POS("SJ_SimpleSyncMono 2======================================================");
            Debug.Log("이벤트 객체 플레이 : " + s.name);
            s.SetActive(true);
            s.SendMessage("PlayStep", SendMessageOptions.DontRequireReceiver);

            go_curPlay = s;
//            GF_PlayerMachine.DEBUG_LOG_POS("SJ_SimpleSyncMono 3======================================================");
        }
    }


    public void _NextPlay()
    {
        //Debug.Log( "다음 이벤트 ---->>>>" );
        if( !is_play ) return;
//        GF_PlayerMachine.DEBUG_LOG_POS("현재 이벤트 PREV ---->>>>======================================================");
        GameObject s = lt_step[cur_idx];
        Debug.Log( "현재 이벤트 EndStep ---->>>>" + s.name );
        s.SendMessage( "EndStep" , SendMessageOptions.DontRequireReceiver );
        s.SetActive(false);
//        GF_PlayerMachine.DEBUG_LOG_POS("현재 이벤트 EndStep ---->>>>======================================================");
        cur_idx++;
        Play();
    }

    static public void NextPlay()
    {
        if( instance != null )
        {
            instance._NextPlay();
        }
    }

    static public void NextPlaySelf()
    {
//        GF_PlayerMachine.DEBUG_LOG_POS("SJ_SimpleSyncMono NextPlaySelf 1==================================================");
        if( instance != null )
        {
            instance.Try_Wait_NextPlay(0.01f);
        }
    }

    bool waiting = false;
    public bool Try_Wait_NextPlay( float time )
    {
        if( waiting ) return false;
        StartCoroutine( _Wait_NextPlay(time) );
        return true;
    }

    IEnumerator _Wait_NextPlay( float time )
    {
        waiting = true;
//        GF_PlayerMachine.DEBUG_LOG_POS("SJ_SimpleSyncMono _Wait_NextPlay 2==================================================");
        yield return new WaitForSeconds( time );
//        GF_PlayerMachine.DEBUG_LOG_POS("SJ_SimpleSyncMono _Wait_NextPlay 3==================================================");
        waiting = false;
        NextPlay();
    }

    static public bool Wait_NextPlay( float time )
    {
        if( instance != null )
        {
            return instance.Try_Wait_NextPlay( time );
        }
        return false;
    }

}
