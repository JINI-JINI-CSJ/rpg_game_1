using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 유니티 LODGroup처럼, 거리 단계(Level)별로 표시할 오브젝트를 지정하고
/// 현재 거리에 해당하는 단계의 오브젝트만 활성화하는 컴포넌트.
/// 각 단계는 "최대 거리 1개"와 "해당 단계에서 보여줄 오브젝트"만 가지면 된다.
/// 에디터 씬 뷰에서도 실시간으로 반경과 현재 활성 단계를 확인할 수 있다.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
[AddComponentMenu("Custom/Distance Activator (LOD Style)")]
public class DistanceActivator : MonoBehaviour
{
    [Serializable]
    public class Level
    {
        public string label = "Level";

        [Tooltip("이 단계가 적용되는 최대 거리. 이 거리 이하일 때 이 단계가 선택된다.\n마지막 단계는 무시되고 항상 '무제한'으로 취급된다.")]
        [Min(0f)] public float maxDistance = 10f;

        [Tooltip("이 단계에서 활성화할 오브젝트")]
        public GameObject target;

        public Color gizmoColor = Color.green;
    }

    [Header("기준점 설정")]
    [Tooltip("비워두면 Play 모드에서는 Player 태그 → 메인 카메라 순으로 자동 탐색, 에디터에서는 Scene 뷰 카메라 사용")]
    public Transform referencePoint;
    public string playerTag = "Player";

    [Header("갱신 설정")]
    [Tooltip("Play 모드에서 몇 프레임마다 거리 체크할지 (성능 최적화용)")]
    [Min(1)] public int updateInterval = 5;

    [Header("거리 단계 목록 (가까운 순서로 등록)")]
    [Tooltip("마지막 단계는 자동으로 '무제한 거리'로 취급됩니다.")]
    public List<Level> levels = new List<Level>();

    [Header("에디터 표시 설정")]
    public bool showGizmos = true;
    public bool showDistanceLabel = true;

    [NonSerialized] public int currentLevelIndex = -1;
    [NonSerialized] public float lastDistance;

    int frameCounter;
    Transform cachedAutoRef;

    void OnEnable()
    {
        frameCounter = UnityEngine.Random.Range(0, Mathf.Max(1, updateInterval));
    }

    Transform GetReference()
    {
        if (referencePoint != null) return referencePoint;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var sceneView = UnityEditor.SceneView.lastActiveSceneView;
            if (sceneView != null && sceneView.camera != null)
                return sceneView.camera.transform;
        }
#endif
        if (cachedAutoRef == null)
        {
            var playerObj = !string.IsNullOrEmpty(playerTag) ? GameObject.FindGameObjectWithTag(playerTag) : null;
            if (playerObj != null) cachedAutoRef = playerObj.transform;
            else if (Camera.main != null) cachedAutoRef = Camera.main.transform;
        }
        return cachedAutoRef;
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            frameCounter++;
            if (frameCounter < updateInterval) return;
            frameCounter = 0;
        }
        Evaluate();
    }

    /// <summary>
    /// 기준점과의 거리를 계산해, 거리가 속하는 첫 번째 단계를 찾아
    /// 그 단계의 오브젝트만 켜고 나머지는 모두 끈다.
    /// 단계는 등록된 순서(가까운 것부터)를 기준으로 판단하며,
    /// 마지막 단계는 무조건 "무제한"으로 취급한다.
    /// </summary>
    public void Evaluate()
    {
        var reference = GetReference();
        if (reference == null || levels == null || levels.Count == 0) return;

        float dist = Vector3.Distance(reference.position, transform.position);
        lastDistance = dist;

        int selected = levels.Count - 1; // 기본값: 마지막 단계(무제한)
        for (int i = 0; i < levels.Count; i++)
        {
            bool isLast = (i == levels.Count - 1);
            if (isLast || dist <= levels[i].maxDistance)
            {
                selected = i;
                break;
            }
        }

        if (selected == currentLevelIndex) return; // 변화 없으면 스킵

        currentLevelIndex = selected;

        for (int i = 0; i < levels.Count; i++)
        {
            var lvl = levels[i];
            if (lvl.target == null) continue;

            bool shouldBeActive = (i == selected);
            if (lvl.target.activeSelf != shouldBeActive)
                lvl.target.SetActive(shouldBeActive);
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || levels == null || levels.Count == 0) return;

        if (!Application.isPlaying) Evaluate();

        Vector3 pos = transform.position;

        for (int i = 0; i < levels.Count; i++)
        {
            var lvl = levels[i];
            bool isLast = (i == levels.Count - 1);

            if (!isLast)
            {
                Gizmos.color = lvl.gizmoColor;
                Gizmos.DrawWireSphere(pos, lvl.maxDistance);
            }
        }

#if UNITY_EDITOR
        if (showDistanceLabel)
        {
            string current = (currentLevelIndex >= 0 && currentLevelIndex < levels.Count)
                ? levels[currentLevelIndex].label
                : "-";
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(pos + Vector3.up * 0.5f,
                $"거리: {lastDistance:F1}m\n활성 단계: {current}");
        }
#endif
    }
}
