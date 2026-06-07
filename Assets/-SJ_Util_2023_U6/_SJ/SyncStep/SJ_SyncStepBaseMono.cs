using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_SyncStepBaseMono : MonoBehaviour
{
    public SJ_SyncStepMng_Mono  par_mng;


    public void NextPlay()
    {
        if( par_mng != null )
            par_mng.mng._NextPlay();
    }

    public void     Wait_NextPlay( float f )
    {
        StartCoroutine( CO_Wait_NextPlay(f) );
    }

    IEnumerator     CO_Wait_NextPlay(float f)
    {
        yield return new WaitForSeconds(f);
        OnEnd_Wait();
        NextPlay();
    }

    virtual public void     OnEnd_Wait(){}
}
