using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class SJ_RandomBatchDuplDel : MonoBehaviour
{
    public enum _ROT_TYPE
    {
        None = 0 , 
        Y_360 ,     // y 축만 360 
        ALL_360 ,   // 모든 각도 360
        Y_RIGHT_ANG , // 직각 각도 , 90,180,270
    }

    public _ROT_TYPE    rot_type = _ROT_TYPE.Y_360;

    public GameObject go_prf_par;
    public List<GameObject> lt_prf;

    [System.Serializable]
    public class PRF_INF : RANDOM_RANGE_STEP_BASE
    {
        public GameObject prf;

        public PRF_INF CopyInst()
        {
            PRF_INF s = new PRF_INF();
            s.prf = prf;
            s.ratio = ratio;
            return s;
        }
    }
    public List<PRF_INF> prf_INFs;

    public enum _HEIGHT_TYPE
    {
        None = 0 ,
        Y_EQ_PAR , // 이 객체 
        RANGE_2 , // 2 범위
    }

    public _HEIGHT_TYPE height_type = _HEIGHT_TYPE.Y_EQ_PAR;
    public float range_y_min = 0;
    public float range_y_max = 0;
    public  int     num_count = 10;
    public  bool    noCheck_Dupl =true;
    public float   Check_Bound_Fix = 1.0f;

    [Header("갯수대로 생성")]
    public bool    COUNT_CREATE_MODE;

    [Header("생성 참조 콜라이더")]
    public Transform tr_AreaColl;

    [Header("생성 객체 부모")]
    public Transform tr_CreatePar;

    [Header("추가 체크 바운드")]
    public List<Transform> tr_CheckBound;

    public bool check_OverLapCollider;

    public List<string> layerName_boundary;

    public int MAX_TRY = 5000;

    public bool GroundFitRayCheck;
    public string layer_Ground;
    public float groundRayHeight = 100;

    public bool DEBUG_LOG;

    bool    init;    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if( CREATE_EXEC )
        // {
        //     CREATE_EXEC = false;
        //     CreateObj(true);
        // }
    }

    [ContextMenu("생성 재배치")]

    public void     CreateObj_Menu()
    {
        CreateObj(true);
    }

    [ContextMenu("일괄 생성 확률")]
    public void AllPrf_Per()
    {
        foreach( var s in prf_INFs ) s.ratio = 100;
    }

    public Transform Clear()
    {
        Transform tr_create_par = tr_CreatePar;
        if (tr_create_par == null) tr_create_par = transform;
        SJ_Unity.Delete_Child(tr_create_par);
        return tr_create_par;
    }

    public  void    CreateObj( bool exec = false , bool clear = true )
    {
        if( init && exec == false ) return;

        //Debug.Log( "생성 랜덤 오브젝트~~~ : " + gameObject.name );

        init = true;

        Transform tr_area_coll = tr_AreaColl;
        if( tr_area_coll == null ) tr_area_coll = transform;
        
        Transform tr_create_par = tr_CreatePar;
        if( tr_create_par == null ) tr_create_par = transform;
        if (clear)
        {
            SJ_Unity.Delete_Child(tr_create_par);
        }

        SphereCollider  sphereCollider = tr_area_coll.GetComponent<SphereCollider>();
        BoxCollider     boxCollider = tr_area_coll.GetComponent<BoxCollider>();

        if( sphereCollider == null && boxCollider == null )
        {
            Debug.LogError( "없음!!!  Collider : " + tr_area_coll.name );
            return;
        }

        int loop = 0;
        int c = 0;

        List<PRF_INF> lt_prf_INF = new List<PRF_INF>();
        
        if( prf_INFs.Count > 0 )
        {
            foreach( var s in prf_INFs )
            {
                lt_prf_INF.Add( s.CopyInst() );
            }
      
        }else{

            if (go_prf_par != null)
            {
                List<Transform> tr_ch = SJ_Unity.GetChildList(go_prf_par.transform);
                foreach (var s in tr_ch)
                {
                    // 없으면 일괄 확률로...
                    PRF_INF inf = new PRF_INF();
                    inf.ratio = 100;
                    inf.prf = s.gameObject;
                    lt_prf_INF.Add(inf);
                }
            }
            else
            {
                foreach (var s in lt_prf)
                {
                    // 없으면 일괄 확률로...
                    PRF_INF inf = new PRF_INF();
                    inf.ratio = 100;
                    inf.prf = s;
                    lt_prf_INF.Add(inf);
                }
            }



        }

        if( lt_prf_INF.Count < 1 )
        {
            Debug.Log( "생성 객체 프리펩 없다.~~~~~~~~~~~~~~~~~~~" );
            return;
        }

        Random_RangeStepList.SetList( lt_prf_INF );      

        while(true)
        {            
            loop++;
            if( loop > MAX_TRY )
            {
                if( DEBUG_LOG )
                    Debug.Log( gameObject.name + " 너무 많음!!!!~~~~~ " );
                break;                
            }            

            PRF_INF p_inf = null;

            if( COUNT_CREATE_MODE )
            {
                // ratio 없으면 

                foreach( var s in lt_prf_INF )
                {
                    if( s.ratio > 0 )
                    {
                        p_inf = s;
                        break;
                    }
                } 

                if( p_inf == null )
                {
                    break;
                }

                //p_inf.ratio--;

            }else{
                p_inf = Random_RangeStepList.GetRandom<PRF_INF>();                
            }

            if( p_inf == null )
            {
                Debug.LogError( "무작위 객체 에러!!!" );
                break;
            }
            GameObject prf = p_inf.prf;
            if( prf == null )
            {
                Debug.LogError( "프리펩 없음!!!! " + gameObject.name );
                break;  
            }

            //GameObject inst = GameObject.Instantiate(prf);
            Vector3 pos_inst = Vector3.zero;
            if( sphereCollider != null )
            {
                pos_inst = SJ_Cood.Random_SphereBound( sphereCollider );
            }
            if( boxCollider != null )
            {
                pos_inst = SJ_Cood.Random_BoxBound(boxCollider);
            }

            switch( height_type )
            {
                case _HEIGHT_TYPE.Y_EQ_PAR:
                    pos_inst.y = transform.position.y;
                break;

                case _HEIGHT_TYPE.RANGE_2:
                    pos_inst.y = UnityEngine.Random.Range( range_y_min , range_y_max );
                break;
            }

            if (GroundFitRayCheck)
            {
                RaycastHit hit;

                Vector3 pos_origin = pos_inst;
                pos_origin.y += groundRayHeight;

                if (SJ_Cood.RayCastLine(pos_origin, Vector3.down, out hit, layer_Ground))
                {
                    pos_inst = hit.point;
                }
                else
                {
                    if( DEBUG_LOG )
                        Debug.Log("주의!!! 그라운드 레이캐스트 없다!!!! : " + layer_Ground);
                    return;
                }
            }

            // inst.transform.position = pos_inst;
            // inst.transform.rotation = GetAngle_ByType( rot_type );
            // inst.SetActive(true);

            Quaternion rot_inst = GetAngle_ByType( rot_type );

            if( noCheck_Dupl == false && SJ_TESTCollOverlap.CheckOverLap( pos_inst , rot_inst , prf ) )
            {
                // if( DEBUG_LOG )
                //     Debug.Log( "겹침~~ : " + inst.gameObject );
                // GameObject.DestroyImmediate(inst);

//                Debug.Log( "겹침~~ : " + prf );
            }else{
                c++;
                if( COUNT_CREATE_MODE ) p_inf.ratio--;
                
                GameObject inst = GameObject.Instantiate(prf);

                //inst.transform.position = pos_inst;
                //inst.transform.rotation = rot_inst;
                inst.transform.parent = tr_create_par;   
                inst.transform.SetPositionAndRotation( pos_inst , rot_inst );
                inst.SetActive(true);

                

//                Debug.Log( "생성!!!! : " + gameObject.name + " : " + prf );
            }

            if( COUNT_CREATE_MODE == false )
                if( c >= num_count ) break;

        }

       // Debug.Log( "생성 " + gameObject.name + " loop:" + loop + "     succ:" + c );
    }

    static public Quaternion GetAngle_ByType( _ROT_TYPE type )
    {
        switch(type )
        {
            case _ROT_TYPE.Y_360:
            {
                return Quaternion.Euler( 0 , UnityEngine.Random.Range(0.0f,360.0f) , 0 );
            }

            case _ROT_TYPE.Y_RIGHT_ANG:
            {
                float ang = SJ_Unity.GetArray_Random( new float[] {0.0f,90.0f,180.0f,270.0f} );
                return Quaternion.Euler( 0 , ang , 0 );
            }

            case _ROT_TYPE.ALL_360:
            {
                return Quaternion.Euler( UnityEngine.Random.Range(0.0f,360.0f) , UnityEngine.Random.Range(0.0f,360.0f) , UnityEngine.Random.Range(0.0f,360.0f) );
            }

            default:
                return Quaternion.identity;
        }
    }

    // public bool CheckCollBound(GameObject go, Vector3 pos , Quaternion rot)
    // {


    //     // if (check_OverLapCollider)
    //     // {
    //     //     if (SJ_Cood.OverlapBOOL(go, layerName_boundary)) return true;
    //     // }
    //     // else
    //     // {
    //     //     List<Transform> tr_check = new List<Transform>();
    //     //     if (tr_AreaColl != null) tr_check.Add(tr_AreaColl);
    //     //     if (tr_CheckBound.Count > 0) tr_check.AddRange(tr_CheckBound);
    //     //     if (tr_check.Count < 1) tr_check.Add(transform);            
    //     //     foreach (var tr in tr_check)
    //     //     {
    //     //         for (int i = 0; i < tr.childCount; i++)
    //     //         {
    //     //             Transform tr_c = tr.GetChild(i);
    //     //             if (tr_c.gameObject == go) continue;

    //     //             if (ObjectOverlapChecker.CheckBoundsOverlap(go, tr_c.gameObject))
    //     //                 return true;
    //     //         }
    //     //     }
    //     // }
    //     // return false;
    // }

    public  bool    Check_Bound( Bounds a , Bounds b )
    {
        Vector3 v_distance = a.center - b.center;
        Vector3 v_total = ((a.size + b.size) / 4) * Check_Bound_Fix;
        if( v_distance.sqrMagnitude < v_total.sqrMagnitude )
        {
            return true;
        }
        return false;
    }

}
