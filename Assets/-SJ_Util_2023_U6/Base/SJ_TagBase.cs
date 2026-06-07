using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public  class SJ_TagBaseObj
{

    public    SJ_TagBaseMng par_mng;

    public  HashSet<int>   reg_tag = new HashSet<int>();

    //=============================================================================================
    //=============================================================================================
    // 전역 
    static    public  object      GetArg( Dictionary<string,object> args , string str )
    {
        if( args.ContainsKey(str) == false ) return null;
        return args[str];
    }
    static    public  int         GetArgInt( Dictionary<string,object> args ,string str )
    {
        object obj = GetArg(args,str);
        if( obj == null ) return 0;
        return (int)obj;
    }
    static    public  float       GetArgFloat( Dictionary<string,object> args ,string str )
    {
        object obj = GetArg(args,str);
        if( obj == null ) return 0;
        return (float)obj;
    }
    static    public  string      GetArgStr( Dictionary<string,object> args ,string str )
    {
        object obj = GetArg(args,str);
        if( obj == null ) return "";
        return (string)obj;
    }

    static    public  Dictionary<string,object>        SetArg(Dictionary<string,object> args , string str , object obj)
    {
        if( args == null ) args = new Dictionary<string, object>();
        args[str] = obj;
        return args;
    }
    //=============================================================================================
    //=============================================================================================

    public  void        AddTag(int val)
    {
        reg_tag.Add( val );
    }

    // string , object
    virtual public  object     OnFunc( int evt_int = 0 , string evt_str = "" ,   Dictionary<string,object> args = null )
    {
        return null;
    }

    virtual public  bool    OnTryAdd_Name(HashSet<SJ_TagBaseObj> hs){return true;}
    virtual public  bool    OnTryAdd_Tag(HashSet<SJ_TagBaseObj> hs){return true;}
    virtual public  void    OnPreAdd( object arg = null ){}
    virtual public  void    OnAdd(){}
    virtual public  void    OnRemove(){}
    virtual public  string  GetName(){return "";}
    virtual public  object  GetGameObject(){return null;}
    virtual public  void    ClearAll(){}
}

public class SJ_TagBaseMng
{
    public  object      obj_have;

    public  object  go_self;

    // 객체 이름 태그
    public  Dictionary<string,HashSet<SJ_TagBaseObj>>  dic_name_hs_obj = new Dictionary<string, HashSet<SJ_TagBaseObj>>();

    // 이벤트 태그
    public  Dictionary<int,HashSet<SJ_TagBaseObj>>  dic_tag_hs_obj = new Dictionary<int, HashSet<SJ_TagBaseObj>>();

	public	delegate	void	DlgFunc_TagMng_Event(SJ_TagBaseObj obj , bool enter_remove ); // true : enter , false : return


	public	DlgFunc_TagMng_Event	dlgFunc_TagMng_Event_Change; // 리스트가 변경되었을때 

    public  void SetFunc_Event_Change(DlgFunc_TagMng_Event dt )
    {
        dlgFunc_TagMng_Event_Change = dt;
    }


    public  void    SetGoSelf( object go )
    {
        go_self = go;
        obj_have = go;
    }



    public  void    ClearAll()
    {
        foreach( KeyValuePair<string , HashSet<SJ_TagBaseObj> > s in dic_name_hs_obj )
        {
            HashSet<SJ_TagBaseObj> hs = s.Value;
            foreach( SJ_TagBaseObj obj in hs )
            {
                obj.OnRemove();
                obj.ClearAll();
            }
        }
        dic_name_hs_obj.Clear();
        dic_tag_hs_obj.Clear();
    }

    public  bool    Add( object go , object arg = null )
    {
        // SJ_TagBaseObj_Mono com_obj = go.GetComponent<SJ_TagBaseObj_Mono>();
        // if( go_self != null )
        // {
        //     SJ_Unity.SetEqTrans( go.transform , null , go_self.transform );
        // }
        //return Add( com_obj.tag_obj );

        return false;
    }

    public  bool    Add( SJ_TagBaseObj obj , object arg = null )
    {
        bool add = false;

        // ===== 객체 =====
        HashSet<SJ_TagBaseObj> hs = null;
        if( dic_name_hs_obj.TryGetValue(obj.GetName() , out hs) == false )
        {
            hs = new HashSet<SJ_TagBaseObj>();
            dic_name_hs_obj[obj.GetName()] = hs;
        }
        // 같은 포인터 불가
        if( hs.Contains( obj ) )
        {
//Debug.Log( "!!!! 같은 포인터 불가 : " + obj.GetName() );
            return false;
        }
        if( obj.OnTryAdd_Name( hs ) == false ) 
            return false;
        hs.Add( obj );
        // ================

        // 태그
//Debug.Log( "태그 갯수 : " + obj.reg_tag.Count );
        foreach( int id in obj.reg_tag )
        {
            if( AddTagID( id , obj ) )
            {
                add  = true;
            }
        }

        obj.par_mng = this;

        if(add)
        {
//Debug.Log( "추가 태그 객체 : " + obj.GetName() );            
            obj.OnPreAdd( arg );
            obj.OnAdd();
        }else{
//Debug.Log( "!!!! 추가 안됨 : " + obj.GetName() + " : 태그 갯수 : " + obj.reg_tag.Count );
        }

        if( dlgFunc_TagMng_Event_Change != null ) dlgFunc_TagMng_Event_Change.Invoke(obj , true);

        return add;
    }

