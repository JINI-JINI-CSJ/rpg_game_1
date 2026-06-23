using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 마법속성정의
// ``ID	이름	설명	정의 태그	유리 속성1	유리2	유리3	불리속성1	불리2	불리3																
public class CSV_MagicPropDefine : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string TAG;

    public string Advantage_1;
    public string Advantage_2;
    public string Advantage_3;

    public string Penalty_1;
    public string Penalty_2;
    public string Penalty_3;


    public HashSet<CSV_MagicPropDefine> hs_Advantage = new HashSet<CSV_MagicPropDefine>();
    public HashSet<CSV_MagicPropDefine> hs_Penalty= new HashSet<CSV_MagicPropDefine>();

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);
        name = Next();
        desc = Next();
        TAG = Next();
        Advantage_1 = Next();
        Advantage_2 = Next();
        Advantage_3 = Next();
        Penalty_1 = Next();
        Penalty_2 = Next();
        Penalty_3 = Next();
    }

    public void LoadAfter( CSV_MagicPropDefinePage page )
    {
        Add_HS( page , Advantage_1 , hs_Advantage );
        Add_HS( page , Advantage_2 , hs_Advantage );
        Add_HS( page , Advantage_3 , hs_Advantage );

        Add_HS( page , Penalty_1 , hs_Penalty );
        Add_HS( page , Penalty_2 , hs_Penalty );
        Add_HS( page , Penalty_3 , hs_Penalty );
    }

    public void Add_HS( CSV_MagicPropDefinePage page , string tag , HashSet<CSV_MagicPropDefine> hs )
    {
        CSV_MagicPropDefine csv = page.FindTag( tag );
        if( csv != null )
        {
            hs.Add(csv);
        }
    }
}


public class CSV_MagicPropDefinePage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_MagicPropDefine();
    }

    public CSV_MagicPropDefine FindTag( string tag )
    {
        foreach( var s in dic_int.Values.Cast<CSV_MagicPropDefine>() )
        {
            if( s.TAG == tag ) return s;
        }
        return null;
    }

    public override void LoadAfter()
    {
        foreach( var s in dic_int.Values.Cast<CSV_MagicPropDefine>() )
        {
            s.LoadAfter( this );
        }
    }
}