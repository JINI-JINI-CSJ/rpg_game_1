using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_SyncStepMng_Mono : MonoBehaviour
{
    public SJ_SyncStepMng   mng = new SJ_SyncStepMng();

    public List<SJ_SyncStepBaseMono>  lt_step_mono = new List<SJ_SyncStepBaseMono>();



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void     Play_First( object obj = null , string func = "" )
    {
        if( lt_step_mono.Count < 1 )
        {
            Debug.Log( "SJ_SyncStepMng_Mono  " );
            return;
        }
        if( mng.lt_func.Count < 1 )
        {
            foreach( SJ_SyncStepBaseMono s in lt_step_mono )
            {
                s.par_mng = this;
                mng._Add_Obj( s , "Play" );
            }
        }
        mng.Play_First( obj , func );
    } 
}
