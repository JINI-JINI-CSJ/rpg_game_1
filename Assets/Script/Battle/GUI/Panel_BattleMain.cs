using System;
using System.Collections.Generic;
using Unity.AppUI.Navigation;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class Panel_BattleMain : MonoBehaviour
{
    public PlayerInput playerInput;

    public CursorDirectionInput cursorDirectionInput;

    public GameObject go_MENU;

    public List<Button> buttons_ChrCmd;

    // 커맨드 입력
    List<CharBase> charBases_inputWait = new();

    bool auto_mode;

    void Awake()
    {
        cursorDirectionInput.RegisterMoveX_One( InputCursor_X );
        cursorDirectionInput.RegisterMoveY_One( InputCursor_Y );
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBattle()
    {
        gameObject.SetActive(true);
        TurnStart_InputCommand();
    }


    public void TurnStart_InputCommand()
    {
        go_MENU.SetActive(true);

        // 전투불능이나 수면 등등 제외하고 입력 가능 캐릭터들
        charBases_inputWait = Player.battleParty.GetBattleCommandAble();
        foreach( var s in charBases_inputWait )
        {
            s.ClearBattleCommand();
        }
        NextInputCommand();
    }


    CharBase cur_input_wait = null;
    public void NextInputCommand()
    {
        BattlePartyView_Player.All_HideInputAni();
        cur_input_wait = null;
        foreach( var s in charBases_inputWait )
        {
            if( s.AbleBattleCommand() )
            {
                cur_input_wait = s;
                break;
            }
        }

        if( cur_input_wait == null )
        {
            TurnStart();
            return;
        }
        cur_input_wait.Call_BattleCommandInputWait(true);
    }

    public void TurnStart()
    {
        // 전투 시작
        go_MENU.SetActive(false);
        // 배틀 턴 시작        
        
    }

    public void OnTurnEnd()
    {
        // 오토 모드면 다시 자동 전투

    }


    // 현재 선택한 스킬 및 일반 공격
    SkillBase skillBase_cur_sel;

    public void OnBT_Attack()
    {
        // BattleCommand command = new();
        // command.cmd_cate = BATTLE_COMMAND_CATE.Attack;
        // cur_input_wait.command = command;
        // NextInputCommand();

        skillBase_cur_sel = cur_input_wait.GetDefaultSkill();

        skillBase_cur_sel.SelectTarget();
    }

    

    public void OnBT_Skill()
    {
        // 전투 스킬창 오픈 
    }

    // 스킬 선택완료
    public void OnBT_Skill_Select( object arg )
    {
        
    }

    public void OnBT_Guard()
    {
        BattleCommand command = new();
        command.cmd_cate = BATTLE_COMMAND_CATE.Guard;
        cur_input_wait.command = command;
        NextInputCommand();
    }

    public void OnBT_Item()
    {
        
    }

    public void OnBT_Item_Select( object arg )
    {
        
    }

    public void OnBT_Escape()
    {
        BattleCommand command = new();
        command.cmd_cate = BATTLE_COMMAND_CATE.Escape;
        cur_input_wait.command = command;
        NextInputCommand();
    }

    public void OnBT_BACK()
    {
        // 이전 캐릭으로 다시 롤백 

        
        // 마지막으로 커맨드 등록된 캐릭
        CharBase charBase_last_cmd = null;
        foreach( var s in charBases_inputWait )
        {
            if( s.AbleBattleCommand() == false )
            {
                charBase_last_cmd = s;
            }
        }

        if( charBase_last_cmd == null ) return;
        // 등록된 커맨드 지우고 넥스트
        
        charBase_last_cmd.command = null;
        NextInputCommand();
    }

    public void OnBT_AutoBattle()
    {
        auto_mode = !auto_mode;

        if( auto_mode )
        {
            AutoBattleSetting();    
            TurnStart();
        }

    }

    void AutoBattleSetting()
    {
        foreach( var s in Player.battleParty.GetBattleCommandAble() )
        {
            if( s.AbleBattleCommand() ) s.Auto_Command();
        }
    }

    public void InputCursor_X( int off )
    {
        //MoveCursor( off , 0 );
    }

    public void InputCursor_Y( int off )
    {
        //MoveCursor( 0 , off );
    }

}
