using UnityEngine;

public class SJ_PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;           // 이동 속도 (m/s)
    public float acceleration = 10f;       // 가속도 (부드러운 움직임용)
    public float deceleration = 10f;       // 감속도 (부드러운 정지용)
    
    [Header("점프 설정")]
    public float jumpHeight = 2f;          // 점프 높이 (m)
    public float gravity = -9.81f;         // 중력 가속도 (m/s²)
    public float groundCheckDistance = 0.1f; // 바닥 체크 거리
    
    [Header("마우스 감도 설정")]
    public float mouseSensitivity = 2f;    // 마우스 감도
    public float verticalLookLimit = 80f;  // 수직 시야 제한 각도 (위아래)
    
    [Header("컴포넌트")]
    private CharacterController controller;
    public Transform playerCamera;
    
    // 이동 관련 변수
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 targetVelocity = Vector3.zero;
    private Vector3 verticalVelocity = Vector3.zero;
    
    // 점프 관련 변수
    private bool isGrounded;
    private bool wasGrounded;
    private float lastGroundedTime;
    
    // 카메라 회전 관련 변수
    private float verticalRotation = 0f;   // 카메라의 위아래 회전값
    private float horizontalRotation = 0f; // 캐릭터의 좌우 회전값
    
    void Start()
    {
        // 컴포넌트 가져오기
        controller = GetComponent<CharacterController>();
        
        // // 자식 오브젝트에서 카메라 찾기
        // playerCamera = GetComponentInChildren<Camera>();
        
        // // 카메라가 없다면 생성
        // if (playerCamera == null)
        // {
        //     GameObject cameraObject = new GameObject("PlayerCamera");
        //     cameraObject.transform.SetParent(transform);
        //     cameraObject.transform.localPosition = new Vector3(0, 1.6f, 0); // 눈 높이
        //     playerCamera = cameraObject.AddComponent<Camera>();
        // }
        
        // CharacterController가 없으면 추가
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        
        // 기본 CharacterController 설정
        controller.height = 2f;
        controller.radius = 0.5f;
        controller.center = new Vector3(0, 1f, 0);
        
        // 마우스 커서 숨기기 및 중앙 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // 초기 회전값 설정
        horizontalRotation = transform.eulerAngles.y;
    }
    
    void Update()
    {
        // 바닥 상태 체크
        //CheckGroundStatus();
        
        // 마우스 입력 처리
        HandleMouseInput();
        
        // 이동 입력 처리
        HandleMovementInput();
        
        // 점프 입력 처리
        HandleJumpInput();
        
        // 이동 적용
        ApplyMovement();
        
        // ESC 키로 마우스 커서 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // 마우스 클릭으로 다시 커서 고정
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void HandleMouseInput()
    {
        // 마우스 커서가 고정되어 있을 때만 카메라 회전
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // 마우스 입력 받기
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            // 좌우 회전 - 캐릭터 전체 회전
            horizontalRotation += mouseX;
            transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
            
            // 위아래 회전 - 카메라만 회전 (제한 적용)
            verticalRotation -= mouseY; // Y축 입력을 반전 (자연스러운 카메라 움직임을 위해)
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
    
    void CheckGroundStatus()
    {
        // 이전 프레임의 바닥 상태 저장
        wasGrounded = isGrounded;
        
        // CharacterController의 isGrounded 사용
        isGrounded = controller.isGrounded;
        
        // 추가적인 바닥 체크 (더 정확한 감지를 위해)
        if (!isGrounded)
        {
            // 캐릭터 발 아래쪽으로 레이캐스트
            Vector3 rayOrigin = transform.position + Vector3.up * (controller.height * 0.1f);
            float rayDistance = controller.height * 0.5f + groundCheckDistance;
            
            if (Physics.Raycast(rayOrigin, Vector3.down, rayDistance))
            {
                isGrounded = true;
            }
        }
        
        // 바닥에 착지한 순간 처리
        if (isGrounded && !wasGrounded)
        {
            lastGroundedTime = Time.time;
            verticalVelocity.y = 0f; // 수직 속도 초기화
        }
    }
    
    void HandleMovementInput()
    {
        // 입력 받기
        float horizontal = Input.GetAxis("Horizontal"); // A, D 키 (좌우 이동)
        float vertical = Input.GetAxis("Vertical");     // W, S 키 (앞뒤 이동)
        
        // 캐릭터 기준으로 방향 계산 (로컬 좌표계)
        Vector3 forward = transform.forward;  // 캐릭터가 바라보는 앞 방향
        Vector3 right = transform.right;      // 캐릭터 기준 오른쪽 방향
        
        // 목표 이동 방향 계산
        // W/S: 앞뒤 이동, A/D: 좌우 스트레이프
        Vector3 inputDirection = (forward * vertical + right * horizontal).normalized;
        
        // 목표 속도 설정
        targetVelocity = inputDirection * moveSpeed;
    }
    
    void HandleJumpInput()
    {
        // 스페이스바 입력 및 바닥 상태 확인
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            // 점프 속도 계산 (물리 공식: v = √(2gh))
            float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
            verticalVelocity.y = jumpVelocity;
        }
    }
    
    void ApplyMovement()
    {
        // 
        if (targetVelocity.magnitude > 0.1f)
        {
            // 가속
            // currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, 
            //                                     acceleration * Time.deltaTime);
            currentVelocity = targetVelocity;
        }
        else
        {
            // 감속
            // currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, 
            //                                     deceleration * Time.deltaTime);
            currentVelocity = Vector3.zero;
        }
        
        // 중력 적용
        //if (!isGrounded)
        if( controller.isGrounded == false )
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }
        else if (verticalVelocity.y < 0)
        {
            verticalVelocity.y = 0f;
        }
        
        // 최종 이동 벡터 계산
        Vector3 finalMovement = currentVelocity + verticalVelocity;
        
        // CharacterController로 이동 적용
        controller.Move(finalMovement * Time.deltaTime);
    }
    
    // 디버그용 정보 표시
    void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 250));
            GUILayout.Label($"바닥 상태: {(isGrounded ? "바닥" : "공중")}");
            GUILayout.Label($"수평 속도: {currentVelocity.magnitude:F2} m/s");
            GUILayout.Label($"수직 속도: {verticalVelocity.y:F2} m/s");
            GUILayout.Label($"캐릭터 회전 (Y): {horizontalRotation:F1}°");
            GUILayout.Label($"카메라 회전 (X): {verticalRotation:F1}°");
            GUILayout.Label($"FPS: {(1f / Time.deltaTime):F0}");
            GUILayout.Label("");
            GUILayout.Label("조작법:");
            GUILayout.Label("WASD: 이동 (A/D는 좌우 스트레이프)");
            GUILayout.Label("마우스: 시점 회전");
            GUILayout.Label("Space: 점프");
            GUILayout.Label("ESC: 마우스 커서 해제");
            GUILayout.EndArea();
        }
    }
    
    // 기즈모로 바닥 체크 범위 시각화
    void OnDrawGizmosSelected()
    {
        if (controller != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 center = transform.position + Vector3.up * (controller.height * 0.1f);
            float rayDistance = controller.height * 0.5f + groundCheckDistance;
            Gizmos.DrawLine(center, center + Vector3.down * rayDistance);
        }
    }
}