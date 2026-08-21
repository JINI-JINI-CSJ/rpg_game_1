using System.Collections.Generic;
using UnityEngine;

public class SJ_UtilMisc_1 
{
}

// 숫자값에 해당하는 범위의 객체 반환
public class SJ_RangeStep
{
    public class _INF
    {
        public int min;
        public int max;
        public object obj;

        public bool Check( int val )
        {
            if( min <= val && max >= val )return true;
            return false;
        }
    }

    public List<_INF> lt = new();

    public void Clear(){lt.Clear();}
    public void Add( int min , int max , object obj )
    {
        _INF s = new();
        s.max = max;
        s.min = min;
        s.obj = obj;
        lt.Add(s);
    }

    public object Result( int val )
    {
        foreach( var s in lt )if( s.Check(val) )return s.obj;
        return null;
    }
}
