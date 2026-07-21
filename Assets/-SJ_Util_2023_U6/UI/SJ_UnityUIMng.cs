using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SJ_UnityUIMng : MonoBehaviour
{
    static  public  SJ_UnityUIMng g;
    public GameObject          go_Black;
    // public SJ_Curve_TransObjToggle  curve_Black;
    // public float                curve_default_time = 0.3f;
    public GameObject           go_Top;    
    public GameObject           go_no_Close;
    public GameObject           go_wait;
    public GameObject           go_BlockInput;    
    public bool                 noClose_ByBackPlane = false;
    public _SJ_GO_FUNC          func_close_popup = new _SJ_GO_FUNC();
    public _SJ_GO_FUNC          func_Top_Click = new _SJ_GO_FUNC();
    public SJ_CallFunc          func_all_ani_end = new SJ_CallFunc();
    public bool                 close_ESC_BACK_KEY = false;
    public GameObject           prf_Popup_Ani;
    public List<GameObject>     lt_no_Ani_Popup;





    public bool useSJ_PlayerInputMng = true;

    public class _POPUP_STOCK_INFO
    {
        public GameObject go;
        public bool no_back;
    }
    public  List<_POPUP_STOCK_INFO>    lt_Popup = new List<_POPUP_STOCK_INFO>();

    public class _QUEUE_INF
    {
        public int open_close;// 1 : open , -1 : close
        public string str;
        public bool no_back;
        public object arg = null;
        public Transform find_par; 
        public bool check_in_screen; 
        public bool no_open_func;
        public GameObject go_popup;
    }

    Queue<_QUEUE_INF> queue = new Queue<_QUEUE_INF>();

    _QUEUE_INF que_cur_ani = null;
    
    CanvasScaler canvasScaler;

    Canvas canvas;

    RectTransform rt_canvas;

    private void Awake() {
        g = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 뒤로가기 버튼
        // 열린창 닫기
        // 자식에
        // NO_CLOSE_ESC
        // 있으면 안한다.
        if( close_ESC_BACK_KEY && Input.GetKeyDown( KeyCode.Escape ) )
        {
            if( g.lt_Popup.Count > 0 )
            {
                _POPUP_STOCK_INFO inf_stock = g.lt_Popup[0];
                Transform tr_NO_CLOSE_ESC = inf_stock.go.transform.Find( "NO_CLOSE_ESC" );
                if( tr_NO_CLOSE_ESC == null )
                {
                    ClosePopup();
                }
            }
        }
    }

    static public Transform Find(string str )
    {
        return g.transform.Find(str);  
    }

    static public void End_CurAniQueue()
    {
        g.que_cur_ani = null;
    }    

    static public GameObject Check_NextQueue()
    {
        if( g.que_cur_ani != null ) return null;
        if( g.queue.Count == 0 ) 
        {
            g.func_all_ani_end.Func();
            g.func_all_ani_end.Init();
            return null;
        }
        

        _QUEUE_INF que = g.queue.Dequeue();
        g.que_cur_ani = que;

        if( que.open_close == 1 )
        {
            g.PrcOpen( que );
        }else if( que.open_close == -1 )
        {
            return g.PrcClosePopup();
        }
        return null;
    }


    static public Transform FindPage(string str)
    {
        return g.transform.Find(str);
    }

    static public   GameObject  OpenPopup( string str , bool no_back = false , object arg = null , Transform find_par = null , bool check_in_screen = false , bool no_open_func = false )
    {
        Transform tr = null;
        if ( find_par != null )
        {
            tr = find_par.Find(str);  
        }else{
            tr = FindPage(str);            
        }

        if( tr == null )
        {
            Debug.LogError( "에러!!! 못찾음 OpenPopup : " + str );
            return null;
        } 
        GameObject go = tr.gameObject;

        if( g.que_cur_ani != null && g.que_cur_ani.go_popup == go )
        {
            Debug.Log( "주의 OpenPopup : 이미 열려 있음!! " + str );
            return null;
        }

        foreach( _QUEUE_INF s in g.queue )
        {
            if( s.go_popup == go && s.open_close == 1 )
            {
                Debug.Log( "주의 OpenPopup : 큐에 이미 있음!! " + str );
                return null;
            }
        }

        SJ_UI_AniDesc sJ_UI_AniDesc = go.GetComponentInChildren<SJ_UI_AniDesc>( true );

        _QUEUE_INF que = new _QUEUE_INF();
        que.open_close = 1;
        que.str = str;
        que.no_back = no_back;
        que.arg = arg;
        que.find_par = find_par;
        que.check_in_screen = check_in_screen;
        que.no_open_func = no_open_func;
        que.go_popup = go;
        if( sJ_UI_AniDesc != null )
        {
            g.queue.Enqueue( que );       
            Check_NextQueue();            
        }else{
            g.PrcOpen( que );
        }

        return go;
    }

    public void PrcOpen( _QUEUE_INF que )
    {
        GameObject go = que.go_popup;
        bool no_back = que.no_back;
        object arg = que.arg;
        bool check_in_screen = que.check_in_screen;
        bool no_open_func = que.no_open_func;

        bool is_opened = false;
        foreach( _POPUP_STOCK_INFO s in g.lt_Popup )
        {
            if( s.go == go )
            {
                //Debug.Log( "주의 OpenPopup : 이미 열려 있음!! " + str );
                //return null;
                is_opened = true;
                break;
            } 
        }

        if( g.go_Black != null && no_back == false )
        {
            g.go_Black.SetActive(true);
            g.go_Black.transform.SetAsLastSibling();
        }else{
            if( g.go_Black != null ) g.go_Black.SetActive(false);
        }
        go.transform.SetAsLastSibling();

        // 스크린 안으로 사각형 들이기
        // 객체는 반드시 중앙 앵커
        if( check_in_screen )
        {
            FitInPopupScreen( go );

        }

        if( is_opened ) return;

        SJ_UI_AniDesc sJ_UI_AniDesc = null;
        if( g.lt_no_Ani_Popup.Contains( go ) == false )
        {
            sJ_UI_AniDesc = go.GetComponentInChildren<SJ_UI_AniDesc>( true );
            if( sJ_UI_AniDesc == null && g.prf_Popup_Ani != null )
            {
                GameObject go_ani = GameObject.Instantiate( g.prf_Popup_Ani );
                sJ_UI_AniDesc =  go_ani.GetComponent<SJ_UI_AniDesc>();
                sJ_UI_AniDesc.LinkCreate( go );
            }
        }

        go.SetActive(true);        

        if( sJ_UI_AniDesc != null )
        {
            sJ_UI_AniDesc.OpenAni();
        }

        _POPUP_STOCK_INFO inf_stock = new _POPUP_STOCK_INFO();
        inf_stock.go = go;
        inf_stock.no_back = no_back;
        g.lt_Popup.Add(inf_stock);
        
        if (g.useSJ_PlayerInputMng)
        {
            SJ_PlayerInputMng.ActiveInput(go);
        }

        if (no_open_func == false)
            SJ_Unity.SendMsg(go, "OpenPopup", arg);

        Debug.Log("OpenPopup : " + go.name );
    }

    // 2D 객체는 (+) 중심점 피벗이야야 한다.
    // 상위 객체는 전체 화면 크기로 되어 있어야 한다.
    static public void FitInPopupScreen( GameObject go )
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if( rt == null ) return;

        // 스크린 
        if( g.canvasScaler == null )
        {
            g.canvasScaler = g.transform.GetComponent<CanvasScaler>();
        }

        if( g.canvasScaler != null )
        {
            Vector2 vs = g.canvasScaler.referenceResolution;
            Rect rc_scr = new Rect(  Vector2.zero , vs );// 

            Vector2 vp = rt.anchoredPosition;
            Vector2 vs_half = new Vector2( vs.x * 0.5f , vs.y * 0.5f );

            Rect rc_obj = new Rect();
            rc_obj.x = vp.x - (rt.rect.width * 0.5f) + vs_half.x;
            rc_obj.y = vp.y - (rt.rect.height * 0.5f) + vs_half.y;
            rc_obj.width = rt.rect.width;
            rc_obj.height = rt.rect.height;
            Vector2Int v = SJ_Cood.Fit_Screen_RECT( rc_scr , rc_obj );
            
            rt.anchoredPosition += v;
        }
    }

    

    static public   void    SetFunc_ClosePopup( MonoBehaviour mono , string func )
    {
        Debug.Log( "닫기 함수 세팅 SetFunc_ClosePopup : " + mono + " , " + func );
        g.func_close_popup.SetMono(mono , func);
    }

    static public   void    Set_NoClose( GameObject go = null )
    {
        g.go_no_Close = go;
    }

    static  public  GameObject    ClosePopup()
    {
//        Debug.Log("Start ClosePopup");

        GameObject go = null;
        if(  g.lt_Popup.Count  > 0 )
        {
            _POPUP_STOCK_INFO inf_stock = g.lt_Popup[ g.lt_Popup.Count - 1 ];
            SJ_UI_AniDesc sJ_UI_AniDesc = inf_stock.go.GetComponentInChildren<SJ_UI_AniDesc>( true );
            if( sJ_UI_AniDesc != null )
            {         
                _QUEUE_INF que = new _QUEUE_INF();
                que.open_close = -1;
                g.queue.Enqueue( que );  

                Check_NextQueue();
            }else{
                go = g.PrcClosePopup( );
            }
        }

        return go;

        // if( ANI_playing > 0 )
        // {
        //     Debug.Log( "ClosePopup 애니메이션 진행중" );
        //     return;
        // }

        // Debug.Log( "SJ_UnityUIMng ==> ClosePopup" );

        // if( g.lt_Popup.Count < 1 )
        // {
        //     if( g.go_Black != null )g.go_Black.SetActive(false);

        //     g.func_close_popup.Func();
        //     g.func_close_popup.Init();
        //     return;
        // }

        // _POPUP_STOCK_INFO inf_stock = g.lt_Popup[ g.lt_Popup.Count - 1 ];

        // if( g.go_no_Close == inf_stock.go )
        // {
        //     Debug.Log( "닫기 금지 : " + g.go_no_Close.name );
        //     return;            
        // }

        // SJ_UI_AniDesc sJ_UI_AniDesc = inf_stock.go.GetComponentInChildren<SJ_UI_AniDesc>();
        // if( sJ_UI_AniDesc != null )
        // {
        //     sJ_UI_AniDesc.CloseAni();
        // }else{
        //     g.Close_Prc( inf_stock.go);
        // }
    }

    public GameObject PrcClosePopup()
    {
        //Debug.Log("SJ_UnityUIMng ==> ClosePopup");

        if (g.lt_Popup.Count < 1)
        {
            if (g.go_Black != null) g.go_Black.SetActive(false);

            g.func_close_popup.Func();
            g.func_close_popup.Init();
            return null;
        }

        _POPUP_STOCK_INFO inf_stock = g.lt_Popup[g.lt_Popup.Count - 1];

        if (g.go_no_Close == inf_stock.go)
        {
            Debug.Log("닫기 금지 : " + g.go_no_Close.name);
            return null;
        }

        SJ_UI_AniDesc sJ_UI_AniDesc = inf_stock.go.GetComponentInChildren<SJ_UI_AniDesc>();
        if (sJ_UI_AniDesc != null)
        {
            sJ_UI_AniDesc.CloseAni();
        }
        else
        {
            g.Close_Prc(inf_stock.go);
        }

        return inf_stock.go;
    }

    public void Close_Prc(GameObject go)
    {
        g.lt_Popup.RemoveAt(g.lt_Popup.Count - 1);
        if (g.lt_Popup.Count > 0)
        {
            _POPUP_STOCK_INFO inf_stock_last = g.lt_Popup[g.lt_Popup.Count - 1];
            if (g.go_Black != null && inf_stock_last.no_back == false)
            {
                g.go_Black.SetActive(true);
                g.go_Black.transform.SetAsLastSibling();
            }
            else
            {
                SJ_Unity.SetActive(g.go_Black, false);
            }
            inf_stock_last.go.transform.SetAsLastSibling();
        }
        else
        {
            if (go_Black != null)
                g.go_Black.SetActive(false);
        }
        func_close_popup.Func();
        func_close_popup.Init();

        SJ_Unity.SendMsg(go, "ClosePopup");

        go.SetActive(false);                 
        if (g.useSJ_PlayerInputMng)
        {
            SJ_PlayerInputMng.RemoveInput(go);
        }
    }

    IEnumerator Wait_Close( GameObject go , float time )
    {
        yield return new WaitForSeconds( time );
        go.SetActive(false);
    }


    static  public  void    ClosePopup_All()
    {
        // while(true)
        // {
        //     if( g.lt_Popup.Count < 1 ) return;
        //     ClosePopup();
        // }

        for( int i= 0 ; i < g.lt_Popup.Count ; i++ )
        {
            ClosePopup();
        }
    }

    public  void    ClosePopup_Common()
    {
        ClosePopup();
    }

    // 인자로 넘긴 것 이전까지 닫는다.
    static   public void     ClosePopup_Limit( GameObject popup )
    {
        while( true ) 
        {
            if( g.lt_Popup.Count == 0 ) return;
            _POPUP_STOCK_INFO inf_stock = g.lt_Popup[ g.lt_Popup.Count - 1 ];
            if( inf_stock.go == popup ) return;
            ClosePopup();
        }
    }

    static public void  Set_Top( bool b )
    {
        if( g.go_Top != null )
        {
            g.go_Top.transform.SetAsLastSibling();
            g.go_Top.SetActive(b);
        }
    }

    static public void  Set_Top_ClickFunc( MonoBehaviour mono = null , string func = "" )
    {
        g.func_Top_Click.SetMono( mono , func );
    }

    static public void  CallFunc_Opened( string func )
    {
        foreach( _POPUP_STOCK_INFO s in g.lt_Popup )
        {
            SJ_CSharpUtil.CallStrFunc_NoArg( s.go , func );
        }
    }

    static public void  SendMsg_Opened( string func )
    {
        foreach( _POPUP_STOCK_INFO s in g.lt_Popup )
        {
            SJ_Unity.SendMsg( s.go , func );
        }
    }

    static public void  LinkChild_Top( GameObject go )
    {
        go.transform.parent = g.transform;
        go.transform.SetAsLastSibling();
    }

    static public void Show_Wait( bool b )
    {
        if( g.go_wait == null ) return;
        if(b)
        {
            g.go_wait.SetActive(true);
            g.go_wait.transform.SetAsLastSibling();
        }else{
            g.go_wait.SetActive(false);
        }
    }

    // 월드 to ui 좌표
    // 일반적인 ui 세팅때만 정확
    // 기타 옵션은 나중에...
    // static public Vector3 TransWorldToUI( Vector3 pos )
    // {
    //     if( g == null ) return Vector3.zero;
    //     if( g.canvasScaler == null )g.canvasScaler = g.transform.GetComponent<CanvasScaler>();
    //     if( g.canvasScaler == null ) return Vector3.zero;
    //     Vector3 pos_vp = Camera.main.WorldToViewportPoint( pos );
    //     pos_vp.x -= 0.5f;
    //     pos_vp.y -= 0.5f;
    //     pos_vp.z = 0;
    //     pos_vp.x = pos_vp.x * g.canvasScaler.referenceResolution.x;
    //     pos_vp.y = pos_vp.y * g.canvasScaler.referenceResolution.y;
    //     return pos_vp;
    // }
    // 간단하게 아래로 사용
    //transform.position = Camera.main.WorldToScreenPoint(target.position);



    public void OpenPopup_Event( string str )
    {
        SJ_UnityUIMng.OpenPopup( str );
    }

    public void ClosePopup_ByBackPlane()
    {
        if( noClose_ByBackPlane == false )
        {
            ClosePopup();
        }
    }


    static public void SetBlockInput( bool b )
    {
        if( g.go_BlockInput != null )
        {
            SJ_Unity.SetActive( g.go_BlockInput , b );     
            if( b )
                g.go_BlockInput.transform.SetAsLastSibling();
        }

    }

    static public bool GetCanvas()
    {
        if( g.canvas == null )
        {
            g.canvas = g.GetComponent<Canvas>();
            if( g.canvas == null ) return false;
        }
        g.rt_canvas = g.canvas.GetComponent<RectTransform>();
        return true;
    }

    static public Vector3 GetViewportPosUI( RectTransform rect )
    {
        // if( g.canvas == null )
        // {
        //     g.canvas = g.GetComponent<Canvas>();
        //     if( g.canvas == null ) return Vector3.zero;
        // }
        if( GetCanvas() == false ) return Vector3.zero;

        Vector3 viewportPos = Vector3.zero;
        switch (g.canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                // 1. UI → 스크린 좌표 변환
                Vector3 screenPos_Overlay = RectTransformUtility.WorldToScreenPoint(null, rect.position);
                // 2. 스크린 좌표 → 뷰포트 좌표 변환
                viewportPos = new Vector3(
                    screenPos_Overlay.x / Screen.width,
                    screenPos_Overlay.y / Screen.height,
                    0f
                );
                break;

            case RenderMode.ScreenSpaceCamera:
            case RenderMode.WorldSpace:
                // World 좌표를 직접 Viewport 좌표로 변환
                viewportPos = Camera.main.WorldToViewportPoint(rect.position);
                break;
        }
        return viewportPos;
    }

    
    // static public SJ_Curve_TransObjToggle curvePanel_cur;
    // static public GameObject Open_CurvePanel( string panel_name , SJ_COMMON.Func_VOID func_openEnd = null )
    // {
    //     if( curvePanel_cur != null ) return null;

    //     if( g.curve_Black == null ) return null;

    //     Transform tr = FindPage(panel_name);
    //     if( tr == null ) return null;

    //     SJ_Curve_TransObjToggle transObjToggle_Panel = tr.GetComponent<SJ_Curve_TransObjToggle>();

    //     g.curve_Black.sJ_Curve.time = g.curve_default_time;
    //     transObjToggle_Panel.sJ_Curve.time = g.curve_default_time;

    //     g.curve_Black.transform.SetAsLastSibling();
    //     transObjToggle_Panel.transform.SetAsLastSibling();

    //     if( g.curve_Black.cur_toggle == false )
    //         g.curve_Black.StartFunc_FWD();

    //     transObjToggle_Panel.StartFunc_FWD( func_openEnd );

    //     _POPUP_STOCK_INFO inf_stock = new _POPUP_STOCK_INFO();
    //     inf_stock.go = transObjToggle_Panel.gameObject;
    //     g.lt_Popup.Add(inf_stock);

    //     SJ_Unity.SendMsg( transObjToggle_Panel.gameObject , "OpenPopup_StartAni" );

    //     return transObjToggle_Panel.gameObject;
    // }

    // static public void Close_CurvePanel(  SJ_COMMON.Func_VOID func_openEnd = null )
    // {
    //     if( g.lt_Popup.Count < 1 ) return;
    //     _POPUP_STOCK_INFO inf_stock = g.lt_Popup[g.lt_Popup.Count-1];
    //     SJ_Curve_TransObjToggle transObjToggle_Panel  = inf_stock.go.GetComponent<SJ_Curve_TransObjToggle>();
    //     if( transObjToggle_Panel == null ) return;

    //     g.lt_Popup.RemoveAt(g.lt_Popup.Count - 1);
    //     if( g.lt_Popup.Count < 1 )
    //     {
    //         g.curve_Black.StartFunc_BACK();
    //     }
    //     else
    //     {
    //         inf_stock = g.lt_Popup[g.lt_Popup.Count-1];
    //         inf_stock.go.transform.SetAsLastSibling();
    //     }

    //     transObjToggle_Panel.StartFunc_BACK( func_openEnd );
    //     return;
    // }

}
