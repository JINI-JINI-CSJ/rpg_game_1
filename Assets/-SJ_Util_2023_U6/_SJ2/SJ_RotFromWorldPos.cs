using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 월드 위치를 현재 체크할 위치와 방향 계산
// 타겟위치를 방향으로 표시하기 위한거
// Y 축 윗 방향 고정
public class SJ_RotFromWorldPos : MonoBehaviour
{
    public Transform tr_PosBase; // 위치 기준 (캐릭터)
    public Transform tr_RotBase; // 방향 기준 (카메라)

    public Quaternion result_rot;//계산된 각도
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalcDir( Vector3 pos_w )
    {
        if( tr_PosBase == null || tr_RotBase == null ) return;
        Vector3 pos_s = tr_PosBase.position;

        pos_s.y = 0;
        pos_w.y = 0;

        Vector3 dir = pos_w - pos_s;
        dir.Normalize();
        Quaternion rot_dir_w = Quaternion.Inverse( Quaternion.LookRotation( dir ) );

        // 카메라로 인버스
        Vector3 dir_rot_base = tr_RotBase.forward;
        dir_rot_base.y = 0;
        Quaternion rot_obj_inv =  Quaternion.LookRotation( dir_rot_base );
        result_rot = rot_dir_w * rot_obj_inv;
    }
}
