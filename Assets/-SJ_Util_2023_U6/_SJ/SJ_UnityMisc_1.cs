using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UnityMisc_1
{
    static  public  void    ChildActiveOne_Random( Transform tr_par )
    {
        int active_idx = UnityEngine.Random.Range(0,tr_par.childCount);
        ChildActiveOne( tr_par , active_idx );
    } 

    static  public  GameObject    ChildActiveOne( Transform tr_par , int active_idx )
    {
        GameObject go = null;
        for( int i = 0 ; i < tr_par.childCount ; i++ )
        {
            if( active_idx == i )
            {
                tr_par.GetChild(i).gameObject.SetActive(true);
                go = tr_par.GetChild(i).gameObject;
            }else{
                tr_par.GetChild(i).gameObject.SetActive(false);
            }
        }
        return go;
    }

    static  public  GameObject    ChildActiveOne_Shift( Transform tr_par , int val )
    {
        // 처음으로 활성화된 자식의 인덱스를 현재 위치로 정한다.
        int cur = 0;
        for( int i = 0 ; i < tr_par.childCount ; i++ )
        {
            if( tr_par.GetChild(i).gameObject.activeSelf )
            {
                cur = i;
                break;
            }
        }

        cur += val;
        if( cur < 0 ) cur = tr_par.childCount - 1;
        if( cur >= tr_par.childCount ) cur = 0;

        return ChildActiveOne( tr_par , cur );
    }

    static  public  GameObject    ChildActiveOne( Transform tr_par , string str )
    {
        GameObject go = null;
        for( int i = 0 ; i < tr_par.childCount ; i++ )
        {
            if( tr_par.GetChild(i).gameObject.name == str )
            {
                tr_par.GetChild(i).gameObject.SetActive(true);
                go = tr_par.GetChild(i).gameObject;
            }else{
                tr_par.GetChild(i).gameObject.SetActive(false);
            }
        }
        return go;
    }

    static public GameObject Find_Parent( Transform tr , string name )
    {
        if( tr.name == name ) return tr.gameObject;
        if( tr.parent == null ) return null;
        return Find_Parent( tr.parent , name );
    }

    static public string    Int_TO_CommaSTR( int val )
    {
        return string.Format("{0:#,###}", val);
    }

    static public string    Random_STR( params string[] strs )
    {
        int idx = UnityEngine.Random.Range( 0, strs.Length );
        return strs[idx];
    }

    static public List<string> GetNameList( List<GameObject> lt )
    {
        List<string> lt_str = new List<string>();
        foreach( GameObject go in lt )
        {
            lt_str.Add( go.name );
        }
        return lt_str;
    }

    static public void  GameObj_SetActive( GameObject go , bool b )
    {
        if( go != null )go.SetActive(b);
    }

    // 두개의 문자 사이에 있는 문자열을 반환한다.
    static public string GetString_Between( string str , string start , string end )
    {
        int idx_start = str.IndexOf( start );
        if( idx_start < 0 ) return "";
        int idx_end = str.IndexOf( end , idx_start + start.Length );
        if( idx_end < 0 ) return "";
        return str.Substring( idx_start + start.Length , idx_end - (idx_start + start.Length) );
    }
}

// 유니티 용 콜 함수 인스펙터
[System.Serializable]
public class SJ_CallFunc_Mono
{
    public MonoBehaviour mono;
    public string       static_name;
    public string       func;

    public bool         use_arg_int;
    public int          arg_int;

    public bool         use_arg_str;
    public string       arg_str;

    SJ_CallFunc sJ_CallFunc = new SJ_CallFunc();

    public void Func( object arg = null )
    {
        if( arg == null )
        {
            if( use_arg_int ) arg = arg_int;
            if( use_arg_str ) arg = arg_str;
        }

        if( string.IsNullOrEmpty(static_name) == false )
        {
            System.Type componentType = System.Type.GetType(static_name );
            if( componentType == null )
            {
                Debug.LogError( "에러!!! 못찾음 : " + static_name );
                return;
            }

            sJ_CallFunc.SetStatic( componentType , func , arg );
            sJ_CallFunc.Func();
            return;
        }

        sJ_CallFunc.SetInst( mono , func  , arg );
        sJ_CallFunc.Func();
    }
}
