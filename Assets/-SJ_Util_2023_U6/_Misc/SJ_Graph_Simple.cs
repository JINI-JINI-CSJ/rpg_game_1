using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SJ_Graph_Simple : MonoBehaviour
{
    public  float   width_unit = 1f;

    int line_c = 0;
    public    LineRenderer lineRenderer;
    public  List<float>     lt_val = new List<float>();

    // public  float   auto_Scl_Max_W = 0;
    // public  float   auto_Scl_Max_H = 0;

    //public  float   add_height_Scl = 1.0f;

    private void Awake() 
    {
        //lineRenderer = GetComponent<LineRenderer>();
        //Init();
    }

    public  void    Init()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition( 0 , new Vector3(0,0,0) );     
//        Debug.Log( "Init :---  LineRenderer" );
    }

    // public  Vector3    AddVal( float height )
    // {
    //     //lineRenderer.positionCount = (lineRenderer.positionCount+1);
    //     Vector3 v = new Vector3( lineRenderer.positionCount*width_unit ,height * add_height_Scl ,0);
    //     //lineRenderer.SetPosition( lineRenderer.positionCount-1 , v );
    //     lineRenderer.SetPosition( lineRenderer.positionCount , v );
    //     return v;
    // }

    public  Vector3    AddVal( float x , float y )
    {
        lineRenderer.positionCount = (lineRenderer.positionCount+1);
        Vector3 v = new Vector3( x , y);
        lineRenderer.SetPosition( lineRenderer.positionCount-1 , v );
        return v;
    }

    public  int     GetPosCount()
    {
        return lineRenderer.positionCount;
    }

    public  Vector3 GetPos(int idx)
    {
        if( lineRenderer.positionCount <= idx ) return Vector3.zero;
        return lineRenderer.GetPosition( idx );
    }


    public  Vector3 GetLastPos()
    {
        if( lineRenderer.positionCount < 1 ) return Vector3.zero;
        return lineRenderer.GetPosition( lineRenderer.positionCount - 1 );
    }

    public  bool    GetLastLine( ref Vector3 v1 , ref Vector3 v2 )
    {
        if( lineRenderer.positionCount < 2 ) return false;

        int ids_1 = lineRenderer.positionCount - 2;
        int ids_2 = lineRenderer.positionCount - 1;

        v1 =    lineRenderer.GetPosition( ids_1 );
        v2 =    lineRenderer.GetPosition( ids_2 );
        return true;
    }

    // public  void    AutoScaleW( float w  )
    // {
    //     if( auto_Scl_Max_W < 1  ) return;

    //     Vector3 v = GetLastPos();

    //     float w_scl = auto_Scl_Max_W / v.x;

    //     Debug.Log( "AutoScaleW :  auto_Scl_Max_W : " + auto_Scl_Max_W + " : w : "  + w  + " : w_scl : " + w_scl );

    //     if( w_scl > 1 ) w_scl = 1;

    //     Vector3 scl = transform.localScale;
    //     scl.x = w_scl;
    //     transform.localScale = scl;
    // }

    // public  void    AutoScaleH( float h  )
    // {
    //     if( auto_Scl_Max_H < 1  ) return;

    //     Vector3 v = GetLastPos();

    //     float h_scl = auto_Scl_Max_H / v.y;
        
    //     Vector3 scl = transform.localScale;
    //     scl.y = h_scl;
    //     transform.localScale = scl;
    // }


}
