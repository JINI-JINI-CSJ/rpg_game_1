using UnityEngine;

/// <summary>
/// 컴파스에 표시할 마커의 타입
/// </summary>
public enum MarkerType
{
    Quest,      // 퀘스트 (황금 !)
    Enemy,      // 적      (빨간 ◆)
    NPC,        // NPC     (파란 ●)
    Location,   // 장소/스폿 (녹색 ▲)
    Waypoint    // 사용자 웨이포인트 (흰 ★)
}

/// <summary>
/// 컴파스 위에 표시할 단일 마커 데이터
/// </summary>
[System.Serializable]
public class CompassMarker
{
    [Tooltip("마커가 가리킬 Target Transform")]
    public Transform target;

    [Tooltip("마커 종류")]
    public MarkerType type;

    [Tooltip("마커 위에 표시할 짧은 이름 (선택)")]
    public string label;

    [Tooltip("false면 컴파스에서 숨김")]
    public bool isActive = true;

    // 런타임 전용 추가 데이터 (필요 시 확장)
    [System.NonSerialized] public object userData;
}
