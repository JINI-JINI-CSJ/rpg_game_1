using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SJ_UnityUI_Util : SJ_Singleton_Mono
{
    static public   List<object>    ListArgToListT<T>( List<T> lt_T )
    {
        List<object> lt_obj = new List<object>();
        foreach( T s in lt_T )
        {
            lt_obj.Add( s as object);
        }
        return lt_obj;
    }

    static public void ClearList(GameObject go_grid)
    {
        SJ_Unity.Active_Child( go_grid.transform , 0 );
    }

    static public   void    ListItem_Add( List<object> lt_data , GameObject go_grid , string func_item , Transform tr_prf = null , 
                                        SendMessageOptions sendMessageOptions = SendMessageOptions.DontRequireReceiver ,
                                        bool noCreateInst = false
                                         )
    {
        if( tr_prf == null )
        {
            if( go_grid.transform.childCount < 1 )
            {
                Debug.LogError( "에러!!! : 리스트 프리펩 없음!!! : " + go_grid.name );
                return;
            }
            tr_prf = go_grid.transform.GetChild(0);
        }

        if( lt_data.Count > go_grid.transform.childCount && noCreateInst == false )
        {
            int add = lt_data.Count - go_grid.transform.childCount;
            for( int i = 0 ; i < add ; i++ ) 
            {
                GameObject inst = GameObject.Instantiate(tr_prf.gameObject);
                SJ_Unity.SetEqTrans( inst.transform , null , go_grid.transform );
            }
        }

        for( int i = 0 ; i < go_grid.transform.childCount ; i++ )
        {
            Transform tr_item = go_grid.transform.GetChild(i);
            if( i < lt_data.Count )
            {
                object obj_arg = lt_data[i];           

                //Debug.Log( tr_item.name );

                tr_item.gameObject.SetActive(true);

                // 주의 !!!!
                // 반드시 엑티브 상태일것!!!!
                // 부모도 액티브인지 확인!!!!

                // 주의 2 !!!!
                // 인자가 "null" 이면 인자 없는 함수가 호출된다.
                // 그래서 인자없는 함수를 만들면 인자가 있어도 인자없는 함수가 호출된다.
                // "null"은 절대 없게 하고 , new object 같은걸로 채운다음에 
                // 받는 쪽에서 캐스팅 해보고 안되면 리턴하는 식으로 하자.
                // 일단 지금은 SJ_UIListItem 에서 채우자.

                tr_item.gameObject.SendMessage( func_item , obj_arg , sendMessageOptions );
            }else{
                tr_item.gameObject.SetActive(false);
            }
        }
    }

    static public void Child_InstListNum(  GameObject go_grid  , int num )
    {
        if( go_grid.transform.childCount < 1 )
        {
            Debug.LogError( "에러!!! : 리스트 프리펩 없음!!! : " + go_grid.name );
            return;
        }
        Transform    tr_prf = go_grid.transform.GetChild(0);
        if( num > go_grid.transform.childCount )
        {
            int add = num - go_grid.transform.childCount;
            for( int i = 0 ; i < add ; i++ ) 
            {
                GameObject inst = GameObject.Instantiate(tr_prf.gameObject);
                SJ_Unity.SetEqTrans( inst.transform , null , go_grid.transform );
            }
        }
        for( int i = 0 ; i < go_grid.transform.childCount ; i++ )
        {
            Transform tr_item = go_grid.transform.GetChild(i);
            if( i < num )
            {
                tr_item.gameObject.SetActive(true);
            }else{
                tr_item.gameObject.SetActive(false);
            }
        }
    }

    static public GameObject  ListItem_UI_Obj_Add( GameObject go_grid , GameObject prf , string msg = "" )
    {
        GameObject inst = GameObject.Instantiate(prf);
        inst.SetActive(true);
        SJ_Unity.SetEqTrans( inst.transform , null , go_grid.transform );

        if( string.IsNullOrEmpty(msg) == false )
        {
            Text tx = inst.GetComponent<Text>();
            if(tx != null) tx.text = msg;
        }
        return inst;
    }

    static public void  ImageSprite( Image img , Sprite spr = null )
    {
        if( img != null )
        {
            img.sprite = spr;
            if( spr != null )
            {
                img.enabled = true;
            }else{
                img.enabled = false;
            }
        }
    }

    static public void      Image_Load( Image image , string path )
    {
        if( image == null || string.IsNullOrEmpty(path) ) return;
        // UnityEngine.Object obj = SJ_ResPoolSys.GetResObj_PathName(  path , false , typeof(Sprite) );
        // if( obj != null )
        //     SJ_UnityUI_Util.ImageSprite( image , obj as Sprite );
        Sprite sprite = SJ_ResPoolSys.GetResObjs_PathName_Sprite( path  );
        if( sprite != null )
        {
            SJ_UnityUI_Util.ImageSprite( image ,sprite );
        }
    }
    static public void      Image_Load_Multi( Image image , string path , string spr_name )
    {
        if( image == null || string.IsNullOrEmpty(path) || string.IsNullOrEmpty(spr_name)  ) return;
        Sprite sprite = SJ_ResPoolSys.GetResObjs_PathName_MultiSpr( path , spr_name  );
        if( sprite != null )
        {
            SJ_UnityUI_Util.ImageSprite( image ,sprite );
        }
    }

    static public void  TextString( Text text , string  str = "" ){if( text != null ) text.text = str;}
    static public void  TextStringTMP( TMP_Text text , string  str = "" ){if( text != null ) text.text = str;}
    static public void  Button_Interactable( Button bt , bool b = true ){if( bt != null ) bt.interactable = b;}
    static public void  Toggle_Interactable( Toggle bt , bool b = true ){if( bt != null ) bt.interactable = b;}

    static public void  Active_UI( Text text , bool show = false )
    {
        if( text != null )text.gameObject.SetActive(show);
    }
}
