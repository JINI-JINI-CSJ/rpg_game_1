using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SJ_Menu_Obj_SendMsg : EditorWindow
{
    string     send_func = "";

    [MenuItem("SJMisc/게임객체 메세지 보내기")]
    static void Init()
    {
        // 생성되어있는 윈도우를 가져온다. 없으면 새로 생성한다. 싱글턴 구조인듯하다.
        SJ_Menu_Obj_SendMsg window = (SJ_Menu_Obj_SendMsg)EditorWindow.GetWindow(typeof(SJ_Menu_Obj_SendMsg));
        window.Show();
    }
    

    void OnGUI()
    {
        if( Selection.activeObject == null )
            return;

        GameObject go = Selection.activeObject as GameObject;
        if( go == null ) return;

        GUILayout.Label("Select Obj : " + go.name , EditorStyles.boldLabel);
        send_func = EditorGUILayout.TextField("Text Field", send_func);
        

        if (GUILayout.Button("Send"))
        {
            Debug.Log( "SendMsg : " + go.name + " : " + send_func );
            SJ_Unity.SendMsg(go , send_func );
        }
    }
}
