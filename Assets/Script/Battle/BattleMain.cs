using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class _BATTLE_RESULT_INF
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


[System.Serializable]
public class _ENEMY_BATTLE_INIT
{
    public int LEVEL_FIX;   // 현 던전 레벨 보정  

    [System.Serializable]
    public class _ENEMY_ID_COUNT
    {
        public int csv_id;        
        public int count = 1;
    }

    public List<_ENEMY_ID_COUNT> enemies = new();

    // 배틀 파티 
    public BattleParty Make_BattleParty()
    {
        BattleParty battleParty = new();
        foreach( var s in enemies )
        {
            for( int i = 0 ; i < s.count ; i++ )
            {
                battleParty.Add(s.csv_id , LEVEL_FIX , _ARMY_FORCE.Enemy);
            }
        }
        return battleParty;
    }
}


public class BattleMain : MonoBehaviour
{
    static public BattleMain G;
    static public _BATTLE_RESULT_INF result_inf = new();
    public GameObject               go_Cam_InputCommand; 
    public GameObject               go_Cam_BattleTurn; 
    public BattlePartyView_Enemy    view_Enemy;
    public BattleParty              battleParty_Enemy;

    public _ENEMY_BATTLE_INIT       TEST_BATTLE;

    public float delay_StartTurn = 0.5f;

    public float delay_Result = 0.5f;

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

    static public void BattleTEST(){G._BattleTEST();}
    public void _BattleTEST()
    {
        InitBattle( TEST_BATTLE.Make_BattleParty() );
    }

    // 적군을 세팅해서 넘겨주기
    public void InitBattle( BattleParty bp_enemy )
    {
        PlayerMover.G.SetInputAble( false );

        // 인트로 애니
        // bgm

        TURN = 1;
        battleParty_Enemy = bp_enemy;
        view_Enemy.Init();

        Panel_BattleMain.InitBattle();
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

    static public void Phase_InputCommand(){G._Phase_InputCommand();}
    public void _Phase_InputCommand()
    {
        Debug.Log( "배틀 커맨드 대기 : " + TURN );
        go_Cam_BattleTurn.SetActive(false);
        // 유아이 열고 , 커맨드 카메라 활성화 
        Panel_BattleMain.G.TurnStart_InputCommand();
        go_Cam_InputCommand.SetActive(true);

    }
    
    static public void Phase_StartTurn()
    {
        G._Phase_StartTurn();
    }

    public void _Phase_StartTurn()
    {
        // 카메라 전환 
        // 기다렸다가  턴 매니저 시작
        go_Cam_BattleTurn.SetActive(true);
        StartCoroutine( CO_WaitTurnStart() );
    }


    IEnumerator CO_WaitTurnStart()
    {
        yield return new WaitForSeconds( delay_StartTurn );
        BattleTurn.TurnStart();
    }

    static public void NextCharAction()
    {
        G._NextCharAction();
    }

    public void _NextCharAction()
    {
        Debug.Log( "NextCharAction ->" );
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

    static public void Phase_EndTurn(){G._Phase_EndTurn();}
    public void _Phase_EndTurn()
    {
        // 턴 증가
        TURN++;

        _Phase_InputCommand();
    }

    public void ResultBattleWin()
    {
        StartCoroutine( CO_WaitResult_Win() );
    }

    IEnumerator CO_WaitResult_Win()
    {
        yield return new WaitForSeconds( delay_Result );

        result_inf.Clear();
        GTF_CSV.ResultBattle( battleParty_Enemy.GetALL() , out result_inf.gold , out result_inf.exp );
        SJ_UnityUIMng_Curve.Open( "Panel_BattleWin" );
    }

    static public void OnOK_ResultPopup()
    {
        G._OnOK_ResultPopup();
    }

    public void _OnOK_ResultPopup()
    {
        // 배틀인풋 카메라 해제
        Panel_BattleMain.G.gameObject.SetActive(false);
        go_Cam_InputCommand.SetActive(false);
        go_Cam_BattleTurn.SetActive(false);
        PlayerMover.G.SetInputAble( true );
    }

}
