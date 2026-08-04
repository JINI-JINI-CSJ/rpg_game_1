using System.Collections.Generic;
using UnityEngine;

public class BATTLE_RESULT_INF
{
    public int gold;
    public int exp;
    public List<ItemBase> items = new();

    public void Clear()
    {
        gold = 0;
        exp = 0;
        items.Clear();
    }
}

public class BattleMain : MonoBehaviour
{
    static public BattleMain G;
    static public BATTLE_RESULT_INF result_inf = new();

    
    public GameObject               go_Cam_InputCommand; 
    public GameObject               go_Cam_BattleTurn; 
    public BattlePartyView_Enemy    view_Enemy;
    //public Panel_BattleMain         panel_BattleMain;    
    public BattleParty              battleParty_Enemy;


    public int TURN;

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

    // 적군을 세팅해서 넘겨주기
    public void InitBattle( BattleParty bp_enemy )
    {
        TURN = 1;
        battleParty_Enemy = bp_enemy;
        view_Enemy.Init();

        Phase_InputCommand();
    }

    // 파티 가져오기 , 아군 ,적군
    static public BattleParty GetBattleParty( _ARMY_FORCE force_self , bool our_force )
    {
        if( force_self == _ARMY_FORCE.Player )
        {
            if( our_force ) return Player.battleParty;
            else            return G.battleParty_Enemy;
        }
        else if( force_self == _ARMY_FORCE.Enemy )
        {
            if( our_force ) return G.battleParty_Enemy;
            else            return Player.battleParty;
        }
        return null;
    }

    static public void Phase_InputCommand()
    {
        G._Phase_InputCommand();
    }

    public void _Phase_InputCommand()
    {
        // 유아이 열고 , 커맨드 카메라 활성화 
        Panel_BattleMain.InitBattle();
        go_Cam_InputCommand.SetActive(true);
    }
    
    static public void Phase_StartTurn()
    {
        G._Phase_StartTurn();
    }

    public void _Phase_StartTurn()
    {
        // 카메라 전환 
        // 턴 매니저 시작
        go_Cam_BattleTurn.SetActive(true);
    }

    static public void NextCharAction()
    {
        G._NextCharAction();
    }

    public void _NextCharAction()
    {
        // 양측 전멸 체크
        if( Player.battleParty.CheckLiveALL() == false )
        {
            // 아군 전멸
            return;
        }
        else if( BattleMain.G.battleParty_Enemy.CheckLiveALL() == false )
        {
            // 적군 전멸    
            ResultBattleWin();
            return;
        }

        BattleTurn.NextCharAction();
    }



    static public void Phase_EndTurn()
    {
        G._Phase_EndTurn();
    }

    public void _Phase_EndTurn()
    {
        
    }


    public void ResultBattleWin()
    {
        result_inf.Clear();
        result_inf.gold = 100;
        result_inf.exp = 100;
        SJ_UnityUIMng_Curve.Open( "Panel_BattleWin" );
    }

    static public void OnOK_ResultPopup()
    {
        
    }

    public void _OnOK_ResultPopup()
    {
        
    }

}
