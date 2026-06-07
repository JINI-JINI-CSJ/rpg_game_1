using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프레임(시간) 에 객체 겹치지 않게 생성
public class SJ_RandomBatchFrame : MonoBehaviour
{
    // 생성 영역 콜리더들.
    public  SphereCollider  sphereCollider;
    public  BoxCollider     boxCollider;

    public  List<GameObject>    lt_prf;

    public  string   tag_name;

    public  int     max_Create = 5;

    public  bool    y_fix = true;

    int             cur_create;

    // 전역 설정
    // 모든 객체가 프레임에 나눠서 생성 (분산)
    public  float   Global_Time_Create_Repeat = 0.02f;
    public  int     Global_Count_Create = 2;

    static public   Dictionary<int,int> dic_Frame_Count = new Dictionary<int, int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void    CreateObj()
    {
        if( cur_create >= max_Create )
        {
            this.enabled = false;
            return;            
        }

        if( Check_CreateAble(this) == false ) return;

        CreateObj_Random();
    }   


    public  Vector3    GetRandomPos()
    {
        Vector3 pos = Vector3.zero;
        if( sphereCollider != null )
        {
            pos = SJ_Cood.Random_SphereBound( sphereCollider );
            if( y_fix ) pos.y = sphereCollider.transform.position.y;
        }
        if( boxCollider != null )
        {
            pos = SJ_Cood.Random_BoxBound(boxCollider);
            if( y_fix ) pos.y = boxCollider.transform.position.y;
        }
        return pos;
    }

    void    CreateObj_Random()
    {
        if( lt_prf.Count < 1 ) 
        {
            this.enabled = false;
            return;
        }

        GameObject prf = SJ_Unity.GetArray_Random( lt_prf.ToArray() );

        Vector3 pos = GetRandomPos();

        GameObject inst = GameObject.Instantiate( prf , pos , Quaternion.identity );
        inst.transform.parent = transform;

        // 객체프리펩은 구 콜리더만 체크
        SphereCollider coll = prf.GetComponent<SphereCollider>();

        if( coll == null )
        {
            cur_create++;
            return;
        }

        Vector3 center = coll.center + pos;
        int layer = 1 << LayerMask.NameToLayer(tag_name);
        Collider[] colls_other = Physics.OverlapSphere( center , coll.radius , layer );
        if( colls_other != null && colls_other.Length > 0 )
        {
            GameObject.DestroyImmediate(inst);
            return;
        }
        cur_create++;
    }

    static public   bool    Check_CreateAble( SJ_RandomBatchFrame mono )
    {
        int cur_frame = (int)(Time.time / mono.Global_Time_Create_Repeat);
        
        int cur_create = 0;
        if( dic_Frame_Count.TryGetValue( cur_frame , out cur_create ) )
        {
            if( cur_create > mono.Global_Count_Create )
            {
                return false;
            }
        }
        cur_create++;
        dic_Frame_Count[cur_frame] = cur_create;

        return true;
    }
}
