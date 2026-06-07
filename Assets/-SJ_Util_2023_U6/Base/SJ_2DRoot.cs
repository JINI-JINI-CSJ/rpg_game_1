using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_2DRoot : MonoBehaviour
{
    static public   SJ_2DRoot   g;
    public  Camera  cam;
    public  float   Half_CoodY;
    public  float   Half_CoodX_Calc;

    private void Awake() {
        g = this;
        cam = GetComponent<Camera>();
        Calc_Screen();
    }

    public  void    Calc_Screen()
    {
        Half_CoodX_Calc = Half_CoodY * cam.aspect;
    }

    static public   Vector3 CoodToTrans( float cx , float cy )
    {
        Vector3 v = Vector3.zero;
        v.x = g.cam.orthographicSize * (cx / g.Half_CoodX_Calc);
        v.y = g.cam.orthographicSize * (cy / g.Half_CoodY);
        return v;
    }

    static public   Vector2 TransToCood( Transform tr )
    {
        Vector2 v = Vector2.zero;
        v.x = g.Half_CoodX_Calc * (tr.localPosition.x / g.cam.orthographicSize);
        v.y = g.Half_CoodY      * (tr.localPosition.y / g.cam.orthographicSize);
        return v;
    }



}
