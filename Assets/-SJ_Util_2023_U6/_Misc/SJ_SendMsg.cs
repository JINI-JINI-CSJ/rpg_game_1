using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_SendMsg : MonoBehaviour
{
    public    _SJ_GO_FUNC[]     msgs;
    public    bool              send_onEnable;

    public  void    Send()
    {
        foreach(  _SJ_GO_FUNC s in msgs )
        {
            s.Func();
        }
    }

    private void OnEnable()
    {
       if( send_onEnable ) Send();
    }
}
