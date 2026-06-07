using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public  class _SJ_QUEUE_FUNC_UNIT
{
    public  _SJ_GO_FUNC go_func = new _SJ_GO_FUNC();

    public  UnityEvent Event = new UnityEvent();

    public  void    Set( MonoBehaviour mono , string func_name , object arg = null )
    {
        go_func.SetMono( mono , func_name , arg );
    }
    public  void    Set( UnityAction at )
    {
        Event.AddListener( at );
    }


    public  void    Call()
    {
        go_func.Func();
        Event.Invoke();
    }

}

[System.Serializable]
public class SJ_QueueFunc
{
    public  List<_SJ_QUEUE_FUNC_UNIT>   lt_SJ_QUEUE_FUNC_UNIT = new List<_SJ_QUEUE_FUNC_UNIT>();


    public  void    AddCall( MonoBehaviour mono , string func_name , object arg = null )
    {
        _SJ_QUEUE_FUNC_UNIT s = new _SJ_QUEUE_FUNC_UNIT();
        s.Set( mono , func_name , arg );
        lt_SJ_QUEUE_FUNC_UNIT.Add( s );
    }

    public  void    AddCall( UnityAction at )
    {
        _SJ_QUEUE_FUNC_UNIT s = new _SJ_QUEUE_FUNC_UNIT();
        s.Set( at );
        lt_SJ_QUEUE_FUNC_UNIT.Add( s );
    } 

    public  void    Call_Next()
    {
        if( lt_SJ_QUEUE_FUNC_UNIT.Count > 0 )
        {
            lt_SJ_QUEUE_FUNC_UNIT[0].Call();
            lt_SJ_QUEUE_FUNC_UNIT.RemoveAt(0);
        }
    }

}
