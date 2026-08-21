using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Panel_BattleMain : MonoBehaviour
{
    static public Panel_BattleMain G;

    // 오른쪽 사이드 메뉴
    public SJ_UIGridMove_PlayerInput input_MenuCommand;

    // 적군 타겟 마크
    public List<GameObject> enemy_target_marks;

    public Text text_TURN;

    public float delay_StartTurn = 0.5f;

    // 커맨드 입력
    List<CharBase> charBases_inputWait = new();

    bool auto_mode;

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

    static public void InitBattle(){G._InitBattle();}
    public void _InitBattle()
    {
        Active_TargetMark_ALL(false);
        auto_mode = false;
        gameObject.SetActive(true);
        TurnStart_InputCommand();        
    }


    public void EndBattle()
    {
        gameObject.SetActive(false);
    }


    public void TurnStart_InputCommand()
    {
        SJ_UnityUI_Util.TextString( text_TURN , BattleMain.G.TURN.ToString() );
        // 버튼들을 여기 함수로 바로 링크 한다.
        input_MenuCommand.gameObject.SetActive(true);

        // 전투불능이나 수면 등등 제외하고 입력 가능 캐릭터들
        charBases_inputWait = Player.battleParty.GetBattleCommandAble_Ready();
        foreach( var s in charBases_inputWait )
        {
            s.ClearBattleCommand();
        }
        NextInputCommand();
    }


    CharBase cur_input_char_wait = null;
    public void NextInputCommand()
    {
        Active_TargetMark_ALL(false);
        MenuInputActive(true);
        BattlePartyView_Player.All_HideInputAni();
        cur_input_char_wait = null;
        skillBase_cur_sel   = null;
        itemBase_cur_sel    = null;
        foreach( var s in charBases_inputWait )
        {
            if( s.Check_ExistCommand() == false )
            {
                cur_input_char_wait = s;
                break;
            }
        }

        if( cur_input_char_wait == null )
        {
            TurnStart();
            return;
        }
        cur_input_char_wait.Call_BattleCommandInputWait(true);
    }

    public void MenuInputActive( bool b )
    {
        input_MenuCommand.gameObject.SetActive(b);
    }

    public void TurnStart()
    {
        // 전투 시작
        MenuInputActive(false);
        // 배틀 턴 시작        
        BattleMain.Phase_StartTurn();
    }

    

    public void OnTurnEnd()
    {
        // 오토 모드면 다시 자동 전투
        if( auto_mode ) TurnStart();
    }


    // 현재 선택한 스킬 및 일반 공격
    SkillBase skillBase_cur_sel;

    public void OnBT_Attack()
    {
        MenuInputActive(false);
        skillBase_cur_sel = cur_input_char_wait.GetDefaultSkill();
        skillBase_cur_sel.SelectTarget( OnOK_TargetSelect , OnCancel_TargetSelect );
    }

    // 셀렉터에서 타겟을 선택했다.
    public void OnOK_TargetSelect( object arg )
    {
        BattleCommand command = new();
        command.sel_group = arg as BATTLE_SEL_GROUP;
        command.skill = skillBase_cur_sel;
        command.item = itemBase_cur_sel;
        cur_input_char_wait.command = command;
        NextInputCommand();
    }
    
    // 셀렉터에서 취소했다.
    public void OnCancel_TargetSelect()
    {
        MenuInputActive(true);
        skillBase_cur_sel = null;
    }

    public void OnBT_Skill()
    {
        MenuInputActive(false);
        // 전투 스킬창 오픈 
        PanelPopup_ItemSkill.Open_CharSkill( cur_input_char_wait , SKILL_ACTIVE_TYPE.Active_BATTLE , OnBT_Skill_Select );
    }

    // 스킬 선택완료
    // 공격과 똑같이 타겟 설정
    public void OnBT_Skill_Select( object arg )
    {
        skillBase_cur_sel = SJ_UIGridMove_PrcActiveObj.GetUserValue<SkillBase>( arg );
        skillBase_cur_sel.SelectTarget( OnOK_TargetSelect , OnCancel_TargetSelect );
    }

    public void OnBT_Guard()
    {
        BattleCommand command = new();
        command.cmd_cate = BATTLE_COMMAND_CATE.Guard;
        cur_input_char_wait.command = command;
        NextInputCommand();
    }

    public void OnBT_Item()
    {
        MenuInputActive(false);
        // 아이템창 오픈 
        PanelPopup_ItemSkill.Open_Item( 0 , 0 , 1 , OnBT_Item_Select );
    }

    ItemBase itemBase_cur_sel;
    public void OnBT_Item_Select( object arg )
    {
        itemBase_cur_sel = SJ_UIGridMove_PrcActiveObj.GetUserValue<ItemBase>( arg );
        itemBase_cur_sel.SelectTarget( cur_input_char_wait , OnOK_TargetSelect , OnCancel_TargetSelect );
    }

    public void OnBT_Escape()
    {
        BattleCommand command = new();
        command.cmd_cate = BATTLE_COMMAND_CATE.Escape;
        cur_input_char_wait.command = command;
        NextInputCommand();
    }

    public void OnBT_BACK()
    {
        // 이전 캐릭으로 다시 롤백 

        
        // 마지막으로 커맨드 등록된 캐릭
        CharBase charBase_last_cmd = null;
        foreach( var s in charBases_inputWait )
        {
            if( s.AbleBattleCommand_Ready() == false )
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
        foreach( var s in Player.battleParty.GetBattleCommandAble_Ready() )
        {
            if( s.AbleBattleCommand_Ready() ) s.Auto_Command();
        }
    }

    // 마크들을 적군 월드 객체랑 미리 연결 
    static public void Active_TargetMark( int idx ,  bool b )
    {
        G.enemy_target_marks[idx].SetActive(b);
    }

    public void Active_TargetMark_ALL( bool b )
    {
        foreach( var s in enemy_target_marks ) s.SetActive(b);
    }

    //=========================================================================
    // 유니티 인풋
    // public void OnNavigate( InputValue value )
    // {
    //     Vector2 input = value.Get<Vector2>();
    // }

    // 오토전투설정이고 전투중일 때 캔슬버튼이라면 
    // 오토 전투 취소 설정
    public void OnCancel( InputValue value )
    {
        
    }

}
