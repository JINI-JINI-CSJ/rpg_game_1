using UnityEngine;

public class SJ_RandomPosCube : MonoBehaviour
{
    public bool local_coord = true;
    public Transform tr_Move;
    public Vector3 size;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("랜덤 위치")]
    public void RandomPos()
    {
        Vector3 pos_r = Vector3.zero;
        pos_r.x = UnityEngine.Random.Range(-size.x, size.x);
        pos_r.y = UnityEngine.Random.Range(-size.y, size.y);
        pos_r.z = UnityEngine.Random.Range(-size.z, size.z);

        Transform tr = transform;
        if (tr_Move != null) tr = tr_Move;

        if (local_coord)
        {
            tr.localPosition = pos_r;
        }
        else
        {
            tr.position += pos_r;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube( Vector3.zero , size * 2);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
