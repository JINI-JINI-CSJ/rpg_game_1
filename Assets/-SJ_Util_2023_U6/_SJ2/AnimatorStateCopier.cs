using UnityEngine;
using System.Collections.Generic;

public class AnimatorStateCopier : MonoBehaviour
{
    [Header("복사 설정")]
    [SerializeField] private GameObject targetObject; // 복사할 대상 오브젝트
    [SerializeField] private bool copyAsChild = false; // 자식으로 복사할지 여부
    [SerializeField] private Vector3 offsetPosition = Vector3.zero; // 복사된 오브젝트의 위치 오프셋
    
    [Header("디버그")]
    [SerializeField] private bool showDebugLog = true;
    
    private void Start()
    {
        // 컴포넌트가 붙은 오브젝트를 기본 타겟으로 설정
        if (targetObject == null)
            targetObject = gameObject;
    }
    
    /// <summary>
    /// 현재 애니메이션 상태를 포함하여 오브젝트를 복사합니다
    /// </summary>
    public GameObject CopyObjectWithAnimatorState()
    {
        return CopyObjectWithAnimatorState(targetObject);
    }
    
    /// <summary>
    /// 지정된 오브젝트를 현재 애니메이션 상태와 함께 복사합니다
    /// </summary>
    /// <param name="original">복사할 원본 오브젝트</param>
    /// <returns>복사된 오브젝트</returns>
    public GameObject CopyObjectWithAnimatorState(GameObject original)
    {
        if (original == null)
        {
            Debug.LogError("복사할 오브젝트가 null입니다!");
            return null;
        }
        
        Animator originalAnimator = original.GetComponent<Animator>();
        if (originalAnimator == null)
        {
            Debug.LogWarning("원본 오브젝트에 Animator 컴포넌트가 없습니다!");
        }
        
        // 오브젝트 복사
        GameObject copiedObject = Instantiate(original);
        
        // 위치 설정
        if (copyAsChild && transform != null)
        {
            copiedObject.transform.SetParent(transform);
            copiedObject.transform.localPosition = offsetPosition;
        }
        else
        {
            copiedObject.transform.position = original.transform.position + offsetPosition;
            copiedObject.transform.rotation = original.transform.rotation;
        }
        
        // 애니메이션 상태 복사
        if (originalAnimator != null)
        {
            Animator copiedAnimator = copiedObject.GetComponent<Animator>();
            if (copiedAnimator != null)
            {
                CopyAnimatorState(originalAnimator, copiedAnimator);
            }
        }
        
        // 이름 설정
        copiedObject.name = original.name + "_Copy";
        
        if (showDebugLog)
        {
            Debug.Log($"오브젝트 복사 완료: {copiedObject.name}");
        }
        
        return copiedObject;
    }
    
    /// <summary>
    /// 애니메이터의 현재 상태를 다른 애니메이터로 복사합니다
    /// </summary>
    /// <param name="source">원본 애니메이터</param>
    /// <param name="target">대상 애니메이터</param>
    public void CopyAnimatorState(Animator source, Animator target)
    {
        if (source == null || target == null) return;
        
        // 애니메이터가 초기화될 때까지 잠시 기다림
        StartCoroutine(CopyAnimatorStateCoroutine(source, target));
    }
    
