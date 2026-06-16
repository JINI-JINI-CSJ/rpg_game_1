using System.Collections;
using System.Collections.Generic;
public class SJ_SimpleSubscribe
{
    public Dictionary<object,SJ_CallFuncClass_Cash> dic = new();

    public void Reg( object obj )
    {
        dic.Add( obj , new SJ_CallFuncClass_Cash() );
    }

    public void Remove( object obj )
    {
        if( dic.ContainsKey(obj) )
        {
            dic.Remove(obj);
        }
    }

    public void RemoveALL()
    {
        dic.Clear();
    }
    
    public void FuncCall( string func )
    {
        foreach( var s in dic )
        {
            //s.Value.Call_Normal( s.Key ,  )
        }
    }
}
