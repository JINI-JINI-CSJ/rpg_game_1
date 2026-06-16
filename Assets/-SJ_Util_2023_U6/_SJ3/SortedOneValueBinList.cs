using System;
using System.Collections;
using System.Collections.Generic;

public class SortedOneValueBinObj : IComparable<SortedOneValueBinObj>
{
    public float float_val;
    public object data;
    public int CompareTo(SortedOneValueBinObj other)
    {
        if( float_val < other.float_val )       return -1;
        else if( float_val > other.float_val )  return 1;
        return 0;
    }
}

public class SortedOneValueBinList
{
    public List<SortedOneValueBinObj> _sorted = new List<SortedOneValueBinObj>();

    public void Clear()
    {
        _sorted.Clear();
    }

    public int Count => _sorted.Count;

    public void Add( float float_val , object data )
    {
        SortedOneValueBinObj binObj = new SortedOneValueBinObj();
        binObj.float_val = float_val;
        binObj.data = data;
        _sorted.Add( binObj );
    }

    public void Sort()
    {
        _sorted.Sort();
    }

    public void Build(IEnumerable<SortedOneValueBinObj> objects)
    {
        _sorted.Clear();
        _sorted.AddRange(objects);
        _sorted.Sort();
    }

// 가장 가까운 객체 반환 — O(log n)
    public SortedOneValueBinObj FindNearest(float query)
    {
        int idx = FindNearestIndex( query );
        return _sorted[idx];
    }

    public int FindNearestIndex(float query)
    {
        if (_sorted.Count == 0)
            throw new InvalidOperationException("목록이 비어 있습니다.");

        SortedOneValueBinObj find_key = new SortedOneValueBinObj();
        find_key.float_val = query;

        // BinarySearch: 정확히 일치하면 양수 인덱스, 없으면 ~삽입위치 반환
        int idx = _sorted.BinarySearch(find_key);

        if (idx >= 0)
            return idx;  // 정확히 일치

        // 삽입될 위치 복원
        idx = ~idx;

        // 경계 처리
        if (idx == 0) return 0;
        if (idx == _sorted.Count) return _sorted.Count-1;

        // 좌우 이웃 중 더 가까운 쪽 반환
        float leftDiff  = query - _sorted[idx - 1].float_val;
        float rightDiff = _sorted[idx].float_val - query;

        return leftDiff <= rightDiff ? idx - 1 : idx;
    }

    public object FindNearest_Data(float query)
    {
        SortedOneValueBinObj binObj = FindNearest(query);
        if( binObj != null ) return binObj.data;
        return null;
    }

    public List<object> FindNearest_Range( float q1 , float q2 )
    {
        List<object> lt = new();
        int idx_1 = FindNearestIndex(q1);
        int idx_2 = FindNearestIndex(q2);

        if( idx_1 < idx_2 )
        {
            for( int i = idx_1 ; i < idx_2 + 1 ; i++ )
            {
                lt.Add( _sorted[i].data );
            }
        }
        return lt;
    }
}
