using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SJ_MeshRect : MonoBehaviour
{
    public  Vector3[]     pos_arr = new Vector3[4];

    MeshFilter meshFilter;

    private void Awake() 
    {
        meshFilter = GetComponent<MeshFilter>();

        CreateMesh();
    }

    public  void    SetPos_CreateMesh( Vector3 p1 , Vector3 p2 , Vector3 p3 , Vector3 p4 )
    {
        pos_arr[0] = p1;
        pos_arr[1] = p2;
        pos_arr[2] = p3;
        pos_arr[3] = p4;
        CreateMesh();
    }

    public  void    CreateMesh()
    {
        if( pos_arr.Length != 4 ) return;

        Mesh mesh = new Mesh();
        int[] ids = new int[6];
        ids[0] = 0;
        ids[1] = 1;
        ids[2] = 2;
        ids[3] = 1;
        ids[4] = 3;
        ids[5] = 2;
        mesh.vertices = pos_arr;
        mesh.triangles = ids;
        meshFilter.mesh = mesh;
    }


}
