using UnityEngine;

/// <summary>
/// CompassHUD 테스트용 데모 스크립트.
/// 빈 씬에 이 스크립트를 가진 GameObject 하나와
/// CompassHUD 컴포넌트를 가진 또 다른 GameObject를 놓으면
/// 플레이어가 돌아다니며 컴파스를 확인할 수 있습니다.
///
/// 조작: WASD 이동, QE 또는 마우스 왼쪽 드래그로 회전
/// </summary>
[RequireComponent(typeof(CompassHUD))]
public class CompassHUDDemo : MonoBehaviour
{
    [Header("Demo 설정")]
    [Tooltip("플레이어 이동 속도 (m/s)")]
    public float moveSpeed = 5f;

    [Tooltip("마우스 회전 감도")]
    public float rotationSpeed = 120f;

    private CompassHUD _hud;
    private Transform  _playerProxy;  // 데모용 플레이어 오브젝트
    private float      _yaw;

    private void Start()
    {
        _hud = GetComponent<CompassHUD>();

        // ── 플레이어 프록시 생성 ──
        _playerProxy = new GameObject("DemoPlayer").transform;
        _playerProxy.position = Vector3.zero;

        _hud.playerTransform  = _playerProxy;
        _hud.cameraTransform  = _playerProxy; // 카메라와 플레이어 방향 동기화

        // ── 마커 등록 ──
        AddDemoMarker("Dragon's Lair",   new Vector3(30f,  0f, 20f),  MarkerType.Quest,    "용의 소굴");
        AddDemoMarker("Black Knight",    new Vector3(-20f, 0f, 35f),  MarkerType.Enemy,    "흑기사");
        AddDemoMarker("Merchant Elara",  new Vector3(15f,  0f, -25f), MarkerType.NPC,      "엘라라");
        AddDemoMarker("Ancient Ruins",   new Vector3(-40f, 0f, -10f), MarkerType.Location, "고대 유적");
        AddDemoMarker("Waypoint Alpha",  new Vector3(60f,  0f, 60f),  MarkerType.Waypoint, "");

        Debug.Log("[CompassHUDDemo] WASD로 이동, Q/E 또는 마우스 드래그로 회전.");
    }

    private void Update()
    {
        HandleInput();
    }

    // ──────────────────────────────────────────────────────────
    //  입력 처리
    // ──────────────────────────────────────────────────────────

    private void HandleInput()
    {
        // 회전: Q/E 키 또는 마우스 X 축
        float rotInput = 0f;

        if (Input.GetKey(KeyCode.Q)) rotInput -= 1f;
        if (Input.GetKey(KeyCode.E)) rotInput += 1f;

        if (Input.GetMouseButton(0))
            rotInput += Input.GetAxis("Mouse X");

        _yaw += rotInput * rotationSpeed * Time.deltaTime;
        _playerProxy.rotation = Quaternion.Euler(0f, _yaw, 0f);

        // 이동: WASD (플레이어가 바라보는 방향 기준)
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) move -= Vector3.forward;
        if (Input.GetKey(KeyCode.A)) move -= Vector3.right;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        if (move.sqrMagnitude > 0f)
        {
            move = _playerProxy.TransformDirection(move.normalized);
            _playerProxy.position += move * (moveSpeed * Time.deltaTime);
        }
    }

    // ──────────────────────────────────────────────────────────
    //  헬퍼
    // ──────────────────────────────────────────────────────────

    private void AddDemoMarker(string goName, Vector3 worldPos, MarkerType type, string label)
    {
        var go = new GameObject(goName);
        go.transform.position = worldPos;
        _hud.AddMarker(go.transform, type, label);
    }
}
