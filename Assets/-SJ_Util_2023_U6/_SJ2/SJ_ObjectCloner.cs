using System.Collections.Generic;
using UnityEngine;

public class SJ_ObjectCloner : MonoBehaviour
{
    [System.Serializable]
    public class MaterialReplacement
    {
        public Material originalMaterial;
        public Material replacementMaterial;
    }

    [Header("Material Replacement Settings")]
    public List<MaterialReplacement> materialReplacements = new List<MaterialReplacement>();

    public SJ_AnimatorSynchronizer sj_syncAnit;

    private GameObject clonedObject;
    private Renderer[] clonedRenderers;

    public string Layer_Name;

    public List<string> ADD_Component;

    /// <summary>
    /// 본인과 똑같은 객체를 복사하는 함수
    /// </summary>
    public GameObject CloneSyncObject()
    {
        // 현재 GameObject를 복사
        clonedObject = Instantiate(gameObject);
        
        // 복사된 객체의 이름 변경 (Clone 접미사 제거)
        clonedObject.name = gameObject.name + "_TransformSync";

        if( string.IsNullOrEmpty(Layer_Name) == false )
        {
            clonedObject.layer = LayerMask.NameToLayer( Layer_Name);
        }
        
        // Animator 컴포넌트를 제외한 모든 컴포넌트 삭제
        RemoveAllComponentsExceptAnimator(clonedObject);
        
        // 원본의 RuntimeAnimatorController를 복사된 객체에 설정
        SetupAnimatorController(clonedObject);
        
        // 머티리얼 교체
        ReplaceMaterials(clonedObject);
        
        // Transform 동기화를 위한 스크립트 추가
        SJ_TransformSynchronizer synchronizer = clonedObject.AddComponent<SJ_TransformSynchronizer>();
        synchronizer.originalTransform = this.transform;
        
        foreach( var s in ADD_Component )
        {
            SJ_CompAddFunc.Add_Comp( clonedObject , s );
        }

        return clonedObject;
    }

    /// <summary>
    /// Animator를 제외한 모든 컴포넌트를 삭제
    /// </summary>
    private void RemoveAllComponentsExceptAnimator(GameObject target)
    {
        // Transform과 Animator를 제외한 모든 컴포넌트 가져오기
        //Component[] components = target.GetComponentsInChildren<Component>();
        Component[] components = target.GetComponents<Component>();
        
        foreach (Component component in components)
        {
            // Transform과 Animator, GameObject는 제거하지 않음
            if (component is Transform || component is Animator || component is GameObject)
                continue;
                
            // 복사된 객체의 ObjectCloner 스크립트도 제거
            if (component is SJ_ObjectCloner)
            {
                DestroyImmediate(component);
                continue;
            }
            
            DestroyImmediate(component);
        }
    }

    /// <summary>
    /// 원본 Animator의 RuntimeAnimatorController를 복사된 객체에 설정
    /// </summary>
    private void SetupAnimatorController(GameObject target)
    {
        Animator originalAnimator = GetComponent<Animator>();
        Animator clonedAnimator = target.GetComponent<Animator>();
        
        if (originalAnimator != null && clonedAnimator != null)
        {
            // 원본의 runtimeAnimatorController를 그대로 참조 (새 인스턴스가 아닌 원본 레퍼런스)
            clonedAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;
            
            // 기타 Animator 설정도 복사
            clonedAnimator.applyRootMotion = originalAnimator.applyRootMotion;
            clonedAnimator.updateMode = originalAnimator.updateMode;
            clonedAnimator.cullingMode = originalAnimator.cullingMode;

            //sj_syncAnit.SetMainAnimator( originalAnimator );
            if( sj_syncAnit == null  )sj_syncAnit = GetComponent<SJ_AnimatorSynchronizer>();
            sj_syncAnit.AddAnimator( clonedAnimator );
        }
    }

    /// <summary>
    /// 머티리얼 교체 목록에 따라 머티리얼을 교체
    /// </summary>
    private void ReplaceMaterials(GameObject target)
    {
        clonedRenderers = target.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in clonedRenderers)
        {
            //Material[] materials = renderer.materials;
            Material[] materials = renderer.sharedMaterials;
            bool materialsChanged = false;
            
            for (int i = 0; i < materials.Length; i++)
            {
                // 매터리얼 교체 목록에서 매칭되는 항목 찾기
                foreach (MaterialReplacement replacement in materialReplacements)
                {
                    if (replacement.originalMaterial != null && 
                        replacement.replacementMaterial != null &&
                        materials[i] == replacement.originalMaterial
                        //materials[i].name.Contains( replacement.originalMaterial.name )
                        )
                    {
                        materials[i] = replacement.replacementMaterial;
                        materialsChanged = true;

                        //Debug.Log( "ReplaceMaterials : " + replacement.replacementMaterial );
                        break;
                    }
                }
            }
            
            // 머티리얼이 변경되었다면 적용
            if (materialsChanged)
            {
                renderer.materials = materials;
            }

            if( string.IsNullOrEmpty(Layer_Name) == false )
            {
                renderer.gameObject.layer = LayerMask.NameToLayer( Layer_Name);
            }
        }
    }

    /// <summary>
    /// 복사된 객체 제거
    /// </summary>
    public void DestroyClonedObject()
    {
        if (clonedObject != null)
        {
            DestroyImmediate(clonedObject);
            clonedObject = null;
            clonedRenderers = null;
        }
    }

    private void OnDestroy()
    {
        // 원본 객체가 삭제될 때 복사된 객체도 함께 삭제
        DestroyClonedObject();
    }
}

