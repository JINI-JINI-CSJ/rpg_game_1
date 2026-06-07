using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class MovementKeyframe
{
    public Vector3 targetPosition;
    public float duration;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public string aniState;
    public string callFunc;
    
    public MovementKeyframe(Vector3 pos, float dur)
    {
        targetPosition = pos;
        duration = dur;
    }

    public void Start(Animator animator , GameObject go)
    {
        pos_recent = Vector3.zero;
        cur_time = 0;

        if( string.IsNullOrEmpty( aniState ) == false && animator != null )
        {
            Debug.Log( aniState );
            animator.CrossFade( aniState , 0.1f );
        }

        if( string.IsNullOrEmpty(callFunc) == false )
        {
            SJ_Unity.SendMsg( go , callFunc , this );
        }   
    }

    public bool Update(float time , ref Vector3 pos_move )
    {
        cur_time += time;
        float t = cur_time / duration;
        float ct = easeCurve.Evaluate( t );
        Vector3 cur_p = targetPosition * ct;
        pos_move = cur_p - pos_recent;
        pos_recent = cur_p;

//        Debug.Log( pos_move );
        
        if( t >= 1.0f ) return true;
        return false;
    }
    
    float cur_time;
    Vector3 pos_recent;

}

public class KeyframeCharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public List<MovementKeyframe> keyframes = new List<MovementKeyframe>();
    public bool autoStart = true;
    public bool loop = false;
    public bool useLocalPosition = false;

    public Animator animator;
    
    [Header("Debug")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.yellow;
    public float gizmoSize = 0.5f;
    
    public CharacterController controller;
    private Vector3 startPosition;
    private Vector3 currentTargetPosition;
    private Vector3 previousPosition;
    private int currentKeyframeIndex = 0;
    private float currentTime = 0f;
    private bool isMoving = false;
    //private Coroutine movementCoroutine;

    MovementKeyframe cur_keyframe;
    

    public UnityEvent event_end;

    public UnityEvent<MovementKeyframe> event_nextEvent;
    
    void Start()
    {
        if( autoStart )StartMovement();
    }
    
    public void StartMovement()
    {
        if (controller == null)
        {
            Debug.LogError("CharacterController component is required!");
            return;
        }

        startPosition = useLocalPosition ? transform.localPosition : transform.position;


        if (keyframes.Count == 0)
        {
            Debug.LogWarning("No keyframes defined!");
            return;
        }
        
        // if (movementCoroutine != null)
        // {
        //     StopCoroutine(movementCoroutine);
        // }
        
        currentKeyframeIndex = 0;
        cur_keyframe = keyframes[0];
        currentTime = 0f;
        isMoving = true;

        cur_keyframe.Start( animator , gameObject );
        //movementCoroutine = StartCoroutine(MovementCoroutine());
    }
    
    public void StopMovement()
    {
        isMoving = false;
        // if (movementCoroutine != null)
        // {
        //     StopCoroutine(movementCoroutine);
        //     movementCoroutine = null;
        // }
    }
    
    public void AddKeyframe(Vector3 position, float duration)
    {
        keyframes.Add(new MovementKeyframe(position, duration));
    }
    
    public void ClearKeyframes()
    {
        keyframes.Clear();
        StopMovement();
    }

    public MovementKeyframe GetLastKey()
    {
        if( keyframes.Count < 1 ) return null;
        return keyframes[keyframes.Count-1];
    }
    
    public MovementKeyframe GetFirstKey()
    {
        if( keyframes.Count < 1 ) return null;
        return keyframes[0];
    }

    void Update()
    {
        UpdateKeyFrame(Time.deltaTime);
    }

    public void UpdateKeyFrame( float elapse )
    {
        if( isMoving == false ) return;

        Vector3 move_cur = Vector3.zero;
        if( cur_keyframe.Update(elapse,ref move_cur ) )
        {
            currentKeyframeIndex++;
            if( currentKeyframeIndex < keyframes.Count)
            {
                cur_keyframe = keyframes[currentKeyframeIndex];
                cur_keyframe.Start( animator , gameObject );
            }
            else
            {
                event_end.Invoke();
                isMoving = false;
            }
        }
//Debug.Log( move_cur );
        controller.Move(move_cur);
    }

    // 사용 안함
    private IEnumerator MovementCoroutine()
    {
        //Debug.Log( "MovementCoroutine" );
        while (isMoving && currentKeyframeIndex < keyframes.Count)
        {
            MovementKeyframe currentKeyframe = keyframes[currentKeyframeIndex];

            event_nextEvent.Invoke( currentKeyframe );

            if( string.IsNullOrEmpty( currentKeyframe.aniState ) == false && animator != null )
            {
                animator.CrossFade( currentKeyframe.aniState , 0.1f );
            }

            if( string.IsNullOrEmpty(currentKeyframe.callFunc) == false )
            {
                SJ_Unity.SendMsg( this.gameObject , currentKeyframe.callFunc , currentKeyframe );
            }            

            // 시작 위치 설정
            Vector3 startPos = useLocalPosition ? transform.localPosition : transform.position;
            Vector3 targetPos = useLocalPosition ? 
                transform.parent.TransformPoint(currentKeyframe.targetPosition) : 
                currentKeyframe.targetPosition + transform.position;
            
            currentTime = 0f;

            //Debug.Log( "MovementCoroutine 1 : " + controller.transform.position );
            // 키프레임 이동 실행
            while (currentTime < currentKeyframe.duration)
            {
                float normalizedTime = currentTime / currentKeyframe.duration;
                float easedTime = currentKeyframe.easeCurve.Evaluate(normalizedTime);
                
                Vector3 currentPosition = Vector3.Lerp(startPos, targetPos, easedTime);
                Vector3 movement = currentPosition - transform.position;
                
//Debug.Log( movement );
                // CharacterController로 이동
                controller.Move(movement);
                
                currentTime += Time.deltaTime;
                yield return null;
            }
            
            // 정확한 최종 위치로 이동
            Vector3 finalMovement = targetPos - transform.position;
            controller.Move(finalMovement);
            
            // 키프레임 도달 이벤트 호출
            //OnKeyframeReached?.Invoke(currentKeyframeIndex);
            
            currentKeyframeIndex++;

            //Debug.Log( "MovementCoroutine 2 : " + controller.transform.position );
            
            // 루프 처리
            if (currentKeyframeIndex >= keyframes.Count)
            {
                if (loop)
                {
                    currentKeyframeIndex = 0;
                }
                else
                {
                    isMoving = false;
                    //OnMovementComplete?.Invoke();
                    event_end.Invoke();
                }
            }
        }
    }
    
    // 현재 진행 상황 정보
    public float GetCurrentProgress()
    {
        if (keyframes.Count == 0 || !isMoving)
            return 0f;
        
        float totalProgress = currentKeyframeIndex;
        if (currentKeyframeIndex < keyframes.Count)
        {
            totalProgress += currentTime / keyframes[currentKeyframeIndex].duration;
        }
        
        return totalProgress / keyframes.Count;
    }
    
    public bool IsMoving()
    {
        return isMoving;
    }
    
    public int GetCurrentKeyframeIndex()
    {
        return currentKeyframeIndex;
    }
    
    // 에디터에서 키프레임 시각화
    void OnDrawGizmos()
    {
        if (!showGizmos || keyframes.Count == 0)
            return;
        
        Gizmos.color = gizmoColor;
        Vector3 basePosition = useLocalPosition && transform.parent != null ? 
            transform.parent.position : Vector3.zero;
        
        // 시작점 표시
        Vector3 startPos = useLocalPosition ? basePosition + startPosition : startPosition;
        if (Application.isPlaying)
        {
            startPos = transform.position;
        }
        
        // 키프레임 위치들 표시
        for (int i = 0; i < keyframes.Count; i++)
        {
            Vector3 keyframePos = useLocalPosition ? 
                basePosition + keyframes[i].targetPosition : 
                keyframes[i].targetPosition;
            
            // 키프레임 위치 표시
            Gizmos.DrawWireSphere(keyframePos, gizmoSize);
            
            // 순서 번호 표시 (에디터에서만)
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(keyframePos + Vector3.up * 0.5f, i.ToString());
            #endif
            
            // 이전 위치와 연결선 그리기
            Vector3 prevPos = (i == 0) ? startPos : 
                (useLocalPosition ? basePosition + keyframes[i-1].targetPosition : keyframes[i-1].targetPosition);
            
            Gizmos.DrawLine(prevPos, keyframePos);
        }
        
        // 현재 이동 중인 경우 진행 상황 표시
        if (Application.isPlaying && isMoving && currentKeyframeIndex < keyframes.Count)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, gizmoSize * 0.7f);
        }
    }

    public void SetKeyTargetPos( MovementKeyframe key , Vector3 pos , bool offset_self = true )
    {
        if( offset_self )
        {
            pos -= transform.position;
        }
        key.targetPosition = pos;
    }
}

