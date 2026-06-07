using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// 이 스크립트가 붙어있는 게임 오브젝트의 BoxCollider를
/// 자신과 모든 자식 오브젝트의 MeshRenderer 경계를 모두 포함하도록 조절합니다.
/// </summary>
[RequireComponent(typeof(BoxCollider))] // 이 스크립트는 BoxCollider가 필요함을 명시합니다.
public class FitColliderToBounds : MonoBehaviour
{
    public float Add_Y = 0;

    public float MeshAddSize_XZ = 0;

    public List<MeshRenderer> mr_user;

    public GameObject go_mr_par;

    public BoxCollider fit_user;

    /// <summary>
    /// 인스펙터 창에서 이 컴포넌트의 ... (케밥 메뉴) 또는 마우스 우클릭 시 메뉴에 표시됩니다.
    /// </summary>
    [ContextMenu("자식 포함 모든 메쉬에 콜리더 맞추기")]
    public void AdjustColliderToBounds()
    {
        // 1. 이 게임 오브젝트에 붙어있는 BoxCollider를 가져옵니다.
        // RequireComponent 속성 덕분에 boxCollider는 null이 될 수 없습니다.
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        // 2. 자신과 모든 자식 오브젝트에 있는 MeshRenderer 컴포넌트를 전부 가져옵니다.
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        if ( mr_user != null && mr_user.Count > 0)
            meshRenderers = mr_user.ToArray();

        if (go_mr_par != null)
        {
            meshRenderers = go_mr_par.GetComponentsInChildren<MeshRenderer>();
        }

        if (meshRenderers.Length == 0)
            {
                Debug.LogWarning("하위 객체에서 MeshRenderer를 찾을 수 없습니다. 콜리더를 조절할 수 없습니다.", this);
                // 메쉬가 없으면 콜리더 크기를 0으로 초기화할 수도 있습니다.
                boxCollider.center = Vector3.zero;
                boxCollider.size = Vector3.zero;
                return;
            }

        // 3. 모든 메쉬를 감싸는 전체 경계(Bounds)를 계산합니다.
        // 첫 번째 활성화된 메쉬의 경계로 초기화합니다.
        Bounds totalBounds = new Bounds();
        bool hasInitializedBounds = false;

        foreach (var renderer in meshRenderers)
        {
            // 비활성화된 렌더러는 계산에서 제외합니다.
            if (!renderer.enabled)
            {
                continue;
            }

            if (!hasInitializedBounds)
            {
                // 첫 번째 유효한 렌더러의 경계로 전체 경계를 초기화합니다.
                totalBounds = renderer.bounds;
                hasInitializedBounds = true;
            }
            else
            {
                // 기존 경계(totalBounds)에 현재 렌더러의 경계(renderer.bounds)를 포함시킵니다.
                totalBounds.Encapsulate(renderer.bounds);
            }
        }

        // 만약 유효한 메쉬가 하나도 없었다면 경고를 출력하고 종료합니다.
        if (!hasInitializedBounds)
        {
            Debug.LogWarning("활성화된 MeshRenderer가 하나도 없습니다.", this);
            return;
        }

        // 4. 계산된 월드 공간 기준의 Bounds를 BoxCollider의 로컬 공간 기준으로 변환합니다.
        // - Center: 월드 좌표계의 중심점을 이 오브젝트의 로컬 좌표계로 변환합니다.
        // - Size: 월드 공간의 크기를 이 오브젝트의 로컬 스케일로 나누어 로컬 크기를 구합니다.
        boxCollider.center = transform.InverseTransformPoint(totalBounds.center);


        // y 추가 
        float off_y_center = Add_Y * 0.5f;
        Vector3 center_1 = boxCollider.center;
        center_1.y += off_y_center;
        boxCollider.center = center_1;


        Vector3 localSize = totalBounds.size;
        // 부모의 스케일 변환에 따른 오차를 줄이기 위해 lossyScale을 사용합니다.
        localSize.x /= transform.lossyScale.x;
        localSize.y /= transform.lossyScale.y;
        localSize.z /= transform.lossyScale.z;

        localSize.y += Add_Y;

        boxCollider.size = localSize;

        // 
        NavMeshObstacle navMeshObstacle = GetComponent<NavMeshObstacle>();
        if (navMeshObstacle != null)
        {
            navMeshObstacle.center = center_1;
            navMeshObstacle.size = localSize;
        }

        localSize.x += MeshAddSize_XZ;
        localSize.z += MeshAddSize_XZ;
        boxCollider.size = localSize;

        Debug.Log("콜리더 조절 완료! Center: " + boxCollider.center + ", Size: " + boxCollider.size, this);
    }

    [ContextMenu("네비 장애물만 맞추기")]
    public void Fit_NavMeshObstacle()
    {
        NavMeshObstacle navMeshObstacle = GetComponent<NavMeshObstacle>();

        if (fit_user != null && navMeshObstacle != null)
        {
            navMeshObstacle.center = fit_user.center;
            navMeshObstacle.size = fit_user.size;
        }
        else
        {
            Debug.Log( "박스 콜리더 , 네비 장애물 없음~~~~" );
        }
    }
}