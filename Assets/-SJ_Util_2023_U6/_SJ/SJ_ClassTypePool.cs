using System.Collections;
using System.Collections.Generic;
using System;


public class SJ_ClassTypePool 
{
    public int  add_extend = 20;

    public Dictionary<System.Type,Queue<object>> dic = new Dictionary<System.Type, Queue<object>>();

    public object GetInst( string class_name )
    {
        System.Type componentType = System.Type.GetType( class_name );
        if( componentType == null )
        {
            return null;
        }
        return GetInst( componentType );
    }

    public object GetInst( System.Type t )
    {
        Queue<object> q = null;
        if( dic.TryGetValue( t , out q ) == false )
        {
            q = new Queue<object>();
            dic[t] = q;
        }
        return Pop( q , t );
    }

    public object   Pop( Queue<object> q  , System.Type t)
    {
        if( q.Count < 1 )
        {
            Extend( q , t );
        } 
        return q.Dequeue();
    }

    public void Extend( Queue<object> q , System.Type t )
    {
        for( int i = 0 ; i < add_extend ; i++ )
        {
            object inst = Activator.CreateInstance(t);
            q.Enqueue( inst );
        }
    }

    public bool Return( Object  obj )
    {
        System.Type t = obj.GetType();
        Queue<object> q = null;
        if( dic.TryGetValue( t , out q ) == false )
        {
            // 
            return false;
        }
        q.Enqueue( obj );
        return true;
    }

    public List<object> Get_All( System.Type t = null )
    {
        List<object> lt = new List<object>();

        if( t == null )
        {
            foreach( Queue<object> s in dic.Values )
            {
                lt.AddRange( s.ToArray() );
            }
        }else{
            Queue<object> q = null;
            if( dic.TryGetValue( t , out q ) )
            {
                lt.AddRange( q.ToArray() );
            }
        }
        return lt;
    }
}
