using System.Collections.Generic;
using UnityEngine;

public class SJ_SimpleInst : MonoBehaviour
{
    public Transform prefab_par;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("인스턴스 생성")]
    public void CreateInst()
    {
        if (prefab_par == null || prefab_par.childCount < 1) return;

        SJ_Unity.Delete_Child(transform);

        List<Transform> lt = SJ_Unity.GetChildList(prefab_par);
        Transform tr_prf = SJ_Unity.GetRandomItem(lt) as Transform;
        if (tr_prf == null) return;
        GameObject go = Instantiate(tr_prf.gameObject);
        if (go == null) return;
        SJ_Unity.SetEqTrans(go.transform, null, this.transform);
    }
}
