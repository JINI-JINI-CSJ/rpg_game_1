using UnityEngine;

/// <summary>
/// 객체 회전 컨트롤러
/// 기능 1: 마우스 드래그로 좌우(Y축) / 위아래(X축) 회전
///         - 드래그 시작 시 회전 없음, 이전 위치와 현재 위치의 차이만큼만 회전
///         - 사용할 마우스 버튼 선택 가능
/// 기능 2: Vector2 입력으로 지속 회전 (새 값이 들어올 때까지 유지)
/// X, Y축 각각 정방향/역방향 설정 가능
/// </summary>
public class ObjectRotationController : MonoBehaviour
{
    // ───────────────────────────────────────────
    //  열거형
    // ───────────────────────────────────────────

    public enum MouseButton
    {
        Left   = 0,
        Right  = 1,
        Middle = 2
    }

    // ───────────────────────────────────────────
    //  Inspector 설정
    // ───────────────────────────────────────────

    [Header("=== 마우스 회전 설정 ===")]
    [Tooltip("마우스 회전 활성화 여부")]
    public bool enableMouseRotation = true;

    [Tooltip("드래그에 사용할 마우스 버튼")]
    public MouseButton mouseButton = MouseButton.Left;

    [Tooltip("마우스 감도 (픽셀당 회전 각도)")]
    public float mouseSensitivity = 0.3f;

    [Header("=== Vector2 회전 설정 ===")]
    [Tooltip("Vector2 입력 회전 활성화 여부")]
    public bool enableVector2Rotation = true;

    [Tooltip("Vector2 회전 속도 (단위: 도/초)")]
    public float vector2RotationSpeed = 90f;

    [Header("=== 축 방향 설정 ===")]
    [Tooltip("X축(상하) 회전 방향 반전")]
    public bool invertX = false;

    [Tooltip("Y축(좌우) 회전 방향 반전")]
    public bool invertY = false;

    [Header("=== X축 각도 제한 (선택) ===")]
    [Tooltip("X축 각도 제한 사용 여부")]
    public bool clampXAngle = false;

    [Tooltip("X축 최소 각도")]
    public float minXAngle = -80f;

    [Tooltip("X축 최대 각도")]
    public float maxXAngle = 80f;

    // ───────────────────────────────────────────
    //  내부 상태
    // ───────────────────────────────────────────

    // 현재 오일러 각도 (누적)
    private float currentX = 0f;
    private float currentY = 0f;

    // Vector2 지속 회전 값 (새 값이 들어올 때까지 유지)
    private Vector2 persistentRotationInput = Vector2.zero;

    // 마우스 드래그 상태
    private bool isDragging = false;

    // 직전 프레임의 마우스 스크린 위치
    private Vector2 lastMousePosition = Vector2.zero;

    // ───────────────────────────────────────────
    //  Unity 생명주기
    // ───────────────────────────────────────────

    private void Start()
    {
        // 현재 오브젝트의 초기 각도를 기준으로 설정
        currentX = transform.eulerAngles.x;
        currentY = transform.eulerAngles.y;

        // 각도를 -180 ~ 180 범위로 정규화
        currentX = NormalizeAngle(currentX);
        currentY = NormalizeAngle(currentY);
    }

    private void Update()
    {
        HandleMouseRotation();
        HandleVector2Rotation();
    }

    // ───────────────────────────────────────────
    //  기능 1: 마우스 회전
    // ───────────────────────────────────────────

    private void HandleMouseRotation()
    {
        if (!enableMouseRotation) return;

        int btn = (int)mouseButton;

        // 버튼을 누른 첫 프레임: 현재 위치를 기준점으로만 저장, 회전 없음
        if (Input.GetMouseButtonDown(btn))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
            return;
        }

        // 버튼을 뗀 경우
        if (Input.GetMouseButtonUp(btn))
        {
            isDragging = false;
            return;
        }

        if (!isDragging) return;

        // 현재 위치와 직전 위치의 차이(픽셀)로 회전량 계산
        Vector2 currentMousePosition = Input.mousePosition;
        Vector2 delta = currentMousePosition - lastMousePosition;
        lastMousePosition = currentMousePosition;

        // delta가 없으면 회전 없음
        if (delta == Vector2.zero) return;

        float deltaY = delta.x * mouseSensitivity; // 좌우 → Y축
        float deltaX = delta.y * mouseSensitivity; // 위아래 → X축

        ApplyDelta(deltaY, deltaX);
    }

    // ───────────────────────────────────────────
    //  기능 2: Vector2 지속 회전
    // ───────────────────────────────────────────

    private void HandleVector2Rotation()
    {
        if (!enableVector2Rotation) return;
        if (persistentRotationInput == Vector2.zero) return;

        // persistentRotationInput.x → 좌우(Y축), persistentRotationInput.y → 위아래(X축)
        float deltaY = persistentRotationInput.x * vector2RotationSpeed * Time.deltaTime;
        float deltaX = persistentRotationInput.y * vector2RotationSpeed * Time.deltaTime;

        ApplyDelta(deltaY, deltaX);
    }

    /// <summary>
    /// Vector2 회전 입력을 설정합니다.
    /// x: 좌우 회전 (-1 ~ 1), y: 위아래 회전 (-1 ~ 1)
    /// 새 값이 들어올 때까지 이 방향으로 계속 회전합니다.
    /// Vector2.zero 를 입력하면 회전을 멈춥니다.
    /// </summary>
    public void SetVector2Input(Vector2 input)
    {
        persistentRotationInput = input;
    }

    /// <summary>
    /// Vector2 회전을 즉시 정지합니다.
    /// </summary>
    public void StopVector2Rotation()
    {
        persistentRotationInput = Vector2.zero;
    }

    // ───────────────────────────────────────────
    //  공통 회전 적용
    // ───────────────────────────────────────────

    /// <summary>
    /// deltaY: Y축(좌우) 변화량, deltaX: X축(상하) 변화량
    /// </summary>
    private void ApplyDelta(float deltaY, float deltaX)
    {
        // 방향 반전 적용
        float signX = invertX ? -1f : 1f;
        float signY = invertY ? -1f : 1f;

        // currentY += deltaY * signY;
        // currentX -= deltaX * signX; // 마우스를 위로 움직이면 위를 바라봄 (부호 반전)
        // // X축 각도 제한 (옵션)
        // if (clampXAngle)
        // {
        //     currentX = Mathf.Clamp(currentX, minXAngle, maxXAngle);
        // }
        // // 회전 적용 (월드 기준 Y → 로컬 X 순서)
        // transform.rotation = Quaternion.Euler(currentX, currentY, 0f);

        Vector3 ang_b = transform.rotation.eulerAngles;

        ang_b.y += deltaY * signY;
        ang_b.x -= deltaX * signX;
        if (clampXAngle)
        {
            currentX = Mathf.Clamp(ang_b.x, minXAngle, maxXAngle);
        }
        transform.rotation = Quaternion.Euler(ang_b);
    }

    // ───────────────────────────────────────────
    //  유틸리티
    // ───────────────────────────────────────────

    /// <summary>
    /// 각도를 -180 ~ 180 범위로 정규화
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        while (angle > 180f)  angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    /// <summary>
    /// 현재 회전 각도를 강제로 설정합니다.
    /// </summary>
    public void SetRotation(float x, float y)
    {
        currentX = x;
        currentY = y;
        transform.rotation = Quaternion.Euler(currentX, currentY, 0f);
    }

    /// <summary>
    /// 현재 회전 각도를 초기화합니다.
    /// </summary>
    public void ResetRotation()
    {
        SetRotation(0f, 0f);
    }
}