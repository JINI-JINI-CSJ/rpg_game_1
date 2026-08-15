using System.Collections.Generic;
using UnityEngine;


// 객체들 2D 맵에 배치하고 , 4방향 이동
// 메뉴 등을 배치하고 4방향으로 선택할때의 유틸
public class SJ_GridObjDir
{
    public Dictionary<Vector2Int,object> dic = new();

    public class _POS_DIST
    {
        public Vector2Int pos;
        public object obj;

        // 거리 계산용
        public int off_x;
        public int off_y;
        public int total_len;
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

        cursor = pos;
    }

    public void SetCursorByObj( object obj )
    {
        foreach( var s in dic )
        {
            if( s.Value == obj )
            {
                cursor = s.Key;
                break;
            }
        }
    }

    public object GetCursor()
    {
        object obj = null;
        dic.TryGetValue( cursor , out obj );
        return obj;
    }

    // 인자는 절대값 1 까지만.
    public object Move( int x , int y )
    {
        // 인자 절대값 1 초과시 에러

        if( dic.Count == 0 ) return null;
        if( dic.Count == 1 )
        {
            List<object> lt = new(dic.Values);
            return lt[0];
        }

        // xy 축으로 배열이 1개 이하이며 다른 축 입력 인자가 있으면 무효
        int xc = 0;
        int yc = 0;
        GetLineCount( out xc , out yc );

        // x 축 입력인데 x 카운트가 1개라면 무효
        if( x != 0 && xc <= 1 ) return null;
        if( y != 0 && yc <= 1 ) return null;
        
        int nx = cursor.x + x;
        int ny = cursor.y + y;

        List<_POS_DIST> ps_nears = FindNear_ByStart( nx , ny );

        if( ps_nears.Count > 0 )
        {
            cursor = ps_nears[0].pos;
            return ps_nears[0].obj;
        }

        return null;
    }

    // 각 축으로 라인 카운트
    void GetLineCount( out int xc , out int yc )
    {
        HashSet<int> hs_x = new();
        HashSet<int> hs_y = new();

        foreach( var s in dic )
        {
            hs_x.Add( s.Key.x );
            hs_y.Add( s.Key.y );
        }
        xc = hs_x.Count;
        yc = hs_y.Count;

        
    }

    List<_POS_DIST> FindNear_ByStart( int nx , int ny )
    {
        //if( nx == 0 && ny == 0 ) return null;

        List<_POS_DIST> lt = new();
        // 시작점 제외하고 가장 가까운 객체 리스트 정렬
        foreach( var s in dic )
        {
            //if( cursor.x == nx && cursor.y == ny )continue;
            _POS_DIST ps = new();
            ps.obj = s.Value;
            ps.pos = s.Key;
            ps.off_x = s.Key.x - nx;
            ps.off_y = s.Key.y - ny;
            ps.total_len = Mathf.Abs( ps.off_x ) +  Mathf.Abs( ps.off_y );
            lt.Add(ps);
        }

        lt.Sort( 
            (x,y) =>
            {
                if( x.total_len < y.total_len ) return -1;
                if( x.total_len > y.total_len ) return 1;
                return 0;
            }
         );

        return lt;
    }

}
