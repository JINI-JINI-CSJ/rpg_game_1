using System;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UnityUIMng_Curve : MonoBehaviour
{
    static public SJ_UnityUIMng_Curve G;

    public SJ_Curve_TransObjToggle  curve_Black;
    public float                    curve_default_time = 0.3f;

    public GameObject               go_BlockInput;

    public SJ_DlgFuncSync funcSync = new();

    public class _STOCK_INF
    {
        public string panel_name;
        public SJ_COMMON.Func_VOID func;

        public GameObject go_cur;
    }

    public List<GameObject> list_popup = new();

    void Awake()
    {
        G = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public GameObject  Open( string panel_name , SJ_COMMON.Func_VOID func_end = null )
    {
        if( G == null )
        {
            Debug.LogError( "에러 : SJ_UnityUIMng_Curve == NULL " );
            return null;            
        }

        if( string.IsNullOrEmpty( panel_name ) )
        {
            Debug.LogError( "에러 : panel_name == null " );
            return null;
        }

        Transform tr = G.transform.Find( panel_name );
        if( tr == null )
        {
            Debug.LogError( "에러 : " + panel_name );
            return null;
        }

        _STOCK_INF s = new();
        s.panel_name = panel_name;
        s.func = func_end;
        s.go_cur = tr.gameObject;
        G.funcSync.Add( G.OpenPrc , s );
        return tr.gameObject;
    }



    _STOCK_INF cur_stock = null;
    public void OpenPrc( object arg )
    {
        cur_stock = arg as _STOCK_INF;

        Transform tr = G.transform.Find( cur_stock.panel_name );
        if( tr == null )
        {
            Debug.LogError( "에러 : " + cur_stock.panel_name );
            return;
        }

        tr.gameObject.SetActive(true);

        SJ_Curve_TransObjToggle transObjToggle_Panel = tr.GetComponentInChildren<SJ_Curve_TransObjToggle>();

        if( transObjToggle_Panel == null )
        {
            Debug.LogError( "에러 : transObjToggle_Panel == null : " + cur_stock.panel_name );
            return;
        }

        list_popup.Add( transObjToggle_Panel.gameObject );

        AlignBackBlack();

        transObjToggle_Panel.sJ_Curve.time = G.curve_default_time;
        tr.SetAsLastSibling();

        transObjToggle_Panel.StartFunc_FWD( G.EndAni_PanelOpen );

        SJ_Unity.SendMsg( transObjToggle_Panel.gameObject , "OpenPopup_StartAni" );
    }

    public void AlignBackBlack()
    {
        if( curve_Black == null ) return;

        if( list_popup.Count == 1 )
        {
            G.curve_Black.sJ_Curve.time = G.curve_default_time;
             
            G.curve_Black.StartFunc_FWD();
        }
        else if( list_popup.Count == 0 )
        {
            G.curve_Black.StartFunc_BACK();
        }
        G.curve_Black.transform.SetAsLastSibling(); 
        
    }

    public void EndAni_PanelOpen()
    {
        cur_stock.func?.Invoke();
        SJ_Unity.SendMsg( cur_stock.go_cur , "OpenPopup_StartAni_End" );
        G.funcSync._Next();
    }

    static public void CloseOne( SJ_COMMON.Func_VOID func_end = null  )
    {
        if( G.list_popup.Count < 1 )
        {
            Debug.Log( "위험!! 열린 창 없다!!!!!!!! inst.list_popup.Count < 1" );
            return;            
        }

        _STOCK_INF s = new();
        s.func = func_end;
        G.funcSync.Add( G.ClosePrc , s );
    }
    public void ClosePrc( object arg )
    {
        _STOCK_INF s = arg as _STOCK_INF;
        if( list_popup.Count < 1 )
        {
            G.funcSync._Next();
            return;
        }

        cur_stock = s;

        GameObject go = list_popup[list_popup.Count-1];
        SJ_Curve_TransObjToggle transObjToggle_Panel = go.GetComponentInChildren<SJ_Curve_TransObjToggle>();

        transObjToggle_Panel.StartFunc_BACK( G.EndAni_PanelClose );
    }

    public void EndAni_PanelClose()
    {
        if( list_popup.Count > 0 )
        {
            GameObject go = list_popup[list_popup.Count-1];
            list_popup.RemoveAt( list_popup.Count-1 );        
            SJ_Unity.SendMsg( go , "ClosePopup_EndAni" );
        }

        AlignBackBlack();
        if( list_popup.Count > 0 )
        {
            GameObject go_last = list_popup[list_popup.Count-1];
            go_last.transform.SetAsLastSibling();
        }

        cur_stock.func?.Invoke();
        G.funcSync._Next();
    }

    static public void ALL_Active( bool b )
    {
        // 자식들만 (판넬들) 적용
        SJ_Unity.Child_Active( G.gameObject , b );
    }

    static public void SetBlockInput( bool b )
    {
        if( G.go_BlockInput == null )
        {
            Debug.LogError( "G.go_BlockInput == null" );
            return;
        }
        G.go_BlockInput.SetActive(b);
        if( b )
        {
            G.go_BlockInput.transform.SetAsLastSibling();
        }
    }

}
