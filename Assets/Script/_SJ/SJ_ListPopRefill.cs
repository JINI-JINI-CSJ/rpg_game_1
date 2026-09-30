using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 목록을 받아서 초기화
/// 랜덤으로 한개씩 꺼내기 , 만약 다 쓰면 원레 목록 복사
/// </summary>
public class SJ_ListPopRefill<T>
{
    public List<T> lt_src_copy;
    public List<T> lt_pop;

    // 델리게이트 랜덤
    public delegate int Func_Random( int max );
    public Func_Random func_Random;
    // 또는 함수 상속 구현
    virtual public int OnRandom( int max ){return -1;}

    public bool InitList( List<T> lt_src , Func_Random func_random = null )
    {
        if( lt_src.Count < 1 ) return false;
        lt_src_copy = new( lt_src ) ;
        lt_pop = new( lt_src_copy );
        func_Random = func_random;
        return true;
    }

    public void AddSrcOne( T t )
    {
        if( lt_src_copy == null )  lt_src_copy = new();
        lt_src_copy.Add(t);
        lt_pop = new( lt_src_copy );
    }


    public T Popup( bool noReFill = false )
    {
        if( lt_pop.Count < 1 )
        {
            if( noReFill )return default;

            lt_pop = new( lt_src_copy );
        }

        int idx = -1;
        if( func_Random != null )
        {
            idx = func_Random( lt_pop.Count );
        }
        else
        {
            idx = OnRandom( lt_pop.Count );
        }
        if( idx == -1 ) return default;

        T t = lt_pop[idx];
        lt_pop.RemoveAt(idx);
        return t;
    }
}
