using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_TweenLightIntensity : SJ_UITweenBase
{
    public Light lightCur;

    public float light_from = 1;
    public float light_to = 200;

    public bool end_destroy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FrameMove( Time.deltaTime );
    }

    override public  void    OnFrameMove()
    {
        if( lightCur == null ) lightCur = GetComponent<Light>();
        if( lightCur == null ) 
        {
            Debug.LogError( "라이트 없다!!! " );
            return;
        }

        //float f = light_to * ratio_cur;
        float f = Mathf.Lerp( light_from , light_to , ratio_cur );
        //Debug.Log( "SJ_TweenLightIntensity : " + ratio_cur );

        lightCur.intensity = f;
    }
    override public  void    OnEndOnce()
    {
        //Debug.Log( "SJ_TweenLightIntensity OnEndOnce " );
        if (end_destroy) GameObject.Destroy(gameObject);
    }
}
