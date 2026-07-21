using System.Collections.Generic;
using UnityEngine;

// 일반 델리게이트 스텝
// 등록 되는 데로 실행
public class SJ_DlgFuncSync 
{
    static public SJ_DlgFuncSync inst;

    // 함수 들어와도 바로 실행
    public bool directExec = true;
    public List<FuncCall_Arg> func_s = new();

    public class FuncCall_Arg
    {
        public SJ_COMMON.Func_Arg   func;
        public object               arg;

        public void Call()
        {
            func.Invoke(arg);
        }
    }

    FuncCall_Arg cur_func;

    public SJ_COMMON.Func_VOID allEnd_func;

    public SJ_DlgFuncSync()
    {
        if( inst == null )inst = this;
    }

    public void ChangeMain()
    {
        inst = this;
    }

    public void Add( SJ_COMMON.Func_Arg func , object arg = null )
    {
        FuncCall_Arg s = new();
        s.func = func;
        s.arg = arg;

        func_s.Add( s );
        if( cur_func == null && directExec )
        {
            _Next();
        }
    }

    static public void Next()
    {
        if( inst == null ) return;
        inst._Next();
    }

    public void _Next()
    {
        cur_func = null;
        if( func_s.Count < 1 )
        {
            allEnd_func?.Invoke();
            return;
        }
        
        cur_func = func_s[0];
        func_s.RemoveAt(0);

        cur_func.Call();
    }
}
