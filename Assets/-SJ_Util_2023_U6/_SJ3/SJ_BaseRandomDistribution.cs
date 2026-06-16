using System.Collections;
using System.Collections.Generic;


// 몇개의 항목을 랜덤 분배한다.
// 미리 몇개는 초기값을 세팅할수도 있다.
public class SJ_BaseRandomDistribution 
{
    // 반드시 구현해야 함
    virtual public int RandomInt( int min , int max ){return 0;}    

    public int total_dist;   // 총 배분
    public int max_unit;     // 각 요소 최대
    public Dictionary<object,int> dic = new Dictionary<object, int>();

    public void SetTotalMax( int _t , int _m ){total_dist = _t;max_unit = _m;}

    public void Clear(){dic.Clear();}
    public void StartAdd( object obj , int val = 0 )
    {
        Clear();
        Add( obj , val );
    }
    public void Add( object obj , int val = 0 )
    {
        dic[obj] = val;
    }

    public void PrcWork()
    {
        int t_total = 0;
        // 일단 각 항목대로 max 기준 랜덤을 넣는다.
        Dictionary<object,int> dic_copy = new Dictionary<object, int>( dic );
        foreach( var s in dic_copy )
        {
            int start_val = s.Value;
            int rd = RandomInt( 0 , max_unit - start_val );
            int t = rd+start_val;
            dic[s.Key] = t;
            t_total += t;
        }

        // 각 요소 총합에서 기준 토탈로 나누어서 그 비율대로 다시 곱함
        float fr = (float)total_dist / (float)t_total;

        // 다시 비율대로 곱했을때 int 변환하면서 모자란 값을 다시 계산하기 위해서..
        int t_i_total = 0;

        dic_copy = new Dictionary<object, int>( dic );
        foreach( var s in dic_copy )
        {
            int start_val = s.Value;
            int t = (int)((float)start_val * fr);
            dic[s.Key] = t;
            t_i_total += t;
        }
        int remain_t = total_dist - t_i_total;

        List<object> lt_obj_remain = new List<object>();        
        // 남은 값 만큼 랜덤 분배 한다.
        while(true)
        {
            if( remain_t < 1 )break;

            // 일단 분배 가능한 목록을 리스트를 만든다.
            lt_obj_remain.Clear();

            foreach( var s in dic )
            {
                // max 가 아닌 목록들
                if(s.Value != max_unit)lt_obj_remain.Add(s.Key);
            }

            if( lt_obj_remain.Count < 1 )break;
            int idx_obj = RandomInt( 0 , lt_obj_remain.Count );

            object obj = lt_obj_remain[idx_obj];

            dic[obj] = dic[obj] + 1;
            remain_t--;
        }
    }

    public int FindValue( object key_obj )
    {
        int find = 0;
        dic.TryGetValue( key_obj , out find );
        return find;
    }
}
