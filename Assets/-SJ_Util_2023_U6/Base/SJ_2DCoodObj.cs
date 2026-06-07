using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_2DCoodObj : MonoBehaviour
{
    public  Vector2     Cood2D;
    bool    setcood;

    // Start is called before the first frame update
    void Start()
    {
        UpdateFromTrans();
    }

    public  void    UpdateFromTrans()
    {
        Cood2D = SJ_2DRoot.TransToCood( transform );
    }

    public  void    UpdateCood()
    {
        if( setcood )
        {
            transform.localPosition = SJ_2DRoot.CoodToTrans( Cood2D.x ,Cood2D.y );
            setcood = false;
        }
    }

    public  void    SetCood2D( float x , float y )
    {
        Cood2D.x = x;
        Cood2D.y = y;
        setcood = true;
    }

    public  void    AddCood2D( float x , float y )
    {
        Cood2D.x += x;
        Cood2D.y += y;
        setcood = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCood();
    }
}
