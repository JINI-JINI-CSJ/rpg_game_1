using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SJ_UITween_Color : SJ_UITweenBase
{
    public  Color   color_from;
    public  Color   color_to;

    [HideInInspector]
    public Color  color_cur;

    public  Image  image;
    public Text text;
    public  SpriteRenderer  spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FrameMove( Time.deltaTime );
    }

    public override void OnFrameMove()
    {
        color_cur = Color.Lerp( color_from , color_to , ratio_cur );
        if( image != null ) image.color = color_cur;
        if( spriteRenderer != null ) spriteRenderer.color = color_cur;
        if( text != null ) text.color = color_cur;
    }

    void OnDisable()
    {
        ReturnColor();
    }

    public void ReturnColor()
    {
        if( image != null ) image.color = color_from;
        if( spriteRenderer != null ) spriteRenderer.color = color_from;
        if( text != null ) text.color = color_from;        
    }

}
