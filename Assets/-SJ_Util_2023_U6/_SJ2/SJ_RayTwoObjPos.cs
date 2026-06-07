using System.Collections.Generic;
using UnityEngine;


// 기준점에서 현재 객체까지의 길이


// 한개의 출발 위치 객체에서 현재 객체로 레이 
// 충돌 지점에 객체 위치

// 팁 6.2v : 유니티에서는 Camera.main 는  "Main Camera" 이름인 카메라 객체여야 한다.

public class SJ_RayTwoObjPos : MonoBehaviour
{
    public GameObject go_Src; // 원점 소스 객체
    public GameObject go_FixPos; // 보정할 객체( 이 객체의 원점 오프셋 )
    public float fix_dist_ray_cast = 0.02f; // 충돌지점에서 약간 앞

    public float sphereRadius = 0.2f;

    public float minDistance = 0.5f;

    public List<string> layers;

    public Vector3 posLocal_First;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posLocal_First = go_FixPos.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        PrcPos();
    }

    public void PrcPos()
    {
        if(Camera.main == null)
        {
            Debug.LogWarning("SJ_RayTwoObjPos : MainCamera 가 없습니다.");
            return;
        }

        Vector3 dir = transform.position - go_Src.transform.position;
        float len = dir.magnitude;

        dir.Normalize();


        float targetDist = len;
        int layerMask = LayerMask.GetMask(layers.ToArray());
        // SphereCast로 충돌 체크
        if (Physics.SphereCast(go_Src.transform.position, sphereRadius, dir, out RaycastHit hit, len, layerMask))
        {
            // 충돌 지점까지 거리에서 nearClipPlane만큼 앞당김
            targetDist = Mathf.Max(minDistance, hit.distance - sphereRadius - Camera.main.nearClipPlane);
        }

        // 최종 카메라 위치
        Vector3 finalPos = go_Src.transform.position + dir * targetDist;

        go_FixPos.transform.position = finalPos;

        //dir.Normalize();
        //Ray ray = new Ray(go_Src.transform.position , dir );
        //int layerMask = LayerMask.GetMask(layers.ToArray());

        //bool hit = false;
        //RaycastHit[] hit_inf;


        //         if (layers.Count == 0)
        //         {
        //             hit_inf = Physics.SphereCastAll(ray, sphereRadius, len);
        //         }
        //         else
        //         {
        //             hit_inf = Physics.SphereCastAll(ray, sphereRadius, len, layerMask);
        //         }

        //         if (hit_inf != null && hit_inf.Length > 0)
        //         {
        //             float dist = 9999999999;
        //             RaycastHit hit_near = new RaycastHit();
        //             foreach (var s in hit_inf)
        //             {
        //                 if (s.distance < dist)
        //                 {
        //                     dist = s.distance;
        //                     hit_near = s;
        //                 }
        //             }

        //             go_FixPos.transform.position = go_Src.transform.position + dir * hit_near.distance;
        // //            Debug.Log( "캠 레이 1 : " + hit_inf.collider.name );
        //         }
        //         else
        //         {

        // if (layers.Count == 0)
        // {
        //     hit_inf = Physics.RaycastAll(ray, len);
        // }
        // else
        // {
        //     hit_inf = Physics.RaycastAll(ray, len, layerMask);
        // }


        // bool hit = false;
        // RaycastHit hit_obj = default;
        // hit = SJ_Cood.RayCastLineLayers( go_Src.transform.position , dir , out hit_obj , layers , len );
        // if (hit)
        // {
        //     Vector3 pos_fix = hit_obj.point + hit_obj.normal * sphereRadius;
        //     go_FixPos.transform.position = pos_fix;

        // }
        // else
        // {
        //     // 이 객체의 월드 회전은 캐릭터와 같다.
        //     // 끝 점 기준으로 좌우로 레이 체크 , 길이는 반지름 만큼
        //     hit = SJ_Cood.RayCastLineLayers(transform.position, transform.right, out hit_obj, layers, sphereRadius);
        //     if (hit)
        //     {
        //         Vector3 pos_fix = hit_obj.point + hit_obj.normal * sphereRadius;
        //         go_FixPos.transform.position = pos_fix;
        //         return;
        //     }

        //     hit = SJ_Cood.RayCastLineLayers(transform.position, -transform.right, out hit_obj, layers, sphereRadius);
        //     if (hit)
        //     {
        //         Vector3 pos_fix = hit_obj.point + hit_obj.normal * sphereRadius;
        //         go_FixPos.transform.position = pos_fix;
        //         return;
        //     }

        //     go_FixPos.transform.localPosition = posLocal_First;
        // }


        //}


    }
}
