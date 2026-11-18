using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SJ_UnityUIMng_Curve : MonoBehaviour
{
    static public SJ_UnityUIMng_Curve G;

    public SJ_Curve_TransObjToggle  curve_Black;
    public float                    curve_default_time = 0.3f;

    public GameObject               go_BlockInput;  //  ui 클릭 입력 막는 투명 판

    public SJ_DlgFuncSync funcSync = new();

    public List<GameObject> objs_StartShow;

    public bool startHideAll;

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
        foreach( var s in objs_StartShow )s.SetActive(true);

        if( startHideAll )
        {
            SJ_Unity.Active_Child( transform , 0 );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public GameObject  Open( string panel_name , SJ_COMMON.Func_VOID func_end = null )
    {
        // if( G == null )
        // {
        //     Debug.LogError( "에러 : SJ_UnityUIMng_Curve == NULL " );
        //     return null;            
        // }
        // if( string.IsNullOrEmpty( panel_name ) )
        // {
        //     Debug.LogError( "에러 : panel_name == null " );
        //     return null;
        // }
        // Transform tr = G.transform.Find( panel_name );
        // if( tr == null )
        // {
        //     Debug.LogError( "에러 : " + panel_name );
        //     return null;
        // }
        // _STOCK_INF s = new();
        // s.panel_name = panel_name;
        // s.func = func_end;
        // s.go_cur = tr.gameObject;

        _STOCK_INF st = MakeOpenStock( panel_name );

        G.funcSync.Add( G.OpenPrc , st );
        return st.go_cur;
    }

    static public _STOCK_INF MakeOpenStock(string panel_name)
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
        s.go_cur = tr.gameObject;
        return s;
    }

    static public async Awaitable<GameObject> Open_Async( string panel_name )
    {
        _STOCK_INF s = MakeOpenStock( panel_name );
        if( s == null ) return null;
        G.cur_stock = s;
        SJ_Curve_TransObjToggle transObjToggle_Panel = G.OpenPrc_CurStock();
        await transObjToggle_Panel.StartFunc_FWD_Awaitable();
        return s.go_cur;
    }


    _STOCK_INF cur_stock = null;
    public void OpenPrc( object arg )
    {
        cur_stock = arg as _STOCK_INF;
        // Transform tr = G.transform.Find( cur_stock.panel_name );
        // if( tr == null )
        // {
        //     Debug.LogError( "에러 : " + cur_stock.panel_name );
        //     return;
        // }
        // // 이미 열린건가?
        // if( tr.gameObject.activeSelf )
        // {
        //     return;
        // }
        // tr.gameObject.SetActive(true);
        // SJ_Curve_TransObjToggle transObjToggle_Panel = tr.GetComponentInChildren<SJ_Curve_TransObjToggle>();
        // if( transObjToggle_Panel == null )
        // {
        //     Debug.LogError( "에러 : transObjToggle_Panel == null : " + cur_stock.panel_name );
        //     return;
        // }
        // list_popup.Add( transObjToggle_Panel.gameObject );
        // AlignBackBlack();
        // transObjToggle_Panel.sJ_Curve.time = G.curve_default_time;
        // tr.SetAsLastSibling();
        // Debug.Log( "StartFunc_FWD : " +  cur_stock.panel_name);
        // SJ_Unity.SendMsg( transObjToggle_Panel.gameObject , "OpenPopup_StartAni" );

        SJ_Curve_TransObjToggle transObjToggle_Panel = OpenPrc_CurStock();
        transObjToggle_Panel?.StartFunc_FWD( G.EndAni_PanelOpen );
    }

    static public GameObject FindPanel( string panel_name )
    {
        Transform tr = G.transform.Find( panel_name );
        if( tr == null )
        {
            Debug.LogError( "에러 : " + panel_name );
            return null;
        }
        return tr.gameObject;
    }

    static public void ActivePanel( bool b , string panel_name )
    {
        GameObject go = FindPanel( panel_name );
        if( go == null ) return;
        go.SetActive( b );
    }

    public SJ_Curve_TransObjToggle OpenPrc_CurStock()
    {
        Transform tr = G.transform.Find( cur_stock.panel_name );
        if( tr == null )
        {
            Debug.LogError( "에러 : " + cur_stock.panel_name );
            return null;
        }
        // 이미 열린건가?
        if( tr.gameObject.activeSelf )
        {
            return null;
        }
        tr.gameObject.SetActive(true);
        SJ_Curve_TransObjToggle transObjToggle_Panel = tr.GetComponentInChildren<SJ_Curve_TransObjToggle>();
        if( transObjToggle_Panel == null )
        {
            Debug.LogError( "에러 : transObjToggle_Panel == null : " + cur_stock.panel_name );
            return null;
        }
        list_popup.Add( transObjToggle_Panel.gameObject );
        AlignBackBlack();
        transObjToggle_Panel.sJ_Curve.time = G.curve_default_time;
        tr.SetAsLastSibling();
        Debug.Log( "StartFunc_FWD : " +  cur_stock.panel_name);
        SJ_Unity.SendMsg( transObjToggle_Panel.gameObject , "OpenPopup_StartAni" );
        return transObjToggle_Panel;
    }


    public void AlignBackBlack()
    {
        if( curve_Black == null ) return;

        if( list_popup.Count == 1 )
        {
            // G.curve_Black.sJ_Curve.time = G.curve_default_time;
            // G.curve_Black.StartFunc_FWD();
            FadeInOut( true );
        }
        else if( list_popup.Count == 0 )
        {
            //G.curve_Black.StartFunc_BACK();
            FadeInOut(false);
        }
        G.curve_Black.transform.SetAsLastSibling(); 
    }

    static public void FadeInOut( bool b , SJ_COMMON.Func_VOID end_func = null )
    {
        G.curve_Black.func_OnEnd = end_func;
        if( b )
        {
            G.curve_Black.sJ_Curve.time = G.curve_default_time;
            G.curve_Black.StartFunc_FWD();
        }
        else
        {
            G.curve_Black.StartFunc_BACK();
        }
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

    static public async Awaitable CloseOneAwait()
    {
        if( G.list_popup.Count < 1 )return;
        GameObject go = G.list_popup[G.list_popup.Count-1];
        SJ_Curve_TransObjToggle transObjToggle_Panel = go.GetComponentInChildren<SJ_Curve_TransObjToggle>();
        await transObjToggle_Panel.StartFunc_BACK_Awaitable();
        G.EndAni_PanelClose();
        return;
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
