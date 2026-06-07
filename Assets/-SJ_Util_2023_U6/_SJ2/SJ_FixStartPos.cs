using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_FixStartPos : MonoBehaviour
{
    public Vector3 pos_init;

    public Vector3 rot_init;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitStartPos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitStartPos()
    {
        pos_init = transform.localPosition;
        rot_init = transform.localEulerAngles;
    }

    void LateUpdate()
    {
        transform.localPosition = pos_init;
        transform.localEulerAngles = rot_init;
    }
}
