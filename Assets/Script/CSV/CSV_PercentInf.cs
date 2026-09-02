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

    // 최소 최대 쌍 확률
    // 2개씩 단위로 최소 최대를 계산한다. 홀수 인덱스라면 무시

    public float GetPerMinMax(Mng_X128SS rd , int step_idx )
    {
        // 인덱스 안되면 에러
        int min_idx = step_idx * 2;
        int max_idx = min_idx + 1;
        if( per_list.Count <= max_idx ) return -1;

        float min = per_list[min_idx];
        float max = per_list[max_idx];

        return rd.NextFloat( min , max );
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

    public float GetPerMinMax(Mng_X128SS rd , string tag , int step_idx )
    {
        CSV_PercentInf csv = Find_Str( tag ) as CSV_PercentInf;
        if( csv == null ) return -1;
        return csv.GetPerMinMax(rd , step_idx );
    }
}
