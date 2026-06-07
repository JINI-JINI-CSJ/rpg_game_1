using System.Collections.Generic;
using UnityEngine;

public class SJ_AnimatorSynchronizer : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator mainAnimator;
    [SerializeField] private List<Animator> otherAnimators = new List<Animator>();
    
    private List<Animator> allAnimators = new List<Animator>();
    
    void Start()
    {
        InitializeAnimators();
    }
    
    private void InitializeAnimators()
    {
        allAnimators.Clear();
        
        if (mainAnimator != null)
        {
            allAnimators.Add(mainAnimator);
        }
        
        foreach (var animator in otherAnimators)
        {
            if (animator != null && animator != mainAnimator)
            {
                allAnimators.Add(animator);
            }
        }
    }
    
    // ==== SET 함수들 (모든 Animator에 적용) ====
    
    public void SetBool(string name, bool value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetBool(name, value);
        }
    }
    
    public void SetBool(int id, bool value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetBool(id, value);
        }
    }
    
    public void SetFloat(string name, float value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetFloat(name, value);
        }
    }
    
    public void SetFloat(int id, float value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetFloat(id, value);
        }
    }
    
    public void SetFloat(string name, float value, float dampTime, float deltaTime)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetFloat(name, value, dampTime, deltaTime);
        }
    }
    
    public void SetFloat(int id, float value, float dampTime, float deltaTime)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetFloat(id, value, dampTime, deltaTime);
        }
    }
    
    public void SetInteger(string name, int value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetInteger(name, value);
        }
    }
    
    public void SetInteger(int id, int value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetInteger(id, value);
        }
    }
    
    public void SetTrigger(string name)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetTrigger(name);
        }
    }
    
    public void SetTrigger(int id)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetTrigger(id);
        }
    }
    
    public void ResetTrigger(string name)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.ResetTrigger(name);
        }
    }
    
    public void ResetTrigger(int id)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.ResetTrigger(id);
        }
    }

    public void ResetTriggerAll()
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
            {
                foreach (AnimatorControllerParameter param in animator.parameters)
                {
                    if (param.type == AnimatorControllerParameterType.Trigger) animator.ResetTrigger(param.name);
                }  
            }
        }
    }
    
    public void SetLayerWeight(int layerIndex, float weight)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetLayerWeight(layerIndex, weight);
        }
    }
    
    // ==== IK 관련 SET 함수들 (모든 Animator에 적용) ====
    
    public void SetIKPositionWeight(AvatarIKGoal goal, float value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetIKPositionWeight(goal, value);
        }
    }
    
    public void SetIKRotationWeight(AvatarIKGoal goal, float value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetIKRotationWeight(goal, value);
        }
    }
    
    public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetIKPosition(goal, goalPosition);
        }
    }
    
    public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetIKRotation(goal, goalRotation);
        }
    }
    
    public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetIKHintPositionWeight(hint, value);
        }
    }
    
    public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetIKHintPosition(hint, hintPosition);
        }
    }
    
    public void SetLookAtWeight(float weight)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetLookAtWeight(weight);
        }
    }
    
    public void SetLookAtWeight(float weight, float bodyWeight)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetLookAtWeight(weight, bodyWeight);
        }
    }
    
    public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetLookAtWeight(weight, bodyWeight, headWeight);
        }
    }
    
    public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight);
        }
    }
    
    public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight, float clampWeight)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }
    }
    
    public void SetLookAtPosition(Vector3 lookAtPosition)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetLookAtPosition(lookAtPosition);
        }
    }
    
    public void SetBoneLocalRotation(HumanBodyBones humanBoneId, Quaternion rotation)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.SetBoneLocalRotation(humanBoneId, rotation);
        }
    }
    
    // ==== GET 함수들 (메인 Animator에서만 가져오기) ====
    
    public bool GetBool(string name)
    {
        if (mainAnimator != null)
            return mainAnimator.GetBool(name);
        return false;
    }
    
    public bool GetBool(int id)
    {
        if (mainAnimator != null)
            return mainAnimator.GetBool(id);
        return false;
    }
    
    public float GetFloat(string name)
    {
        if (mainAnimator != null)
            return mainAnimator.GetFloat(name);
        return 0f;
    }
    
    public float GetFloat(int id)
    {
        if (mainAnimator != null)
            return mainAnimator.GetFloat(id);
        return 0f;
    }
    
    public int GetInteger(string name)
    {
        if (mainAnimator != null)
            return mainAnimator.GetInteger(name);
        return 0;
    }
    
    public int GetInteger(int id)
    {
        if (mainAnimator != null)
            return mainAnimator.GetInteger(id);
        return 0;
    }
    
    public float GetLayerWeight(int layerIndex)
    {
        if (mainAnimator != null)
            return mainAnimator.GetLayerWeight(layerIndex);
        return 0f;
    }
    
    // ==== IK 관련 GET 함수들 (메인 Animator에서만 가져오기) ====
    
    public float GetIKPositionWeight(AvatarIKGoal goal)
    {
        if (mainAnimator != null)
            return mainAnimator.GetIKPositionWeight(goal);
        return 0f;
    }
    
    public float GetIKRotationWeight(AvatarIKGoal goal)
    {
        if (mainAnimator != null)
            return mainAnimator.GetIKRotationWeight(goal);
        return 0f;
    }
    
    public Vector3 GetIKPosition(AvatarIKGoal goal)
    {
        if (mainAnimator != null)
            return mainAnimator.GetIKPosition(goal);
        return Vector3.zero;
    }
    
    public Quaternion GetIKRotation(AvatarIKGoal goal)
    {
        if (mainAnimator != null)
            return mainAnimator.GetIKRotation(goal);
        return Quaternion.identity;
    }
    
    public float GetIKHintPositionWeight(AvatarIKHint hint)
    {
        if (mainAnimator != null)
            return mainAnimator.GetIKHintPositionWeight(hint);
        return 0f;
    }
    
    public Vector3 GetIKHintPosition(AvatarIKHint hint)
    {
        if (mainAnimator != null)
            return mainAnimator.GetIKHintPosition(hint);
        return Vector3.zero;
    }
    
    // public Vector3 GetBonePosition(HumanBodyBones humanBoneId)
    // {
    //     if (mainAnimator != null)
    //         return mainAnimator.GetBonePosition(humanBoneId);
    //     return Vector3.zero;
    // }
    
    // public Quaternion GetBoneRotation(HumanBodyBones humanBoneId)
    // {
    //     if (mainAnimator != null)
    //         return mainAnimator.GetBoneRotation(humanBoneId);
    //     return Quaternion.identity;
    // }
    
    // ==== 기타 유용한 함수들 ====
    
    public void Play(string stateName)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.Play(stateName);
        }
    }
    
    public void Play(string stateName, int layer)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.Play(stateName, layer);
        }
    }
    
    public void Play(string stateName, int layer, float normalizedTime)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.Play(stateName, layer, normalizedTime);
        }
    }
    
    public void CrossFade(string stateName, float normalizedTransitionDuration)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.CrossFade(stateName, normalizedTransitionDuration);
        }
    }
    
    public void CrossFade(string stateName, float normalizedTransitionDuration, int layer)
    {
        foreach (var animator in allAnimators)
        {
            if (animator != null)
                animator.CrossFade(stateName, normalizedTransitionDuration, layer);
        }
    }
    
    // ==== 메인 Animator에서만 가져오는 상태 정보 ====
    
    public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex = 0)
    {
        if (mainAnimator != null)
            return mainAnimator.GetCurrentAnimatorStateInfo(layerIndex);
        return new AnimatorStateInfo();
    }
    
    public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex = 0)
    {
        if (mainAnimator != null)
            return mainAnimator.GetNextAnimatorStateInfo(layerIndex);
        return new AnimatorStateInfo();
    }
    
    public bool IsInTransition(int layerIndex = 0)
    {
        if (mainAnimator != null)
            return mainAnimator.IsInTransition(layerIndex);
        return false;
    }
    
    // ==== 관리 함수들 ====
    
    public void AddAnimator(Animator animator)
    {
        if (animator != null && !otherAnimators.Contains(animator) && animator != mainAnimator)
        {
            otherAnimators.Add(animator);
            InitializeAnimators();
        }
    }
    
    public void RemoveAnimator(Animator animator)
    {
        if (otherAnimators.Contains(animator))
        {
            otherAnimators.Remove(animator);
            InitializeAnimators();
        }
    }
    
    public void SetMainAnimator(Animator animator)
    {
        mainAnimator = animator;
        InitializeAnimators();
    }
    
    public Animator GetMainAnimator()
    {
        return mainAnimator;
    }
    
    public List<Animator> GetAllAnimators()
    {
        return new List<Animator>(allAnimators);
    }
    
    public int GetAnimatorCount()
    {
        return allAnimators.Count;
    }
    
    // ==== 디버그 함수 ====
    
    [ContextMenu("Debug Animator Info")]
    public void DebugAnimatorInfo()
    {
        Debug.Log($"Main Animator: {(mainAnimator != null ? mainAnimator.name : "None")}");
        Debug.Log($"Other Animators Count: {otherAnimators.Count}");
        Debug.Log($"Total Animators: {allAnimators.Count}");
        
        for (int i = 0; i < otherAnimators.Count; i++)
        {
            Debug.Log($"Other Animator {i}: {(otherAnimators[i] != null ? otherAnimators[i].name : "None")}");
        }
    }
}