using UnityEngine;

public static class SpherePenetrationResolver
{
    /// <summary>
    /// Sphere가 여러 콜라이더와 겹쳐 있을 경우
    /// ComputePenetration을 반복 사용하여
    /// 완전히 겹침이 해소되는 위치를 반환한다.
    /// </summary>
    public static Vector3 Resolve(
        Vector3 startPosition,
        float radius,
        int collisionMask,
        int maxIterations = 6
    )
    {
        Vector3 position = startPosition;

        SphereCollider sphere = GetVirtualSphere(radius);

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            bool anyPenetration = false;

            Collider[] overlaps = Physics.OverlapSphere(
                position,
                radius,
                collisionMask,
                QueryTriggerInteraction.Ignore
            );

            foreach (var other in overlaps)
            {
                if (Physics.ComputePenetration(
                    sphere,
                    position,
                    Quaternion.identity,
                    other,
                    other.transform.position,
                    other.transform.rotation,
                    out Vector3 direction,
                    out float distance
                ))
                {
                    // 최소 분리 벡터 적용
                    position += direction * distance;
                    anyPenetration = true;
                }
            }

            // 더 이상 겹침이 없다면 종료
            if (!anyPenetration)
                break;
        }

        return position;
    }

    // 가상 SphereCollider (1회 생성 후 재사용)
    private static SphereCollider _virtualSphere;

    private static SphereCollider GetVirtualSphere(float radius)
    {
        if (_virtualSphere == null)
        {
            GameObject go = new GameObject("VirtualSphereCollider");
            go.hideFlags = HideFlags.HideAndDontSave;
            _virtualSphere = go.AddComponent<SphereCollider>();
        }

        _virtualSphere.radius = radius;
        _virtualSphere.center = Vector3.zero;
        return _virtualSphere;
    }
}
