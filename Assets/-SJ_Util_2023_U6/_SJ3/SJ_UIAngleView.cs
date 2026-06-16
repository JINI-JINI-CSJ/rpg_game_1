using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유아이 방향 표시
// 피격등등
public class SJ_UIAngleView : MonoBehaviour
{
    // 기준 객체 , 보통 카메라
    public Transform tr_LinkMain;

    public Transform tr_Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRot();
    }

    public void UpdateRot()
    {
        if( tr_LinkMain == null || tr_Target == null ) return;

        // x,z 평면 기준으로 계산.
        Vector3 v_main = tr_LinkMain.forward;
        v_main.y = 0;
        v_main.Normalize();

        Vector3 v_tar = tr_Target.position - tr_LinkMain.position;
        v_tar.y = 0;
        v_tar.Normalize();

        float ang = SJ_Cood.GetAngle(v_tar  , v_main );

        //Debug.Log( v_main + " : " +  v_tar + " : " + ang );

        transform.localRotation = Quaternion.Euler( 0 , 0 , ang );
    }
}
