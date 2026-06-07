using System.Collections;
using System.Collections.Generic;


public class SJ_RandomRange 
{
    public  List<string>    lt_str = new List<string>();
    public  List<int>       lt_per = new List<int>();

    public  List<int>       lt_total_per = new List<int>();

    public  int             total_per = 0;

    public  void    Clear()
    {
        lt_str.Clear();
        lt_per.Clear();
        lt_total_per.Clear();
        total_per = 0;
    }

    public  int     Count()
    {
        return lt_per.Count;
    }

    public  bool    Add( string str , int per )
    {
        if( per < 1 ) return false;

        lt_str.Add(str);
        lt_per.Add(per);

        lt_total_per.Add(total_per);

        total_per += per;

        return true;
    }

    public string   Random()
    {
        if( lt_str.Count < 1 ) return "";
        if( lt_str.Count < 2 ) return lt_str[0];

        int per = UnityEngine.Random.Range( 0 , total_per );

        int sel_idx = 0;
        for( int i = 0 ; i < lt_total_per.Count ; i++ )
        {
            if( lt_total_per[i] > per )
            {
                break;
            }
            sel_idx = i;            
        }

        // 예 ) 3가지  , 50 씩이면

        // 0 , 50 , 100 , 150 

        // lt_total_per = 0 , 50 , 100
        // total_per = 150

        // 0~49 -> 0 ,  50 ~ 99 -> 1 , 100 ~ 149 -> 2
        // 75 -> 1

        return lt_str[sel_idx];
    }
}
