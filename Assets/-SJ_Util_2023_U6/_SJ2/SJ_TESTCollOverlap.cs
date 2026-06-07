using System.Collections.Generic;
using UnityEngine;

// 원하는 트랜스폼에 오버랩 콜리더를 체크한다.
public class SJ_TESTCollOverlap : MonoBehaviour
{
    static public SJ_TESTCollOverlap G;

    public GameObject go_test;
    public SphereCollider sphereCollider;
    public BoxCollider boxCollider;

    public List<string> layerName_boundary;

    void Awake()
    {
        G = this;       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public bool CheckOverLap( Vector3 pos , Quaternion rot , GameObject go_prf )
    {
        if( G == null )
        {
            G = GameObject.FindAnyObjectByType<SJ_TESTCollOverlap>();
            if( G == null )
            {
                Debug.LogError( "객체 없다. SJ_TESTCollOverlap" );
                return false;
            }
        }
        return G._CheckOverLap( pos , rot , go_prf );
    }

    // 충돌 체크 안될때

    // 레딧 참조
//     CharacterController가 움직임을 처리하는 방식과 관련 있는 것 같아. 몇 가지 해결책이 있는 이걸 찾았어. CharacterController를 일시적으로 비활성화하거나

// Edit > Project Settings > Physics로 가서 "Auto Sync Transforms" 상자를 체크해 봐.

    public bool _CheckOverLap( Vector3 pos , Quaternion rot , GameObject go_prf )
    {
        // if( go_test == null )
        // {
            
        //     sphereCollider = go_test.AddComponent<SphereCollider>();
        //     boxCollider = go_test.AddComponent<BoxCollider>();
        //     go_test.SetActive(true);
        // }

        // go_test.layer = go_prf.layer;
        // go_test.transform.position = pos;
        // go_test.transform.rotation = rot;

        // SphereCollider sp = go_prf.GetComponent<SphereCollider>();
        // BoxCollider box = go_prf.GetComponent<BoxCollider>();

        // if( sp != null && sp.enabled == true )
        // {
        //     sphereCollider.enabled = true;
        //     boxCollider.enabled = false;

        //     sphereCollider.center = sp.center;
        //     sphereCollider.radius = sp.radius;

        // }else if( box != null && box.enabled == true )
        // {
        //     sphereCollider.enabled = false;
        //     boxCollider.enabled = true;

        //     boxCollider.center = box.center;
        //     boxCollider.size = box.size;
        // }
        // else
        // {
        //     Debug.LogError( "_CheckOverLap : 콜리더 없음 : " + go_prf.name );
        //     return false;
        // }
        // return SJ_Cood.OverlapBOOL(go_test, layerName_boundary , true);



        SphereCollider sp = go_prf.GetComponent<SphereCollider>();
        BoxCollider box = go_prf.GetComponent<BoxCollider>();

        Collider[] colls = null;
        if( sp != null && sp.enabled == true )
        {
            colls = SJ_Cood.OverLapSphereCollider( sp.radius , pos , layerName_boundary );
        }
        else if( box != null && box.enabled == true )
        {
            Vector3 bos_center_rot = rot * box.center;
            Vector3 pos_box = pos + bos_center_rot;
            Vector3 half_size = box.size * 0.5f;

//            Debug.Log( "half_size : " + half_size );

            colls = SJ_Cood.OverLapBoxCollider( pos_box , half_size , rot ,layerName_boundary  );
        }
        else
        {
            Debug.LogError( "_CheckOverLap : 콜리더 없음 : " + go_prf.name );
            return false;
        }

        if( colls.Length > 0 )
        {
            // foreach( var s in colls )
            // {
            //     Debug.Log( "OVER_COLLS : " + s.gameObject.name );
            // }
            return true;
        } 
        return false;
    }

    static public void ClearGO()
    {
        // if( G != null )
        // {
        //     if( G.go_test != null )
        //     {
        //         GameObject.DestroyImmediate(G.go_test);
        //     }
        // }
    }
}
