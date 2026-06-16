using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UIListItem : MonoBehaviour
{
    public GameObject go_List;
    
    public void Listing<T>( List<T> lt , string func = "OnInit" )
    {
        gameObject.SetActive(true);
        List<object> lt_arg = SJ_UnityUI_Util.ListArgToListT( lt );

        // "null" 이 있는지.
        bool find_null = false;
        foreach( var s in lt_arg )
        {
            if( s == null )
            {
                find_null = true;
                break;
            }
        }

        List<object> lt_arg_fill = null;
        if( find_null )
        {
            lt_arg_fill = new List<object>();
            foreach( var s in lt_arg )
            {
                if( s != null )
                {
                    lt_arg_fill.Add(s);
                }
                else
                {
                    lt_arg_fill.Add(new object());
                }
            }
        }
        else
        {
            lt_arg_fill = lt_arg;
        }


        SJ_UnityUI_Util.ListItem_Add( lt_arg_fill , go_List , func );
    }
}
