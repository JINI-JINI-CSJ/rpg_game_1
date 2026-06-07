using UnityEngine;

public class SJ_TweenPos : SJ_UITweenBase
{
    public Vector3 pos_start;

    public Vector3 pos_end;

    public bool pos_world;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        // color_cur = Color.Lerp( color_from , color_to , ratio_cur );
        // if( image != null ) image.color = color_cur;
        // if( spriteRenderer != null ) spriteRenderer.color = color_cur;
        // if( text != null ) text.color = color_cur;

        Vector3 pos = Vector3.Lerp( pos_start , pos_end , ratio_cur );
        if( pos_world )
        {
            transform.position = pos;
        }
        else
        {
            transform.localPosition = pos;
        }
    }
}
