using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_TweenAmbientColor : SJ_UITween_Color
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        FrameMove( Time.deltaTime );
    }

    override public  void    OnFrameMove()
    {
        base.OnFrameMove();
        RenderSettings.ambientLight = color_cur;
    }
}
