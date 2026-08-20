using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CSV_TagDefine : SJ_CSV_BaseObj
{
    public string tagPart;

    public string tag;

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);
        Next();
        Next();
        tagPart = Next();
        tag = Next();
    }
}


public class CSV_TagDefinePage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_TagDefine();
    }

    public List<CSV_TagDefine> GetTagPart( string part )
    {
        List<CSV_TagDefine> lt = new();
        foreach( var s in dic_int.Values.Cast<CSV_TagDefine>() )if( s.tagPart == part ) lt.Add(s);
        return lt;
    }

    public List<string> GetTagPart_Str( string part )
    {
        List<string> lt = new();
        foreach( var s  in GetTagPart(part) )
        {
            lt.Add( s.tag );
        }
        return lt;
    }
}