using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 페이지 형태의 팝업
// 여기는 베이스 기능과 기본 기능
public class SJ_UIPagePopup : MonoBehaviour
{
    // 최대 갯수
    public int      page_MAX_Count;
    public Button bt_before;
    public Button bt_next;
    public Text txt_bt_next;
    public Text txt_page_num;

    [HideInInspector]
    public int cur_page;

    _SJ_GO_FUNC func_end = new _SJ_GO_FUNC();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPopup()
    {
        OnFirst();
        ActivePage(0);
    }
    virtual public void OnFirst(){}

    public void SetFuncEnd( MonoBehaviour mono , string func )
    {
        func_end.SetMono(mono , func);
    }

    void ActivePage( int idx )
    {   
        if( idx < 0 || idx >= page_MAX_Count ) return;
        cur_page = idx;

        if( txt_page_num != null )
        {
            txt_page_num.text = (cur_page+1).ToString() + "/" + page_MAX_Count.ToString();
        }

        bool last = false;
        if( cur_page == page_MAX_Count - 1 )last = true;

        if( cur_page == 0 )
        {
            bt_before.interactable = false;
        }
        else
        {
            bt_before.interactable = true;
        }
        OnActivePage(last);
    }
    virtual public void OnActivePage(bool last){}

    public void OnClick_Prev()
    {
        ActivePage( --cur_page );
    }

    public void OnClick_Next()
    {
        // 마지막인데 다음 눌림
        if( cur_page == page_MAX_Count - 1 )
        {
            SJ_UnityUIMng.ClosePopup();
            func_end.Func();
            return;
        }

        ActivePage( ++cur_page );
    }

    public void OnNavigate(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        int shift_idx = 0;
        if (input.x < -0.1f) shift_idx = -1;
        if (input.x > 0.1f) shift_idx = 1;

        if( shift_idx == -1 )
        {
            OnClick_Prev();
        }
        else if( shift_idx == 1 )
        {
            OnClick_Next();
        }
    }
}

