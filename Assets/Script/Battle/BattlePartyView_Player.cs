using System.Collections.Generic;
using UnityEngine;

// 배틀 파티 플레이어 뷰어
// 화면 하단 6명 
// 전투 화면에서 UI 모노 객체

public class BattlePartyView_Player : MonoBehaviour
{
    static public BattlePartyView_Player G;

    public List<UIItem_BattleChr> ui_chr_front;
    public List<UIItem_BattleChr> ui_chr_back;

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

    static public void All_HideInputAni()
    {
        G._All_HideInputAni();
    }

    public void _All_HideInputAni()
    {
        foreach( var s in ui_chr_front )s.Active_CommandInput(false);
        foreach( var s in ui_chr_back )s.Active_CommandInput(false);
    }

    static public void Update_Player()
    {
        
    }

}
