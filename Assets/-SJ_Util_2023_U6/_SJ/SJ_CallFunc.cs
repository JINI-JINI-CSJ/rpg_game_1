using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class SJ_CallFunc
{
    public object       obj_inst;
    public System.Type  type_static;
    public string       func;
    public object[]     lt_args;

    public object       arg_one;

    MethodInfo methodInfo_cash;

    public void     Init()
    {
        obj_inst = null;
        type_static = null;
        func = null;
    }

    public bool CallAble()
    {
        if( string.IsNullOrEmpty(func) ) return false;
        if( obj_inst == null && type_static == null ) return false;
        return true;
    }

    public void     SetInst( object _obj , string _func , object arg = null , object[] args = null )
    {
        methodInfo_cash = null;
        type_static = null;
        obj_inst = _obj;
        func = _func;
        arg_one = arg;
        lt_args = args;
    }

    public void     SetStatic( System.Type t, string _func , object arg = null , object[] args = null )
    {
        methodInfo_cash = null;
        type_static = t;
        obj_inst = null;
        func = _func;
        arg_one = arg;
        lt_args = args;
    }


    public void     Func( params object[] args_user )
    {
        if( string.IsNullOrEmpty(func) ) return;

        object[] args = args_user;
        if( args == null || args.Length == 0 )
        {
            args = lt_args;
            if( args == null || args.Length == 0 )
            {
                if( arg_one != null )
                {
                    args = new object[] {arg_one};
                }
            }
        }

        FuncCall(args);
    }

    public void     FuncOneArg( object arg )
    {
        if( string.IsNullOrEmpty(func) ) return;
        object[] args = new object[] {arg};

        FuncCall(args);
    }

    void FuncCall( object[] args )
    {
        if(obj_inst != null  )
        {        
            if( methodInfo_cash == null )
                methodInfo_cash = SJ_CSharpUtil.Get_MethodInfo_Inst( obj_inst , func );
            
            if( methodInfo_cash != null )
            {
                SJ_CSharpUtil.Call_MethodInfo(methodInfo_cash, obj_inst, args);
            }
        }
        else if( type_static != null )
        {
            if( methodInfo_cash == null )
                methodInfo_cash = SJ_CSharpUtil.Get_MethodInfo_Static( type_static , func );
            
            if( methodInfo_cash != null )
            {
                SJ_CSharpUtil.Call_MethodInfo(methodInfo_cash, null, args);
            }
        }
    }
}
