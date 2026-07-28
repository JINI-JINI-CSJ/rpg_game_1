using System.Collections.Generic;
using UnityEngine;

// X 축 기준 정렬
// 중심점을 기준으로 양 옆으로 이동 
public class SJ_LineGridPos : MonoBehaviour
{
    public float w_size = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlignLine( bool inc_hide_child = false )
    {
        List<Transform> trs = new();
        for( int i = 0 ; i < transform.childCount ; i++ )
        {
            Transform tr_ch = transform.GetChild(i);
            if( inc_hide_child )
            {
                trs.Add( tr_ch );
            }
            else
            {
                if( tr_ch.gameObject.activeSelf )
                    trs.Add( tr_ch );
            }
        }

        float start_pos = trs.Count * w_size * 0.5f;

        for( int i = 0 ; i < trs.Count ; i++ )
        {
            float grid_p = start_pos + i * w_size;
            Vector3 pos = Vector3.zero;
            pos.x = grid_p;
            trs[i].localPosition = pos;
        }
    }
}