    public System.Collections.IEnumerator CopyAnimatorStateCoroutine(Animator source, Animator target)
    {
        // 한 프레임 기다려서 애니메이터가 완전히 초기화되도록 함
        yield return null;
        
        try
        {
            // 모든 레이어의 상태 복사
            for (int layerIndex = 0; layerIndex < source.layerCount; layerIndex++)
            {
                if (layerIndex >= target.layerCount) break;
                
                // 현재 상태 정보 가져오기
                AnimatorStateInfo currentStateInfo = source.GetCurrentAnimatorStateInfo(layerIndex);
                AnimatorStateInfo nextStateInfo = source.GetNextAnimatorStateInfo(layerIndex);
                
                // 현재 재생 중인 애니메이션으로 이동
                if (currentStateInfo.fullPathHash != 0)
                {
                    // 정확한 시간으로 애니메이션 재생
                    target.Play(currentStateInfo.fullPathHash, layerIndex, currentStateInfo.normalizedTime);
                    
                    if (showDebugLog)
                    {
                        Debug.Log($"레이어 {layerIndex}: 상태 복사 - Hash: {currentStateInfo.fullPathHash}, Time: {currentStateInfo.normalizedTime:F3}");
                    }
                }
                
                // 트랜지션 중인 경우 처리
                if (source.IsInTransition(layerIndex) && nextStateInfo.fullPathHash != 0)
                {
                    AnimatorTransitionInfo transitionInfo = source.GetAnimatorTransitionInfo(layerIndex);
                    
                    // 트랜지션 정보도 함께 설정 (가능한 경우)
                    target.CrossFade(nextStateInfo.fullPathHash, transitionInfo.duration, layerIndex, nextStateInfo.normalizedTime);
                    
                    if (showDebugLog)
                    {
                        Debug.Log($"레이어 {layerIndex}: 트랜지션 복사 - 진행률: {transitionInfo.normalizedTime:F3}");
                    }
                }
            }
            
            // 애니메이터 파라미터 복사
            CopyAnimatorParameters(source, target);
            
            // 애니메이터 속도 복사
            target.speed = source.speed;
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"애니메이터 상태 복사 중 오류 발생: {e.Message}");
        }
    }
    
    /// <summary>
    /// 애니메이터 파라미터들을 복사합니다
    /// </summary>
    /// <param name="source">원본 애니메이터</param>
    /// <param name="target">대상 애니메이터</param>
    private void CopyAnimatorParameters(Animator source, Animator target)
    {
        if (source.parameterCount == 0) return;
        
        for (int i = 0; i < source.parameterCount; i++)
        {
            AnimatorControllerParameter param = source.parameters[i];
            
            try
            {
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Float:
                        target.SetFloat(param.nameHash, source.GetFloat(param.nameHash));
                        break;
                        
                    case AnimatorControllerParameterType.Int:
                        target.SetInteger(param.nameHash, source.GetInteger(param.nameHash));
                        break;
                        
                    case AnimatorControllerParameterType.Bool:
                        target.SetBool(param.nameHash, source.GetBool(param.nameHash));
                        break;
                        
                    case AnimatorControllerParameterType.Trigger:
                        // 트리거는 현재 상태를 정확히 복사하기 어려우므로 건너뜀
                        break;
                }
                
                if (showDebugLog)
                {
                    Debug.Log($"파라미터 복사: {param.name} ({param.type})");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"파라미터 '{param.name}' 복사 실패: {e.Message}");
            }
        }
    }
    
    /// <summary>
    /// 버튼이나 외부에서 호출할 수 있는 공개 메서드
    /// </summary>
    [ContextMenu("오브젝트 복사")]
    public void CopyObject()
    {
        CopyObjectWithAnimatorState();
    }
    
    /// <summary>
    /// 특정 오브젝트를 지정해서 복사
    /// </summary>
    /// <param name="objectToCopy">복사할 오브젝트</param>
    public void CopySpecificObject(GameObject objectToCopy)
    {
        CopyObjectWithAnimatorState(objectToCopy);
    }
    
    /// <summary>
    /// 현재 씬의 모든 Animator가 있는 오브젝트들을 복사
    /// </summary>
    public void CopyAllAnimatorObjects()
    {
        Animator[] allAnimators = FindObjectsOfType<Animator>();
        
        foreach (Animator animator in allAnimators)
        {
            if (animator.gameObject != gameObject) // 자기 자신은 제외
            {
                CopyObjectWithAnimatorState(animator.gameObject);
            }
        }
        
        if (showDebugLog)
        {
            Debug.Log($"총 {allAnimators.Length}개의 Animator 오브젝트를 복사했습니다.");
        }
    }
}