// 사용 예제를 위한 간단한 컨트롤러
public class KeyframeMovementController : MonoBehaviour
{
    public KeyframeCharacterMovement movementSystem;
    
    void Start()
    {
        if (movementSystem == null)
            movementSystem = GetComponent<KeyframeCharacterMovement>();
        
        // 이벤트 연결
        //movementSystem.OnKeyframeReached += OnKeyframeReached;
        //movementSystem.OnMovementComplete += OnMovementComplete;
    }
    
    void Update()
    {
        // // 테스트용 입력
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     //movementSystem.StartMovement();
        // }
        
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     //movementSystem.StopMovement();
        // }
        
        // // 런타임에 키프레임 추가 예제
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     Vector3 randomPos = transform.position + new Vector3(
        //         Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        //     movementSystem.AddKeyframe(randomPos, 2f);
        // }
    }
    
    private void OnKeyframeReached(int keyframeIndex)
    {
        Debug.Log($"Keyframe {keyframeIndex} reached!");
    }
    
    private void OnMovementComplete()
    {
        Debug.Log("Movement sequence completed!");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        
        GUILayout.Label($"Current Progress: {movementSystem.GetCurrentProgress():F2}");
        GUILayout.Label($"Current Keyframe: {movementSystem.GetCurrentKeyframeIndex()}");
        GUILayout.Label($"Is Moving: {movementSystem.IsMoving()}");
        
        GUILayout.Space(10);
        GUILayout.Label("Controls:");
        GUILayout.Label("Space: Start Movement");
        GUILayout.Label("S: Stop Movement");
        GUILayout.Label("A: Add Random Keyframe");
        
        GUILayout.EndArea();
    }


}