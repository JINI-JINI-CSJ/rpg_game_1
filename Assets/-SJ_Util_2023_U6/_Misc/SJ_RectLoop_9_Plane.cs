using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 9개의 객체로 기준점 기준으로 바둑판처럼 배치한다.
 0 점이 객체의 중심 
*/
public class SJ_RectLoop_9_Plane : MonoBehaviour
{
    public  GameObject  go_center;

    public  int     width;  //x
    public  int     height; //y
    public  int     length; //z

    public  List<GameObject>    go_maked = new List<GameObject>();

    public  int     cood_x;
    public  int     cood_y;
    public  int     cood_z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void    MakeObj()
    {
        if( go_maked.Count > 0 )
        {
            Debug.LogError( "SJ_RectLoop_9_Plane : 이미 객체 생성 : " + gameObject.name );
            return;
        }

        for( int i=0; i<8 ;i++ )
        {
            GameObject inst = GameObject.Instantiate( go_center );
            go_maked.Add(inst);
            inst.transform.parent = transform;
//            Debug.LogError( "SJ_RectLoop_9_Plane : " + i );
        }

        

        go_maked[0].transform.localPosition = GetPosByCood(-1,-1, 0);
        go_maked[1].transform.localPosition = GetPosByCood( 0,-1, 0);
        go_maked[2].transform.localPosition = GetPosByCood( 1,-1, 0);
        go_maked[3].transform.localPosition = GetPosByCood(-1, 0, 0);
        go_maked[4].transform.localPosition = GetPosByCood( 1, 0, 0);
        go_maked[5].transform.localPosition = GetPosByCood(-1, 1, 0);
        go_maked[6].transform.localPosition = GetPosByCood( 0, 1, 0);
        go_maked[7].transform.localPosition = GetPosByCood( 1, 1, 0);

    }

    public  void    CalcCood( Vector3 v ) 
    {
        CalcCood( v.x , v.y , v.z );
    }

    public  void    CalcCood( float x , float  y , float z )
    {
        cood_x = ((int)(x) + (width/2)) / width;

        if( y > 0 )
        {
            y = y + height/2;
        }else{
            y = y - height/2;
        }

        cood_y = (int)(y) / height;
        cood_z = ((int)(z) + (length/2)) / length;

        transform.localPosition = GetPosByCood( cood_x , cood_y , cood_z );

        // for( int cy = 0 ; cy < 3 ; cy++ )
        // {
        //     for( int cx = 0 ; cx < 3 ; cx++ ) BatchCood( cy*3+cx , cx,cy,0 );
        // }
    }

    public  Vector3 GetPosByCood(int cx , int cy , int cz)
    {
        return new Vector3( cx*width , cy*height , cz*length );
    }

    // public  void    BatchCood( int i , int cx , int cy , int cz )
    // {
    //     if( i >= go_maked.Count ) return;
    //     GameObject go = go_maked[i];
    //     go.transform.localPosition = new Vector3( cx*width , cy*height , cz*length );
    // }

}
