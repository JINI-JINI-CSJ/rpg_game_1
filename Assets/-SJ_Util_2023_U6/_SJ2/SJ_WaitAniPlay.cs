using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_WaitAniPlay : MonoBehaviour
{
    public Animator anit;
    public string   aniState;
    public float wait_time = 0;
    public float delay_time = 0;

    public SJ_CallFunc_Mono func_end;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlay()
    {
        StartCoroutine( CO_Play() );
    }

    IEnumerator CO_Play()
    {
        yield return new WaitForSeconds(wait_time);
        anit.Play( aniState );
        yield return new WaitForSeconds(delay_time);
        func_end.Func();
    }
}
