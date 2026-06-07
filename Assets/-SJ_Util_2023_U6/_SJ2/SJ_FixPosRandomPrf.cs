using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 지정한 위치에 무작위 프리펩을 만든다. 
// 지정된 횟수만큼만..
// 한번 생성된 위치는 안한다.

public class SJ_FixPosRandomPrf : MonoBehaviour
{
    public List<Transform>  PosList;
    public Transform        tr_childList;
    public List<GameObject> prefabs;
    public int CreateNum;

    // 생성후 로테이션 없으면 안함
    public List<float> RotY; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Transform> MakePosList()
    {
        List<Transform> posList_pop = new List<Transform>();
        if( PosList.Count > 0 )
        {
            posList_pop.AddRange( PosList );
        }
        else if( tr_childList != null )
        {
            posList_pop.AddRange( SJ_Unity.GetChildList( tr_childList ) );
        }
        return posList_pop;
    }

    public void ClearInst()
    {
        List<Transform> posList_pop = MakePosList();
        foreach( var s in posList_pop )
        SJ_Unity.Delete_Child( s );
    }

    public void StartCreate()
    {
        ClearInst();

        List<Transform> posList_pop = MakePosList();
        for( int i = 0; i < CreateNum ; i++ )
        {
            if( posList_pop.Count < 1 )
            {
                return;
            }
            Transform tr_pos = SJ_Unity.GetRandom_Pop( posList_pop );
            GameObject prf = SJ_Unity.GetArray_Random( prefabs.ToArray() );
            Quaternion rot = Quaternion.identity;
            if( RotY.Count > 0 )
            {
                rot = Quaternion.Euler( 0, SJ_Unity.GetArray_Random( RotY.ToArray() ) , 0 );
            }
            GameObject inst = GameObject.Instantiate( prf );
            inst.transform.rotation = rot;
            SJ_Unity.SetEqTrans( inst.transform , null , tr_pos );
        }
    }

}
