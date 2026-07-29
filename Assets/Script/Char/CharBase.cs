using System.Collections.Generic;
using UnityEngine;

public enum _ARMY_FORCE
{
    None = 0,
    Player , 
    Enemy , 
}

// 캐릭터 객체 기본 , 플레이어 파티 , 적군 등등
public class CharBase 
{

    public _ARMY_FORCE armyForce;

    public CSV_CharBaseStat csv;

    public int cur_HP;
    public int cur_MP;

    public SJ_COMMON.Func_Arg_BOOL func_BattleCommandInputWait;
    public SJ_COMMON.Func_VOID func_ANI_ATK;
    public SJ_COMMON.Func_VOID func_ANI_Damage;

    public BattleCommand command;

    public void SetCSV( CSV_CharBaseStat _csv )
    {
        csv = _csv;

    }

    public bool AbleBattleCommand()
    {
        if( cur_HP < 1 ) return false;
        return true;
    }
    
    public void Call_BattleCommandInputWait( bool b )
    {
        func_BattleCommandInputWait?.Invoke(b);
    }
    
    public void Call_ANI_ATK()
    {
        func_ANI_ATK?.Invoke();
    }

    public void Call_ANI_Damage()
    {
        func_ANI_Damage?.Invoke();
    }

    public bool IsLive()
    {
        if( cur_HP > 0 ) return true;
        return false;
    }

    public List<SkillBase> GetSkills_Battle()
    {
        List<SkillBase> skills = new();
        return skills;
    }

    public void ClearBattleCommand()
    {
        command = null;
    }

    public bool InputAble_BattleCommand()
    {
        if( command == null ) return true;
        return false;
    }


    public void Auto_Command()
    {
        // 공격형 , 지원형 등에 따라서 ..
        // 지원형이고 , 아군에 회복 등이 필요한 경우이면 회복 가능 스킬

        // 일단 무조건 공격
        command = new BattleCommand();
        command.cmd_cate = BATTLE_COMMAND_CATE.Attack;
    }


    public void ANI_ATK_Start()
    {
        Call_ANI_ATK();
    }

}
