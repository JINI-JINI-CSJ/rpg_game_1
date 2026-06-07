using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class SJ_WWWForm : MonoBehaviour
{
    static public   SJ_WWWForm  g;

    public  string  url_main;
    public  string  url_sub;
    public  Dictionary<string,object>   dic_arg = new Dictionary<string, object>();
    public  _SJ_GO_FUNC func_recv;
    public  _SJ_GO_FUNC func_recv_error;

    public  bool    log;

    public  bool    cur_sending;

    public  bool    offline_mode;

    private void Awake() {
        g = this;
    }

    public  void    Clear_Arg()
    {
        dic_arg.Clear();
    }

    public  void    AddField( string name , object  val )
    {
        if( val == null )
        {
            UnityEngine.Debug.LogError( "Error!!! AddField : " + name );
        }

        dic_arg[name] = val;
    }

    void    _AddField_Arg( WWWForm form )
    {
        foreach( KeyValuePair<string , object> kv in dic_arg )
        {
            object val = kv.Value;
            Type t = val.GetType();
            if (t.Equals(typeof(int)))
            {
                form.AddField(kv.Key , (int)val );
            }
            else if (t.Equals(typeof(string)))
            {
                form.AddField(kv.Key , (string)val );
            }else{
                UnityEngine.Debug.LogError( "form.AddField : 지원 안하는 타입!!! "  );
            }
        }
    }

    public  void    SetErrorFunc( GameObject go , string func )
    {
        
        func_recv_error.Set(go , func );
    }

    public  bool    SendHttp( string _sub_url , GameObject go_recv = null , string func_name = "" )
    {
        url_sub = _sub_url;
        func_recv.Set( go_recv , func_name );     
        if( offline_mode )
        {
            func_recv.Func("" ); 
            return true;
        }
        if( cur_sending ) return false;

        if( log )UnityEngine.Debug.Log( "SendHttp : " + _sub_url );
    
        return Start_Send();
    }

    public  bool    Start_Send()
    {
        if( cur_sending ) return false;
        StartCoroutine( CO_ReqHttpPost() );
        return true;
    }

    IEnumerator CO_ReqHttpPost() 
    {
        WWWForm form = new WWWForm();
        _AddField_Arg( form );

        string url = url_main + url_sub;

        if( log )UnityEngine.Debug.Log(url);

        UnityWebRequest www = UnityWebRequest.Post(url_main + url_sub, form);
        //www.SetRequestHeader("Content-Type", "application/text");
        yield return www.SendWebRequest();
        if(www.error != null) {
            UnityEngine.Debug.Log(www.error);
            func_recv_error.Func( www );
            UnityEngine.Debug.LogError("CO_ReqHttpPost www.error !!= null");            
        }
        else {
            if( log )UnityEngine.Debug.Log(www.downloadHandler.text);
            //Debug.Log("Form upload complete!");

            func_recv.Func( www.downloadHandler.text ); // UnityWebRequest www
        }
        cur_sending = false;
    }

    public  void    Stop_Cancel()
    {
        StopAllCoroutines();
    }

}
