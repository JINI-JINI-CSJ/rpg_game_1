using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transform 동기화를 담당하는 별도 스크립트
/// </summary>
public class SJ_TransformSynchronizer : MonoBehaviour
{
    //[HideInInspector]
    public Transform originalTransform;

    public bool noDestroy;

    public bool usePos = true;
    public bool useRot = true;
    public bool useScl = true;

    private void LateUpdate()
    {
        if (originalTransform != null)
        {
            // 원본 객체의 Transform 값을 그대로 복사
            if( usePos )transform.position = originalTransform.position;
            if( useRot )transform.rotation = originalTransform.rotation;
            if( useScl )transform.localScale = originalTransform.localScale;
        }
        else
        {
            // 원본 객체가 없으면 자신을 삭제
            if( noDestroy == false )
                Destroy(gameObject);
        }
    }
}