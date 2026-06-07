using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
//using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// 캐릭터 필살기 연출 등등

// 주의!!!! 애니매이트 트랙은 반드시  Apply Scene Offset 으로 설정되어야 한다.

public class SJ_CineLinkAnimator : MonoBehaviour
{
    public	PlayableDirector playableDirector;
    public List<GameObject> go_hide;
    public _SJ_GO_FUNC      func_end = new _SJ_GO_FUNC();
    public _SJ_GO_FUNC      func_signal = new _SJ_GO_FUNC();
    public SJ_CallFunc_Mono callFuncEnd = new SJ_CallFunc_Mono();
    public Animator         animator_Play;

    public bool EndDestroy = false; // 종료시 오브젝트 파괴 여부

    public bool EndNoHide = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach( var s in go_hide ) s.SetActive(false);

        var timeline = playableDirector.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            //Debug.Log( track.name );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PDBind( string trackName, Object obj)
    {

//        Debug.Log( "트랙 바인드 시도 : " + trackName + " : " + obj.name  );
        var timeline = playableDirector.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == trackName)
            {
                playableDirector.SetGenericBinding(track, obj);
//                Debug.Log( "트랙 바인드 성공 ");
                return;
            }
        }
        Debug.Log( "트랙 바인드 실패 ");
    }

    // 기준 객체로 전체 시네머신위치를 맞춘다.
    // 예) 궁극기를 사용하는 캐릭터
    public void SetBaseObj_Trans( GameObject go_base )
    {
        if( go_base == null ) return;

        transform.position = go_base.transform.position;
        transform.rotation = go_base.transform.rotation;
    }

    static public void Inst_Play(SJ_CineLinkAnimator prefab_cine, GameObject go_base = null, MonoBehaviour mono_func = null, string func_name_end = "", object func_arg = null,
            CinemachineBrain cinemachineBrain = null, Animator bind_anit = null, string bind_brain_track_name = "Cinemachine Track",
             string bind_anit_track_name = "Animation Track",
            bool play = true)
    {
        SJ_CineLinkAnimator go_cine = GameObject.Instantiate(prefab_cine);
        go_cine.gameObject.SetActive(true);
        go_cine.SetBaseObj_Trans_AnitTrack(go_base, mono_func, func_name_end, func_arg,
            cinemachineBrain, bind_anit, bind_brain_track_name, bind_anit_track_name, play);
    }

    // 기준객체로 위치 맞추고 animator 를 기본 트랙 애니에 맞춘다.
    // 위 함수들 합치고 디폴트 애니트랙 이름
    public void SetBaseObj_Trans_AnitTrack(GameObject go_base = null, MonoBehaviour mono_func = null, string func_name_end = "", object func_arg = null,
            CinemachineBrain cinemachineBrain = null, Animator bind_anit = null, string bind_brain_track_name = "Cinemachine Track",
             string bind_anit_track_name = "Animation Track",
            bool play = true)
    {
        StopAllCoroutines();

        SetBaseObj_Trans(go_base);

        if (bind_anit != null)
        {
            //Debug.Log( "트랙 바인드 : " + bind_anit_track_name + " : " + bind_anit.name  );
            PDBind(bind_anit_track_name, bind_anit);
        }

        if (cinemachineBrain != null)
        {
            PDBind(bind_brain_track_name, cinemachineBrain);
        }

        func_signal.SetMono(mono_func);

        if (play)
        {
            gameObject.SetActive(true);
            Play();
            func_end.SetMono(mono_func, func_name_end);

        }
    }

    IEnumerator CO_WaitEnd_PD(float wait)
    {
//        Debug.Log("CO_WaitEnd_PD : " + wait);

        yield return new WaitForSeconds(wait);
        func_end.debug = true;
        func_end.Func();
        callFuncEnd.Func();

        if( EndNoHide == false ) gameObject.SetActive(false);
        if( EndDestroy ) GameObject.Destroy( gameObject );
    }

    // 심플 싱크 모노 용도
    public void PlayStep()
    {
        Play();
    }

    public void Play()
    {
        gameObject.SetActive(true);
        playableDirector.Play();
        StopAllCoroutines();
        StartCoroutine( CO_WaitEnd_PD( (float)playableDirector.duration ) );        
    }

    public void Stop()
    {
        playableDirector.Stop();
        gameObject.SetActive(false);
    }

    public void OnSignal( string tag )
    {
        //Debug.Log( " 시그널 : " + tag );

        func_signal.func = tag;
        func_signal.Func();
    }

    public void OnSignal_Anit( string act_name )
    {
        if( animator_Play == null ) return;
        animator_Play.Play( act_name );
    }

}
