
using UnityEngine;
using UnityEngine.UI;

public class SJ_Curve_Color_Mono : MonoBehaviour
{
    public SJ_Curve_Color curve_Color = new();

    public bool enable_start;

    public Image image;
    public SpriteRenderer spriteRenderer;
    public Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        if( enable_start )StartPlay();
    }

    public void StartPlay( SJ_COMMON.Func_VOID func_End = null )
    {
        enabled = true;
        curve_Color.func_Update = OnUpdateColor;
        curve_Color.func_End = func_End;
        curve_Color.StartTime();
    }

    public void OnUpdateColor()
    {
        if( image != null )image.color = curve_Color.col_cur;
        if( spriteRenderer != null )spriteRenderer.color = curve_Color.col_cur;
        if( text != null )text.color = curve_Color.col_cur;
    }

    // Update is called once per frame
    void Update()
    {
        curve_Color.UpdateCurve();
        if( curve_Color.play == false ) enabled = false;
    }
}
