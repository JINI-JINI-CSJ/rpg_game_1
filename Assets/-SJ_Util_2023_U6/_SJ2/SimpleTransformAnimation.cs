using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformKeyframe
{
    public float time;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;
    
    public TransformKeyframe(float t, Vector3 pos, Vector3 rot, Vector3 scl)
    {
        time = t;
        position = pos;
        rotation = rot;
        scale = scl;
    }
    public SJ_CallFunc_Mono func;
    public Transform tr_Target; // 있을경우 대상 위치

    public string aniState;

    [HideInInspector]
    public bool calc_Target_First_pos;
    [HideInInspector]
    public Vector3 pos_WorldSelf;

    [HideInInspector]
    public Vector3 pos_NextMove;

    public void Ready()
    {
        calc_Target_First_pos = false;
    }
}

public class SimpleTransformAnimation : MonoBehaviour
{
    [Header("애니메이션 설정")]
    public bool playOnStart = true;
    public bool loop = true;
    public float animationSpeed = 1.0f;
    
    [Header("키프레임 설정")]
    public List<TransformKeyframe> keyframes = new List<TransformKeyframe>();
    
    [Header("애니메이션 타입")]
    public bool animatePosition = true;
    public bool animateRotation = true;
    public bool animateScale = false;
    
    [Header("좌표계 설정")]
    public bool useWorldSpace = true;
    
    [Header("타겟 Transform")]
    [Tooltip("애니메이션을 적용할 Transform (null이면 자신의 Transform 사용)")]
    public Transform targetTransform;

    [Header("타겟 CharacterController(위 인자보다 우선)")]
    public CharacterController chrCtrl;
    //public Rigidbody rigid;

    public Animator anit;
    
    private float currentTime = 0f;
    private bool isPlaying = false;
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private Vector3 originalScale;
    private Transform animationTarget; // 실제 애니메이션 적용 대상
    

    public SJ_CallFunc_Mono sj_func_end;


    TransformKeyframe recentKeyframe;

    TransformKeyframe callFunc_Keyframe; // 호출된 함수들

    float startTime;

    void Start()
    {
        
    }

    void OnEnable()
    {
        if (playOnStart)
        {
            StartPlay();
        }
    }


    public void StartPlay()
    {
        gameObject.SetActive(true);

        enabled = true;
        recentKeyframe = null;
        callFunc_Keyframe = null;

        startTime = Time.time;

        // 타겟 Transform 설정 (null이면 자신의 Transform 사용)
        animationTarget = targetTransform != null ? targetTransform : transform;

        //if( rigid != null )animationTarget = rigid.transform;
        if( chrCtrl != null )animationTarget = chrCtrl.transform;
        
        // 원본 Transform 값 저장 (좌표계에 따라)
        if (useWorldSpace)
        {
            originalPosition = animationTarget.position;
            originalRotation = animationTarget.eulerAngles;
        }
        else
        {
            originalPosition = animationTarget.localPosition;
            originalRotation = animationTarget.localEulerAngles;
        }
        originalScale = animationTarget.localScale; // 스케일은 항상 로컬
        
        // 기본 키프레임이 없으면 생성
        // if (keyframes.Count == 0)
        // {
        //     CreateDefaultKeyframes();
        // }

        if( keyframes.Count == 0 )
        {
            enabled = false;
            Debug.Log( "주의!!! 키프레임 없음 : " + gameObject.name );
            return;
        }

        foreach( var s in keyframes ) s.Ready();
        
        //if (playOnStart)
        {
            PlayAnimation();
        }        
    }
    
    void Update()
    {
        if (isPlaying)
        {
            UpdateAnimation();
        }
    }

    void FixedUpdate()
    {
        // TransformKeyframe find_key = null;
        // float time_cur = Time.time - startTime;
        // foreach( var s in keyframes )
        // {
        //     if( s.time <= time_cur )
        //     {
        //         find_key = null;
        //     }else{
        //         break;
        //     }
        // }

        // if( find_key != callFunc_Keyframe )
        // {
        //     find_key.func.Func();
        //     callFunc_Keyframe = find_key;
        //     if( anit != null && string.IsNullOrEmpty( find_key.aniState ) == false ) 
        //         anit.CrossFade( find_key.aniState , 0.1f );
        // }

        // if( rigid != null )
        // {
        //     rigid.MovePosition( rigid.transform.position + pos_ctrl_move );
        //     pos_ctrl_move = Vector3.zero;
        // }
    }

    public TransformKeyframe FindIdx( int idx )
    {
        if( keyframes.Count <= idx ) return null;
        return keyframes[idx];
    }

