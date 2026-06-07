using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 4방향 그리드 맵 유틸 
public class SJ_Util4GridMap 
{
    static  public  List<Vector2Int>    GetRange( Vector2Int pos , int range )
    {
        List<Vector2Int> lt = new List<Vector2Int>();

        // y 축 중심에서 위아래로 가면서
        // 마름모 꼴로 한줄씩
        int total_row = range + 1;

        for( int y = 0 ; y < total_row ; y++ )
        {
            for( int x = 0 ; x < (total_row - y) ; x++ )
            {
                lt.Add( new Vector2Int( x ,y ) + pos );
                if( x != 0 ) lt.Add( new Vector2Int( -x , y ) + pos );
                if( y != 0 ) lt.Add( new Vector2Int(  x , -y ) + pos );
                if( x != 0 && y != 0 ) lt.Add( new Vector2Int(  -x , -y ) + pos );
            }
        }

        return lt;
    }

    static public int GetDistance( Vector2Int pos1 , Vector2Int pos2 )
    {
        return Mathf.Abs( pos1.x - pos2.x ) + Mathf.Abs( pos1.y - pos2.y );
    }
}

