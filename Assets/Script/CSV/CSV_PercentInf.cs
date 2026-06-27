using System.Collections.Generic;
using UnityEngine;

// ``ID	이름	태그	등급1	등급2	3	4	5	6	7	8	9	10	
public class CSV_PercentInf : SJ_CSV_BaseObj
{
    public string name;
    public string TAG;

    public List<float> per_list;

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);

        name = Next();
        TAG = Next();
        per_list = Remain_Data_Float();
    }

    public int GetPerIndex( Mng_X128SS rd )
    {
        return rd.Step_Random_Idx( per_list );
    }
}

public class CSV_PercentInfPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_PercentInf();
    }

    public int GetPerIdx( Mng_X128SS rd , string tag , ref int max_arg )
    {
        CSV_PercentInf csv = Find_Str( tag ) as CSV_PercentInf;
        if( csv == null ) return -1;
        max_arg = csv.per_list.Count;
        return csv.GetPerIndex( rd );
    }
}
