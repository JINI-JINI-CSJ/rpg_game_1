using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 메뉴와 하위 버튼
// 본인 객체는 보통 페이지
// 메뉴는 상위에서 보여질것
public class SJ_MonoMenu : MonoBehaviour
{
    SJ_MonoMenu root;
    SJ_MonoMenu par_menu;

    bool record_menu_cur_global_by_root = true;
    SJ_MonoMenu menu_cur_global_by_root; // 현재 루트에서 현재 열린 메뉴 , 루트만 기억한다.

    // 본인이 상위 메뉴일때 하위 메뉴들
    public List<SJ_MonoMenu> child_menu;

    //bool cur_focus;
    // 버튼일때 , 일단 토글만
    public Toggle toggle_menu;
    // 각 페이지 마다 인풋 가지고 있기
    //public PlayerInput playerInput;

    // 본인이 하위 메뉴일때 이웃 연결 
    // 4 방향 연결
    [System.Serializable]
    public class _DIR_OTHER
    {
        public SJ_MonoMenu N_MENU;
        public SJ_MonoMenu S_MENU;
        public SJ_MonoMenu W_MENU;
        public SJ_MonoMenu E_MENU;

        // 유니티 인풋 좌표계
        // 위가 + , 아래가 -
        public SJ_MonoMenu Get(int x, int y)
        {
            if (y == 1) return N_MENU;
            if (y == -1) return S_MENU;
            if (x == -1) return W_MENU;
            if (x == 1) return E_MENU;
            return null;
        }
    }
    public _DIR_OTHER dir_other;

    // 현재 열려있는 메뉴
    // 에디터에서 미리 지정하면 이거부터 시작
    public SJ_MonoMenu menu_cur;

    public AudioClip snd_Navi;
    public AudioClip snd_Submit;
    public AudioClip snd_Cancel;


    public void InitRoot()
    {
        root = this;
        InitNode(root);
        OnFocusSelf(true);

        FocusCurMenu();
    }

    public void FocusCurMenu()
    {
        if (menu_cur != null)
        {
            menu_cur.OnFocusSelf(true);
        }
    }

    public void InitNode(SJ_MonoMenu _root)
    {
        foreach (var s in child_menu)
        {
            s.root = _root;
            s.par_menu = this;
            s.OnFocusSelf(false);
            s.OnOpenSelf(false);
            s.InitNode(_root);
        }
        OnInitNode();
    }

    virtual public void OnInitNode() { }

    public bool CheckRoot()
    {
        if (root == this)
            return true;
        return false;
    }

    public void FocusMenu(int idx)
    {
        SJ_MonoMenu node = null;
        for (int i = 0; i < child_menu.Count; i++)
        {
            if (idx == i) node = child_menu[i];
            else child_menu[i].OnFocusSelf(false);
        }

        if (node != null)
        {
            node.OnFocusSelf(true);
            menu_cur = node;
        }
    }

    public void FocusMenu(SJ_MonoMenu menu )
    {
        for (int i = 0; i < child_menu.Count; i++)
        {
            if( menu !=  child_menu[i] )
                child_menu[i].OnFocusSelf(false);
        }

        if (menu != null)
        {
            menu.OnFocusSelf(true);
            menu_cur = menu;
        }
    }   

    public void FocusMenu(int x, int y)
    {
        SJ_MonoMenu node = null;
        if (menu_cur == null)
        {
            // 첫번째 메뉴
            if (child_menu.Count > 0) menu_cur = child_menu[0];
            else
            {
                Debug.Log("FocusMenu : no child_menu");
                return;
            }
            node = menu_cur;
        }
        else
        {
            node = menu_cur.dir_other.Get(x, y);
        }

        if (node != null)
        {
            foreach (var s in child_menu)
            {
                if (s != node) s.OnFocusSelf(false);
            }
            node.OnFocusSelf(true);
            menu_cur = node;
        }
        else
        {
            //Debug.Log("FocusMenu : no dir_other " + x + "," + y);
        }

    }

    virtual public void OnFocusSelf(bool b)
    {
        //cur_focus = b;
        if (toggle_menu != null)
        {
            //Debug.Log("OnFocusSelf : toggle_menu : " + toggle_menu.gameObject.name + " " + b);
            toggle_menu.SetIsOnWithoutNotify(b);
        }

    }

    public void OpenMenu(int idx = -1)
    {
        if (idx > -1)
        {
            FocusMenu(idx);
        }

        if (idx >= 0 && idx < child_menu.Count)
            OpenMenu(child_menu[idx]);
        else if (menu_cur != null)
            OpenMenu(menu_cur);

    }

    public void OpenMenu(SJ_MonoMenu menu)
    {
        SJ_MonoMenu node = null;
        for (int i = 0; i < child_menu.Count; i++)
        {
            if (child_menu[i] == menu) node = child_menu[i];
            else child_menu[i].OnOpenSelf(false);
        }

        if (menu != null)
        {
            menu_cur = menu;            
            menu.OnOpenSelf(true);
            
            OnFocusSelf(false);
            OnOpenSelf(false);
        }
        OnOpenMenu(menu);
    }

    virtual public void OnOpenMenu(SJ_MonoMenu node) { }


