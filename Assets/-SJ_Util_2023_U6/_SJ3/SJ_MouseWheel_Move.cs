using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SJ_MouseWheel_Move : MonoBehaviour 
{
    //public float start_len = 3;
    public float min_len = 1;
    public float max_len = 5;

    public float cur_len = -3;

    public float mouse_step = 0.1f;
    public float lerp = 0.05f;

    public Transform tr_move;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PrcWork();
    }

    

    public void PrcWork()
    {
        if( tr_move == null )return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        cur_len += scroll * mouse_step;
        cur_len = Mathf.Clamp( cur_len , min_len , max_len );

        Vector3 v = new Vector3( 0,0,cur_len );
        tr_move.localPosition = Vector3.Lerp( tr_move.localPosition , v , lerp );
    }

    // public void OnScroll(PointerEventData eventData)
    // {
    //     float scrollDelta = eventData.scrollDelta.y;

    //     Debug.Log( scrollDelta );

    //     //if( cur_len < 0 ) cur_len = start_len;
    //     cur_len += scrollDelta * mouse_step;
    //     cur_len = Mathf.Clamp( cur_len , min_len , max_len );
    // }
}
