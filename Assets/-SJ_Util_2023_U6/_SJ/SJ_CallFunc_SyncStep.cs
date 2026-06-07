using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

public class SJ_CallFunc_SyncStep
{
    public class _ADD_INF
    {
        public object call_obj ;
        public string str_class;
        public string str_func; 
        public object[] args;
    }

    // 일단 SJ_CallFuncClass_Cash 이거로 처리
    // 나중에 간단한 클래스..
    public List<_ADD_INF>  lt_q = new List<_ADD_INF>();

    public void Add( object call_obj , string str_class , string str_func , object[] args )
    {
        _ADD_INF inf = new _ADD_INF();
        inf.call_obj = call_obj;
        inf.str_class = str_class;
        inf.str_func = str_func;
        inf.args = args;

        lt_q.Add( inf );
    }

    public bool Play()
    {
        if( lt_q.Count == 0 ) return false;

        _ADD_INF inf = lt_q[0];
        lt_q.RemoveAt(0);

        SJ_CallFuncClass_Cash cc = new SJ_CallFuncClass_Cash();
        cc.Call_Normal( inf.call_obj , inf.str_class , inf.str_func , inf.args );

        return true;
    }
}