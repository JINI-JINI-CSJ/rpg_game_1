//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 디폴트 숏 팝업 메세지
public class SJ_UIDefaultShortMsg : MonoBehaviour
{
    static public SJ_UIDefaultShortMsg G;
    public float        show_time;
    public AudioClip    snd;
    public SJ_Curve     sJ_Curve = new SJ_Curve();
    public Transform    tr_shortMsg;
    public Text         text_msg;

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
        if( sJ_Curve.play )
        {
            float val_cur = sJ_Curve.UpdateCurve();
            OnShortMsg_Trans(val_cur);
            if( sJ_Curve.play == false )
            {
                tr_shortMsg.gameObject.SetActive(false);
            }
            // 일단 간단하게 맨아래로 매 프레임 마다.
            transform.SetAsLastSibling();
        }
    }

    virtual public void OnShortMsg_Trans( float val )
    {
        // 기본 , y 스케일
        tr_shortMsg.localScale = new Vector3( 1 , val , 1 );
    }

    virtual public void OnSetMsg( string msg )
    {
        tr_shortMsg.gameObject.SetActive(true);
        text_msg.text = msg;
        sJ_Curve.StartTime();
    }

    static public void SetMsg( string msg )
    {
        G?.OnSetMsg( msg );
    }

    virtual public void OnShortEnd()
    {
        tr_shortMsg.gameObject.SetActive(false);
    }


}
