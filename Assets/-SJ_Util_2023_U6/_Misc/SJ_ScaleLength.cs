using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_ScaleLength : MonoBehaviour
{
    public    enum _AXIS_COOD
    {
        X , Y ,Z
    }
    public  _AXIS_COOD  _axis_cood;
    public  float       Length_Init;
    public  float       Scl_Cur = 1.0f;


    public  void    CalcScale( float tar_length )
    {
        Scl_Cur = tar_length / Length_Init;

        Vector3 s = transform.localScale;
        switch(_axis_cood)
        {
            case _AXIS_COOD.X: s.x = Scl_Cur;break;
            case _AXIS_COOD.Y: s.y = Scl_Cur;break;
            case _AXIS_COOD.Z: s.z = Scl_Cur;break;
        }
        transform.localScale = s;
    }

}
