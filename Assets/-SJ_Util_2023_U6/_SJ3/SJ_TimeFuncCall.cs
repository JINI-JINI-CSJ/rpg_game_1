using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SJ_TimeFuncCall 
{
    // 인자 1 개 받는거 기본
    public delegate void FUNC_CALL( object arg );

    public class TIMER_FUNC
    {
        public FUNC_CALL func;
        public float repeat_time;
        public bool repeat_call;

        public object arg;

        public float cur_time;

        public bool Update( float time )
        {
            cur_time += time;
            if( cur_time >= repeat_time )
            {
                int play_num = (int)(cur_time / repeat_time);
                cur_time = Mathf.Repeat( cur_time , repeat_time );
                if( play_num > 0 && repeat_call == false )
                {
                    play_num = 1;
                }
                for( int i = 0 ; i < play_num ; i++ )
                {
                    func.Invoke(arg);
                }
                if( repeat_call == false ) return false;
            }

            return true;
        }
    }

    // 함수 기준으로 정렬
    // 같은 함수는 중복 안되게
    public Dictionary<FUNC_CALL , TIMER_FUNC > dic = new();


    public void Clear()
    {
        dic.Clear();
    }

    public bool StartFunc( FUNC_CALL func , float repeat_time , bool repeat_call , object arg = null )
    {
        if( dic.ContainsKey( func ) ) return false;
        TIMER_FUNC timer_func = new TIMER_FUNC();
        timer_func.func = func;
        timer_func.repeat_time = repeat_time;
        timer_func.repeat_call = repeat_call;
        timer_func.arg = arg;
        dic[func] = timer_func;
        return true;
    }

    public void Update( float time )
    {
        List<TIMER_FUNC> cp = new( dic.Values );
        foreach( var s in cp )
        {
            if( s.Update(time) == false )
            {
                dic.Remove( s.func );
            }
        }
    }
}
