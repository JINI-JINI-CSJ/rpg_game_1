using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_GridPos : MonoBehaviour
{

    public  float   w_size = 2;
    public  float   h_size = 2;
    public  bool    Y_Z = true; // false y , true  z
    public  int     fixed_w_column = 5;

    public  bool    local_pos = true;

    public  Dictionary<Vector2Int,GameObject>   dic_coord = new Dictionary<Vector2Int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("재배치")]
    public void     AlignChild_Menu()
    {
        AlignChild();
    }

    public  void    AlignChild( bool    inc_hide_child = true )
    {
        dic_coord.Clear();

        List<Transform> lt_tr = new List<Transform>();
        for( int i = 0; i < transform.childCount ; i++ )
        {
            Transform th_c = transform.GetChild(i);
            if( inc_hide_child == false )
            {
                if( th_c.gameObject.activeSelf )
                    lt_tr.Add( th_c );
            }else{
                lt_tr.Add( th_c );
            }
        }

        int x = 0;
        int y = 0;
        float fx = 0;
        float fy = 0;
        foreach( Transform s in lt_tr )
        {
            fx = x * w_size;
            fy = y * h_size;
            Vector3 pos = new Vector3( fx , fy , 0 );
            if( Y_Z )
            {
                pos.y = 0;
                pos.z = fy;
            }

            if( local_pos )
            {
                s.localPosition = pos;
            }else{
                s.position = pos;
            }

            SJ_GridPosObj sJ_GridPosObj = s.GetComponent<SJ_GridPosObj>();
            if( sJ_GridPosObj != null )
            {
                sJ_GridPosObj.pos_int = new Vector2Int( x , y );
                SJ_Unity.SendMsg( s.gameObject , "OnSJGrid_PosInit" );
            }

            Vector2Int pos_int = new Vector2Int(x,y);
            dic_coord[pos_int] = s.gameObject;

            ++x;
            if( x >= fixed_w_column )
            {
                x = 0;
                ++y;
            }
        }
    }


    public  void    NewInstObj( int x_count , int y_count , GameObject prefab , bool use_x_column = true )
    {
        if( use_x_column )
        {
            fixed_w_column = x_count;
        }

        SJ_Unity.Delete_Child( transform );
        int t = x_count * y_count;
        for( int i = 0 ; i < t ;i++ )
        {
            GameObject inst = GameObject.Instantiate(prefab);
            inst.SetActive(true);
            inst.transform.parent = transform;
        }
        AlignChild();
    }

    public  GameObject  GetObj_PosInt( int x , int y )
    {
        Vector2Int v = new Vector2Int(x,y);
        GameObject go_pos = null;
        if( dic_coord.TryGetValue( v , out go_pos ) )
        {
            return go_pos;
        }
        return null;
    }
}
