using System.Collections.Generic;
using UnityEngine;

public class BattleMain : MonoBehaviour
{
    static public BattleMain G;
    public GameObject               go_CameraBattle; // 배틀 카메라
    public BattlePartyView_Enemy    view_Enemy;
    public Panel_BattleMain         panel_BattleMain;    

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
        go_CameraBattle.SetActive(true);
        panel_BattleMain.StartBattle();
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
}
