using System.Collections.Generic;
using UnityEngine;

public class BattleTurn : MonoBehaviour
{
    static public BattleTurn G;

    // 각각 캐릭터가 행동을 등록하고 실행한다.
    public SJ_SimpleSync syncTurn = new();

    public const string BATTLE_SYNC_ACTION = "BATTLE_SYNC_ACTION";

    public List<CharBase> char_turn = new();

    void Awake()
    {
        G = this;
        syncTurn.SetGlobalInstName( BATTLE_SYNC_ACTION );
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void TurnStart()
    {
        // 
        G._TurnStart();
        G._NextCharAction();
    }

    public void _TurnStart()
    {
        char_turn.Clear();
        
        char_turn.AddRange( Player.battleParty.GetBattleLive() );
        char_turn.AddRange( BattleMain.G.battleParty_Enemy.GetBattleLive() );

        // 행동 속도로 순서
        char_turn.Sort( 
            (x,y) =>
            {
                if( x.csv.charPrcValue.ACTION_SPEED > y.csv.charPrcValue.ACTION_SPEED ) return -1;
                if( x.csv.charPrcValue.ACTION_SPEED < y.csv.charPrcValue.ACTION_SPEED ) return 1;
                return 0;
            }
        );
    }

    static public void NextCharAction()
    {
        G._NextCharAction();
    }

    public void _NextCharAction()
    {
        if( char_turn.Count < 1 )
        {
            // 턴 종료
            BattleMain.Phase_EndTurn();
            return;
        }
        CharBase charBase = char_turn[0];
        charBase.TurnAction_Start();
    }



}
