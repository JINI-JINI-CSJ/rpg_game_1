using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 페이드 인 하고 , 호출 , 페이드 아웃
public class SJ_UIFadeFunc : MonoBehaviour
{
    static public SJ_UIFadeFunc G;
    public SJ_UITween_Color uITween_Color;
    public Image img;
    public delegate void FUNC_PRC( object arg );

    FUNC_PRC func_load;
    object arg_load;
    FUNC_PRC func_Complete;
    object arg_Complete;

    public delegate void FUNC_START_END( bool start_end ); // true -> 시작 , false -> 종료

    public FUNC_START_END func_start_end;

    // 중복방지
    bool play;

    void Awake()
    {
        G = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public bool CheckPlay()
    {
        return G.play;
    }

    static public void StartFadeFunc( FUNC_PRC _func_load , object _arg_lode = null , FUNC_PRC _func_complete = null , object _arg_complete = null )
    {
        if( CheckPlay() ) return;

        G.StartFade( _func_load ,_arg_lode , _func_complete , _arg_complete );
    }

    public void StartFade( FUNC_PRC _func_load , object _arg_lode , FUNC_PRC _func_complete , object _arg_complete )
    {
        func_start_end?.Invoke(true);

        play = true;
        func_load = _func_load;
        arg_load = _arg_lode;
        func_Complete = _func_complete;
        arg_Complete = _arg_complete;
        OnStartFadeIn();
    }

    virtual public void OnStartFadeIn()
    {
        img.enabled = true;
        img.transform.SetAsLastSibling();
        uITween_Color.PlayFwd();
        StartCoroutine( CO_OnStartFadeIn(uITween_Color.PlayTime) );
    }

    IEnumerator CO_OnStartFadeIn( float wait )
    {
        yield return new WaitForSecondsRealtime( wait );
        OnStartFadeIn_End();
    }

    virtual public void OnStartFadeIn_End()
    {
        func_load.Invoke(arg_load);
        OnStartFadeOut();
    }   

    virtual public void OnStartFadeOut()
    {
        uITween_Color.PlayBack();
        StartCoroutine( CO_OnStartFadeOut(uITween_Color.PlayTime) );
    }

    IEnumerator CO_OnStartFadeOut( float wait )
    {
        yield return new WaitForSecondsRealtime( wait );
        OnStartFadeOut_End();
        play = false;
        func_start_end?.Invoke(false);
    }

    virtual public void OnStartFadeOut_End()
    {
        func_Complete?.Invoke(arg_Complete);
        img.enabled = false;
    }




}
