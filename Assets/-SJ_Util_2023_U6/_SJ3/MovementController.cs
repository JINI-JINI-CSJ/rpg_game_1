using UnityEngine;

/// <summary>
/// 객체 이동 및 회전 가감속 컨트롤러 (후진 포함)
/// 외부에서 InputMoveSpeed(float ratio), InputRotate(float ratio) 를 호출하여 조작
/// </summary>
public class MovementController : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("최대 전진 속도 (m/s)")]
    public float maxForwardSpeed = 10f;

    [Tooltip("최대 후진 속도 (m/s)")]
    public float maxReverseSpeed = 5f;

    [Tooltip("가속도 (m/s²) - 입력 방향과 현재 이동 방향이 같을 때")]
    public float acceleration = 8f;

    [Tooltip("자연 감속도 (m/s²) - 입력 없을 때")]
    public float deceleration = 12f;

    [Tooltip("급감속/제동 (m/s²) - 입력 방향과 현재 이동 방향이 반대일 때")]
    public float brakeDeceleration = 25f;

    [Header("회전 설정")]
    [Tooltip("최대 회전 속도 (deg/s)")]
    public float maxRotateSpeed = 120f;

    [Tooltip("회전 가속도 (deg/s²)")]
    public float rotateAcceleration = 200f;

    [Tooltip("회전 감속도 (deg/s²) - 입력 없을 때 자동 감속")]
    public float rotateDeceleration = 300f;

    // ───────── 내부 상태 ─────────
    // _currentMoveSpeed: 음수 = 후진, 양수 = 전진
    private float _currentMoveSpeed = 0f;
    private float _currentRotateSpeed = 0f;

    private float _moveInput = 0f;    // -1 ~ 1
    private float _rotateInput = 0f;  // -1 ~ 1
    private bool _hasMoveInput = false;
    private bool _hasRotateInput = false;

    // ───────── Public API ─────────

    /// <summary>
    /// 이동 입력 (-1 ~ 1)
    ///  1 : 전진 가속
    ///  0 : 입력 없음 → 자연 감속 후 정지
    /// -1 : 전진 중이면 급감속 → 정지 후 후진 가속
    /// </summary>
    public void InputMoveSpeed(float ratio)
    {
        _moveInput = Mathf.Clamp(ratio, -1f, 1f);
        _hasMoveInput = !Mathf.Approximately(_moveInput, 0f);
    }

    /// <summary>
    /// 회전 입력 (-1 ~ 1 / -1: 좌회전, 0: 입력 없음 → 자동 감속, 1: 우회전)
    /// </summary>
    public void InputRotate(float ratio)
    {
        _rotateInput = Mathf.Clamp(ratio, -1f, 1f);
        _hasRotateInput = !Mathf.Approximately(_rotateInput, 0f);
    }

    // ───────── Unity Loop ─────────

    private void Update()
    {
        UpdateMoveSpeed();
        UpdateRotateSpeed();
        ApplyMovement();
    }

    // ───────── 이동 속도 업데이트 ─────────

    private void UpdateMoveSpeed()
    {
        if (!_hasMoveInput)
        {
            // ── 입력 없음: 부호 유지한 채 자연 감속 → 0 ──
            _currentMoveSpeed = MoveTowardsZero(_currentMoveSpeed, deceleration * Time.deltaTime);
            return;
        }

        // 입력 부호와 현재 속도 부호 비교
        bool inputIsForward  = _moveInput > 0f;
        bool movingForward   = _currentMoveSpeed > 0f;
        bool movingReverse   = _currentMoveSpeed < 0f;
        bool isOppositeDir   = (inputIsForward && movingReverse) || (!inputIsForward && movingForward);

        if (isOppositeDir)
        {
            // ── 반대 방향 입력: 급감속 → 0 에 도달하면 다음 프레임부터 반대 방향 가속 ──
            float brakeAmount = brakeDeceleration * Mathf.Abs(_moveInput) * Time.deltaTime;
            _currentMoveSpeed = MoveTowardsZero(_currentMoveSpeed, brakeAmount);
        }
        else
        {
            // ── 같은 방향 입력: 가속 ──
            float accelAmount = acceleration * Mathf.Abs(_moveInput) * Time.deltaTime;
            _currentMoveSpeed += inputIsForward ? accelAmount : -accelAmount;

            // 최대 속도 클램프
            float maxSpeed = inputIsForward ? maxForwardSpeed : maxReverseSpeed;
            _currentMoveSpeed = inputIsForward
                ? Mathf.Min(_currentMoveSpeed, maxSpeed)
                : Mathf.Max(_currentMoveSpeed, -maxSpeed);
        }
    }

    // ───────── 회전 속도 업데이트 ─────────

    private void UpdateRotateSpeed()
    {
        if (_hasRotateInput)
        {
            float targetRotateSpeed = maxRotateSpeed * _rotateInput;
            _currentRotateSpeed = Mathf.MoveTowards(
                _currentRotateSpeed,
                targetRotateSpeed,
                rotateAcceleration * Time.deltaTime
            );
        }
        else
        {
            _currentRotateSpeed = Mathf.MoveTowards(
                _currentRotateSpeed,
                0f,
                rotateDeceleration * Time.deltaTime
            );
        }
    }

    // ───────── 이동/회전 적용 ─────────

    private void ApplyMovement()
    {
        // _currentMoveSpeed 부호가 방향을 결정 (양수: 전진, 음수: 후진)
        transform.position += transform.forward * (_currentMoveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, _currentRotateSpeed * Time.deltaTime, Space.World);
    }

    // ───────── 유틸 ─────────

    /// <summary>0 방향으로만 이동 (부호 반전 없음)</summary>
    private static float MoveTowardsZero(float current, float delta)
    {
        if (current > 0f) return Mathf.Max(0f, current - delta);
        if (current < 0f) return Mathf.Min(0f, current + delta);
        return 0f;
    }

    // ───────── 읽기 전용 프로퍼티 ─────────

    /// <summary>현재 이동 속도 (양수: 전진, 음수: 후진)</summary>
    public float CurrentMoveSpeed => _currentMoveSpeed;

    /// <summary>현재 회전 속도 (deg/s)</summary>
    public float CurrentRotateSpeed => _currentRotateSpeed;

    public float Input_Move => _moveInput;
    public float Input_Rot => _rotateInput;


    /// <summary>전진/후진 정규화 속도 (-1 ~ 1)</summary>
    public float NormalizedMoveSpeed
    {
        get
        {
            if (_currentMoveSpeed >= 0f)
                return maxForwardSpeed > 0f ? _currentMoveSpeed / maxForwardSpeed : 0f;
            else
                return maxReverseSpeed > 0f ? _currentMoveSpeed / maxReverseSpeed : 0f;
        }
    }

    public void Stop()
    {
        _currentMoveSpeed = 0;
        _currentRotateSpeed = 0;
    }

#if UNITY_EDITOR
    // private void OnGUI()
    // {
    //     string dir = _currentMoveSpeed > 0.01f ? "전진" : _currentMoveSpeed < -0.01f ? "후진" : "정지";
    //     GUILayout.BeginArea(new Rect(10, 10, 300, 110));
    //     GUILayout.Label($"Move Speed  : {_currentMoveSpeed:F2} m/s  [{dir}]");
    //     GUILayout.Label($"Rotate Speed: {_currentRotateSpeed:F2} deg/s");
    //     GUILayout.Label($"Move Input  : {_moveInput:F2}   Rotate Input: {_rotateInput:F2}");
    //     GUILayout.Label($"Normalized  : {NormalizedMoveSpeed:F2}");
    //     GUILayout.EndArea();
    // }
#endif
}