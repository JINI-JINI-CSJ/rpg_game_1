using System;
using UnityEngine;

/// <summary>
/// 커서 방향 입력 처리기
/// 외부에서는 입력값이 변경될 때만 SetInput()을 호출하면 되며,
/// 내부적으로 Update()에서 타이밍을 자체 처리합니다.
/// </summary>
public class CursorDirectionInput : MonoBehaviour
{
    // ── 타이밍 설정 ─────────────────────────────────────────
    [Header("Input Timing")]
    [Tooltip("첫 입력 후 연속 이동이 시작되기까지의 초기 딜레이 (초)")]
    public float initialDelay = 0.4f;

    [Tooltip("연속 이동 시 각 칸 사이의 간격 (초)")]
    public float repeatInterval = 0.1f;

    [Tooltip("입력으로 인식할 아날로그 스틱 최소 기울기 (데드존)")]
    public float deadZone = 0.3f;

    // ── 이동 알림 이벤트 ────────────────────────────────────
    /// <summary>X축으로 1칸 이동했을 때 호출. 인자: +1(오른쪽) 또는 -1(왼쪽)</summary>
    public event Action<int> OnMoveX;

    /// <summary>Y축으로 1칸 이동했을 때 호출. 인자: +1(위) 또는 -1(아래)</summary>
    public event Action<int> OnMoveY;

    // ── 내부 축 상태 ─────────────────────────────────────────
    private AxisState _axisX;
    private AxisState _axisY;

    // ─────────────────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 입력값이 변경되었을 때만 외부에서 호출합니다.
    /// 내부적으로 값을 기억하며 Update()에서 타이밍을 처리합니다.
    /// </summary>
    /// <param name="move_key_x">X축 입력값 (키보드: -1/0/1, 아날로그: -1.0 ~ 1.0)</param>
    /// <param name="move_key_y">Y축 입력값 (키보드: -1/0/1, 아날로그: -1.0 ~ 1.0)</param>
    public void SetInput(float move_key_x, float move_key_y)
    {
        _axisX.SetDirection(ToDirection(move_key_x));
        _axisY.SetDirection(ToDirection(move_key_y));
    }

    /// <summary>X축 이동 알림 리스너를 등록합니다.</summary>
    public void RegisterMoveX(Action<int> callback) => OnMoveX += callback;

    /// <summary>Y축 이동 알림 리스너를 등록합니다.</summary>
    public void RegisterMoveY(Action<int> callback) => OnMoveY += callback;

    /// <summary>X축 이동 알림 리스너를 해제합니다.</summary>
    public void UnregisterMoveX(Action<int> callback) => OnMoveX -= callback;

    /// <summary>Y축 이동 알림 리스너를 해제합니다.</summary>
    public void UnregisterMoveY(Action<int> callback) => OnMoveY -= callback;

    public void RegisterMoveX_One(Action<int> callback)
    {
        if( OnMoveX?.GetInvocationList().Length > 0 ) return;
        OnMoveX += callback;
    }

    public void RegisterMoveY_One(Action<int> callback)
    {
        if( OnMoveY?.GetInvocationList().Length > 0 ) return;
        OnMoveY += callback;
    }

    // ─────────────────────────────────────────────────────────
    // Unity 생명주기
    // ─────────────────────────────────────────────────────────

    private void Update()
    {
        float dt = Time.deltaTime;
        _axisX.Tick(dt, v => OnMoveX?.Invoke(v), initialDelay, repeatInterval);
        _axisY.Tick(dt, v => OnMoveY?.Invoke(v), initialDelay, repeatInterval);
    }

    // ─────────────────────────────────────────────────────────
    // 내부 헬퍼
    // ─────────────────────────────────────────────────────────

    private int ToDirection(float value)
    {
        if (value >  deadZone) return  1;
        if (value < -deadZone) return -1;
        return 0;
    }

    // ─────────────────────────────────────────────────────────
    // 축 상태 구조체
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 단일 축(X 또는 Y)의 입력 상태와 타이밍을 관리합니다.
    /// </summary>
    private struct AxisState
    {
        private int   _dir;        // 현재 방향 (-1, 0, 1)
        private float _timer;      // 경과 시간
        private bool  _initFired;  // 첫 칸 이동 완료 여부

        /// <summary>
        /// 외부에서 방향이 바뀔 때 호출합니다. 방향이 동일하면 무시합니다.
        /// </summary>
        public void SetDirection(int newDir)
        {
            if (newDir == _dir) return;

            _dir       = newDir;
            _timer     = 0f;
            _initFired = false;
        }

        /// <summary>
        /// 매 Update마다 호출되어 이동 타이밍을 처리합니다.
        /// </summary>
        public void Tick(float dt, Action<int> onMove, float initialDelay, float repeatInterval)
        {
            if (_dir == 0) return;

            // ── 첫 번째 칸: 즉시 이동 ─────────────────────
            if (!_initFired)
            {
                onMove(_dir);
                _initFired = true;
                _timer     = 0f;
                return;
            }

            _timer += dt;

            // ── 초기 딜레이 대기 ───────────────────────────
            if (_timer < initialDelay) return;

            // ── 연속 반복 구간 ─────────────────────────────
            float elapsed = _timer - initialDelay;
            if (elapsed >= repeatInterval)
            {
                onMove(_dir);
                // 나머지 시간 보존으로 타이밍 누적 오차 최소화
                _timer = initialDelay + (elapsed % repeatInterval);
            }
        }
    }
}
