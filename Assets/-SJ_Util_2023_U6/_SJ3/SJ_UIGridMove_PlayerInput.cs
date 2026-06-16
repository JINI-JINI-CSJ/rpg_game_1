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
    public SJ_UIGridMove uIGridMove;
    public CursorDirectionInput cursorInput;
    public event Action<GameObject> OnMoveObj;
    public event Action<GameObject> OnOK_Input;
    public event Action<GameObject> OnCancel_Input;

    void Awake()
    {
        uIGridMove = GetComponent<SJ_UIGridMove>();
        cursorInput = GetComponent<CursorDirectionInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();
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

    public void RegOnMoveObj_One( Action<GameObject> callback )
    {
        if( OnMoveObj?.GetInvocationList().Length > 0 ) return;
        OnMoveObj += callback;
    }

    public void RegOnOK_Input_One( Action<GameObject> callback )
    {
        if( OnOK_Input?.GetInvocationList().Length > 0 ) return;
        OnOK_Input += callback;
    }

    public void RegOnCancel_Input_One( Action<GameObject> callback )
    {
        if( OnCancel_Input?.GetInvocationList().Length > 0 ) return;
        OnCancel_Input += callback;
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
        OnMoveObj?.Invoke( uIGridMove.recent_active );    
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
        OnOK_Input?.Invoke( uIGridMove.recent_active );

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

    }

    public void OnCancel( InputValue value )
    {
        OnCancel_Input?.Invoke( uIGridMove.recent_active );
    }
}
