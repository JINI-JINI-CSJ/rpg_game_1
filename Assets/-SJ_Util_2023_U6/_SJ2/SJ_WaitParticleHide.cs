using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_WaitParticleHide : MonoBehaviour
{
    public float time_hide = 1;
    public float hide_after_destroy_time = 1;

    public bool start_Play;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        if( start_Play ) StartHide();
    }

    public void StartHide()
    {
        StartCoroutine( CO_Wait() );
    }

    IEnumerator CO_Wait()
    {
        yield return new WaitForSeconds(time_hide);
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach( var s in particles ) s.Stop();

        if( hide_after_destroy_time <= 0 )
        {
            yield return null;
        }

        yield return new WaitForSeconds(hide_after_destroy_time);
        GameObject.Destroy(gameObject);
    }
}
