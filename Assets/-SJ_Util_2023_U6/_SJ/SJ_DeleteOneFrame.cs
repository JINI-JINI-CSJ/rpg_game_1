using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 단순하게 매 프레임마다 
// 충돌되는 객체 한개씩만 삭제하기
public class SJ_DeleteOneFrame : MonoBehaviour
{
    static public GameObject G_Delete = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Bounds bd = SJ_Cood.GetAllBounds( gameObject );
        // Debug.Log(bd.center);
    }


    private void OnTriggerStay(Collider other) {
        //G_Delete = gameObject;
    }

    private void LateUpdate() {
        // if( G_Delete != null )
        // {
        //     GameObject.DestroyImmediate(G_Delete);
        //     G_Delete = null;
        // }
    }
}
