using UnityEngine;

/// <summary>
/// 씬에 배치된 오브젝트에 붙여서
/// 자동으로 CompassHUD에 마커를 등록/해제하는 컴포넌트.
///
/// 사용법:
///   1. 퀘스트 NPC, 적, 장소 오브젝트에 이 스크립트를 부착
///   2. CompassHUD 인스턴스를 연결하거나 FindObjectOfType으로 자동 탐색
/// </summary>
public class CompassMarkerRegistrar : MonoBehaviour
{
    [Header("Compass Settings")]
    public CompassHUD compassHUD;

    [Tooltip("이 오브젝트의 마커 타입")]
    public MarkerType markerType = MarkerType.Location;

    [Tooltip("컴파스에 표시될 짧은 이름 (빈 칸이면 미표시)")]
    public string markerLabel = "";

    [Tooltip("컴파스에 처음부터 표시 여부")]
    public bool showOnStart = true;

    // 내부 참조
    private CompassMarker _registeredMarker;

    private void Start()
    {
        if (compassHUD == null)
            compassHUD = FindObjectOfType<CompassHUD>();

        if (compassHUD == null)
        {
            Debug.LogWarning($"[CompassMarkerRegistrar] CompassHUD를 찾을 수 없습니다. ({gameObject.name})");
            return;
        }

        if (showOnStart)
            Register();
    }

    private void OnDestroy()
    {
        Unregister();
    }

    // ──────────────────────────────────────────────────────────
    //  공개 API
    // ──────────────────────────────────────────────────────────

    /// <summary>컴파스에 이 오브젝트의 마커를 표시합니다.</summary>
    public void Register()
    {
        if (_registeredMarker != null) return;
        if (compassHUD == null) return;

        _registeredMarker = compassHUD.AddMarker(transform, markerType, markerLabel);
    }

    /// <summary>컴파스에서 마커를 제거합니다.</summary>
    public void Unregister()
    {
        if (_registeredMarker == null || compassHUD == null) return;
        compassHUD.RemoveMarker(_registeredMarker);
        _registeredMarker = null;
    }

    /// <summary>마커 표시/숨김 토글.</summary>
    public void SetVisible(bool visible)
    {
        if (_registeredMarker != null)
            _registeredMarker.isActive = visible;
    }

    /// <summary>레이블 텍스트 변경.</summary>
    public void SetLabel(string newLabel)
    {
        markerLabel = newLabel;
        if (_registeredMarker != null)
            _registeredMarker.label = newLabel;
    }

    /// <summary>마커 타입 변경 (예: 퀘스트 완료 후 NPC 타입으로 전환).</summary>
    public void SetMarkerType(MarkerType newType)
    {
        markerType = newType;
        if (_registeredMarker != null)
            _registeredMarker.type = newType;
    }
}
