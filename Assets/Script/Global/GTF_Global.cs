using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GTF_Global : MonoBehaviour
{
    static public GTF_Global G;
    public Image    img_FirstBlack;

    // 현재 씬의 기본 창 ,ui
    public List<GameObject> lt_BaseCurUI;

    void Awake()
    {
        G = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 플레이어 던전 이동 여부
    // gui 입력 여부
    static public void PlayerInputAble( bool player_move , bool ui )
    {
        PlayerMover.G.SetInputAble( player_move );
        SJ_UnityUIMng_Curve.SetBlockInput( !ui );
    }


    // 
    static public SJ_UIGameTalkChr OpenUI_TalkChr()
    {
        GameObject go = SJ_UnityUIMng_Curve.Open( "PanelPopup_TalkChrImg" );
        return go.GetComponent<SJ_UIGameTalkChr>();
    }

    // 게임 현재 주 메뉴 보이기 여부
    // 씬마다 다르다.
    static public void ShowMainUI( bool b )
    {
        foreach( var s in G.lt_BaseCurUI )
        {
            s.SetActive(b);
        }
    } 
}