    public TransformKeyframe FindIdxLast()
    {
        if( keyframes.Count < 1 ) return null;
        return keyframes[keyframes.Count-1];
    }

    // void CreateDefaultKeyframes()
    // {
    //     // 기본 키프레임 예제 (위아래 움직임)
    //     keyframes.Add(new TransformKeyframe(0f, originalPosition, originalRotation, originalScale));
    //     keyframes.Add(new TransformKeyframe(1f, originalPosition + Vector3.up * 2f, originalRotation, originalScale));
    //     keyframes.Add(new TransformKeyframe(2f, originalPosition, originalRotation, originalScale));
    // }
    
    Vector3 pos_ctrl_move;
    void UpdateAnimation()
    {
        if (keyframes.Count < 2) return;
        
        currentTime += Time.deltaTime * animationSpeed;
        
        // 총 애니메이션 시간 계산
        float totalTime = keyframes[keyframes.Count - 1].time;
        
        // 루프 처리
        if (currentTime >= totalTime)
        {
            // if( rigid != null )
            // {
            //     rigid.linearVelocity = Vector3.zero;
            //     rigid.angularVelocity = Vector3.zero;
            //     rigid.Sleep();
            // }

            if (loop)
            {
                currentTime = 0f;
            }
            else
            {
                currentTime = totalTime;
                isPlaying = false;
                enabled = false;
                sj_func_end.Func();
                gameObject.SetActive(false);
                return;
            }
        }
        
        // 현재 시간에 해당하는 키프레임 찾기
        TransformKeyframe key_pre = null;
        TransformKeyframe key_next = null;
        TransformKeyframe currentKeyframe = GetInterpolatedKeyframe(currentTime , ref key_pre , ref key_next );

        if( key_pre != callFunc_Keyframe )
        {
            callFunc_Keyframe = key_pre;            
            callFunc_Keyframe.func.Func();

            if( anit != null && string.IsNullOrEmpty( callFunc_Keyframe.aniState ) == false ) 
                anit.CrossFade( callFunc_Keyframe.aniState , 0.1f );
        }

        // Transform 적용 (좌표계에 따라)
        if (animatePosition)
        {
            //if( rigid != null )
            //{
                // Vector3 vel = Vector3.zero;
                // float time_temp = key_next.time - key_pre.time;
                // if( key_next.tr_Target != null )
                // {
                //     vel = (key_next.pos_NextMove - key_pre.position) /  time_temp;
                // }else{
                //     vel = (key_next.position - key_pre.position) /  time_temp;
                // }
                //Debug.Log( " UpdateAnimation : vel : " + vel );
                //rigid.linearVelocity = vel;                
            //}

            if( chrCtrl != null )
            {
                Vector3 pos_chrCtrl = currentKeyframe.position;
                if( recentKeyframe != null )
                {
                    pos_chrCtrl = currentKeyframe.position - recentKeyframe.position;
                }               
                chrCtrl.Move( pos_chrCtrl );

                Debug.Log( "chrCtrl :" + chrCtrl.transform.position );

            }else{
                if (useWorldSpace)
                    animationTarget.position = currentKeyframe.position;
                else
                    animationTarget.localPosition = currentKeyframe.position;                 
            }
        }
        
        if (animateRotation)
        {
            if (useWorldSpace)
                animationTarget.eulerAngles = currentKeyframe.rotation;
            else
                animationTarget.localEulerAngles = currentKeyframe.rotation;
        }
        
        if (animateScale)
            animationTarget.localScale = currentKeyframe.scale; // 스케일은 항상 로컬
        recentKeyframe = currentKeyframe;
    }
    
