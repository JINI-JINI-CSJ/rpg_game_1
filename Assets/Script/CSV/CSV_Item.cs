using System.Collections.Generic;
using UnityEngine;

// ``ID	이름	설명	리소스	등급	클래스	인자1	2	3	4	5	
public class CSV_Item : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string res;
    public int grade;
    public string class_name;
    public List<string> args;

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);

        name = Next();
        desc = Next();
        res = Next();
        grade = Next_Int();
        class_name = Next();
        Remain_Data( args );
    }

    public string GetName(){return name;}
    public string GetDesc(){return desc;}
}

public class CSV_ItemPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_Item();
    }
}