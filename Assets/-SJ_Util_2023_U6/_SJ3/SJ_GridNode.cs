using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SJ_GridNode
{
    // 전체 길이 , 제곱하면 넓이
    public float max_dist;

    // 등분 갯수 , 한 변 , 제곱하면 전체 셀
    public int grid_num;

    public List<SJ_GridNodeUnit> sJ_Grids = new();   
    public class SJ_GridNodeUnit
    {
        public List<object> data_list = new List<object>();

        public void Add( object obj ){data_list.Add(obj);}
    }


    public void Create( float max , int grid )
    {
        max_dist = max;
        grid_num = grid;
        int t = grid_num * grid_num;

        sJ_Grids.Clear();
        for( int i = 0 ; i < t ; i++ )
        {
            sJ_Grids.Add( new SJ_GridNodeUnit() );
        }
    }

    public int GetIndexPos( Vector2 pos )
    {
        float unit = max_dist / grid_num;
        int x = (int)(pos.x / unit);
        int y = (int)(pos.y / unit);
        return y * grid_num + x;
    }

    public int GetIndexPos_Center( Vector2 pos )
    {
        pos.x += max_dist * 0.5f;
        pos.y += max_dist * 0.5f;
        return GetIndexPos(pos);
    }

    public SJ_GridNodeUnit GetNode( Vector2 pos )
    {
        int idx = GetIndexPos( pos );
        if( idx < 0 || idx >= sJ_Grids.Count )return null;
        return sJ_Grids[idx];
    }

    public SJ_GridNodeUnit GetNode_Center( Vector2 pos )
    {
        int idx = GetIndexPos_Center( pos );
        if( idx < 0 || idx >= sJ_Grids.Count )return null;
        return sJ_Grids[idx];
    }

    public void AddData( Vector2 pos , object arg_data )
    {
        SJ_GridNodeUnit unit = GetNode( pos );
        if( unit != null ) unit.Add( arg_data );
    }

    public void AddData_Center( Vector2 pos , object arg_data )
    {
        SJ_GridNodeUnit unit = GetNode_Center( pos );
        if( unit != null ) unit.Add( arg_data );
    }
}


public class SJ_GridNodeGroup
{
    public Dictionary<string,SJ_GridNode> dic = new();

    public void Create( float max , int grid , params string[] args_name )
    {
        if( args_name.Length < 1 ) return;

        foreach( var s in args_name )
        {
            SJ_GridNode sJ_Grid = new SJ_GridNode();
            sJ_Grid.Create( max , grid );
            dic[s] = sJ_Grid;
        }
    }

    public SJ_GridNode Find( string tag )
    {
        SJ_GridNode node = null;
        dic.TryGetValue( tag , out node );
        return node;
    }

    public SJ_GridNode.SJ_GridNodeUnit GetNode( string tag , Vector2 pos )
    {
        SJ_GridNode node = Find( tag );
        if( node == null ) return null;
        return node.GetNode(pos);
    }

    public SJ_GridNode.SJ_GridNodeUnit GetNode_Center( string tag , Vector2 pos )
    {
        SJ_GridNode node = Find( tag );
        if( node == null ) return null;
        return node.GetNode_Center(pos);
    }

    public void AddData( string tag , Vector2 pos , object arg_data )
    {
        SJ_GridNode node = Find( tag );
        if( node != null )node.AddData( pos , arg_data );
    }

    public void AddData_Center( string tag , Vector2 pos , object arg_data )
    {
        SJ_GridNode node = Find( tag );
        if( node != null )node.AddData_Center( pos , arg_data );
    }
    
}