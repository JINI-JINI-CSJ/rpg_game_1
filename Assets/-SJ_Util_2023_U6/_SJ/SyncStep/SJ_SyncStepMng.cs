using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_SyncStepMng 
{
    static public  SJ_SyncStepMng  main;

    public List<SJ_CallFunc>    lt_func = new List<SJ_CallFunc>();

    public SJ_CallFunc          func_all_end = new SJ_CallFunc();

    int cur_idx = 0;

    public bool queue_mode = false;

    // public SJ_SyncStepMng()
    // {
    //     if( main == null )main = this;
    // }

    public void  SetMainCur()
    {
        main = this;
    }

    public void     Add( SJ_CallFunc call_func ) 
    {
        lt_func.Add(call_func);
    }

    public void     _Add_Obj( object obj , string func , object arg_one = null , object[] arr_arg = null ) 
    {
        SJ_CallFunc call_func = new SJ_CallFunc();
        call_func.SetInst( obj , func , arg_one , arr_arg );
    }

    static public void Add_Obj( object obj , string func , object arg_one = null , object[] arr_arg = null ) 
    {
        if( main != null )
        {
            main._Add_Obj( obj , func , arg_one , arr_arg );
        }
    }

    public void     Play_First(object obj = null , string func = "" )
    {
        func_all_end.SetInst(obj , func);
        cur_idx = 0;
        NextPlay();
    }   

    public bool     _NextPlay()
    {
        if( queue_mode)
        {
            if( lt_func.Count == 0 )
            {
                func_all_end.Func();
                return true;
            }

            SJ_CallFunc f = lt_func[0];
            lt_func.RemoveAt(0);
            f.Func();

        }else{
            if( cur_idx >= lt_func.Count )
            {
                func_all_end.Func();
                return true;
            }

            SJ_CallFunc f = lt_func[cur_idx];
            f.Func();
            cur_idx++;            
        }
        return false;
    }

    static public void NextPlay()
    {
        if( main != null )
        {
            main._NextPlay();
        }
    }



}
