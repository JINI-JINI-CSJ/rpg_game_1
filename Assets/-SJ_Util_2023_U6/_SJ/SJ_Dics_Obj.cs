using System.Collections;
using System.Collections.Generic;


// 변수형태 : 문자열 <문자열 , 객체>
// 래핑

public class SJ_Dics_Obj 
{
    public Dictionary<string,Dictionary<string,object>> dic = new Dictionary<string, Dictionary<string, object>>();

    public Dictionary<string,object> Find_Main( string main , bool new_inst = false )
    {
        Dictionary<string,object> d = null;
        if( dic.TryGetValue( main , out d ) == false )   
        {
            if( new_inst )
            {
                d = new Dictionary<string, object>();
                dic[main] = d;
                return d;
            }
        }
        return d;
    }

    public object Find_Value( string main , string sub )
    {
        Dictionary<string,object> d = Find_Main(main);
        if( d == null ) return null;

        object obj = null;
        d.TryGetValue( sub , out obj );
        return obj;
    }

    public int Find_Value_INT( string main , string sub )
    {
        Dictionary<string,object> d = Find_Main(main);
        if( d == null ) return 0;

        object obj = null;
        d.TryGetValue( sub , out obj );

        if( obj == null ) return 0;
        int? val = (int)obj;

        return val.Value;
    }


    public string Find_Value_STR( string main , string sub )
    {
        Dictionary<string,object> d = Find_Main(main);
        if( d == null ) return "";

        object obj = null;
        d.TryGetValue( sub , out obj );

        if( obj == null ) return "";
        string val = obj as string;

        return val;
    }

    public void SetValue( string main , string sub ,object val )
    {
        Dictionary<string,object> d = Find_Main(main , true);
        d[sub] = val;
    }
}


public class SJ_DIC<T>
{
    Dictionary<T,object> dic = new Dictionary<T,object>();

    public void Clear()
    {
        dic.Clear();
    }

    public void SetValue(T key, object val) { dic[key] = val; }
    public object Find_Value( T key )
    {
        object obj = null;
        dic.TryGetValue( key , out obj );
        return obj;
    }

    public int Find_Value_INT( T key)
    {
        object obj = Find_Value(key);
        if( obj == null ) return 0;            
        
        int? val = (int)obj;
        return val.Value;
    }

    public float Find_Value_FLOAT( T key )
    {
        object obj = Find_Value(key);
        if( obj == null ) return 0;
        float? val = (float)obj;
        return val.Value;
    }


    public string Find_Value_STR( T key )
    {
        object obj = Find_Value(key);
        if( obj == null ) return "";
        string val = (string)obj;
        return val;
    }

    public int Add_Value_INT( T key , int add )
    {
        int val = Find_Value_INT( key );
        val += add;
        SetValue( key , val );
        return val;
    }

    public float Add_Value_FLOAT( T key , float add )
    {
        float val = Find_Value_FLOAT( key );
        val += add;
        SetValue( key , val );
        return val;
    }
}