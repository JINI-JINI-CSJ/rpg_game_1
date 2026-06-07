using UnityEngine;

// 3디 액션의 카메라 , 엘든링 같은 캠
// 기본 움직임
public class SJ_Base3DActionCamObj : MonoBehaviour
{
    // 필수 캐릭터 또는 캐릭터의 높이 고정된 객체
    public GameObject   go_FallowPos;
    public float        FallowPosLerp = 5.0f; // 위치 따라 다니는 감도
   
    // 타겟
    public GameObject   go_TargetView;
    public float        TargetRotLerp = 5.0f; // 회전 바라보는 감도도

    // 이 자체 객체 회전 감도
    public float        RotSensitivity = 5;


    // 이 객체가 바라보는 방향으로 회전할 객체 , 보통 캐릭터
    public GameObject   go_Fallow_Rot;

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Update_Prc()
    {
        transform.position = Vector3.Lerp( transform.position , go_FallowPos.transform.position , Time.deltaTime * FallowPosLerp );

        if( go_TargetView != null )
        {         
            Rot_Target( gameObject , go_TargetView.transform.position , TargetRotLerp );
        }
    }

    public void ObjFallow_RotFwd()
    {
        Rot_Target( go_Fallow_Rot , go_FallowPos.transform.position + transform.forward , TargetRotLerp , true );
    }

    public void RotInit()
    {
        transform.rotation = Quaternion.identity;
    }

    // 고정 기준 방향을 카메라 회전 방향으로 전환하여 회전
    // y 는 0 으로 전달된다.
    public void ObjFallow_RotDir( Vector3 dir )
    {
        Vector3 fwd = transform.forward;
        fwd.y = 0;
        Quaternion qt_fwd = Quaternion.LookRotation( fwd );

        dir = qt_fwd * dir;
        Rot_Target( go_Fallow_Rot , go_FallowPos.transform.position + dir , TargetRotLerp , true );
    }

    static public void Rot_Target( GameObject go_self  , Vector3 v_target , float lerp , bool y_zero = false )
    {
        Vector3 targetPos = v_target;
        Vector3 SelfPos = go_self.transform.position;
        if( y_zero )
        {
            targetPos.y = SelfPos.y = 0;
        }
        Quaternion targetRotation = Quaternion.LookRotation( targetPos - SelfPos );
        go_self.transform.rotation = Quaternion.Slerp( go_self.transform.rotation, targetRotation, Time.deltaTime * lerp );   
    }



    public void RotObj(Vector2 input)
    {
        // 본체 좌우 회전
        transform.Rotate(new Vector3(0 , input.x ,0 ) * RotSensitivity * Time.deltaTime, Space.World);
    }
}
