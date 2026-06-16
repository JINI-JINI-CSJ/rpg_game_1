using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스카이림 스타일 컴파스 HUD 시스템
/// 캔버스 없이 OnGUI 방식으로 동작 (빠른 프로토타이핑)
/// 또는 RectTransform 기반 UI로 변환 가능
/// </summary>
public class CompassHUD : MonoBehaviour
{
    [Header("References")]
    [Tooltip("추적할 플레이어 Transform")]
    public Transform playerTransform;

    [Tooltip("추적할 카메라 Transform (null이면 Camera.main 사용)")]
    public Transform cameraTransform;

    [Header("Compass Visual Settings")]
    [Tooltip("컴파스 바의 화면 Y 위치 (0~1, 0=위)")]
    [Range(0f, 0.15f)]
    public float compassYPosition = 0.02f;

    [Tooltip("컴파스 바의 너비 (화면 너비의 비율)")]
    [Range(0.3f, 1f)]
    public float compassWidthRatio = 0.6f;

    [Tooltip("컴파스 바의 높이 (픽셀)")]
    public float compassHeight = 48f;

    [Tooltip("마커 아이콘 크기")]
    public float markerSize = 24f;

    [Tooltip("방위 텍스트 크기")]
    public int cardinalFontSize = 14;

    [Header("Compass Field of View")]
    [Tooltip("컴파스에 표시되는 시야각 (도). 180 = 절반 세계, 90 = 좁게")]
    [Range(60f, 180f)]
    public float compassFOV = 120f;

    [Header("Marker Icons (선택적 커스텀 텍스처)")]
    public Texture2D questMarkerTexture;
    public Texture2D enemyMarkerTexture;
    public Texture2D npcMarkerTexture;
    public Texture2D locationMarkerTexture;
    public Texture2D waypointMarkerTexture;

    // 런타임 마커 목록
    private List<CompassMarker> _markers = new List<CompassMarker>();

    // 방위 레이블
    private static readonly (float angle, string label)[] Cardinals = new[]
    {
        (0f,   "N"),
        (45f,  "NE"),
        (90f,  "E"),
        (135f, "SE"),
        (180f, "S"),
        (225f, "SW"),
        (270f, "W"),
        (315f, "NW"),
    };

    // GUI 스타일 캐시
    private GUIStyle _labelStyle;
    private GUIStyle _markerStyle;
    private bool _stylesInitialized = false;

    // 컬러 팔레트
    private static readonly Color ColBackground  = new Color(0f,    0f,    0f,    0.55f);
    private static readonly Color ColBorder      = new Color(1f,    0.85f, 0.4f,  0.6f);
    private static readonly Color ColCardinal    = new Color(1f,    1f,    1f,    1f);
    private static readonly Color ColCardinalSub = new Color(1f,    1f,    1f,    0.5f);
    private static readonly Color ColTick        = new Color(1f,    1f,    1f,    0.3f);

    // ──────────────────────────────────────────────────────────
    //  공개 API
    // ──────────────────────────────────────────────────────────

    /// <summary>마커를 컴파스에 등록합니다.</summary>
    public CompassMarker AddMarker(Transform target, MarkerType type, string label = "")
    {
        var marker = new CompassMarker
        {
            target = target,
            type   = type,
            label  = label,
            isActive = true
        };
        _markers.Add(marker);
        return marker;
    }

    /// <summary>마커를 제거합니다.</summary>
    public void RemoveMarker(CompassMarker marker) => _markers.Remove(marker);

    /// <summary>모든 마커를 제거합니다.</summary>
    public void ClearMarkers() => _markers.Clear();

    // ──────────────────────────────────────────────────────────
    //  Unity 라이프사이클
    // ──────────────────────────────────────────────────────────

