using System.Collections.Generic;
using UnityEngine;


// 객체들 2D 맵에 배치하고 , 4방향 이동
// 메뉴 등을 배치하고 4방향으로 선택할때의 유틸
public class SJ_GridObjDir
{
    public Dictionary<Vector2Int,object> dic = new();

    public class _POS_OBJ
    {
        public Vector2Int pos;
        public object obj;

        // 계산된 거리
        public float sqlen = 0;
    }

    public Vector2Int max;

    public Vector2Int cursor;   // 현재 위치

    public void Clear()
    {
        dic.Clear();
        max = Vector2Int.zero;
    }

    public void Add( int x , int y , object obj )
    {
        Vector2Int pos = new Vector2Int( x , y );
        dic[pos] = obj;
    }


    // 인자는 절대값 1 까지만.
    public object Move( int x , int y )
    {
        if( dic.Count == 0 ) return null;
        if( dic.Count == 1 )
        {
            List<object> lt = new(dic.Values);
            return lt[0];
        }

        return null;
    }

    List<_POS_OBJ> FindNear_ByStart( int tx , int ty )
    {
        if( tx == 0 && ty == 0 ) return null;

        // 시작점 제외하고 가장 가까운 객체 리스트 정렬

        return null;
    }

}
