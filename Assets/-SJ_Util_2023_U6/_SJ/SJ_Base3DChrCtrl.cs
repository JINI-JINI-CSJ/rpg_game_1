using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class _SJ_LERP_POS
{
    public Vector3 PosInput;
    public Vector3 PosBlend;
    public float   blend = 5;

    public void UpdateTime( float elapse )
    {
        PosBlend = Vector3.Lerp(PosBlend, PosInput, blend * elapse);
    }

    public void ZeroPos()
    {
        PosInput = PosBlend = Vector3.zero;
    }
}

public class SJ_Base3DChrCtrl : MonoBehaviour
{
    public	float			gravity = -9.8f;
    public float            XZ_Force_decrease = 10;
    public float move_speed = 2.0f;
    public float RotSensitivity = 1.0f;
    public float jump_pow = 50;
    public float pitch = 0f;  // 수직 회전 값 저장
    public float minPitch = -45f;  // 아래로 볼 수 있는 최대 각도
    public float maxPitch = 60f;   // 위로 볼 수 있는 최대 각도
    public CharacterController chr_ctrl;
    public Transform tr_cam_X_Rot;
    public Transform tr_MoveBase; // 무빙 기준 방향 
    public bool updateFunc = true;
    public bool use_newInputAction = false;
    public bool apply_MoveKey = true;
    public bool inputAble = true;
    public bool Able_MouseLock = true; // 마우스 잠김 상태만 움직임 가능
    public bool Able_Mouse_X_Pitch = true; // 마우스 X축 회전 가능 여부
    public Vector3 velocity_Y;
    public Vector3 velocity_XZ;

    public _SJ_LERP_POS Lerp_Move = new _SJ_LERP_POS();
    public _SJ_LERP_POS Lerp_Rot = new _SJ_LERP_POS();

    public _SJ_LERP_POS Lerp_MoveWorld = new _SJ_LERP_POS();

    public bool Recent_Grounded = true; // 최근 바닥에 닿았는지 여부    
    public SJ_MONO_FUNC func_Recent_Grounded = new SJ_MONO_FUNC();
    public BoxCollider boxColl_GroundCheck; // 바닥 체크용 박스 콜라이더

    public bool No_Gravity;

    public List<string> lt_GroundCheck_LayerName;

    [HideInInspector]
    public Vector3 ChrFwdBase_Dir;    // 현재 움직임 방향 캐릭터 포워드 기준 , 속도도 적용되 있다.
    public bool useWorldInput;

    public Vector3 DEBUG_misc;
    public Vector3 DEBUG_inputVector;
    public Vector3 DEBUG_ChrFwdBase_Dir;

    public bool DEBUG_BREAK;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Active( bool b )
    {
        enabled = b;
        chr_ctrl.enabled = b;
    }

    // Update is called once per frame
    void Update()
    {
        if( updateFunc )
        {
            Update_CtrlPrc();
        }
        
    }

    public void Update_CtrlPrc()
    {
//        GF_PlayerMachine.DEBUG_LOG_POS( "SJ_Base3DChrCtrl Update_CtrlPrc 1" );
        if( inputAble )
        {     
            Input_Ctrl();
        }

        Lerp_Move.UpdateTime( Time.deltaTime );
        Lerp_Rot.UpdateTime( Time.deltaTime );
        Lerp_MoveWorld.UpdateTime( Time.deltaTime );

//        GF_PlayerMachine.DEBUG_LOG_POS( "SJ_Base3DChrCtrl Update_CtrlPrc 2" );
    }

    public void Clear_Input()
    {

    }

    public Transform CurTrans()
    {
        if(tr_MoveBase != null)return tr_MoveBase;
        return transform;
    }

    public void Input_Ctrl()
    {
        Vector3 inputVector = new Vector3();

        Transform tr = CurTrans();

        if( apply_MoveKey )
        {
            if( useWorldInput )
            {
                inputVector = Lerp_MoveWorld.PosBlend * move_speed;

                ChrFwdBase_Dir =  Quaternion.Inverse( tr.rotation ) * inputVector;

                DEBUG_inputVector = Lerp_MoveWorld.PosInput;
                DEBUG_ChrFwdBase_Dir = ChrFwdBase_Dir;
                DEBUG_misc = inputVector;
            }else{
                ChrFwdBase_Dir = Lerp_Move.PosBlend * move_speed;
                inputVector = Quaternion.Euler( 0 , tr.eulerAngles.y , 0 ) * ChrFwdBase_Dir;
            }
        }
        Vector3 v_total = (inputVector  + velocity_Y + velocity_XZ) * Time.deltaTime;
        chr_ctrl.Move( v_total  );

        // 바닥 체크
        if( boxColl_GroundCheck != null )
        {
            // 올라가는 중에는 체크 안하기
            if( velocity_Y.y > 0.001f )
            {
                Recent_Grounded = false;
            }else{            
                Check_Ground();
            }
        }

        if( Recent_Grounded && No_Gravity == false )velocity_Y = Vector3.zero; // 바닥에 닿았을 때 Y축 속도 초기화
    }

