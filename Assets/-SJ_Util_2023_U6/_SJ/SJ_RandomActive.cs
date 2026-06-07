using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_RandomActive : MonoBehaviour
{
    bool active;
    public  List<GameObject>    lt_go = new List<GameObject>();
    public  int                 Active_Num = 1;         // 명시적 보일 갯수
    public  float               Active_AllPer = 100.0f; // 목록마다 보일 확률 , 100 이면 전부 보임

    private void OnEnable() 
    {

    }

    [ContextMenu("랜덤 활성화")]
    public void     ShowActive_Menu()
    {
        ShowActive( true );
    }


    public void     ShowActive( bool exec = false )
    {
        if( active && exec == false ) return;
        //

        if( lt_go.Count < 1 )
        {
            for( int i = 0 ; i < transform.childCount ; i++ )
            {
                lt_go.Add( transform.GetChild(i).gameObject );
            }
        }

        if( lt_go.Count < 1 ) return;

        List<GameObject> lt = new List<GameObject>( lt_go );
        if( lt.Count < 1 )
        {
            for( int i = 0 ; i < transform.childCount ; i++ )
            {
                lt.Add( transform.GetChild(i).gameObject );
            }
        }
        
        if( lt.Count < 1 ) return;

        foreach( GameObject go in lt ) go.SetActive(false);


        int act_num = Active_Num;
        if( act_num < 1 )
        {
            act_num = (int)((float)lt.Count * (Active_AllPer*0.01f));
        }

        for( int i = 0 ; i < act_num ; i++ )
        {
            if( lt.Count < 1 ) return;
            GameObject go = SJ_Unity.GetRandom_Pop<GameObject>(lt);
            go.SetActive(true);            
        }
        active = true;        
    }

}
