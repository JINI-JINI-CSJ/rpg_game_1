using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SJ_Canvas
{
    public  delegate    void    OnUpdate_Obj( Button bt_obj  , int idx );
    //public  delegate    void    OnClick_ScrView( int idx );

    static  public      void    Text_SetStr( GameObject go , string str )
    {
        Text ui_text=    go.GetComponentInChildren<Text>();
        if( ui_text != null ) ui_text.text = str;
    }



    static  public       void    Init_ScrView_OneObj( Transform tr_grid , int count , OnUpdate_Obj upate_func = null )
    {
        List<GameObject> lt_del_childe = new List<GameObject>();
        for( int i = 1 ; i < tr_grid.childCount ; i++ )
        {
            //GameObject.DestroyImmediate( tr_grid.GetChild(i).gameObject );
            lt_del_childe.Add( tr_grid.GetChild(i).gameObject );
        }
        foreach( GameObject s in lt_del_childe ) GameObject.DestroyImmediate( s );

        GameObject obj_1 = tr_grid.GetChild(0).gameObject;
        for( int i = 1 ; i < count ; i++ )
        {
            GameObject bt_child = GameObject.Instantiate( obj_1 );
            //bt_child.name = i.ToString();
            bt_child.transform.parent = tr_grid;
        }
        for( int i = 0 ; i < tr_grid.childCount ; i++ )
        {
            GameObject bt_child = tr_grid.GetChild(i).gameObject;
            Button uibt = bt_child.GetComponent<Button>();

            if( uibt != null )
            {
                //UnityAction<GameObject, int > ac = new UnityAction<GameObject  , int>( dg_func );
                //uibt.onClick.AddListener( ac );

                //UnityEngine.Events.UnityAction buttonCallback = () => mono.dg_func;

                //uibt.onClick.AddListener( buttonCallback );
            }
                //uibt.onClick.AddListener(  dg_func( bt_child , i ) );

            if( upate_func != null )
            {
                upate_func( uibt  , i);
            }
        }
    }
}