    virtual public void OnOpenSelf(bool b)
    {
//        Debug.Log("OnOpenSelf : " + gameObject.name + " " + b);

        if (b)
        {
            if (root.record_menu_cur_global_by_root)
                root.menu_cur_global_by_root = this;
        }

        if( b )SJ_PlayerInputMng.ActiveInput( gameObject );

        //if (playerInput != null) playerInput.enabled = b;
    }

    public void OpenParent()
    {
        if (par_menu == null) return;

        OnFocusSelf(false);
        OnOpenSelf(false);

        par_menu.OnFocusSelf(true);
        par_menu.OnOpenSelf(true);
        par_menu.FocusCurMenu();
        par_menu.OnParentMenu();
    }

    // 
    virtual public void OnParentMenu() { }

    // 바로 대상 메뉴를 연다.
    // 1. 하위메뉴 상태라면 루트까지 역 오픈한다.
    // 2. 루트로 부터 대상 메뉴까지 오픈한다.
    public void OpenALL_TargetMenu(SJ_MonoMenu menu_target)
    {
        root.record_menu_cur_global_by_root = false;

        // 상위로 역 오픈
        if (root.menu_cur_global_by_root != null)
        {
            SJ_MonoMenu cur = root.menu_cur_global_by_root;
            while (true)
            {
                if (cur == null || cur == root) break;
                cur.OpenParent();
                cur = cur.par_menu;
            }
        }

        root.record_menu_cur_global_by_root = true;
        // 타겟 매뉴까지 연다
        List<SJ_MonoMenu> path = FindMenu_ChildAll(menu_target);
        if (path != null)
        {
            foreach (var m in path)
            {
                if (m == root) continue;
                //m.OpenMenuSelf_Parent();
                if (m.par_menu != null)
                {
                    m.par_menu.FocusMenu(m);
                    m.par_menu.OpenMenu( m );
                }
            }
        }
    }

    public void OpenALL_TargetMenu(int child_idx)
    {
        if (child_idx >= child_menu.Count)
        {
            return;
        }
        OpenALL_TargetMenu(child_menu[child_idx]);
    }

    // 본인이 직접 오픈 및 상위 처리
    // void OpenMenuSelf_Parent()
    // {
    //     OnFocusSelf(true);
    //     OnOpenSelf(true);
    //     if (par_menu != null)
    //     {
    //         par_menu.OnFocusSelf(false);
    //         par_menu.OnOpenSelf(false);
    //         par_menu.OnOpenMenu(this);
    //     }
    // }

    // 루트로 부터 대상 메뉴까지의 경로
    public List<SJ_MonoMenu> FindMenu_ChildAll(SJ_MonoMenu menu_target)
    {
        foreach (var m in child_menu)
        {
            if (m == menu_target)
            {

                List<SJ_MonoMenu> lt = new List<SJ_MonoMenu>();
                SJ_MonoMenu cur = m;
                while (true)
                {
                    lt.Add(cur);
                    cur = cur.par_menu;
                    if (cur == null || root == cur) break;
                }
                // 뒤집어서 순서대로
                lt.Reverse();
                return lt;
            }

            List<SJ_MonoMenu> lt_chd = m.FindMenu_ChildAll(menu_target);
            if (lt_chd != null) return lt_chd;
        }

        return null;
    }

    //=============================================================================================
    // 인풋 장치

    // 방향키
    public void OnNavigate(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            // 좌우 이동
            int shift_idx = 0;
            if (input.x < -0.1f) shift_idx = -1;
            if (input.x > 0.1f) shift_idx = 1;

            if (OnUserNavi_Hori(shift_idx) == false)
                FocusMenu(shift_idx, 0);
        }
        else
        {
            // 상하 이동
            // 유니티 인풋시스템 상하 반대
            int shift_idx = 0;
            if (input.y < -0.1f) shift_idx = -1; // down
            if (input.y > 0.1f) shift_idx = 1;   // up
            if (OnUserNavi_Vert(shift_idx) == false)
                FocusMenu(0, shift_idx);
        }

        SJSound.PlaySound( root.snd_Navi );
    }

    virtual public bool OnUserNavi_Hori(int idx) { return false; }
    virtual public bool OnUserNavi_Vert(int idx) { return false; }

    //
    public void OnSubmit(InputValue value)
    {
        Debug.Log("OnSubmit : " + gameObject.name);
        if (OnUserSubmit() == false)
        {
            if (menu_cur != null)
            {
                Debug.Log("OnSubmit : OpenMenu");
                OpenMenu();
            }
            else
            {
                Debug.Log("OnSubmit : FocusMenu");
                FocusMenu(0);
            }
        }

        SJSound.PlaySound( root.snd_Submit );
    }

    virtual public bool OnUserSubmit() { return false; }

    public void OnCancel(InputValue value)
    {
        if (OnUserCancel() == false)
            OpenParent();

        SJSound.PlaySound( root.snd_Cancel );
    }

    virtual public bool OnUserCancel() { return false; }

    // 메뉴들이 토글로 되 있을경우
    // 각 메뉴들은 상위 메뉴의 이 함수를 등록한다.
    public void OnToggleMenu_Push()
    {
        // 하위 메뉴들의 토글ui 를 찾는다.
        // 그것이 isOn 이면 그 메뉴 오픈
        foreach (var s in child_menu)
        {
            // if (s.toggle_menu != null && s.toggle_menu.isOn)
            // {
            //     OpenALL_TargetMenu(s);
            //     return;
            // }

            Debug.Log( s.name + " : " + s.toggle_menu.isOn );
        }
    }
}
