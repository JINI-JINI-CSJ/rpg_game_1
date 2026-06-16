using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UIWorldPos : MonoBehaviour
{
    public Transform tr_target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( tr_target == null ) return;
        transform.position = Camera.main.WorldToScreenPoint(tr_target.position);
    }
}
