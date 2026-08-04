using System.Collections.Generic;
using UnityEngine;

public enum _EQUIP_CHR_PART
{
    None = -1,

    // 일단 간단하게 , 무기 , 방어구 , 악세 1234
    Weapon , 
    Armor , 
    Acc_1 , 
    Acc_2 ,
    Acc_3 , 
    Acc_4 ,

    MAX ,

}

public enum _ARMY_FORCE
{
    None = 0,
    Player , 
    Enemy , 
}

// 캐릭터 객체 기본 , 플레이어 파티 , 적군 등등
public class CharBase 
{
    public CSV_CharBaseStat csv;    
    public int LEVEL;
    public _ARMY_FORCE armyForce;
    public int front_back; // 1 : 전열 , 2 : 후열
    public int cur_HP;
    public int cur_MP;

    public SJ_COMMON.Func_Arg_BOOL func_BattleCommandInputWait;
    public SJ_COMMON.Func_Arg_BOOL func_SelectTarget;           // 스킬 및 아이템 대상 선택
    public SJ_COMMON.Func_VOID func_ANI_ATK;
    public SJ_COMMON.Func_VOID func_ANI_Damage;
    public SJ_COMMON.Func_VOID func_ANI_KO;
    public BattleCommand command;

    // 전투에서 공격을 선택했을때 기본 공격
    public SkillBase skillBase_Default;


    public List<ItemBase> items_EQ = new();

    // 캐릭터 내장 스킬
    public List<SkillBase> skills_Chr = new();


    // 추가 스킬
    public List<SkillBase> skills_ADD = new();

    public CharBase()
    {
        for( int i = 0 ; i < (int)_EQUIP_CHR_PART.MAX ; i++ )
        {
            items_EQ.Add(null);
        }
    }

    public SkillBase GetDefaultSkill()
    {
        return skillBase_Default;
    }

    public void Make( CSV_CharBaseStat _csv , int level , _ARMY_FORCE _force )
    {
        SetCSV( csv );
        LEVEL = level;
        armyForce = _force;
        cur_HP = csv.charPrcValue.HP;
        cur_MP = csv.charPrcValue.MP;
        InitSkill_Chr();
    }

    public SkillBase AddSkill( CSV_Skill csv_skill , List<SkillBase> skills)
    {
        SkillBase skill = SkillBase.InstSkill( csv_skill );

        skills.Add( skill );
        return skill;
    }

    public void InitSkill_Chr()
    {
        if( skills_Chr.Count > 0 ) return;

        // 기본 스킬, 일단 없는 경우는 없게..
        CSV_Skill csv_skill_weapon = GTF_CSV.csv_SkillPage_NORMAL.Find_Int( csv.Weapon_ID ) as CSV_Skill;
        if( csv_skill_weapon == null )
        {
            Debug.LogError( "기본 스킬 없음!!! : " + csv.ID_int );
            return;
        }
        skillBase_Default = AddSkill( csv_skill_weapon , skills_Chr );

        CSV_Skill csv_skill_armor = GTF_CSV.csv_SkillPage_NORMAL.Find_Int( csv.Armor_ID ) as CSV_Skill;
        if( csv_skill_armor != null )
        {
            AddSkill( csv_skill_armor , skills_Chr );
        }
        
    }

    public void SetCSV( CSV_CharBaseStat _csv )
    {
        csv = _csv.Copy();
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

    public void Call_SelectTarget( bool b )
    {
        func_SelectTarget?.Invoke( b );
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


    public void TurnAction_Start()
    {
        Call_ANI_ATK();
    }

    // 객체 뷰어에서 호출한다.
    public void OnEnd_TurnAction()
    {
        BattleMain.NextCharAction();
    }
}
