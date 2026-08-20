using System.Collections.Generic;
using UnityEngine;

public class StockMakeInf
{
    public class _STOCK
    {
        public string str;
        public object obj;
    }

    public List<_STOCK> lt = new();

    public void Add( string _str , object _obj = null )
    {
        _STOCK s = new();
        s.str = _str;
        s.obj = _obj;
    }

    public int Count(){return lt.Count;}

    public void AddRange( List<string> strings )
    {
        foreach( var s in strings )Add( s );
    }

    public void AddRange( List<object> objs )
    {
        foreach( var s in objs )Add( null , s );
    }

    public _STOCK RandomPop( Mng_X128SS rd )
    {
        if( lt.Count < 1 ) return null;
        return rd.RandomList(lt);
    }

    public string RandomPop_Str( Mng_X128SS rd )
    {
        _STOCK s = RandomPop(rd);
        if( s != null ) return s.str;
        return "";
    }

    public object RandomPop_Obj( Mng_X128SS rd )
    {
        _STOCK s = RandomPop(rd);
        if( s != null ) return s.obj;
        return null;
    }
}

// 메이크 작업 단위 기본 클래스
public class MakeBase : MonoBehaviour
{
    // 월드 최초 생성 , 게임 시나리오 처음 시작할때만.
    virtual public void OnMake(){}

    virtual public void OnSave(){}

    virtual public void OnLoad(){}

    virtual public void OnAfterWork(){}
}
