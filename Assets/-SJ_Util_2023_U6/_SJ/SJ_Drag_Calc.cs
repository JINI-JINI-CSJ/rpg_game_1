using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SJ_Drag_Calc : MonoBehaviour
{
    public bool     isLimit;
    public SJ_RECT  rc_limit = new SJ_RECT();
    public bool     play;

    // 바닥 기준면
    public float    bottom_y = 0;

    // 높이에 따른 드래그 거리 보정
    public float    fix_move_height = 0.1f;

    public Transform    tr_move;

    public Transform    tr_height_fix;

    Vector3         pos_first_tr_move;

    Vector3         pos_mouse_start;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void     BeginDrag(  )
    {
        pos_first_tr_move = tr_move.position;
        pos_mouse_start = Input.mousePosition;
    }

    public void     Drag(  )
    {
        Vector3 v_off_m = pos_mouse_start - Input.mousePosition;
        
        Vector3 v_off = new Vector3( v_off_m.x , 0 , v_off_m.y );


        float y = tr_height_fix.position.y - bottom_y;
        v_off *= (fix_move_height * y);

        Vector3 pos = pos_first_tr_move + v_off;

        Debug.Log( "v_off : " + v_off );

        if( isLimit )
        {
            //if( rc_limit.Contain( new Vector2Int( (int)pos.x , (int)pos.z ) ) == false ) return;
            Vector2Int v2 = new Vector2Int( (int)pos.x , (int)pos.z );
            rc_limit.Limit( ref v2 );
            pos.x = v2.x;
            pos.z = v2.y;
        }

        Debug.Log( "pos : " + pos );
        tr_move.position = pos;
    }

    public void     EndDrag(  )
    {

    }

    // set limit
    //..


}
