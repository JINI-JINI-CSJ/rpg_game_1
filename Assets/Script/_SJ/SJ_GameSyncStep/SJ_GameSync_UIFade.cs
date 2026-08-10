using UnityEngine;
using UnityEngine.UI;

// 1. 컬러만 지정하기
// 2. 페이드 인 아웃 하기
public class SJ_GameSync_UIFade : SJ_GameSyncStepBase
{
    // 바로 색 적용 
    public bool PlayColor;
    public Color color;
    public bool  colorObjShow;

    // 페이드 애니 적용
    public bool PlayFadeAni;
    // true 페이드 인 , false 아웃
    public bool FadeIn;

    // 컬러 커브 세팅은 무조건 정방향 세팅
    // 페이드 아웃은 커브에서 역방향 플레이 할꺼다.
    public SJ_Curve_Color curve_Color;

    public Image image;

    public void PlayStep()
    {
        if( image == null)
        {
            if( SJ_UnityUIMng_Curve.G == null || SJ_UnityUIMng_Curve.G.curve_Black == null )
            {
                Debug.LogError( "SJ_UnityUIMng_Curve.G == null || SJ_UnityUIMng_Curve.G.curve_Black == null" );
                return;
            }

            image = SJ_UnityUIMng_Curve.G.curve_Black.image_Color;
        }



        if( PlayColor )
        {
            image.color = color;
            image.gameObject.SetActive( colorObjShow );
            SJ_SimpleSyncMono.NextPlaySelf();
            return;
        }

        if( PlayFadeAni )
        {
            image.gameObject.SetActive( true );
            curve_Color.func_Update = OnUpdateCurve;
            curve_Color.func_End = OnEndCurve;
            curve_Color.StartTime_PlayDir(FadeIn);            
            return;
        }

        Debug.Log( "주의!!! : 페이드 설정 없음" );
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