    public  HashSet<SJ_TagBaseObj>  Find_Name( string name )
    {
        HashSet<SJ_TagBaseObj> hs = null;
        dic_name_hs_obj.TryGetValue(name , out hs);
        return hs;
    }

    public  int Find_Name_Count( string name )
    {
        HashSet<SJ_TagBaseObj> hs =Find_Name( name );
        if( hs == null ) return 0;
        return hs.Count;
    }

    public  bool    AddTagID( int id , SJ_TagBaseObj obj )
    {
        HashSet<SJ_TagBaseObj> hs = null;
        if( dic_tag_hs_obj.TryGetValue(id , out hs) == false )
        {
            hs = new HashSet<SJ_TagBaseObj>();
            dic_tag_hs_obj[id] = hs;
        }

        // 완전히 같은 객체 포인터는 안됨...
        // 중복으로 넣을꺼면 새로운 객체 생성
        if( hs.Contains( obj ) )
        {
            return false;
        }

        if( obj.OnTryAdd_Tag( hs ) == false ) 
            return false;

        hs.Add( obj );

        return true;
    }

    public  void    Remove( SJ_TagBaseObj obj )
    {
        HashSet<SJ_TagBaseObj> hs = null;
        if( dic_name_hs_obj.TryGetValue(obj.GetName() , out hs) )
        {
            if(hs.Contains(obj) )
            {
                hs.Remove( obj );
            }
        }

        foreach( int id in obj.reg_tag )
        {
            RemoveTagID(id , obj);
        }
        obj.OnRemove();

        if( dlgFunc_TagMng_Event_Change != null ) dlgFunc_TagMng_Event_Change.Invoke(obj , false);
    }


    public  void    RemoveTagID( int id , SJ_TagBaseObj obj )
    {
        HashSet<SJ_TagBaseObj> hs = null;
        if( dic_tag_hs_obj.TryGetValue(id , out hs) )
        {
            if( hs.Contains( obj ) )
            {
                hs.Remove( obj );
            }   
        }
    }

    public  HashSet<SJ_TagBaseObj>  Find_Tag( int tag )
    {
        HashSet<SJ_TagBaseObj> hs = null;
        dic_tag_hs_obj.TryGetValue( tag , out hs );
        return hs;
    }

    static public  Dictionary<string,object>     MakeArg(  object[,] args )
    {
        Dictionary<string,object> dic_arg = new Dictionary<string, object>();
        if( args != null )
        {
            for( int i = 0; i < args.Length ; i++)
            {
                dic_arg[ (string) args[i,0]] = args[i,1];
            }
        }
        return dic_arg;
    }
    public  object    FuncCall( int tag , int evt_int = 0 , string evt_str = "" , Dictionary<string,object> args = null )
    {
        object obj_r = null;
        HashSet<SJ_TagBaseObj> hs = Find_Tag(tag);
        if( hs != null )
        {
            foreach( SJ_TagBaseObj s in hs )
            {
                object r = s.OnFunc( evt_int , evt_str , args );
                if( r != null )
                {
                    obj_r = r;
                }
            }
        }
        return obj_r;
    }

    public  Dictionary<string,object>    FuncCall_ArgsReturn( int tag , int evt_int = 0 , string evt_str = "" , Dictionary<string,object> args = null )
    {
        if( args == null )
        {
            args = new Dictionary<string, object>();
        }
        FuncCall( tag , evt_int , evt_str , args );
        return args;
    }



    public  Dictionary<string,HashSet<SJ_TagBaseObj>>   TagAll_Name()
    {
        return dic_name_hs_obj;
    }


    static public   object  GetArgs( Dictionary<string,object> args , string name )
    {
        object obj = null;
        if( args.TryGetValue( name , out obj ) == false )
        {
            return null;
        }
        return obj;
    }

    static public   string  GetArgs_STR( Dictionary<string,object> args , string name )
    {
        object obj = GetArgs( args , name );
        if( obj == null ) return "";
        return obj as string;
    }

    static public   int  GetArgs_Int( Dictionary<string,object> args , string name )
    {
        object obj = GetArgs( args , name );
        if( obj == null ) return 0;
        return (int)obj;
    }

    static public   float  GetArgs_Float( Dictionary<string,object> args , string name )
    {
        object obj = GetArgs( args , name );
        if( obj == null ) return 0;
        return (float)obj;
    }

}



