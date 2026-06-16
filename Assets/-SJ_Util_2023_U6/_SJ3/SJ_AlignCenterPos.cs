using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_AlignCenterPos : MonoBehaviour
{
    public float width = 1;

    public _XYZ coord_axis = _XYZ.X;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Align()
    {
        float total_w = width * transform.childCount;
        float fix = -(total_w * 0.5f);

        for( int i = 0 ; i < transform.childCount ; i++ )
        {
            float pos = i * width + fix;
            Vector3 v = Vector3.zero;
            switch( coord_axis )
            {
                case _XYZ.X: v.x = pos; break;
                case _XYZ.Y: v.y = pos; break;
                case _XYZ.Z: v.z = pos; break;
            }
            transform.GetChild(i).localPosition = v;
        }
    }
}
