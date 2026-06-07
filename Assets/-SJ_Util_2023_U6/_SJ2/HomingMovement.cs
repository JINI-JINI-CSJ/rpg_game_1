using UnityEngine;

public class HomingMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 90f; // 초당 회전 각도
    
    [Header("타겟 설정")]
    [SerializeField] private Transform target;
    
    [Header("디버그")]
    [SerializeField] private bool showDebugInfo = true;
    
    // 물리 계산용 변수들 (FixedUpdate에서 계산)
    private Vector3 nextPosition;
    private Quaternion nextRotation;
    
    // 보간용 변수들 (Update에서 사용)
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    
    // 내부 변수들
    private Vector3 randomDirection;

    // 대기 시간
    public float waitTime = -1;
    float waitTime_cur = -1;

    bool play = false;
    
    void Start()
    {
        Init();
    }

    public void LookAtDir(Vector3 dir , Transform tr_target = null)
    {
        target = tr_target;
        transform.rotation = Quaternion.LookRotation(dir);
        Init();
    }

    public void Play()
    {
        waitTime_cur = 0;
        if( waitTime > float.Epsilon )
            play = false;
        else
            play = true;
    }

    public void Init()
    {
        // 초기 설정
        currentPosition = transform.position;
        currentRotation = transform.rotation;
        nextPosition = currentPosition;
        nextRotation = currentRotation;
    }
    
    void Update()
    {
        if( !play ) return;

        // 현재 위치와 다음 위치 사이를 보간
        float interpolationFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        interpolationFactor = Mathf.Clamp01(interpolationFactor);
        
        // 위치 보간
        Vector3 interpolatedPosition = Vector3.Lerp(currentPosition, nextPosition, interpolationFactor);
        transform.position = interpolatedPosition;
        
        // 회전 보간
        if( target != null && rotationSpeed > float.Epsilon )
        {
            Quaternion interpolatedRotation = Quaternion.Lerp(currentRotation, nextRotation, interpolationFactor);
            transform.rotation = interpolatedRotation;
        }

    }
    
    void FixedUpdate()
    {
        if (!play)
        {
            if (waitTime > 0)
            {
                waitTime_cur += Time.fixedDeltaTime;
                if (waitTime_cur >= waitTime)
                {
                    play = true;
                }
                else
                {
                    return;
                }
            }
        }

        // 현재 물리 프레임의 시작 위치와 회전 저장
            currentPosition = nextPosition;
        currentRotation = nextRotation;
        
        // 다음 회전 계산
        if( target != null && rotationSpeed > float.Epsilon )
            CalculateNextRotation();
        
        // 다음 위치 계산
        CalculateNextPosition();
    }
    
    private void CalculateNextRotation()
    {
        Vector3 targetDirection;
        
        // if (target != null)
        // {
            // 타겟이 있으면 타겟 방향으로 회전
            targetDirection = (target.position - nextPosition).normalized;
        // }
        // else
        // {
        //     // 타겟이 없으면 랜덤 방향으로 회전
        //     targetDirection = randomDirection;
        // }
        
        // 목표 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        
        // 점진적으로 회전 (rotationSpeed 속도로)
        float rotationStep = rotationSpeed * Time.fixedDeltaTime;
        nextRotation = Quaternion.RotateTowards(nextRotation, targetRotation, rotationStep);
    }
    
    private void CalculateNextPosition()
    {
        // 현재 바라보는 방향으로 이동
        Vector3 moveDirection = nextRotation * Vector3.forward;
        Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
        
        nextPosition += movement;
    }
    
    private void SetRandomDirection()
    {
        // 완전 랜덤한 3D 방향 생성
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
    }
    
    // 외부에서 타겟 설정
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    // 타겟 제거
    public void ClearTarget()
    {
        target = null;
        //SetRandomDirection();
    }
    
    // 이동 속도 설정
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    // 회전 속도 설정
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;
        
        // 타겟 연결선 표시
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
        
        // 현재 이동 방향 표시
        Gizmos.color = Color.blue;
        Vector3 forwardDirection = transform.rotation * Vector3.forward;
        Gizmos.DrawRay(transform.position, forwardDirection * 3f);
        
        // 랜덤 방향 표시 (타겟이 없을 때)
        if (target == null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, randomDirection * 2f);
        }
    }
}