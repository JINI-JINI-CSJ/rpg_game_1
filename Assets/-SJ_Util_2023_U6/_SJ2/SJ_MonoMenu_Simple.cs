using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SJ_MonoMenu_Simple : MonoBehaviour
{
    [System.Serializable]
    public class _MENU
    {
        public SJ_MonoMenu_SimpleUnit menu_unit;
        public Vector2Int pos;
    }

    public List<_MENU> menus;

    public SJ_MonoMenu_SimpleUnit default_menu;

    _MENU menu_cur;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCurMenu(default_menu);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCurMenu(_MENU _menu)
    {
        menu_cur = _menu;
        foreach (var s in menus)
        {
            if (menu_cur == s) s.menu_unit.SetActive(true);
            else s.menu_unit.SetActive(false);
        }
    }

    public void SetCurMenu( SJ_MonoMenu_SimpleUnit _menu )
    {
        foreach (var s in menus)
        {
            if (s.menu_unit == _menu)
            {
                SetCurMenu(s);
                return;
            }
        }
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
            Menu_ActivePos(shift_idx, 0);
        }
        else
        {
            // 상하 이동
            // 유니티 인풋시스템 상하 반대
            int shift_idx = 0;
            if (input.y < -0.1f) shift_idx = 1; // down
            if (input.y > 0.1f) shift_idx = -1;   // up
            Menu_ActivePos(0, shift_idx);
        }
    }

    public _MENU FindPos(Vector2Int pos)
    {
        foreach (var s in menus)
        {
            if (s.pos == pos) return s;
        }
        return null;
    }

    // 메뉴 좌표 이동
    public bool Menu_ActivePos(int off_x, int off_y , int _depth = 0)
    {
        // 행열 축 기준으로 최소대 값 구하고 오프셋 더하기
        // 동시에 가로세로는 안된다. 그런 경우가 있으면 2번 호출하자.

        if( _depth > 1000 ) return false;

        int min = 999, max = -1;

        // 같은 행열에 있는 것들만 모으기
        // 최소대 값 및 처리
        Vector2Int pos_next = menu_cur.pos;
        pos_next.x += off_x;
        pos_next.y += off_y;

        List<_MENU> lt = new List<_MENU>();
        if (off_x == 0)
        {
            int x = menu_cur.pos.x;
            foreach (var s in menus)
            {
                if (s.pos.x != x) continue;
                lt.Add(s);
                if (s.pos.y < min) min = s.pos.y;
                if (s.pos.y > max) max = s.pos.y;
            }
            if (pos_next.y < min) pos_next.y = max;
            if (pos_next.y > max) pos_next.y = min;
        }
        if (off_y == 0)
        {
            int y = menu_cur.pos.y;
            foreach (var s in menus)
            {
                if (s.pos.y != y) continue;
                lt.Add(s);
                if (s.pos.x < min) min = s.pos.x;
                if (s.pos.x > max) max = s.pos.x;
            }
            if (pos_next.x < min) pos_next.x = max;
            if (pos_next.x > max) pos_next.x = min;
        }


        _MENU find_menu = FindPos(pos_next);
        if (find_menu == null)
        {
            return false;
        }

        // 메뉴객체가 활성화가 아니라면 한번더 해본다.
        // 같은 방향으로 딮스로 추가 해서 한다.
        if( find_menu.menu_unit == null || find_menu.menu_unit.gameObject.activeSelf == false )
        {
            // 1 씩 더해본다.
            if( off_x > 0 ) off_x++;
            else if( off_x < 0 ) off_x--;
            if( off_y > 0 ) off_y++;
            else if( off_y < 0 ) off_y--;

            return Menu_ActivePos(off_x, off_y , ++_depth );
        }

        SetCurMenu(find_menu);
        return true;
    }

    public void OnSubmit(InputValue value)
    {
        menu_cur.menu_unit.CallFunc();
    }
}
