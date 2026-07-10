using System;
using UnityEngine;

// 문자열 아이디
// ``ID	이름	설명	큰 직업 카테(공통,전,마,지)	무기,방어구,악세	메인 태그	보조태그1	보조태그2	보조태그3																	
public class CSV_EqItemDefine : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public JOB_BASE jOB_BASE;
    public EQ_ITEM_BASE eQ_ITEM_BASE;

    public string TAG_MAIN;

    public string TAG_SUB_1;
    public string TAG_SUB_2;
    public string TAG_SUB_3;

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);
        name = Next();
        desc = Next();

        Enum.TryParse<JOB_BASE>( Next() , out jOB_BASE );
        Enum.TryParse<EQ_ITEM_BASE>( Next() , out eQ_ITEM_BASE );
        TAG_MAIN = Next();

        TAG_SUB_1 = Next();
        TAG_SUB_2 = Next();
        TAG_SUB_3 = Next();
    }
}

public class CSV_EqItemDefinePage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_EqItemDefine();
    }
}