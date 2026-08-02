using UnityEngine;
using UnityEngine.UI;

public class Panel_BattleWin : MonoBehaviour
{

    public GameObject go_Gold;
    public Text text_Gold;
    public GameObject go_Exp;
    public Text text_Exp;

    // 아이템
    public SJ_UIListItem go_GridItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPopup_StartAni()
    {
        go_Gold.SetActive(false);
        go_Exp.SetActive(false);
        go_GridItem.gameObject.SetActive(false);
    }

    public void OpenPopup_StartAni_End()
    {
        SetInf();
    }

    public void SetInf()
    {
        BATTLE_RESULT_INF bfi = BattleMain.result_inf;
        go_Gold.SetActive(true);
        go_Exp.SetActive(true);
        go_GridItem.gameObject.SetActive(true);

        SJ_UnityUI_Util.TextString( text_Gold , bfi.gold.ToString() );
        SJ_UnityUI_Util.TextString( text_Exp , bfi.exp.ToString() );
        go_GridItem.Listing( bfi.items );        
    }

    public void OnOK()
    {
        
    }
}
