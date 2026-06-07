using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_WaitNextMono : MonoBehaviour
{
    public ParticleSystem ps;

    public SJ_CallFunc callFunc = new SJ_CallFunc();

    bool isReturn = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void     WaitNext( float time , object obj_func = null , string func = "" , bool _isReturn = false )
    {
        isReturn = _isReturn;
        if( ps != null )ps.Play();
        
        callFunc.SetInst( obj_func , func );
        StartCoroutine( WaitNext_Coroutine( time ) );
    }

    IEnumerator WaitNext_Coroutine( float time )
    {
        yield return new WaitForSeconds( time );
        callFunc.Func();
        if( isReturn )
        {
            SJPool.ReturnInst_Or_Destroy(gameObject);
        }
    }

}