using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SJ_UIGridMove 의 디폴트 동작
// 모노 , 객체 
public class SJ_UIGridMove_PrcActiveObj : MonoBehaviour
{
    public SJ_UIGridMove_PlayerInput move_PlayerInput;

    public MonoBehaviour mono;
    public GameObject obj;
    public void OnGrid_MOVE( bool active )
    {
        if( mono != null )
        {
            mono.enabled = active;
        }

        if( obj != null )
        {
            obj.SetActive(active);
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
}
