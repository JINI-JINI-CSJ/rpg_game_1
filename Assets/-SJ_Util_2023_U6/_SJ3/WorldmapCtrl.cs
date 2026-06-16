using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// RectTransform 기반 미니맵 컨트롤러
/// - 마우스 드래그로 이동
/// - 마우스 휠로 줌 인/아웃 (Scale 조정)
/// - 특정 좌표를 부모 중앙으로 이동
/// 
/// 사용법:
/// 1. 미니맵 콘텐츠 RectTransform 오브젝트에 이 스크립트를 부착합니다.
/// 2. 해당 오브젝트는 클리핑 역할을 하는 부모(Mask 등) 안에 있어야 합니다.
/// </summary>
public class WorldmapCtrl : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
{
    [Header("줌 설정")]
    [Tooltip("최소 줌 스케일")]
    public float minScale = 0.5f;

    [Tooltip("최대 줌 스케일")]
    public float maxScale = 5f;

    [Tooltip("휠 한 틱당 줌 변화량")]
    public float zoomStep = 0.1f;

    [Tooltip("줌 스무딩 속도 (0이면 즉시 적용)")]
    public float zoomSmoothSpeed = 0f;

    [Header("드래그 설정")]
    [Tooltip("드래그 감도 배율")]
    public float dragSensitivity = 1f;

    // ── 내부 상태 ──────────────────────────────────────────────
    public RectTransform _rectTransform;   // 이 오브젝트의 RectTransform (미니맵 콘텐츠)
    private RectTransform _parentRect;      // 부모 RectTransform (뷰포트)
    private Canvas        _canvas;          // 최상위 Canvas

    private bool  _isDragging  = false;
    private float _targetScale;

    // ────────────────────────────────────────────────────────────

    void Awake()
    {
        if( _rectTransform == null )
        {
            _rectTransform = GetComponent<RectTransform>();            
        }


        _parentRect    = transform.parent as RectTransform;
        _canvas        = GetComponentInParent<Canvas>();

        if (_parentRect == null)
            Debug.LogWarning("[WorldmapCtrl] 부모 RectTransform을 찾을 수 없습니다.");
        if (_canvas == null)
            Debug.LogWarning("[WorldmapCtrl] Canvas를 찾을 수 없습니다.");

        _targetScale = _rectTransform.localScale.x;
    }

    void Update()
    {
        // 줌 스무딩 처리
        if (zoomSmoothSpeed > 0f)
        {
            float current = _rectTransform.localScale.x;
            float next    = Mathf.Lerp(current, _targetScale, Time.deltaTime * zoomSmoothSpeed);
            _rectTransform.localScale = Vector3.one * next;
        }
    }

    // ── IPointerDownHandler ──────────────────────────────────────
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }

    // ── IPointerUpHandler ───────────────────────────────────────
    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }

    // ── IDragHandler ────────────────────────────────────────────
    /// <summary>
    /// 드래그 이동: 현재 스케일을 반영하여 이동 거리를 보정합니다.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        // Canvas 스케일을 고려한 실제 델타
        float canvasScale = _canvas != null ? _canvas.scaleFactor : 1f;

        // 스크린 델타 → 로컬 델타 변환
        // 줌 스케일이 클수록 같은 픽셀 이동에 더 적게 이동해야 하므로 나눔
        float currentScale = _rectTransform.localScale.x;
        Vector2 delta = eventData.delta / canvasScale / currentScale * dragSensitivity;

        _rectTransform.anchoredPosition += delta;
    }

    // ── IScrollHandler ──────────────────────────────────────────
    /// <summary>
    /// 마우스 휠 줌: 마우스 포인터 위치를 기준으로 줌합니다.
    /// </summary>
    public void OnScroll(PointerEventData eventData)
    {
        float scrollDelta = eventData.scrollDelta.y;
        if (Mathf.Approximately(scrollDelta, 0f)) return;

        float oldScale = _rectTransform.localScale.x;
        float newScale = Mathf.Clamp(oldScale + scrollDelta * zoomStep, minScale, maxScale);
        _targetScale   = newScale;

        // 즉시 적용 모드
        if (zoomSmoothSpeed <= 0f)
        {
            _rectTransform.localScale = Vector3.one * newScale;
        }

        // 마우스 포인터 위치를 고정점으로 줌 보정
        AdjustPositionForZoom(eventData.position, oldScale, newScale);
    }

    // ── 줌 시 위치 보정 ──────────────────────────────────────────
    /// <summary>
    /// 줌할 때 포인터 위치가 콘텐츠 상에서 고정되도록 anchoredPosition을 보정합니다.
    /// </summary>
    private void AdjustPositionForZoom(Vector2 screenPointerPos, float oldScale, float newScale)
    {
        if (_parentRect == null || _canvas == null) return;

        // 스크린 좌표 → 부모 로컬 좌표
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRect,
            screenPointerPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 pointerLocalPos
        );

        // 포인터가 콘텐츠의 어느 위치에 해당하는지 계산 후 스케일 변화량만큼 보정
        Vector2 currentAnchoredPos = _rectTransform.anchoredPosition;
        Vector2 offset             = currentAnchoredPos - pointerLocalPos;
        float   scaleFactor        = newScale / oldScale;
        _rectTransform.anchoredPosition = pointerLocalPos + offset * scaleFactor;
    }

    // ── 공개 API ────────────────────────────────────────────────

    /// <summary>
    /// 지정한 콘텐츠 로컬 좌표가 부모(뷰포트)의 중앙에 오도록 이동합니다.
    /// </summary>
    /// <param name="contentLocalPos">
    /// 미니맵 콘텐츠(이 오브젝트)의 로컬 좌표 기준 목표 위치
    /// (예: 월드 맵 위에 찍힌 핀의 로컬 좌표)
    /// </param>
    public void CenterOn(Vector2 contentLocalPos)
    {
        // 목표 anchoredPosition:
        //   부모 중앙(0,0)에 contentLocalPos가 오려면
        //   anchoredPosition = -(contentLocalPos * scale)
        float scale = _rectTransform.localScale.x;
        _rectTransform.anchoredPosition = -contentLocalPos * scale;
    }

    /// <summary>
    /// 지정한 콘텐츠 로컬 좌표로 이동하면서 줌 스케일도 함께 변경합니다.
    /// </summary>
    public void CenterOn(Vector2 contentLocalPos, float newScale)
    {
        newScale     = Mathf.Clamp(newScale, minScale, maxScale);
        _targetScale = newScale;

        if (zoomSmoothSpeed <= 0f)
            _rectTransform.localScale = Vector3.one * newScale;

        _rectTransform.anchoredPosition = -contentLocalPos * newScale;
    }

    /// <summary>
    /// 현재 스케일을 반환합니다.
    /// </summary>
    public float CurrentScale => _rectTransform.localScale.x;

    /// <summary>
    /// 스케일을 직접 설정합니다.
    /// </summary>
    public void SetScale(float scale)
    {
        _targetScale = Mathf.Clamp(scale, minScale, maxScale);
        if (zoomSmoothSpeed <= 0f)
            _rectTransform.localScale = Vector3.one * _targetScale;
    }

    /// <summary>
    /// 미니맵을 초기 위치(0,0) 및 스케일(1)로 리셋합니다.
    /// </summary>
    public void ResetView()
    {
        _rectTransform.anchoredPosition = Vector2.zero;
        _targetScale = 1f;
        if (zoomSmoothSpeed <= 0f)
            _rectTransform.localScale = Vector3.one;
    }
}
