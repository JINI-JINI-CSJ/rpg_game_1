using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_ReturnDestroy_Coll : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        SJPool.ReturnInst( gameObject );
    }

    private void OnTriggerEnter2D(Collider2D other) {
        SJPool.ReturnInst( gameObject );
    }
}
