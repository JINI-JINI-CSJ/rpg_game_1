using UnityEngine;

// 타겟 객체를 지정한 객체의 위치,회전으로 지정된 시간만큼 러프하게 따라오게 한다.
// 지정한 객체가 없으면 현재 이 객체로 지정
// 위치,회전,크기는 로컬 트랜스 기준이다.

// 지정된 시간이 지나면 컴포넌트는 비활성화 한다.
public class SJ_PosLerpObj : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Update_Lerp();
    }

    public Transform tr_Target;
    public Transform tr_MoveObj;
    public float PlayTime = 0.1f;

    float startTime;

    public Vector3 offset;
    public bool usePos = true;
    public bool useRot = true;
    //public bool useScl = false;

    public void StartPlay(Transform _tr_move = null)
    {
        if (_tr_move != null) tr_MoveObj = _tr_move;
        if (tr_MoveObj == null)
        {
            Debug.LogError("Error! : SJ_PosLerpObj : StartPlay : tr_Target == null");
            return;
        }

        if (tr_Target == null) tr_Target = transform;

        startTime = Time.time;
        enabled = true;
        Update_Lerp();
    }

    // 특수 상황 함수
    // 1. 제3인자 객체의 트랜스 정보를 tr_SrcObj 정보로 복사
    // 2. 제3인자를 tr_SrcObj 의 자식으로 설정
    // 3. StartPlay 함수 호출

    // 현재 컷신 캐릭터 위치 보정용

    // 컷신일때 하이어라키 구조
    // 모노 SJ_PosLerpObj 객체 , tr_Target , 본인
    //   - tr_MoveObj
    //       - tr_attach (캐릭터 모델 루트)

    public void StartPlay_Attach(Transform tr_attach, Transform _tr_move = null)
    {
        if (tr_attach == null)
        {
            Debug.LogError("Error! : SJ_PosLerpObj : StartPlay_Attach : tr_target == null || tr_attach == null");
            return;
        }
        if (tr_Target == null) tr_Target = transform;

        tr_MoveObj.position = tr_attach.position;
        tr_MoveObj.rotation = tr_attach.rotation;
        //tr_MoveObj.localScale = tr_attach.localScale;

        tr_attach.parent = tr_MoveObj;

        StartPlay(_tr_move);
    }

    public void Update_Lerp()
    {
        if (tr_MoveObj == null || tr_Target == null)
        {
            enabled = false;
            return;
        }

        float t = (Time.time - startTime) / PlayTime;

        if (t > 1)
        {
            t = 1;
            enabled = false;
        }

        if (usePos) tr_MoveObj.position = Vector3.Lerp(tr_MoveObj.position, tr_Target.position + offset, t);
        if (useRot) tr_MoveObj.rotation = Quaternion.Slerp(tr_MoveObj.rotation, tr_Target.rotation, t);
        //if (useScl) tr_MoveObj.localScale = Vector3.Lerp(tr_MoveObj.localScale, tr_Target.localScale, t);
    }
}
