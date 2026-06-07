using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_TagBaseObj_Mono : MonoBehaviour
{
    public  class  SJ_TagBaseObj_mono_call : SJ_TagBaseObj
    {
        public  SJ_TagBaseObj_Mono  mono;

        public override object OnFunc(int evt_int = 0, string evt_str = "", Dictionary<string,object> args = null)
        {
            base.OnFunc(evt_int, evt_str, args);
            return mono.OnFunc( evt_int, evt_str , args );
        }

        override public  bool    OnTryAdd_Tag(HashSet<SJ_TagBaseObj> hs)
        {
            return mono.OnTryAdd(hs);
        }

        public override void OnPreAdd( object arg = null )
        {
            mono.OnPreAdd( arg );
        }

        override public  void    OnAdd()
        {
            mono.OnAdd();
        }
        override public  void    OnRemove()
        {
            mono.OnRemove();
        }

        override     public  string  GetName()
        {
            return    mono.GetName();
        }

        // public override GameObject GetGameObject()
        // {
        //     return mono.gameObject;
        // }

        override public  void    ClearAll()
        {
            mono.ClearAll();
        }
    }

    public    SJ_TagBaseObj_mono_call   tag_obj = new SJ_TagBaseObj_mono_call();

    public  SJ_TagBaseMng   par_mng;

    public    GameObject  go_self_ref;

    void Awake()
    {
//        Debug.Log( "어워크 태그 오브젝투~~~~~~~~~ : " + gameObject.name );        
        tag_obj.mono = this;           
        OnAwake_TagObj();
    }

    virtual public  void    OnAwake_TagObj(){}

    public void     AddTag(int val)
    {
        tag_obj.AddTag(val);
    }

    virtual     public  void    Init(){}

    virtual    public  object   OnFunc(int evt_int = 0, string evt_str = "", Dictionary<string,object> args = null){return null;}
    virtual    public  bool     OnTryAdd(HashSet<SJ_TagBaseObj> hs){return true;}

    virtual     public  void    OnPreAdd(object arg = null){}
    virtual    public  void     OnAdd(){}
    virtual    public  void     OnRemove(){}

    virtual     public  void        OnAddGameObj(){}

    virtual     public  void    ClearAll(){}

    virtual     public  string  GetName()
    {
        //Debug.Log( "클래스 이름 가져오기 : " + GetType().Name );
        return GetType().Name;
    }


    // 인자 
    static public  object      GetArg(Dictionary<string,object> args , string name )       {return  SJ_TagBaseObj.GetArg(args,name);}
    static public  int         GetArgInt(Dictionary<string,object> args , string str )     {return (int)GetArg(args,str);}
    static public  float       GetArgFloat(Dictionary<string,object> args , string str )   {return (float)GetArg(args,str);}
    static public  string      GetArgStr(Dictionary<string,object> args, string str )     {return (string)GetArg(args,str);}

    static  public  Dictionary<string,object> SetArg(Dictionary<string,object> args , string str , object obj ){return SJ_TagBaseObj.SetArg(args,str,obj);}
    static public  void        AddArgInt(Dictionary<string,object> args , string str , int val )
    {
        int src = 0;
        object obj = GetArg(args,str);
        if( obj != null )src = GetArgInt(args,str);
        src += val;
        SetArg(args,str,src);
    }
    static public  void        AddArgFloat(Dictionary<string,object> args, string str , float val )
    {
        float src = 0;
        object obj = GetArg(args,str);
        if( obj != null )src = GetArgFloat(args,str);
        src += val;
        SetArg(args,str,src);
    }



    public  void    Add( SJ_TagBaseMng mng , object arg = null )
    {
//        Debug.Log( " 추가하기 태그 오브젝투!!!!!" );


        // go_self_ref = mng.go_self;
        // par_mng = mng;
        // mng.Add( tag_obj , arg );

        // if( go_self_ref != null )
        // {
        //     SJ_Unity.SetEqTrans( transform , null , go_self_ref.transform );
        //     OnAddGameObj();
        // }
    }

    public  void    Remove()
    {
        if(par_mng != null)
           par_mng.Remove( tag_obj );
        par_mng = null;

        OnRemove_Mono();
    }

    virtual public  void    OnRemove_Mono(){}

    public  void    Wait_Remove( float wait )
    {
        StartCoroutine( CO_Wait_Remove(wait) );
    }

    IEnumerator   CO_Wait_Remove( float wait )
    {
        yield return new WaitForSeconds(wait);
        Remove();
    }

}
