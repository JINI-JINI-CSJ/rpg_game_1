using System.Collections;
using UnityEngine;

public class SJ_TransformFollower : MonoBehaviour
{
    [Header("설정")]
    public Transform target; // 따라갈 대상 객체
    public float duration = 2f; // 따라가는데 걸리는 시간

    public Transform moveObj; // 현재의 이 객체로 다른 대상을 따라오게한다.
    
    [Header("옵션")]
    public bool followPosition = true;
    public bool followRotation = true;
    public bool followScale = true;
    
    [Header("애니메이션 커브")]
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private bool isFollowing = false;
    
    Vector3 pos_tar;
    Quaternion rot_tar;
    Vector3 scl_tar;

    void Start()
    {
        // 시작시 자동으로 따라가기 시작하려면 주석 해제
        // StartFollowing();
    }
    
    /// <summary>
    /// 타겟을 따라가기 시작
    /// </summary>
    public void StartFollowing()
    {
        enabled = true;

        startTime = Time.time;

        if( moveObj == null )
        {
            startPos = transform.position;
            startRot = transform.rotation;
            startScale = transform.localScale;    

        }else{
            startPos = moveObj.position;
            startRot = moveObj.rotation;
            startScale = moveObj.localScale;              
        }

    }

    public void StartFollowingVal( Vector3 pos , Quaternion rot , Vector3 scl , bool use_pos , bool use_rot , bool use_scl )
    {
        enabled = true;
        startTime = Time.time;        

        startPos = transform.position;
        startRot = transform.rotation;
        startScale = transform.localScale;   


        pos_tar = pos;
        rot_tar = rot;
        scl_tar = scl;

        followPosition = use_pos;
        followRotation = use_rot;
        followScale = use_scl;

    }

    
    /// <summary>
    /// 타겟을 따라가기 중단
    /// </summary>
    public void StopFollowing()
    {
        StopAllCoroutines();
        isFollowing = false;
    }
    
    /// <summary>
    /// 새로운 타겟과 시간을 설정하고 따라가기 시작
    /// </summary>
    public void StartFollowing(Transform newTarget, float newDuration = -1)
    {
        target = newTarget;
        if( newDuration > 0 )
            duration = newDuration;
        StartFollowing();
    }

    Vector3 startPos;
    Quaternion startRot;
    Vector3 startScale;
    //bool play = false;
    float startTime;
    public void LateUpdate()
    {
        //return;


        if (target == null) return;

        float elapsedTime = Time.time - startTime;

        bool end_play = false;
        float progress = elapsedTime / duration;

        if( progress >= 1 )
        {
            end_play = true;
            progress = 1;
        }
// 애니메이션 커브 적용
        float easedProgress = easeCurve.Evaluate(progress);

        //        Debug.Log( "easedProgress : " + gameObject.name  + " : " +  easedProgress );

        LerpTrans( easedProgress );

        if (end_play)
        {
            enabled = false;
        }
    }

    public void LerpTrans( float easedProgress)
    {
        if( moveObj != null )
        {
            LerpTrans( moveObj , transform , easedProgress );
        }
        else if( target != null )
        {
            LerpTrans( transform , target , easedProgress );
        }else{
            LerpTrans( transform , null , easedProgress );
        }
    }

    void LerpTrans( Transform move , Transform target , float easedProgress )
    {
    
        Vector3 targetPos = pos_tar;
        Quaternion targetRot = rot_tar;
        Vector3 targetScale = scl_tar;

        if( target != null )
        {
            targetPos = target.position;
            targetRot = target.rotation;
            targetScale = target.localScale;  
        }

        // 각 Transform 컴포넌트를 부드럽게 보간
        if (followPosition)
        {
            //Debug.Log( "move : " + move.name + "    target : " + target.name );
            //Debug.Log( "move.position 1 : " + easedProgress + " : " + startPos + " : " + targetPos + " : " + move.localPosition  );
            
            move.position = Vector3.Lerp(startPos, targetPos, easedProgress);
            
Debug.Log( "move.position 2 : " + easedProgress + " : " + startPos + " : " + targetPos + " : " + move.localPosition  );
        }
        
        if (followRotation)
        {
            move.rotation = Quaternion.Lerp(startRot, targetRot, easedProgress);
        }
        
        if (followScale)
        {
            move.localScale = Vector3.Lerp(startScale, targetScale, easedProgress);
        }

    }

    /// <summary>
    /// 현재 따라가는 중인지 확인
    /// </summary>
    public bool IsFollowing()
    {
        return isFollowing;
    }
    
    /// <summary>
    /// 즉시 타겟 Transform과 동일하게 설정
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null) return;
        
        StopFollowing();
        
        if (followPosition)
            transform.position = target.position;
        
        if (followRotation)
            transform.rotation = target.rotation;
        
        if (followScale)
            transform.localScale = target.localScale;

    }
}

// 사용 예시를 위한 추가 스크립트
[System.Serializable]
public class FollowSettings
{
    public Transform target;
    public float duration = 1f;
    public bool followPosition = true;
    public bool followRotation = true;
    public bool followScale = false;
}