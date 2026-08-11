using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 아이템 이름 , 유저 데이터
public class _GRID_MOVE_DEFAULT_DATA
{
    public string name;
    public object userData;
}

// SJ_UIGridMove 의 디폴트 동작
// 모노 , 객체 
public class SJ_UIGridMove_PrcActiveObj : MonoBehaviour
{
    public SJ_UIGridMove_PlayerInput move_PlayerInput;
    public MonoBehaviour    mono_cur_active;
    public GameObject       obj_cur_active;

    // 아이템 목록 메인으로 사용할때


    public Text text_name;
    public _GRID_MOVE_DEFAULT_DATA grid_move_data;

    public void OnInit( object arg )
    {
        grid_move_data  = arg as _GRID_MOVE_DEFAULT_DATA;
        if( grid_move_data == null ) return;
        SJ_UnityUI_Util.TextString( text_name , grid_move_data.name );
    }
    

    public void OnGrid_MOVE( bool active )
    {
        if( mono_cur_active != null )
        {
            mono_cur_active.enabled = active;
        }

        if( obj_cur_active != null )
        {
            obj_cur_active.SetActive(active);
        }
    }

    public void OnCurObj_Active()
    {
        if( move_PlayerInput != null )
        {
            move_PlayerInput.uIGridMove.SelectByGameObj( gameObject );
            move_PlayerInput.ActiveObj();
        }
    }

    static public T GetUserValue<T>( object obj )
    {
        GameObject go = obj as GameObject;
        if( go == null ) return default;
        SJ_UIGridMove_PrcActiveObj prcActiveObj = go.GetComponent<SJ_UIGridMove_PrcActiveObj>();
        if( prcActiveObj == null || prcActiveObj.grid_move_data == null ) return  default;

        return (T)prcActiveObj.grid_move_data.userData;
    }
}
