using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class NavAgentStatus
{
    [Header("기본 상태")]
    public bool isOnNavMesh;
    public bool hasPath;
    public bool pathPending;
    public NavMeshPathStatus pathStatus;
    public bool isStopped;
    
    [Header("위치 정보")]
    public Vector3 destination;
    public Vector3 velocity;
    public float remainingDistance;
    public float stoppingDistance;
    
    [Header("경로 정보")]
    public int pathCornerCount;
    public Vector3 nextPosition;
    public Vector3 steeringTarget;
    
    [Header("에이전트 설정")]
    public float speed;
    public float angularSpeed;
    public float acceleration;
    public float radius;
    public float height;
    public int areaMask;
    public int agentTypeID;
    
    [Header("장애물 회피")]
    public float avoidancePriority;
    public ObstacleAvoidanceType obstacleAvoidanceType;
    
    [Header("오프메시 링크")]
    public bool isOnOffMeshLink;
    public OffMeshLinkData currentOffMeshLinkData;
    
    [Header("기타")]
    public bool autoRepath;
    public bool autoTraverseOffMeshLink;
    public bool autoBraking;
}

public class NavAgentInspector : MonoBehaviour
{
    [Header("NavMeshAgent 컴포넌트")]
    [SerializeField] private NavMeshAgent navAgent;
    
    [Header("실시간 상태 (읽기 전용)")]
    [SerializeField] private NavAgentStatus status = new NavAgentStatus();
    
    [Header("디버그 옵션")]
    [SerializeField] private bool showPathInScene = true;
    [SerializeField] private bool showVelocityGizmo = true;
    [SerializeField] private bool showDestinationGizmo = true;
    [SerializeField] private Color pathColor = Color.yellow;
    [SerializeField] private Color velocityColor = Color.red;
    [SerializeField] private Color destinationColor = Color.green;
    
    void Start()
    {
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();
            
        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent 컴포넌트를 찾을 수 없습니다!");
            enabled = false;
        }
    }
    
    void Update()
    {
        if (navAgent != null)
            UpdateStatus();
    }
    
    void UpdateStatus()
    {
        // 기본 상태

        if( navAgent.enabled == false )
        {
            return;
        }

        status.isOnNavMesh = navAgent.isOnNavMesh;
        status.hasPath = navAgent.hasPath;
        status.pathPending = navAgent.pathPending;
        status.pathStatus = navAgent.pathStatus;
        status.isStopped = navAgent.isStopped;
        
        // 위치 정보
        status.destination = navAgent.destination;
        status.velocity = navAgent.velocity;
        status.remainingDistance = navAgent.remainingDistance;
        status.stoppingDistance = navAgent.stoppingDistance;
        
        // 경로 정보
        if (navAgent.path != null)
        {
            status.pathCornerCount = navAgent.path.corners.Length;
        }
        
        if (navAgent.isOnNavMesh)
        {
            status.nextPosition = navAgent.nextPosition;
            status.steeringTarget = navAgent.steeringTarget;
        }
        
        // 에이전트 설정
        status.speed = navAgent.speed;
        status.angularSpeed = navAgent.angularSpeed;
        status.acceleration = navAgent.acceleration;
        status.radius = navAgent.radius;
        status.height = navAgent.height;
        status.areaMask = navAgent.areaMask;
        status.agentTypeID = navAgent.agentTypeID;
        
        // 장애물 회피
        status.avoidancePriority = navAgent.avoidancePriority;
        status.obstacleAvoidanceType = navAgent.obstacleAvoidanceType;
        
        // 오프메시 링크
        status.isOnOffMeshLink = navAgent.isOnOffMeshLink;
        if (navAgent.isOnOffMeshLink)
        {
            status.currentOffMeshLinkData = navAgent.currentOffMeshLinkData;
        }
        
        // 기타
        status.autoRepath = navAgent.autoRepath;
        status.autoTraverseOffMeshLink = navAgent.autoTraverseOffMeshLink;
        status.autoBraking = navAgent.autoBraking;
    }
    
    void OnDrawGizmos()
    {
        if (navAgent == null || !navAgent.isOnNavMesh) return;
        
        // 경로 표시
        if (showPathInScene && navAgent.hasPath)
        {
            Gizmos.color = pathColor;
            var path = navAgent.path;
            Vector3 prevCorner = transform.position;
            
            foreach (var corner in path.corners)
            {
                Gizmos.DrawLine(prevCorner, corner);
                Gizmos.DrawWireSphere(corner, 0.1f);
                prevCorner = corner;
            }
        }
        
        // 속도 벡터 표시
        if (showVelocityGizmo)
        {
            Gizmos.color = velocityColor;
            Vector3 velocityEnd = transform.position + navAgent.velocity;
            Gizmos.DrawLine(transform.position, velocityEnd);
            Gizmos.DrawWireSphere(velocityEnd, 0.05f);
        }
        
        // 목적지 표시
        if (showDestinationGizmo && navAgent.hasPath)
        {
            Gizmos.color = destinationColor;
            Gizmos.DrawWireSphere(navAgent.destination, 0.2f);
            Gizmos.DrawLine(transform.position, navAgent.destination);
        }
        
        // 에이전트 반지름 표시
        Gizmos.color = Color.blue;
        //Gizmos.DrawWireCylinder(transform.position, navAgent.radius * 2, navAgent.height);
    }
    
    // 유용한 public 메서드들
    public string GetDetailedStatus()
    {
        if (navAgent == null) return "NavMeshAgent가 없습니다.";
        
        return $"상태: {(navAgent.hasPath ? "경로 있음" : "경로 없음")}\n" +
               $"목적지까지 거리: {navAgent.remainingDistance:F2}m\n" +
               $"속도: {navAgent.velocity.magnitude:F2}m/s\n" +
               $"경로 상태: {navAgent.pathStatus}\n" +
               $"정지됨: {navAgent.isStopped}";
    }
    
    public void SetDestination(Vector3 target)
    {
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(target);
        }
    }
    
    public void Stop()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }
    }
    
    public void Resume()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }
    }
}

// Gizmos 확장 메서드
public static class GizmosExtensions
{
    public static void DrawWireCylinder(Vector3 position, float diameter, float height)
    {
        float radius = diameter * 0.5f;
        Vector3 top = position + Vector3.up * height * 0.5f;
        Vector3 bottom = position - Vector3.up * height * 0.5f;
        
        // 상단과 하단 원
        DrawWireCircle(top, radius);
        DrawWireCircle(bottom, radius);
        
        // 세로 선들
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        foreach (var dir in directions)
        {
            Vector3 offset = dir * radius;
            Gizmos.DrawLine(top + offset, bottom + offset);
        }
    }
    
    private static void DrawWireCircle(Vector3 center, float radius)
    {
        int segments = 16;
        float angleStep = 360f / segments;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * radius, 0, Mathf.Sin(angle1) * radius);
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * radius, 0, Mathf.Sin(angle2) * radius);
            
            Gizmos.DrawLine(point1, point2);
        }
    }
}