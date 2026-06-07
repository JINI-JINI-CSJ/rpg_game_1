using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
// 프로퍼티 등록 , 해제
///

public class SJ_PropReg 
{
    public delegate void OnFunc_PropReg( object _args );

    public  class _PROP_REG
    {
        public  _PROP_REG(OnFunc_PropReg add , OnFunc_PropReg remove , object _a )
        {
            func_reg = add;
            func_remove = remove;
            args = _a;
        }

        public  OnFunc_PropReg  func_reg;
        public  OnFunc_PropReg  func_remove;
        public  object          args;
        public  void Coll_REG()
        {
            if( func_reg != null ) func_reg.Invoke(args);
        }
        public  void Coll_REMOVE()
        {
            if( func_remove != null ) func_remove.Invoke(args);
        }
    }

    public  Dictionary<object , _PROP_REG> dic_prop = new Dictionary<object, _PROP_REG>();

    public  Dictionary<string , _PROP_REG> dic_prop_str = new Dictionary<string, _PROP_REG>();

    public  void    Clear()
    {
        dic_prop.Clear();
        dic_prop_str.Clear();
    }

    public  bool    Add( object obj , OnFunc_PropReg _func_reg , OnFunc_PropReg _func_remove , object _args )
    {
        _PROP_REG val = null;
        if( dic_prop.TryGetValue( obj , out val ) )
        {
            return false;
        }
        _PROP_REG s = new _PROP_REG( _func_reg , _func_remove , _args );
        dic_prop[obj] = s;
        s.Coll_REG();
        return true;
    }

    public  void    Remove( object obj )
    {
        _PROP_REG val = null;
        if( dic_prop.TryGetValue( obj , out val ) )
        {
            val.Coll_REMOVE();            
            return;
        }
    }


    public  bool    Add( string obj , OnFunc_PropReg _func_reg , OnFunc_PropReg _func_remove , object _args , bool exe_force = false )
    {
        _PROP_REG val = null;
        if( dic_prop_str.TryGetValue( obj , out val ) )
        {
            if( exe_force ) val.Coll_REG();
            return false;
        }
        _PROP_REG s = new _PROP_REG( _func_reg , _func_remove , _args );
        dic_prop_str[obj] = s;
        s.Coll_REG();
        return true;
    }

    public  void    Remove( string obj )
    {
        _PROP_REG val = null;
        if( dic_prop_str.TryGetValue( obj , out val ) )
        {
            val.Coll_REMOVE();            
            return;
        }
    }
}
