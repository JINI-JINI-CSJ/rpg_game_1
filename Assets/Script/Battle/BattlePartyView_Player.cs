using System.Collections.Generic;
using System.Linq;
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
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void Show( bool b )
    {
        G.gameObject.SetActive(b);
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
        G._Update_Player();
    }

    public void _Update_Player()
    {
        gameObject.SetActive(true);

        foreach( var s in ui_chr_front )s.Clear();
        foreach( var s in ui_chr_back )s.Clear();

        UpdateLine( Player.battleParty.chars_Front , ui_chr_front );
        UpdateLine( Player.battleParty.chars_Back , ui_chr_back );
    }

    public void UpdateLine( CharBase[] chars , List<UIItem_BattleChr> uis )
    {
        for( int i = 0 ; i < chars.Length ;i++ )
        {
            UIItem_BattleChr ui = uis[i];
            ui.SetChar( chars[i] );
        }
    }

    

}
