using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Unity UGUI 월드 미니맵 조작 시스템 (직교 카메라 연동 포함)
///
/// ─ 미니맵 조작 ───────────────────────────────────────────
///   • 마우스 드래그 / 단일 터치 드래그 이동
///   • 마우스 휠 줌 (PC)
///   • 두 손가락 핀치 줌 (모바일)
///   • 최대·최소 이동/줌 경계 클램프
///   • 관성(Inertia) 슬라이딩
///
/// ─ 직교 카메라 연동 ──────────────────────────────────────
///   • 미니맵 스케일(줌) → Camera.orthographicSize 역비례 매핑
///   • 미니맵 anchoredPosition → 카메라 월드 좌표 매핑
///   • 미니맵 UI 좌표계(X, Y)를 카메라 3D 축(X, Y, Z)에
///     자유롭게 라우팅 (부호 반전 포함)
///
/// ─ RectTransform 미연결 시 수동 폴백 ────────────────────
///   • mapRect    == null → manualMapSize / manualMapPosition / manualMapScale 로 동작
///   • boundsRect == null → manualViewportSize 로 동작
///
/// ─ 입력 전용 영역 지정 ───────────────────────────────────
///   • inputRect 를 연결하면 해당 Rect 안에서만 드래그·휠·핀치를 수락합니다.
///   • inputRect == null 이면 boundsRect(또는 이 오브젝트)가 입력 영역이 됩니다.
///   • 런타임에서 SetInputZone(RectTransform) 으로 변경 가능합니다.
///
/// ─ 사용법 ────────────────────────────────────────────────
///   1. 미니맵 뷰포트(Mask) 오브젝트에 이 스크립트를 추가합니다.
///   2. Inspector에서 mapRect, boundsRect, orthoCamera 를 연결합니다.
///      연결하지 않으려면 Manual Fallback 섹션의 값을 직접 입력합니다.
///   3. 드래그 입력 영역을 따로 지정하려면 inputRect 를 연결합니다.
///   4. 좌표 매핑 및 스케일 파라미터를 조정합니다.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class MinimapController : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // ═══════════════════════════════════════════════════════
    // Inspector — RectTransform 연결 (선택)
    // ═══════════════════════════════════════════════════════

    [Header("RectTransform 연결 (외부 링크 — 선택)")]
    [Tooltip("이동·줌 대상이 되는 미니맵 이미지 RectTransform.\n" +
             "null 이면 아래 Manual Fallback (Map) 값을 사용합니다.")]
    public RectTransform mapRect;

    [Tooltip("미니맵 뷰포트(마스크) RectTransform.\n" +
             "null 이면 아래 Manual Fallback (Viewport) 값을 사용합니다.")]
    public RectTransform boundsRect;

    [Tooltip("마우스 드래그·휠 스크롤·두 손가락 터치를 허용할 입력 전용 영역 RectTransform.\n\n" +
             "▸ 연결된 경우   → 이 Rect 안에서만 모든 입력을 수락합니다.\n" +
             "▸ null 인 경우  → boundsRect(또는 이 오브젝트)를 입력 영역으로 사용합니다.\n\n" +
             "예) 미니맵 전체 패널 중 일부(버튼 제외 영역)만 드래그를 허용하고 싶을 때 사용합니다.")]
    public RectTransform inputRect;

    // ═══════════════════════════════════════════════════════
    // Inspector — Manual Fallback : mapRect 미연결 시
    // ═══════════════════════════════════════════════════════

    [Header("Manual Fallback — mapRect 미연결 시")]
    [Tooltip("mapRect 가 null 일 때 사용할 미니맵 이미지 크기 (px).\n" +
             "경계 클램프 계산에 사용됩니다.")]
    public Vector2 manualMapSize = new Vector2(512f, 512f);

    [Tooltip("mapRect 가 null 일 때 사용할 초기 anchoredPosition (px).")]
    public Vector2 manualMapPosition = Vector2.zero;

    [Tooltip("mapRect 가 null 일 때 사용할 초기 localScale.\n" +
             "defaultScale 이 우선 적용되며, 이 값은 최초 1회만 참조됩니다.")]
    public float manualMapScale = 1f;

    // ═══════════════════════════════════════════════════════
    // Inspector — Manual Fallback : boundsRect 미연결 시
    // ═══════════════════════════════════════════════════════

    [Header("Manual Fallback — boundsRect 미연결 시")]
    [Tooltip("boundsRect 가 null 이고 이 오브젝트의 RectTransform 도 신뢰할 수 없을 때\n" +
             "뷰포트 크기(px)를 직접 지정합니다.\n" +
             "Vector2.zero 이면 이 오브젝트의 RectTransform.rect.size 를 사용합니다.")]
    public Vector2 manualViewportSize = Vector2.zero;

    // ═══════════════════════════════════════════════════════
    // Inspector — 직교 카메라 연동
    // ═══════════════════════════════════════════════════════

    [Header("직교 카메라 연동")]
    [Tooltip("연동할 Orthographic Camera.\nnull 이면 카메라 연동을 비활성화합니다.")]
    public Camera orthoCamera;

    [Tooltip("카메라 연동 활성화 여부")]
    public bool enableCameraSync = true;

    [Header("미니맵 → 카메라 좌표 매핑")]
    [Tooltip("미니맵 X 축을 카메라의 어느 월드 축으로 매핑할지 선택합니다.")]
    public WorldAxis mapXToWorldAxis = WorldAxis.X;

    [Tooltip("미니맵 X 이동 방향을 반전합니다.")]
    public bool invertMapX = false;

    [Tooltip("미니맵 Y 축을 카메라의 어느 월드 축으로 매핑할지 선택합니다.\n예) Y → Z  (탑뷰 월드)")]
    public WorldAxis mapYToWorldAxis = WorldAxis.Z;

    [Tooltip("미니맵 Y 이동 방향을 반전합니다.\n예: UI Y↑ → 카메라 Z↓(남쪽 이동)")]
    public bool invertMapY = true;

    [Tooltip("X·Y 로 매핑되지 않은 나머지 축의 카메라 고정값.\n" +
             "예) mapX→X, mapY→Z 이면 카메라 Y(높이) 고정값.")]
    public float cameraFixedAxisValue = 50f;

    [Header("OrthographicSize 매핑")]
    [Tooltip("미니맵이 최대로 줌인(= maxScale)되었을 때의 orthographicSize.\n" +
             "카메라가 가장 좁은 영역을 비출 때의 값입니다. 예) 100")]
    public float minOrthoSize = 5f;

    [Tooltip("미니맵이 최대로 줌아웃(= minScale)되었을 때의 orthographicSize.\n" +
             "카메라가 가장 넓은 영역을 비출 때의 값입니다. 예) 5000")]
    public float maxOrthoSize = 200f;

    [Tooltip("OrthoSize 보간 곡선.\n" +
             "Linear  : 스케일에 비례하는 직선 매핑.\n" +
             "Inverse : 1/scale 역비례 곡선 — 줌인 구간에서 더 빠르게 반응합니다.")]
    public OrthoSizeMode orthoSizeMode = OrthoSizeMode.Linear;

    [Header("위치 스케일")]
    [Tooltip("미니맵 픽셀 1px 이동 시 카메라 월드 이동 거리.\n" +
             "= 월드맵 크기(유닛) / 미니맵 이미지 크기(px) 로 계산하세요.")]
    public float pixelToWorldScale = 1f;

    [Tooltip("카메라 위치 보간 속도 (lerp). 0 이면 즉시 이동합니다.")]
    [Range(0f, 30f)]
    public float cameraMoveLerpSpeed = 10f;

    [Tooltip("카메라 OrthographicSize 보간 속도 (lerp). 0 이면 즉시 변경됩니다.")]
    [Range(0f, 30f)]
    public float cameraSizeLerpSpeed = 10f;

    // ═══════════════════════════════════════════════════════
    // Inspector — 줌 설정
    // ═══════════════════════════════════════════════════════

    [Header("줌 설정")]
    [Tooltip("초기(기본) 줌 스케일")]
    [Range(0.1f, 10f)]
    public float defaultScale = 1f;

    [Tooltip("최소 줌 스케일")]
    [Range(0.1f, 5f)]
    public float minScale = 0.5f;

    [Tooltip("최대 줌 스케일")]
    [Range(1f, 20f)]
    public float maxScale = 4f;

    [Tooltip("줌 보간 속도 (lerp)")]
    [Range(1f, 30f)]
    public float zoomLerpSpeed = 15f;

    [Tooltip("마우스 휠 줌 감도")]
    [Range(0.01f, 1f)]
    public float mouseScrollSensitivity = 0.1f;

    // ═══════════════════════════════════════════════════════
    // Inspector — 드래그/이동 설정
    // ═══════════════════════════════════════════════════════

    [Header("드래그 설정")]
    [Tooltip("드래그 이동 감도 배율")]
    [Range(0.1f, 100f)]
    public float dragSensitivity = 1f;

    [Tooltip("이동 보간 속도 (lerp)")]
    [Range(1f, 30f)]
    public float moveLerpSpeed = 20f;

    [Tooltip("관성(inertia) 활성화")]
    public bool enableInertia = true;

    [Tooltip("관성 감쇠 계수 (0에 가까울수록 빠르게 멈춤)")]
    [Range(0.8f, 0.99f)]
    public float inertiaFriction = 0.93f;

    // ═══════════════════════════════════════════════════════
    // Inspector — 경계 제한
    // ═══════════════════════════════════════════════════════

    [Header("경계 제한")]
    [Tooltip("맵이 뷰포트 밖으로 나가지 않도록 제한 (true 권장)")]
    public bool clampToBounds = true;

    [Tooltip("경계 여백 (px). 음수를 주면 일부 여백을 허용합니다.")]
    public float boundsPadding = 0f;

    // ═══════════════════════════════════════════════════════
    // 열거형
    // ═══════════════════════════════════════════════════════

    /// <summary>미니맵 UI 축을 매핑할 카메라 월드 축</summary>
    public enum WorldAxis { X, Y, Z }

    /// <summary>
    /// 미니맵 스케일 → orthographicSize 매핑 곡선 선택.
    ///
    ///  Linear  : scale 을 [minScale, maxScale] 에서 [maxOrthoSize, minOrthoSize] 로 선형 보간.
    ///            직관적이고 예측 가능합니다.
    ///
    ///  Inverse : orthoSize = (minOrthoSize × maxOrthoSize 의 기하평균 기준) / scale 역비례.
    ///            줌인할수록 orthoSize 변화가 빠르고, 줌아웃할수록 완만해집니다.
    ///            지도 앱에서 흔히 쓰는 방식입니다.
    /// </summary>
    public enum OrthoSizeMode { Linear, Inverse }

    // ═══════════════════════════════════════════════════════
    // Private — 런타임 상태
    // ═══════════════════════════════════════════════════════

    // RectTransform 연결 여부 플래그
    private bool _hasMapRect;
    private bool _hasBoundsRect;

    // 뷰포트 참조 (boundsRect 또는 자기 자신)
    private RectTransform _viewportRect;

    // 입력 허용 영역 (inputRect → _viewportRect 순 폴백)
    // 마우스 휠·드래그 시작·핀치 시작의 히트 테스트에 사용됩니다.
    private RectTransform _inputZoneRect;

    private Canvas        _canvas;

    // 미니맵 논리 상태 (mapRect 유무에 관계없이 이 값으로 동작)
    private Vector2 _currentPosition;   // 실제 적용 중인 anchoredPosition
    private Vector2 _targetPosition;    // 목표 anchoredPosition
    private float   _currentScale;      // 실제 적용 중인 scale
    private float   _targetScale;       // 목표 scale

    // 드래그
    private bool    _isDragging;
    private Vector2 _lastDragPos;
    private Vector2 _dragVelocity;

    // 핀치 줌
    private bool    _isPinching;
    private float   _pinchStartDist;
    private float   _pinchStartScale;
    private Vector2 _pinchCenter;

    // ═══════════════════════════════════════════════════════
    // 프로퍼티 — 맵/뷰포트 크기·위치 (수동 or RectTransform 자동)
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// 현재 유효한 미니맵 이미지 크기 (px).
    /// mapRect 가 연결된 경우 rect.size, 아니면 manualMapSize 를 반환합니다.
    /// </summary>
    private Vector2 MapSize =>
        _hasMapRect ? mapRect.rect.size : manualMapSize;

    /// <summary>
    /// 현재 유효한 뷰포트 크기 (px).
    /// boundsRect 가 연결된 경우 rect.size,
    /// manualViewportSize 가 0 이 아니면 그 값,
    /// 둘 다 아니면 이 오브젝트의 RectTransform.rect.size 를 반환합니다.
    /// </summary>
    private Vector2 ViewportSize
    {
        get
        {
            if (_hasBoundsRect) return _viewportRect.rect.size;
            if (manualViewportSize.sqrMagnitude > 0f) return manualViewportSize;
            return GetComponent<RectTransform>().rect.size;
        }
    }

    // csj 추가
    public bool Get_isDragging(){return _isDragging;}

    // ═══════════════════════════════════════════════════════
    // 초기화
    // ═══════════════════════════════════════════════════════

    private void Awake()
    {
        _hasMapRect    = mapRect    != null;
        _hasBoundsRect = boundsRect != null;

        // 뷰포트 RectTransform : boundsRect 없으면 자기 자신
        _viewportRect = _hasBoundsRect ? boundsRect : GetComponent<RectTransform>();

        // 입력 영역 : inputRect → _viewportRect 순 폴백
        _inputZoneRect = inputRect != null ? inputRect : _viewportRect;

        _canvas = GetComponentInParent<Canvas>();

        // 연결 상태 로그
        if (!_hasMapRect)
            Debug.Log("[MinimapController] mapRect 미연결 → " +
                      $"manualMapSize={manualMapSize}, manualMapPosition={manualMapPosition} 로 동작합니다.");

        if (!_hasBoundsRect)
            Debug.Log("[MinimapController] boundsRect 미연결 → " +
                      (manualViewportSize.sqrMagnitude > 0f
                          ? $"manualViewportSize={manualViewportSize} 로 동작합니다."
                          : "이 오브젝트의 RectTransform.rect.size 를 뷰포트로 사용합니다."));

        Debug.Log("[MinimapController] 입력 영역 → " +
                  (inputRect != null
                      ? $"inputRect ({inputRect.name})"
                      : $"폴백: {_inputZoneRect.name} (boundsRect 또는 자기 자신)"));

        if (orthoCamera != null && !orthoCamera.orthographic)
            Debug.LogWarning("[MinimapController] orthoCamera 가 Orthographic 모드가 아닙니다.");

        if (mapXToWorldAxis == mapYToWorldAxis)
            Debug.LogWarning("[MinimapController] mapXToWorldAxis 와 mapYToWorldAxis 가 같은 축입니다.");
    }

    private void Start()
    {
        // 초기 위치 결정 : mapRect 있으면 그 값, 없으면 manualMapPosition
        Vector2 initPos = _hasMapRect ? mapRect.anchoredPosition : manualMapPosition;

        _currentPosition = initPos;
        _targetPosition  = initPos;
        _currentScale    = defaultScale;
        _targetScale     = defaultScale;

        ApplyMinimapTransform(initPos, defaultScale);

        // 카메라가 연결된 경우 현재 카메라 상태로 미니맵 역산 초기화
        if (orthoCamera != null && enableCameraSync)
            SyncFromCamera();
    }

    // ═══════════════════════════════════════════════════════
    // Update
    // ═══════════════════════════════════════════════════════

    private void Update()
    {
        HandleMouseScroll();
        HandleTouchPinch();
        HandleInertia();
        SmoothApply();
    }

    // ═══════════════════════════════════════════════════════
    // 입력 처리
    // ═══════════════════════════════════════════════════════

    private void HandleMouseScroll()
    {
        if (!IsPointerOverInputZone()) return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.001f) return;

        float newScale = _targetScale * (1f + scroll * mouseScrollSensitivity);
        ZoomTo(Mathf.Clamp(newScale, minScale, maxScale), GetLocalMousePos());
    }

    private void HandleTouchPinch()
    {
        if (Input.touchCount != 2)
        {
            _isPinching = false;
            return;
        }

        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        if (!_isPinching)
        {
            // 두 터치 포인트가 모두 입력 허용 영역 안에 있어야 핀치를 시작합니다.
            bool t0Inside = IsTouchInsideInputZone(t0.position);
            bool t1Inside = IsTouchInsideInputZone(t1.position);
            if (!t0Inside || !t1Inside) return;

            _pinchStartDist  = Vector2.Distance(t0.position, t1.position);
            _pinchStartScale = _targetScale;
            _pinchCenter     = (t0.position + t1.position) * 0.5f;
            _isPinching      = true;
            _isDragging      = false;
            return;
        }

        float currentDist = Vector2.Distance(t0.position, t1.position);
        if (_pinchStartDist < 0.1f) return;

        float ratio    = currentDist / _pinchStartDist;
        float newScale = Mathf.Clamp(_pinchStartScale * ratio, minScale, maxScale);

        // 핀치 중심점 좌표는 _viewportRect 기준 로컬로 변환 (줌 기준점)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _viewportRect, _pinchCenter, GetRenderCamera(), out Vector2 localCenter);

        ZoomTo(newScale, localCenter);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isPinching) return;

        // 드래그 시작 지점이 입력 허용 영역 안에 있어야 합니다.
        if (!IsPointerEventInsideInputZone(eventData)) return;

        _isDragging   = true;
        _lastDragPos  = eventData.position;
        _dragVelocity = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _isPinching) return;

        Vector2 delta = eventData.position - _lastDragPos;
        _lastDragPos  = eventData.position;

        float canvasScale = _canvas != null ? _canvas.scaleFactor : 1f;
        delta /= canvasScale;

        _targetPosition += delta * dragSensitivity;
        _dragVelocity    = delta * dragSensitivity;

        ClampPosition();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }

    // ═══════════════════════════════════════════════════════
    // 줌 (기준점 고정)
    // ═══════════════════════════════════════════════════════

    private void ZoomTo(float newScale, Vector2 pivotLocalPos)
    {
        float scaleDelta = newScale / _targetScale;
        _targetPosition  = pivotLocalPos + (_targetPosition - pivotLocalPos) * scaleDelta;
        _targetScale     = newScale;
        ClampPosition();
    }

    // ═══════════════════════════════════════════════════════
    // 관성
    // ═══════════════════════════════════════════════════════

    private void HandleInertia()
    {
        if (!enableInertia || _isDragging || _isPinching) return;
        if (_dragVelocity.sqrMagnitude < 0.01f)
        {
            _dragVelocity = Vector2.zero;
            return;
        }

        _targetPosition += _dragVelocity;
        _dragVelocity   *= inertiaFriction;
        ClampPosition();
    }

    // ═══════════════════════════════════════════════════════
    // 경계 클램프
    // ═══════════════════════════════════════════════════════

    private void ClampPosition()
    {
        if (!clampToBounds) return;

        // mapRect 연결 여부에 관계없이 MapSize / ViewportSize 프로퍼티 사용
        Vector2 vp  = ViewportSize;
        Vector2 map = MapSize * _targetScale;

        float halfVW = vp.x  * 0.5f;
        float halfVH = vp.y  * 0.5f;
        float halfMW = map.x * 0.5f;
        float halfMH = map.y * 0.5f;
        float pad    = boundsPadding;

        float minX, maxX, minY, maxY;

        if (map.x <= vp.x) { minX = maxX = 0f; }
        else { minX = -(halfMW - halfVW) - pad; maxX = (halfMW - halfVW) + pad; }

        if (map.y <= vp.y) { minY = maxY = 0f; }
        else { minY = -(halfMH - halfVH) - pad; maxY = (halfMH - halfVH) + pad; }

        _targetPosition.x = Mathf.Clamp(_targetPosition.x, minX, maxX);
        _targetPosition.y = Mathf.Clamp(_targetPosition.y, minY, maxY);
    }

    // ═══════════════════════════════════════════════════════
    // Lerp 적용 (미니맵 UI + 카메라)
    // ═══════════════════════════════════════════════════════

    private void SmoothApply()
    {
        float dt = Time.unscaledDeltaTime;

        _currentScale = Mathf.Lerp(_currentScale, _targetScale,   dt * zoomLerpSpeed);
        _currentPosition = Vector2.Lerp(_currentPosition, _targetPosition, dt * moveLerpSpeed);

        ApplyMinimapTransform(_currentPosition, _currentScale);

        if (enableCameraSync && orthoCamera != null)
            SyncCameraFromMinimap(_currentPosition, _currentScale);
    }

    // ═══════════════════════════════════════════════════════
    // 미니맵 트랜스폼 적용
    // mapRect 연결 → RectTransform 직접 조작
    // mapRect 미연결 → 내부 상태값만 유지 (외부에서 CurrentPosition/CurrentScale 로 읽음)
    // ═══════════════════════════════════════════════════════

    private void ApplyMinimapTransform(Vector2 position, float scale)
    {
        if (_hasMapRect)
        {
            mapRect.anchoredPosition = position;
            mapRect.localScale       = Vector3.one * scale;
        }
        // mapRect 미연결 : _currentPosition / _currentScale 자체가 상태이므로
        // 외부에서 CurrentPosition, CurrentScale 을 읽어 직접 처리합니다.
    }

    // ═══════════════════════════════════════════════════════
    // 카메라 동기화
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// 미니맵 position / scale → 직교 카메라 위치 / orthographicSize 변환.
    ///
    /// 【변환 원리】
    ///   맵을 +X 방향으로 밀면 카메라는 -X 방향의 지역을 바라봄 → 부호 반전
    ///     worldOffset = -(pos / scale) × pixelToWorldScale
    ///
    ///   orthoSize 는 orthoSizeMode 에 따라 결정됩니다.
    ///     Linear  : scale ∈ [minScale, maxScale] → orthoSize ∈ [maxOrthoSize, minOrthoSize] 역선형
    ///     Inverse : 기하평균 기준 역비례 k / scale
    /// </summary>
    private void SyncCameraFromMinimap(Vector2 minimapPos, float minimapScale)
    {
        // ── 1. orthographicSize 계산 ──────────────────────────────
        //
        //  [Linear 모드]
        //    scale 이 minScale 일 때 orthoSize = maxOrthoSize (줌아웃 최대)
        //    scale 이 maxScale 일 때 orthoSize = minOrthoSize (줌인 최대)
        //    → scale 을 [minScale, maxScale] 구간에서 [maxOrthoSize, minOrthoSize] 로 역선형 매핑
        //
        //  [Inverse 모드]
        //    baseOrthoSize = √(minOrthoSize × maxOrthoSize) 기하평균을 기준으로
        //    orthoSize = baseOrthoSize² / (orthoSize at defaultScale)
        //    실제로는 minScale↔maxOrthoSize, maxScale↔minOrthoSize 를 동시에 만족하는
        //    역비례 상수 k = minOrthoSize × maxScale = maxOrthoSize × minScale 로 계산합니다.
        //
        float targetSize;
        if (orthoSizeMode == OrthoSizeMode.Linear)
        {
            // t=0 → minScale(줌아웃) → maxOrthoSize
            // t=1 → maxScale(줌인)   → minOrthoSize
            float t = Mathf.InverseLerp(minScale, maxScale, minimapScale);
            targetSize = Mathf.Lerp(maxOrthoSize, minOrthoSize, t);
        }
        else // Inverse
        {
            // k = minOrthoSize * maxScale  (= maxOrthoSize * minScale 와 동일하게 맞추려면
            //   Inspector 에서 minOrthoSize * maxScale == maxOrthoSize * minScale 이어야 완벽하게 맞음)
            // 일반적으로는 두 경계값의 기하평균 기준 역비례를 사용합니다.
            float k = Mathf.Sqrt(minOrthoSize * maxOrthoSize) * Mathf.Sqrt(minScale * maxScale);
            targetSize = k / minimapScale;
            targetSize = Mathf.Clamp(targetSize, minOrthoSize, maxOrthoSize);
        }

        // 2. 픽셀 오프셋 → 월드 거리
        float worldOffsetU = -(minimapPos.x / minimapScale) * pixelToWorldScale;
        float worldOffsetV = -(minimapPos.y / minimapScale) * pixelToWorldScale;

        if (invertMapX) worldOffsetU = -worldOffsetU;
        if (invertMapY) worldOffsetV = -worldOffsetV;

        // 3. 축 배분 (매핑 안 된 축 = cameraFixedAxisValue)
        float wx = cameraFixedAxisValue;
        float wy = cameraFixedAxisValue;
        float wz = cameraFixedAxisValue;

        switch (mapXToWorldAxis)
        {
            case WorldAxis.X: wx = worldOffsetU; break;
            case WorldAxis.Y: wy = worldOffsetU; break;
            case WorldAxis.Z: wz = worldOffsetU; break;
        }
        switch (mapYToWorldAxis)
        {
            case WorldAxis.X: wx = worldOffsetV; break;
            case WorldAxis.Y: wy = worldOffsetV; break;
            case WorldAxis.Z: wz = worldOffsetV; break;
        }

        Vector3 targetPos = new Vector3(wx, wy, wz);

        // 4. Lerp 적용
        float dt = Time.unscaledDeltaTime;

        orthoCamera.transform.position =
            cameraMoveLerpSpeed > 0f
                ? Vector3.Lerp(orthoCamera.transform.position, targetPos, dt * cameraMoveLerpSpeed)
                : targetPos;

        orthoCamera.orthographicSize =
            cameraSizeLerpSpeed > 0f
                ? Mathf.Lerp(orthoCamera.orthographicSize, targetSize, dt * cameraSizeLerpSpeed)
                : targetSize;
    }

    // ═══════════════════════════════════════════════════════
    // 유틸리티
    // ═══════════════════════════════════════════════════════

    // ═══════════════════════════════════════════════════════
    // 입력 영역 히트 테스트
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// 마우스 커서가 입력 허용 영역(_inputZoneRect) 안에 있는지 확인합니다.
    /// 마우스 휠 처리에 사용됩니다.
    /// </summary>
    private bool IsPointerOverInputZone()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            _inputZoneRect, Input.mousePosition, GetRenderCamera());
    }

    /// <summary>
    /// PointerEventData 의 스크린 좌표가 입력 허용 영역 안에 있는지 확인합니다.
    /// OnPointerDown (드래그 시작) 에 사용됩니다.
    /// </summary>
    private bool IsPointerEventInsideInputZone(PointerEventData eventData)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            _inputZoneRect, eventData.position, GetRenderCamera());
    }

    /// <summary>
    /// 터치 스크린 좌표가 입력 허용 영역 안에 있는지 확인합니다.
    /// 핀치 시작 판정에 사용됩니다.
    /// </summary>
    private bool IsTouchInsideInputZone(Vector2 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            _inputZoneRect, screenPos, GetRenderCamera());
    }

    /// <summary>
    /// 마우스 위치를 뷰포트(_viewportRect) 로컬 좌표로 변환합니다.
    /// 줌 기준점 계산에 사용됩니다.
    /// </summary>
    private Vector2 GetLocalMousePos()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _viewportRect, Input.mousePosition, GetRenderCamera(), out Vector2 localPos);
        return localPos;
    }

    private Camera GetRenderCamera()
    {
        if (_canvas == null) return null;
        return _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
    }

    private static float GetAxisValue(Vector3 v, WorldAxis axis) => axis switch
    {
        WorldAxis.X => v.x,
        WorldAxis.Y => v.y,
        WorldAxis.Z => v.z,
        _           => 0f,
    };

    // ═══════════════════════════════════════════════════════
    // 공개 API
    // ═══════════════════════════════════════════════════════

    /// <summary>미니맵과 카메라를 초기 상태(위치·줌)로 리셋합니다.</summary>
    public void ResetView()
    {
        _targetPosition  = _hasMapRect ? mapRect.anchoredPosition : manualMapPosition;
        _targetScale     = defaultScale;
        _dragVelocity    = Vector2.zero;
    }

    /// <summary>미니맵을 특정 anchoredPosition 으로 이동합니다.</summary>
    public void SetPosition(Vector2 localPosition)
    {
        _targetPosition = localPosition;
        _dragVelocity   = Vector2.zero;
        ClampPosition();
    }

    /// <summary>미니맵 스케일(줌)을 직접 설정합니다.</summary>
    public void SetScale(float scale)
    {
        _targetScale = Mathf.Clamp(scale, minScale, maxScale);
        ClampPosition();
    }

    /// <summary>
    /// 런타임에서 입력 허용 영역을 변경합니다.
    /// null 을 전달하면 boundsRect(또는 이 오브젝트)로 폴백됩니다.
    /// </summary>
    public void SetInputZone(RectTransform zone)
    {
        inputRect      = zone;
        _inputZoneRect = zone != null ? zone : _viewportRect;

        Debug.Log("[MinimapController] 입력 영역 변경 → " +
                  (_inputZoneRect != null ? _inputZoneRect.name : "none"));
    }

    /// <summary>
    /// 입력 허용 영역을 해제하고 기본 뷰포트(_viewportRect)로 되돌립니다.
    /// </summary>
    public void ClearInputZone()
    {
        SetInputZone(null);
    }

    /// <summary>현재 유효한 입력 허용 영역 RectTransform (읽기 전용)</summary>
    public RectTransform InputZoneRect => _inputZoneRect;

    /// <summary>
    /// mapRect 가 null 일 때 런타임에서 맵 크기를 변경합니다.
    /// 경계 클램프 재계산이 즉시 반영됩니다.
    /// </summary>
    public void SetManualMapSize(Vector2 size)
    {
        manualMapSize = size;
        ClampPosition();
    }

    /// <summary>
    /// boundsRect 가 null 이고 manualViewportSize 를 사용 중일 때
    /// 런타임에서 뷰포트 크기를 변경합니다.
    /// </summary>
    public void SetManualViewportSize(Vector2 size)
    {
        manualViewportSize = size;
        ClampPosition();
    }

    /// <summary>
    /// 카메라의 현재 월드 위치 / orthographicSize 를 기준으로
    /// 미니맵 상태를 역산하여 동기화합니다.
    /// </summary>
    public void SyncFromCamera()
    {
        if (orthoCamera == null) return;

        Vector3 camPos = orthoCamera.transform.position;

        float worldU = GetAxisValue(camPos, mapXToWorldAxis);
        float worldV = GetAxisValue(camPos, mapYToWorldAxis);

        if (invertMapX) worldU = -worldU;
        if (invertMapY) worldV = -worldV;

        // orthographicSize → scale 역산 (Linear: 선형 역산, Inverse: 역비례 역산)
        float currentOrthoSize = orthoCamera.orthographicSize;
        float newScale;
        if (orthoSizeMode == OrthoSizeMode.Linear)
        {
            float t = Mathf.InverseLerp(maxOrthoSize, minOrthoSize, currentOrthoSize);
            newScale = Mathf.Lerp(minScale, maxScale, t);
        }
        else
        {
            float k = Mathf.Sqrt(minOrthoSize * maxOrthoSize) * Mathf.Sqrt(minScale * maxScale);
            newScale = k / currentOrthoSize;
        }
        newScale = Mathf.Clamp(newScale, minScale, maxScale);

        float px = -worldU * newScale / pixelToWorldScale;
        float py = -worldV * newScale / pixelToWorldScale;

        _targetScale    = newScale;
        _targetPosition = new Vector2(px, py);
        _dragVelocity   = Vector2.zero;
        ClampPosition();
    }

    // ── 읽기 전용 상태 ────────────────────────────────────

    /// <summary>
    /// 현재 렌더링 중인 미니맵 anchoredPosition.
    /// mapRect 미연결 시에도 내부 상태값을 반환합니다.
    /// </summary>
    public Vector2 CurrentPosition => _currentPosition;

    /// <summary>현재 렌더링 중인 미니맵 스케일 값</summary>
    public float CurrentScale => _currentScale;

    /// <summary>현재 카메라 목표 orthographicSize (현재 모드 기준으로 계산)</summary>
    public float TargetOrthoSize
    {
        get
        {
            if (orthoCamera == null) return 0f;
            if (orthoSizeMode == OrthoSizeMode.Linear)
            {
                float t = Mathf.InverseLerp(minScale, maxScale, _targetScale);
                return Mathf.Lerp(maxOrthoSize, minOrthoSize, t);
            }
            else
            {
                float k = Mathf.Sqrt(minOrthoSize * maxOrthoSize) * Mathf.Sqrt(minScale * maxScale);
                return Mathf.Clamp(k / _targetScale, minOrthoSize, maxOrthoSize);
            }
        }
    }

    /// <summary>mapRect 가 실제로 연결되어 있는지 여부</summary>
    public bool HasMapRect => _hasMapRect;

    /// <summary>boundsRect 가 실제로 연결되어 있는지 여부</summary>
    public bool HasBoundsRect => _hasBoundsRect;
}