    public bool Check_Ground()
    {
        bool temp = Recent_Grounded;

        // 바닥 체크용 박스 콜라이더의 위치와 크기를 사용하여 바닥 체크

        Collider[] colliders = null;
        if( lt_GroundCheck_LayerName.Count > 0 )
        {
            int layerMask = LayerMask.GetMask(lt_GroundCheck_LayerName.ToArray());
            colliders = Physics.OverlapBox( boxColl_GroundCheck.transform.position, boxColl_GroundCheck.size / 2 , Quaternion.identity , layerMask );
        }else{
            colliders = Physics.OverlapBox( boxColl_GroundCheck.transform.position, boxColl_GroundCheck.size / 2 , Quaternion.identity );
        }   

        if( colliders.Length > 0 )
        {
            Recent_Grounded = true;
        }else{
            Recent_Grounded = false;
        }

        // 공중에서 착지 했을때만
        if( Recent_Grounded && temp != Recent_Grounded )
        {
            return true;
        }
        return false;
    }

    
    private void FixedUpdate() 
    {
        if( updateFunc == false ) return;

        if( Recent_Grounded == false && No_Gravity == false )
        {
            velocity_Y += new Vector3(0,gravity,0)* Time.fixedDeltaTime;
            if( DEBUG_BREAK )Debug.Log( "1" );
        }else{
            //velocity_Y = Vector3.zero; // 바닥에 닿았을 때 Y축 속도 초기화  
            if( DEBUG_BREAK )Debug.Log( "2" );
        }

        // 수평 힘 감소
        if( velocity_XZ.magnitude > 0.01f )
        {
            Vector3 src = velocity_XZ;

            Vector3 v_Opp = velocity_XZ * -1;
            v_Opp.Normalize();
            v_Opp *= XZ_Force_decrease* Time.fixedDeltaTime;
            velocity_XZ += v_Opp;

            // 방향이 반전 된건지
            if( ( src.x > 0 && velocity_XZ.x < 0) ||
                ( src.x < 0 && velocity_XZ.x > 0) ||
                ( src.y > 0 && velocity_XZ.y < 0) ||
                ( src.y < 0 && velocity_XZ.y > 0) )
             {
                velocity_XZ = Vector3.zero;
             }
        }
    }

    public void MoveDir( Vector3 dir = default )
    {   
        //Debug.Log( "MoveDir : " + dir );
        Lerp_Move.PosInput = dir;
    }

    public void MoveDirWorld( Vector3 dir = default )
    {
        //Debug.Log( "MoveDirWorld : " + dir );
        Lerp_MoveWorld.PosInput = dir;
    }

    public void JumpPow()
    {
        velocity_Y = new Vector3( 0, jump_pow , 0 );
        Recent_Grounded = false;
    }

    public void RisePow( float f )
    {
        velocity_Y = new Vector3( 0, f , 0 );
        Recent_Grounded = false;
    }

    public void XZ_Pow( Vector3 dir , float Force_decrease = -1 )
    {
        //Debug.Log( "XZ_Pow : " + dir );
        velocity_XZ = dir;
        if( Force_decrease > 0 ) XZ_Force_decrease = Force_decrease;
    }

    public void RotObj(Vector2 input)
    {
        // 본체 좌우 회전
        CurTrans().Rotate(new Vector3(0 , input.x ,0 ) * RotSensitivity * Time.deltaTime, Space.World);
    }

    // 본인 바라보는 기준으로 즉각 방향 전환
    public void RotDirect( Vector3 dir )
    {
        dir = CurTrans().rotation * dir;
        CurTrans().LookAt( dir );
    }

    public void Zero_Pow()
    {
        velocity_Y = Vector3.zero;
        velocity_XZ = Vector3.zero;
    }

    public void ZeroMoveRot()
    {
        Lerp_Move.ZeroPos();
        Lerp_MoveWorld.ZeroPos();
        Lerp_Rot.ZeroPos();
    }

}

