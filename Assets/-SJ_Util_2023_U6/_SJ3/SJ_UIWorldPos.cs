using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UIWorldPos : MonoBehaviour
{
    public Transform tr_target;
    // Start is called before the first frame update
    void Start()
    {
        UpdatePos();
    }

    void OnEnable()
    {
        UpdatePos();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePos();
    }

    public void UpdatePos()
    {
        if( tr_target == null ) return;
        transform.position = Camera.main.WorldToScreenPoint(tr_target.position);        
    }
}
