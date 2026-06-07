using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SJ_DragDrop_Mono : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Canvas      canvas;

    Vector2     start_pos;
    Transform   start_par;


    public HashSet<string>     lt_name_target = new HashSet<string>();

    public HashSet<string>     lt_name_target_check;
    public SJ_CallFunc      func_Begin = new SJ_CallFunc();
    public SJ_CallFunc      func_Drag = new SJ_CallFunc();
    public SJ_CallFunc      func_End = new SJ_CallFunc();


    public bool COPY_DRAG = false;
    GameObject go_Copy;

    int cur_Parent_Idx;

    // 버그인듯?
    // 첫드래그에만 eventData.hovered 에 처음 위치가 포함된다.
    // 그 이후에는 정상 동작
    // 첫드래그 버그 방지
    //public HashSet<GameObject>    lt_start_hovered = new HashSet<GameObject>();


    static public bool DRAGGING_NOW = false;

    public void SetNameFunc_Params( MonoBehaviour mono , string begin_func, string drag_func , string end_func  , params string[] name_s )
    {
        List<string> lt = new List<string>( name_s );
        SetNameFunc( mono , begin_func, drag_func , end_func , lt.ToArray() );
    }



    public void SetNameFunc( MonoBehaviour mono , string begin_func, string drag_func , string end_func  , string[] name_s )
    {
        func_Begin.SetInst( mono , begin_func );
        func_Drag.SetInst( mono , drag_func );
        func_End.SetInst( mono , end_func );
        lt_name_target.Clear();
        foreach( string s in name_s )
        {
            lt_name_target.Add(s);
        }
    }

    public GameObject   Get_Name_hovered( List<GameObject> lt )
    {
        foreach( GameObject s in lt )
        {
            if( lt_name_target_check.Contains( s.name ) )return s;
        }
        return null;
    }

    private void Awake() 
    {

        
    }

    public void RollBack()
    {
        transform.parent = start_par;
        rectTransform.anchoredPosition = start_pos;

        transform.SetSiblingIndex( cur_Parent_Idx );

        if( go_Copy != null ) GameObject.DestroyImmediate(go_Copy);
    }

   // GameObject start_go_hovered;
    public void OnBeginDrag(PointerEventData eventData)
    {
        cur_Parent_Idx = transform.GetSiblingIndex();

        if( COPY_DRAG )
        {
            if( go_Copy != null ) GameObject.DestroyImmediate(go_Copy);
            go_Copy = GameObject.Instantiate( gameObject );
            SJ_Unity.SetEqTrans( go_Copy.transform , null , transform.parent );

            go_Copy.transform.SetSiblingIndex( cur_Parent_Idx );
        }
 
        DRAGGING_NOW = true;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();        
        canvas = GetComponentInParent<Canvas>();

        //canvasGroup.alpha = .6f;
        if( canvasGroup != null )
            canvasGroup.blocksRaycasts = false;
        else{
            Debug.Log( "주의!!!!! 캔버스 그룹 없음" );
        }

        
        lt_name_target_check = new HashSet<string>( lt_name_target );

        start_pos = rectTransform.anchoredPosition;
        start_par = transform.parent;

        // 제외할 것
        string exp_par = "";
        foreach( string s in lt_name_target_check )
        {
            if( SJ_UnityMisc_1.Find_Parent( start_par , s ) != null )
            {
                Debug.Log( "제외 할 상위 : " + s );
                exp_par = s;

            }
        }

        if( string.IsNullOrEmpty( exp_par ) == false )
        {
            lt_name_target_check.Remove( exp_par );            
        }

        transform.parent = canvas.transform;
        transform.SetAsLastSibling();
        func_Begin.Func( this );
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 이전 이동과 비교해서 얼마나 이동했는지를 보여줌
        // 캔버스의 스케일과 맞춰야 하기 때문에
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

// Debug.Log( "종료  --------------------------- "  );

//         foreach( GameObject go in eventData.hovered )
//         {
//             Debug.Log( "호버드  : " + go.name );
//         }

//         if( eventData.pointerDrag != null )
//         {
//             Debug.Log( "포인터 드래그  : " + eventData.pointerDrag.name );
//         }

        //canvasGroup.alpha = 1f;
        if( canvasGroup != null )
            canvasGroup.blocksRaycasts = true;

        GameObject go_hovered = Get_Name_hovered( eventData.hovered );

        if( go_hovered != null )
        {
            //Debug.Log( "최종 호버  : " + go_hovered.name );

        }else{
            List<RaycastResult> rs = new List<RaycastResult>();
            EventSystem.current.RaycastAll( eventData , rs );
            List<GameObject>    lt_h = new List<GameObject>();
            foreach( RaycastResult s in rs )
            {
                //Debug.Log( "레이케스트 : "  + s.gameObject.name );

                lt_h.Add( s.gameObject );
            }

            go_hovered = Get_Name_hovered( lt_h );

            //if( go_hovered != null )Debug.Log( "레이캐스트 호버  : " + go_hovered.name );
        }

        if( go_Copy != null ) GameObject.DestroyImmediate(go_Copy);

        func_End.Func( this , go_hovered );
        DRAGGING_NOW = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
// Debug.Log( "드롭  --------------------------- "  );

//         foreach( GameObject go in eventData.hovered )
//         {
//             Debug.Log( "호버드  : " + go.name );
//         }

//         if( eventData.pointerDrag != null )
//         {
//             Debug.Log( "포인터 드래그  : " + eventData.pointerDrag.name );
//         }
    }

    static public void ALLChild_DragFunc( GameObject go_par , MonoBehaviour mono , string begin_func, string drag_func , string end_func  , params string[] name_s )
    {
        SJ_DragDrop_Mono[] sj_drags = go_par.GetComponentsInChildren<SJ_DragDrop_Mono>();
        foreach( SJ_DragDrop_Mono s in sj_drags )
        {
            s.SetNameFunc( mono , begin_func , drag_func, end_func, name_s );
        }
    }

}