    private void Awake()
    {
        if (playerTransform == null)
            playerTransform = transform;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void OnGUI()
    {
        if (playerTransform == null) return;

        InitStyles();

        float playerYaw = GetPlayerYaw();

        float sw = Screen.width;
        float sh = Screen.height;

        float barWidth  = sw * compassWidthRatio;
        float barX      = (sw - barWidth) * 0.5f;
        float barY      = sh * compassYPosition;

        Rect barRect = new Rect(barX, barY, barWidth, compassHeight);

        DrawCompassBackground(barRect);
        DrawCardinalLabels(barRect, playerYaw);
        DrawMarkers(barRect, playerYaw);
        DrawCenterNeedle(barRect);
        DrawBorderLines(barRect);
    }

    // ──────────────────────────────────────────────────────────
    //  내부 드로잉 메서드
    // ──────────────────────────────────────────────────────────

    private void DrawCompassBackground(Rect bar)
    {
        GUI.color = ColBackground;
        GUI.DrawTexture(bar, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawBorderLines(Rect bar)
    {
        // 위, 아래 테두리 라인
        GUI.color = ColBorder;
        GUI.DrawTexture(new Rect(bar.x, bar.y,          bar.width, 1f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(bar.x, bar.yMax - 1f,  bar.width, 1f), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawCenterNeedle(Rect bar)
    {
        float cx = bar.x + bar.width * 0.5f;
        // 삼각형 역방향 노치 (아래를 가리키는 플레이어 방향 표시)
        GUI.color = new Color(1f, 0.85f, 0.3f, 1f);
        GUI.DrawTexture(new Rect(cx - 1f, bar.y, 2f, bar.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawCardinalLabels(Rect bar, float playerYaw)
    {
        float halfFOV = compassFOV * 0.5f;

        foreach (var (angle, label) in Cardinals)
        {
            float delta = Mathf.DeltaAngle(playerYaw, angle);

            if (Mathf.Abs(delta) > halfFOV) continue;

            float t = (delta / halfFOV) * 0.5f + 0.5f;   // 0(왼쪽) ~ 1(오른쪽)
            float x = bar.x + bar.width * t;

            bool isPrimary = label.Length == 1; // N E S W는 강조

            // 작은 틱 마크
            float tickH = isPrimary ? bar.height * 0.45f : bar.height * 0.25f;
            GUI.color = isPrimary ? ColBorder : ColTick;
            GUI.DrawTexture(new Rect(x - 0.5f, bar.yMax - tickH, 1f, tickH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // 텍스트 레이블
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = isPrimary ? cardinalFontSize : Mathf.RoundToInt(cardinalFontSize * 0.8f),
                fontStyle = isPrimary ? FontStyle.Bold : FontStyle.Normal,
                alignment = TextAnchor.UpperCenter,
                normal    = { textColor = isPrimary ? ColCardinal : ColCardinalSub }
            };

            float labelW = 28f;
            float labelH = 20f;
            GUI.Label(new Rect(x - labelW * 0.5f, bar.y + 4f, labelW, labelH), label, style);
        }
    }

    private void DrawMarkers(Rect bar, float playerYaw)
    {
        float halfFOV = compassFOV * 0.5f;

        for (int i = _markers.Count - 1; i >= 0; i--)
        {
            var m = _markers[i];
            if (m == null || m.target == null || !m.isActive) continue;

            // 플레이어 → 마커 방향 계산 (수평 평면)
            Vector3 dir = m.target.position - playerTransform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.001f) continue;

            float markerAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            if (markerAngle < 0f) markerAngle += 360f;

            float delta = Mathf.DeltaAngle(playerYaw, markerAngle);
            if (Mathf.Abs(delta) > halfFOV) continue;

            float t = (delta / halfFOV) * 0.5f + 0.5f;
            float x = bar.x + bar.width * t;
            float cy = bar.y + bar.height * 0.5f;

            float dist = Vector3.Distance(playerTransform.position, m.target.position);

            DrawSingleMarker(m, x, cy, dist);
        }
    }

    private void DrawSingleMarker(CompassMarker m, float x, float cy, float dist)
    {
        Color mc = GetMarkerColor(m.type);
        Texture2D tex = GetMarkerTexture(m.type);

        float half = markerSize * 0.5f;

        if (tex != null)
        {
            GUI.color = mc;
            GUI.DrawTexture(new Rect(x - half, cy - half, markerSize, markerSize), tex);
            GUI.color = Color.white;
        }
        else
        {
            // 텍스처가 없으면 기본 도형으로 폴백
            DrawFallbackIcon(m.type, x, cy, mc);
        }

        // 거리 텍스트 (마커 아래)
        if (dist >= 1f)
        {
            string distText = dist >= 1000f
                ? $"{dist / 1000f:F1}km"
                : $"{Mathf.RoundToInt(dist)}m";

            var distStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 9,
                alignment = TextAnchor.UpperCenter,
                normal    = { textColor = new Color(mc.r, mc.g, mc.b, 0.85f) }
            };
            GUI.Label(new Rect(x - 24f, cy + half + 2f, 48f, 16f), distText, distStyle);
        }

        // 레이블 텍스트
        if (!string.IsNullOrEmpty(m.label))
        {
            var lbStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter,
                normal    = { textColor = mc }
            };
            GUI.Label(new Rect(x - 40f, cy - half - 16f, 80f, 18f), m.label, lbStyle);
        }
    }

    private void DrawFallbackIcon(MarkerType type, float x, float cy, Color c)
    {
        float half = markerSize * 0.5f;
        GUI.color = c;

        switch (type)
        {
            case MarkerType.Quest:
                // 물음표 원형 배경
                DrawFilledCircle(x, cy, half * 0.9f, c);
                var qs = new GUIStyle(GUI.skin.label)
                {
                    fontSize  = Mathf.RoundToInt(markerSize * 0.6f),
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal    = { textColor = Color.white }
                };
                GUI.Label(new Rect(x - half, cy - half, markerSize, markerSize), "!", qs);
                break;

            case MarkerType.Enemy:
                // 빨간 마름모
                DrawDiamond(x, cy, half * 0.85f, c);
                break;

            case MarkerType.NPC:
                // 흰 원
                DrawFilledCircle(x, cy, half * 0.8f, c);
                break;

            case MarkerType.Location:
                // 삼각형 (핀 모양 근사)
                GUI.DrawTexture(new Rect(x - half * 0.6f, cy - half, markerSize * 0.6f, markerSize * 0.8f),
                    Texture2D.whiteTexture);
                break;

            case MarkerType.Waypoint:
                // 별(★) 문자 사용
                var ws = new GUIStyle(GUI.skin.label)
                {
                    fontSize  = Mathf.RoundToInt(markerSize * 0.8f),
                    alignment = TextAnchor.MiddleCenter,
                    normal    = { textColor = c }
                };
                GUI.Label(new Rect(x - half, cy - half, markerSize, markerSize), "★", ws);
                break;

            default:
                GUI.DrawTexture(new Rect(x - half * 0.5f, cy - half * 0.5f, half, half),
                    Texture2D.whiteTexture);
                break;
        }

        GUI.color = Color.white;
    }

    // ──────────────────────────────────────────────────────────
    //  유틸리티
    // ──────────────────────────────────────────────────────────

    private float GetPlayerYaw()
    {
        Transform t = cameraTransform != null ? cameraTransform : playerTransform;
        float yaw = t.eulerAngles.y;
        return yaw < 0f ? yaw + 360f : yaw;
    }

    private Color GetMarkerColor(MarkerType type)
    {
        return type switch
        {
            MarkerType.Quest     => new Color(1.0f, 0.85f, 0.2f, 1f),  // 황금
            MarkerType.Enemy     => new Color(0.9f, 0.2f,  0.2f, 1f),  // 빨강
            MarkerType.NPC       => new Color(0.3f, 0.8f,  1.0f, 1f),  // 파랑
            MarkerType.Location  => new Color(0.5f, 1.0f,  0.5f, 1f),  // 녹색
            MarkerType.Waypoint  => new Color(1.0f, 1.0f,  1.0f, 1f),  // 흰색
            _                   => Color.white
        };
    }

    private Texture2D GetMarkerTexture(MarkerType type)
    {
        return type switch
        {
            MarkerType.Quest    => questMarkerTexture,
            MarkerType.Enemy    => enemyMarkerTexture,
            MarkerType.NPC      => npcMarkerTexture,
            MarkerType.Location => locationMarkerTexture,
            MarkerType.Waypoint => waypointMarkerTexture,
            _                  => null
        };
    }

    private static void DrawFilledCircle(float cx, float cy, float r, Color c)
    {
        // OnGUI에서 원 근사: 작은 사각형을 중앙에 배치
        GUI.color = c;
        GUI.DrawTexture(new Rect(cx - r, cy - r, r * 2f, r * 2f), Texture2D.whiteTexture);
    }

    private static void DrawDiamond(float cx, float cy, float r, Color c)
    {
        // 45도 회전 사각형 근사 (OnGUI 한계로 사각형으로 표현)
        float s = r * 1.1f;
        GUI.color = c;

        Matrix4x4 prev = GUI.matrix;
        GUIUtility.RotateAroundPivot(45f, new Vector2(cx, cy));
        GUI.DrawTexture(new Rect(cx - s * 0.5f, cy - s * 0.5f, s, s), Texture2D.whiteTexture);
        GUI.matrix = prev;
    }

    private void InitStyles()
    {
        if (_stylesInitialized) return;
        _labelStyle  = new GUIStyle(GUI.skin.label);
        _markerStyle = new GUIStyle(GUI.skin.box);
        _stylesInitialized = true;
    }
}
