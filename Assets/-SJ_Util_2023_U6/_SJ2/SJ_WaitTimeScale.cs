using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_WaitTimeScale : MonoBehaviour
{
    public float scaleTime = 1;
    public float waitTime = 1.0f;

    public float StartTime;

    _SJ_GO_FUNC func = new _SJ_GO_FUNC();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( Time.realtimeSinceStartup - StartTime >= waitTime )
        {
            enabled = false;
            Time.timeScale = 1.0f;
            func.Func();
        }
    }

    public void StartWait( MonoBehaviour mono = null , string func_str = "" )
    {
        enabled = true;
        func.SetMono( mono , func_str  );
        Time.timeScale = scaleTime;
        StartTime = Time.realtimeSinceStartup;
    }

}
