using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용안함 , 카메라 밖으로 나가면 의도하지 않은 작동
public class SJ_UILookAtWorldPos : MonoBehaviour
{
    
    public RectTransform rect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Update_Look( Vector3 pos_w )
    {
        if( rect == null )
        {
            rect = GetComponent<RectTransform>();
            if( rect == null ) return;
        }

        // 본인의 2d 좌표와 월드의 2d 좌표
        Vector3 view_s = SJ_UnityUIMng.GetViewportPosUI( rect );
        Vector3 view_w = Camera.main.WorldToViewportPoint(pos_w);

        view_s.z = view_w.z = 0;
        Vector3 dir_v = view_w - view_s;
        dir_v.Normalize();
        float z_ang = SJ_Cood.GetAngle(Vector3.up,dir_v , _XYZ.Z);
        Debug.Log( view_s + " : " + view_w );
        transform.rotation = Quaternion.Euler( 0,0, z_ang );
    }
}
