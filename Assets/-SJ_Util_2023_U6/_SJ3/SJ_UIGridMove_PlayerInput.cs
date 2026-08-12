using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(SJ_UIGridMove))]
[RequireComponent(typeof(CursorDirectionInput))]
public class SJ_UIGridMove_PlayerInput : MonoBehaviour
{
    [HideInInspector]
    public SJ_UIGridMove uIGridMove;
    [HideInInspector]
    public CursorDirectionInput cursorInput;

    // 다른 컴포넌트에 있을수도 있다.
    PlayerInput playerInput;

    public bool close_OK_curve = true;

    public SJ_COMMON.Func_Arg func_MoveObj;
    public SJ_COMMON.Func_Arg func_OnOK;
    public SJ_COMMON.Func_Arg func_Cancel;

    // 아이템 목록일때만.
    public SJ_UIListItem    uIListItem;

    void Awake()
    {
        uIGridMove = GetComponent<SJ_UIGridMove>();
        cursorInput = GetComponent<CursorDirectionInput>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void SetFunc( SJ_COMMON.Func_Arg func_ok , SJ_COMMON.Func_Arg func_move = null , SJ_COMMON.Func_Arg func_cancel = null )
    {
        func_OnOK = func_ok;
        func_MoveObj = func_move;
        func_Cancel = func_cancel;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetInputAble( bool b )
    {
        if( playerInput != null ) playerInput.enabled = b;
    }

    public void ListingDefault( List<_GRID_MOVE_DEFAULT_DATA> list_default )
    {
        if( uIListItem == null )
        {
            Debug.Log( "ListingDefault : uIListItem == null : " + gameObject.name );
            return;
        }
        uIListItem.Listing( list_default );
        uIGridMove.Align_By_GridLayoutGroup();
    } 

    public void Listing_SetFunc_OK( List<_GRID_MOVE_DEFAULT_DATA> list_default , SJ_COMMON.Func_Arg func_ok , bool input_able = true )
    {
        ListingDefault(list_default );
        SetFunc( func_ok );
        SetInputAble(input_able);
    }

    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        cursorInput.RegisterMoveX_One( OnInputMoveX );
        cursorInput.RegisterMoveY_One( OnInputMoveY );
        FirstUpdate();
    }

    public void FirstUpdate()
    {
        uIGridMove.Init();
        ActiveObj();
    }

    public void OnInputMoveX( int v )
    {
        if( v == 0 ) return;
        uIGridMove.MoveX(v);
        ActiveObj();
    }

    public void OnInputMoveY( int v )
    {
        if( v == 0 ) return;
        uIGridMove.MoveY(v);
        ActiveObj();
    }

    public void ActiveObj()
    {
        func_MoveObj?.Invoke( uIGridMove.recent_active );    
        OnFunc_Move( uIGridMove.recent_active );
    }

    public GameObject GetCurObj()
    {
        return uIGridMove.recent_active;
    }

    //=========================================================================
    // 유니티 인풋
    public void OnNavigate( InputValue value )
    {
        Vector2 input = value.Get<Vector2>();
        cursorInput.SetInput( input.x , -input.y );
    }

    public void OnSubmit( InputValue value )
    {
        func_OnOK?.Invoke( uIGridMove.recent_active );
        OnFunc_OK( uIGridMove.recent_active );

        // 가능한 gui 호출들...
        GameObject go = uIGridMove.recent_active;
        if( go != null )
        {
            Button button = go.GetComponent<Button>();
            if( button != null )
            {
                button.onClick.Invoke();                
            }
            
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if( trigger != null  )
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(trigger.gameObject , eventData, ExecuteEvents.pointerClickHandler);
            }
        }

        if( close_OK_curve )
        {
            SJ_UnityUIMng_Curve.CloseOne();
        }

    }

    public void OnCancel( InputValue value )
    {
        func_Cancel?.Invoke( uIGridMove.recent_active );
        OnFunc_Cancel( uIGridMove.recent_active );
    }
    //
    //=========================================================================

    virtual public void OnFunc_Move( GameObject go ){}
    virtual public void OnFunc_OK( GameObject go ){}
    virtual public void OnFunc_Cancel( GameObject go ){}
}
