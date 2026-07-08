using UnityEngine;
using UnityEngine.UI;

public class SJ_GameSync_UIFade : SJ_GameSyncStepBase
{
    // true 페이드 인 , false 아웃
    public bool FadeIn;
    public SJ_Curve_Color curve_Color;

    public Image image;

    public void PlayStep()
    {
        image.gameObject.SetActive( true );
        curve_Color.func_Update = OnUpdateCurve;
        curve_Color.func_End = OnEndCurve;
        curve_Color.StartTime_PlayDir(FadeIn);
    }

    void Update()
    {
        curve_Color.UpdateCurve();

    }

    public void OnUpdateCurve()
    {
        image.color = curve_Color.col_cur;        
    }

    public void OnEndCurve()
    {
        SJ_SimpleSyncMono.NextPlay();
    }

    public void EndStep()
    {
        // 닫기 차례라면 숨기기
        if( FadeIn == false )
        {
            image.gameObject.SetActive( false );
        }
    }


}
