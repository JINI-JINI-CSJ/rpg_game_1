using UnityEngine;

namespace TilemapTool
{
    /// <summary>
    /// XZ 평면을 내려다보는 탑다운 카메라용 이동/줌 컨트롤러.
    /// Orthographic / Perspective 카메라 모두 지원.
    /// 타일맵 툴의 카메라(RuntimeTilemapEditor.targetCamera)에 그대로 붙여서 사용.
    ///
    /// 조작법
    ///   WASD / 방향키   : 평면 이동
    ///   마우스 휠 클릭 드래그 : 패닝
    ///   마우스 휠 스크롤 : 줌 (Orthographic은 size, Perspective는 높이 조절)
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class TopDownCameraController : MonoBehaviour
    {
        [Header("이동")]
        public float panSpeed = 10f;
        public int dragButton = 2; // 0=좌클릭, 1=우클릭, 2=휠클릭
        public bool useEdgePan = false;
        public float edgePanBorder = 12f;

        [Header("줌")]
        public float zoomSpeed = 10f;
        public float minOrthoSize = 3f;
        public float maxOrthoSize = 60f;
        public float minHeight = 3f;
        public float maxHeight = 80f;

        private Camera cam;
        private Vector3 dragOrigin;
        private bool dragging;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            HandleKeyboardPan();
            HandleDragPan();
            if (useEdgePan) HandleEdgePan();
            HandleZoom();
        }

        private void HandleKeyboardPan()
        {
            float h = Input.GetAxisRaw("Horizontal"); // A/D, 좌우 화살표
            float v = Input.GetAxisRaw("Vertical");   // W/S, 상하 화살표
            if (h == 0f && v == 0f) return;

            float scale = GetSpeedScale();
            Vector3 move = new Vector3(h, 0f, v) * panSpeed * scale * Time.deltaTime;
            transform.position += move;
        }

        private void HandleDragPan()
        {
            if (Input.GetMouseButtonDown(dragButton))
            {
                dragging = true;
                dragOrigin = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(dragButton))
            {
                dragging = false;
            }

            if (!dragging) return;

            Vector3 currentMouse = Input.mousePosition;
            Vector3 diff = currentMouse - dragOrigin;
            dragOrigin = currentMouse;

            float scale = GetSpeedScale();
            // 화면상 드래그 방향과 반대로 카메라를 이동시켜 "잡고 끄는" 느낌을 준다.
            Vector3 move = new Vector3(-diff.x, 0f, -diff.y) * 0.01f * scale;
            transform.position += move;
        }

        private void HandleEdgePan()
        {
            Vector2 mouse = Input.mousePosition;
            Vector3 move = Vector3.zero;

            if (mouse.x <= edgePanBorder) move.x -= 1f;
            if (mouse.x >= Screen.width - edgePanBorder) move.x += 1f;
            if (mouse.y <= edgePanBorder) move.z -= 1f;
            if (mouse.y >= Screen.height - edgePanBorder) move.z += 1f;

            if (move == Vector3.zero) return;

            float scale = GetSpeedScale();
            transform.position += move.normalized * panSpeed * scale * Time.deltaTime;
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Approximately(scroll, 0f)) return;

            if (cam.orthographic)
            {
                cam.orthographicSize = Mathf.Clamp(
                    cam.orthographicSize - scroll * zoomSpeed,
                    minOrthoSize, maxOrthoSize);
            }
            else
            {
                Vector3 pos = transform.position;
                pos.y = Mathf.Clamp(pos.y - scroll * zoomSpeed * 10f, minHeight, maxHeight);
                transform.position = pos;
            }
        }

        // 카메라가 멀리 있을수록(높이/사이즈가 클수록) 이동 속도를 비례해서 키운다.
        private float GetSpeedScale()
        {
            if (cam.orthographic)
                return Mathf.Max(1f, cam.orthographicSize / 10f);
            return Mathf.Max(1f, transform.position.y / 10f);
        }
    }
}
