using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_StartWaitUpdate : MonoBehaviour
{
    public _SJ_GO_FUNC  func_awake;
    public List<_SJ_GO_FUNC>  lt_func_start;
    public _SJ_GO_FUNC  func_update;
    public int          wait_frame = 5;
    public bool         hide_go = true;
    public int          wait_frame_cur = 0;

    public void     SetFunc_Update( MonoBehaviour mono , string func )
    {
        func_update.SetMono( mono , func );
    }

    private void Awake() {
        func_awake.Func();
    }

    // Start is called before the first frame update
    void Start()
    {
        wait_frame_cur = 0;
        foreach( _SJ_GO_FUNC s in lt_func_start )
        {
            s.Func();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if( func_update.Check_Func() == false ) return;

        wait_frame_cur++;
        if( wait_frame_cur == wait_frame )
        {
            func_update.Func();
            if( hide_go ) gameObject.SetActive(false);
        }
    }
}