    TransformKeyframe GetInterpolatedKeyframe(float time , ref TransformKeyframe pre , ref TransformKeyframe next)
    {
        // 시간 범위 내에서 보간할 두 키프레임 찾기
        TransformKeyframe prevKeyframe = keyframes[0];
        TransformKeyframe nextKeyframe = keyframes[keyframes.Count - 1];
        
        for (int i = 0; i < keyframes.Count - 1; i++)
        {
            if (time >= keyframes[i].time && time <= keyframes[i + 1].time)
            {
                prevKeyframe = keyframes[i];
                nextKeyframe = keyframes[i + 1];
                break;
            }
        }

        pre = prevKeyframe;
        next = nextKeyframe;

//        Debug.Log( time + " : " + prevKeyframe.time + " : " + nextKeyframe.time );
        
        // 보간 비율 계산
        float duration = nextKeyframe.time - prevKeyframe.time;
        float t = duration > 0 ? (time - prevKeyframe.time) / duration : 0f;
        
        // 부드러운 보간을 위한 이징 함수 (선택사항)
        t = EaseInOut(t);


        // 특수 계산
        // 타겟이 설정되 있을경우 
        // 대상을 향한다.
        Vector3 pos_Next = nextKeyframe.position;
        if( nextKeyframe.tr_Target != null )
        {
            if( nextKeyframe.calc_Target_First_pos == false )
            {
                nextKeyframe.calc_Target_First_pos = true;
                nextKeyframe.pos_WorldSelf = animationTarget.position;
            }
            //pos_Prev = 
            pos_Next = nextKeyframe.tr_Target.position - nextKeyframe.pos_WorldSelf;
            nextKeyframe.pos_NextMove = pos_Next;
        }
        
        // 보간된 값 계산
        Vector3 interpolatedPosition = Vector3.Lerp(prevKeyframe.position, pos_Next , t);
        Vector3 interpolatedRotation = Vector3.Lerp(prevKeyframe.rotation, nextKeyframe.rotation, t);
        Vector3 interpolatedScale = Vector3.Lerp(prevKeyframe.scale, nextKeyframe.scale, t);
        
        return new TransformKeyframe(time, interpolatedPosition, interpolatedRotation, interpolatedScale);
    }
    
    // 부드러운 애니메이션을 위한 이징 함수
    float EaseInOut(float t)
    {
        return t * t * (3.0f - 2.0f * t);
    }
    
    // 공개 메서드들
    public void PlayAnimation()
    {
        isPlaying = true;
        currentTime = 0f;
    }
    
    public void StopAnimation()
    {
        isPlaying = false;
        currentTime = 0f;
        ResetToOriginal();
    }
    
    public void PauseAnimation()
    {
        isPlaying = false;
    }
    
    public void ResumeAnimation()
    {
        isPlaying = true;
    }
    
    public void ResetToOriginal()
    {
        if (animationTarget == null) return;
        
        if (useWorldSpace)
        {
            animationTarget.position = originalPosition;
            animationTarget.eulerAngles = originalRotation;
        }
        else
        {
            animationTarget.localPosition = originalPosition;
            animationTarget.localEulerAngles = originalRotation;
        }
        animationTarget.localScale = originalScale; // 스케일은 항상 로컬
    }
    
    // 키프레임 추가 메서드
    public void AddKeyframe(float time, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        TransformKeyframe newKeyframe = new TransformKeyframe(time, position, rotation, scale);
        keyframes.Add(newKeyframe);
        
        // 시간 순으로 정렬
        keyframes.Sort((a, b) => a.time.CompareTo(b.time));
    }
    
    // 현재 Transform으로 키프레임 추가 (좌표계에 따라)
    public void AddCurrentTransformAsKeyframe(float time)
    {
        Vector3 currentPos, currentRot;
        
        if (useWorldSpace)
        {
            currentPos = transform.position;
            currentRot = transform.eulerAngles;
        }
        else
        {
            currentPos = transform.localPosition;
            currentRot = transform.localEulerAngles;
        }
        
        AddKeyframe(time, currentPos, currentRot, transform.localScale);
    }
    
    // 좌표계 변경 시 기존 키프레임 변환 (에디터용)
    public void ConvertKeyframesToCurrentSpace()
    {
        if (keyframes.Count == 0) return;
        
        for (int i = 0; i < keyframes.Count; i++)
        {
            Vector3 convertedPos, convertedRot;
            
            if (useWorldSpace)
            {
                // 로컬 -> 월드 변환
                convertedPos = transform.TransformPoint(keyframes[i].position);
                convertedRot = transform.TransformDirection(keyframes[i].rotation);
            }
            else
            {
                // 월드 -> 로컬 변환
                convertedPos = transform.InverseTransformPoint(keyframes[i].position);
                convertedRot = transform.InverseTransformDirection(keyframes[i].rotation);
            }
            
            keyframes[i].position = convertedPos;
            keyframes[i].rotation = convertedRot;
        }
    }
    
    public void ClearKeyframes()
    {
        keyframes.Clear();
    }
    
    // 에디터에서 미리보기용 (선택사항)
    void OnDrawGizmos()
    {
        if (keyframes.Count > 1)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < keyframes.Count; i++)
            {
                Gizmos.DrawWireSphere(keyframes[i].position, 0.1f);
                
                if (i < keyframes.Count - 1)
                {
                    Gizmos.DrawLine(keyframes[i].position, keyframes[i + 1].position);
                }
            }
        }
    }
}