using UnityEngine;

public class SJ_SimpleBillboard : MonoBehaviour
{

    public enum BillboardMode
    {
        Full,       // 완전 빌보드 (카메라를 정면으로 완전히 바라봄)
        YAxis,      // Y축 고정 빌보드 (가장 흔히 쓰이는 방식, 나무/캐릭터 스프라이트 등)
        XAxis,      // X축 고정 빌보드
        ZAxis       // Z축 고정 빌보드
    }
 
    [Header("빌보드 설정")]
    [Tooltip("빌보드 방식을 선택하세요.")]
    public BillboardMode mode = BillboardMode.YAxis;
 
    [Tooltip("비워두면 자동으로 Camera.main을 사용합니다.")]
    public Camera targetCamera;
 
    [Tooltip("true일 경우 카메라의 반대 방향(등)을 바라봅니다. 평면 스프라이트가 뒤집혀 보일 때 체크하세요.")]
    public bool flip = false;
 
    [Tooltip("LateUpdate에서 매 프레임 갱신할지 여부. 끄면 수동으로 UpdateBillboard()를 호출해야 합니다.")]
    public bool autoUpdate = true;
 
    private Transform _camTransform;
 
    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
 
        if (targetCamera != null)
            _camTransform = targetCamera.transform;
        else
            Debug.LogWarning($"[Billboard] {name}: 대상 카메라를 찾을 수 없습니다. targetCamera를 직접 할당해주세요.");
    }
 
    private void LateUpdate()
    {
        if (autoUpdate)
            UpdateBillboard();
    }
 
    /// <summary>
    /// 빌보드 회전을 갱신합니다. autoUpdate가 꺼져 있을 때 외부에서 직접 호출할 수 있습니다.
    /// </summary>
    public void UpdateBillboard()
    {
        if (_camTransform == null)
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
 
            if (targetCamera == null)
                return;
 
            _camTransform = targetCamera.transform;
        }
 
        switch (mode)
        {
            case BillboardMode.Full:
                ApplyFullBillboard();
                break;
 
            case BillboardMode.YAxis:
                ApplyAxisBillboard(Vector3.up);
                break;
 
            case BillboardMode.XAxis:
                ApplyAxisBillboard(Vector3.right);
                break;
 
            case BillboardMode.ZAxis:
                ApplyAxisBillboard(Vector3.forward);
                break;
        }
    }
 
    // 완전 빌보드: 카메라를 향해 모든 축이 정렬됨
    private void ApplyFullBillboard()
    {
        Vector3 dirToCamera = _camTransform.position - transform.position;
 
        if (dirToCamera.sqrMagnitude < 0.0001f)
            return;
 
        if (flip)
            dirToCamera = -dirToCamera;
 
        transform.rotation = Quaternion.LookRotation(-dirToCamera, _camTransform.up);
        // 참고: 스프라이트 기준 앞면이 +Z가 아니라면 -dirToCamera 대신 dirToCamera로 바꿔서 테스트해보세요.
    }
 
    // 특정 축을 고정한 채 나머지 축만 카메라를 향해 회전
    private void ApplyAxisBillboard(Vector3 lockedAxisLocal)
    {
        // 고정할 축을 월드 공간 기준으로 변환 (부모 회전이 있을 경우 대비)
        Vector3 lockedAxisWorld = transform.parent != null
            ? transform.parent.TransformDirection(lockedAxisLocal)
            : lockedAxisLocal;
 
        Vector3 dirToCamera = _camTransform.position - transform.position;
 
        // 고정 축 성분을 제거하여 해당 평면 위에서만 카메라를 바라보게 함
        Vector3 projected = Vector3.ProjectOnPlane(dirToCamera, lockedAxisWorld);
 
        if (projected.sqrMagnitude < 0.0001f)
            return;
 
        if (flip)
            projected = -projected;
 
        Quaternion targetRotation = Quaternion.LookRotation(-projected, lockedAxisWorld);
        transform.rotation = targetRotation;
    }


}
