using UnityEngine;

public class SJTween_MaterialPropertyBlock : SJ_UITweenBase
{
    public GPT_DynamicSimpleLitMaterial gPT_Dynamic;

    public Color color_s;
    public Color color_e;
    

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
        Color color_cur = Color.Lerp( color_s , color_e , ratio_cur );
        SetColor( color_cur );
    }

    public void SetColor( Color col )
    {
        gPT_Dynamic.color = col;
        gPT_Dynamic.emissionColor = col;
    }

    public void RollBack()
    {
        SetColor(color_s);
    }
}
