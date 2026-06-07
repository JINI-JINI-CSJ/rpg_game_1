using UnityEngine;

// 대상 박스콜리더를 참조해서 사이즈 오프셋
public class SJ_BOXColliderOffset : MonoBehaviour
{
    public BoxCollider box_src;
    public BoxCollider box_target;
    public Vector3 size_offset;

    [ContextMenu("사이즈 확장")]
    public void Work()
    {
        if (box_src == null || box_target == null)
        {
            Debug.Log("주의!!! 콜리더 없다. " + gameObject.name);
            return;
        }
        box_target.size = box_src.size + size_offset;
    }
}
