using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SJ_AlignPosLine : MonoBehaviour
{
    public  _XYZ    axis;
    public  float   gap;

    public  bool    ext = true;

    // Start is called before the first frame update
    void Start()
    {
        ext = true;
    }

    // Update is called once per frame
    void Update()
    {
        if( ext  )
        {
            Align();
            ext = false;   
        }
    }

    public  void    Add_Child( Transform tr )
    {
        tr.parent = transform;
    }

    public  void    Align()
    {
        List<Transform> trs = new List<Transform>();
        for( int i = 0 ; i < transform.childCount ; i++ )
        {
            Transform tr = transform.GetChild(i);
            if( tr.gameObject.activeSelf )
            {
                trs.Add(tr);
            }
        }

        float max_width = gap * (trs.Count-1);
        int c = 0;
        foreach( Transform s in trs )
        {
            float pos = (gap * c) - (max_width * 0.5f);
            Vector3 v = Vector3.zero;
            switch( axis )
            {
                case _XYZ.X: v.x = pos;break;
                case _XYZ.Y: v.y = pos;break;
                case _XYZ.Z: v.z = pos;break;
            }
            s.localPosition = v;
            c++;
        }
    }

    public HashSet<Transform>     All_Child_Par_Null( bool show = false )
    {
        HashSet<Transform> hs = new HashSet<Transform>();
        for( int i = 0 ; i < transform.childCount ; i++ )
        {
            Transform tr = transform.GetChild(i);
            hs.Add(tr);
        }

        foreach( Transform s in hs )
        {
            s.parent = null;
            s.gameObject.SetActive(show);
        }
        return hs;
    }
}
