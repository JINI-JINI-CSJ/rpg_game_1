using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SJ_SyncStep_Playables : SJ_SyncStepBaseMono
{
    public PlayableDirector playableDirector;

    public _SJ_GO_FUNC      func_user = new _SJ_GO_FUNC();

    public bool             end_go_hide;

    public void     Play()
    {
        playableDirector.Play();
        Wait_NextPlay( (float)playableDirector.duration );
    }

     public void     PlayUser( MonoBehaviour mono , string func )
     {
        func_user.SetMono(mono , func);
        Play();
     }

    public override void OnEnd_Wait()
    {
        func_user.Func();
        func_user.Init();

        if( end_go_hide )  
        {
            playableDirector.gameObject.SetActive(false);
        }
    }

}
