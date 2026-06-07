using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_RECT 
{
    public Vector2Int min;
    public Vector2Int max;

    public void Init_Minus(int val = 9999999)
    {
        min = new Vector2Int(  val ,  val );
        max = new Vector2Int( -val , -val );
    }

    public void Extend( Vector2Int p )
    {
        if( p.x < min.x ) min.x = p.x;
        if( p.x > max.x ) max.x = p.x;
        if( p.y < min.y ) min.y = p.y;
        if( p.y > max.y ) max.y = p.y;
    }

    // 민 맥스 다 포함
    public bool Contain( Vector2Int p )
    {
        if( p.x < min.x || p.x > max.x ||
            p.y < min.y || p.y > max.y )
        {
            return false;
        }
        return true;
    }

    public void Limit( ref Vector2Int p )
    {
        if( p.x < min.x )p.x = min.x;
        if( p.x > max.x )p.x = max.x;
        if( p.y < min.y )p.y = min.y;
        if( p.y > max.y )p.y = max.y;
    }
}
