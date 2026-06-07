using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 세로형 목록
// 자동으로 아이템 선택하면 스크롤 포커스
public class SJ_UIScrollRectList : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject go_Grid;
    public GameObject go_SelectCur;


    public UnityEvent evt_SetCursorPos;

    public UnityEvent evt_EmptyCursor;

    public bool hori_vert = true; // 가로 : false , 세로 true



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ListAdd<T>(List<T> lt)
    {
//        Debug.Log( "ListAdd : " + lt.Count );
        
        if (lt.Count < 1)
        {
            SJ_UnityUI_Util.ClearList(go_Grid);
            return;
        }

        List<object> lt_obj = SJ_UnityUI_Util.ListArgToListT(lt);
        SJ_UnityUI_Util.ListItem_Add(lt_obj, go_Grid, "InitItem");

        if (go_Grid.transform.childCount > 0)
        {
            go_SelectCur = go_Grid.transform.GetChild(0).gameObject;
            if (go_SelectCur.activeSelf)
                SetCursorPos();
            else
                EmptyCursor();
        }
        else
        {
            EmptyCursor();
        }
    }

    void EmptyCursor()
    {
        OnEmptyCursor();
        evt_EmptyCursor.Invoke();
    }

    // 외부에서 강제로 선택 , 예) 마우스로 선택
    public void SetCursorPos_User(GameObject go)
    {
        go_SelectCur = go;
        SetCursorPos(false);
    }

    void SetCursorPos( bool scroll_cur = true )
    {
        if (go_SelectCur == null || go_SelectCur.activeSelf == false)
        {
            EmptyCursor();
            return;
        }
        int idx = 0, total = 0;
        GetSiblingIndex_Total(ref idx, ref total);
        if (scroll_cur)
        {
            if (hori_vert)
                scrollRect.verticalNormalizedPosition = 1.0f - (float)idx / (float)(total - 1);
            else
                scrollRect.horizontalNormalizedPosition = 1.0f - (float)idx / (float)(total - 1);            
        }
        OnSetCursorPos();
        evt_SetCursorPos.Invoke();
    }

    virtual public void OnEmptyCursor() { }

    virtual public void OnSetCursorPos() { }

    // 아이템 커서 한칸씩 이동
    public void SelectMoveNextItem(int add_step)
    {
        if (go_SelectCur == null || go_SelectCur.activeSelf == false) return;
        int idx = 0, total = 0;
        GetSiblingIndex_Total(ref idx, ref total);

        idx += add_step;
        if (idx < 0) idx = 0;
        if (idx >= total) idx = total - 1;

        Transform tr_child = go_Grid.transform.GetChild(idx);
        go_SelectCur = tr_child.gameObject;

        SetCursorPos();
    }

    // 활성화 된것들만 계산
    void GetSiblingIndex_Total(ref int idx_cur, ref int total)
    {
        List<Transform> tr_child = new List<Transform>();
        for (int i = 0; i < go_Grid.transform.childCount; i++)
        {
            Transform tr = go_Grid.transform.GetChild(i);
            if (tr.gameObject.activeSelf)
            {
                tr_child.Add(tr);
            }
        }
        total = tr_child.Count;
        for (int i = 0; i < tr_child.Count; i++)
        {
            if (tr_child[i] == go_SelectCur.transform)
            {
                idx_cur = i;
                break;
            }
        }
    }
}
