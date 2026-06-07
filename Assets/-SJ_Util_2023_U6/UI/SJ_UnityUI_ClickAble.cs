using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SJ_UnityUI_ClickAble : MonoBehaviour
{
    static public Dictionary<string, SJ_UnityUI_ClickAble> clickAbleList = new Dictionary<string, SJ_UnityUI_ClickAble>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        RegName();
    }

    public void RegName()
    {
        //clickAbleList.Add(gameObject.name, this);        
        clickAbleList[gameObject.name] = this;

        Debug.Log( "SJ_UnityUI_ClickAble : " + gameObject.name );
    }


    public void _SetClickAble(bool isClickAble)
    {
        Image image = GetComponent<Image>();
        if( image != null)
        {
            image.raycastTarget = isClickAble;
        }

        SJ_UITween_Color sJ_UITween_Color = GetComponent<SJ_UITween_Color>();
        if( sJ_UITween_Color != null)
        {
            if(  isClickAble == false )
            {
                sJ_UITween_Color.Stop();
                sJ_UITween_Color.ReturnColor();
            }
            else
            {
                sJ_UITween_Color.Play();
            }
        }
    }

    static public void SetClickAble(string name, bool isClickAble)
    {
        if (clickAbleList.ContainsKey(name) == false)
        {
            Debug.LogError("clickAbleList does not contain " + name);
            return;
        }

        clickAbleList[name]._SetClickAble(isClickAble);
    }

    static public void ALL_RegNameCanvas()
    {
        GameObject go_canvas = GameObject.Find("Canvas");
        SJ_UnityUI_ClickAble[] clickAbles = go_canvas.GetComponentsInChildren<SJ_UnityUI_ClickAble>(true);
        foreach (SJ_UnityUI_ClickAble clickAble in clickAbles)
        {
            clickAble.RegName();
        }
    }


    static public void SetClickAbleAll(bool isClickAble)
    {
        foreach (KeyValuePair<string, SJ_UnityUI_ClickAble> kvp in clickAbleList)
        {
            kvp.Value._SetClickAble(isClickAble);
        }
    }
}
