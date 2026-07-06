using System.Collections.Generic;
using UnityEngine;

// ``ID	이름	설명	확률 태그	확률	클래스	인자1	2	3	4	5
public class CSV_GOD : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string TAG;
    public float  per;
    public string class_name;
    public List<string> args;
    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);
        name = Next();
        desc = Next() ;
        TAG = Next();
        per = Next_Float();
        class_name = Next();
        Remain_Data( args );
    }
}

public class CSV_GODPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_GOD();
    }
}