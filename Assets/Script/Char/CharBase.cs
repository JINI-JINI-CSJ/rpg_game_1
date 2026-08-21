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
    public CSV_CharBaseStat csv;    
    public int LEVEL;
    public _ARMY_FORCE armyForce;
    public int front_back; // 1 : 전열 , 2 : 후열
    public int cur_HP;
    public int cur_MP;

    public SJ_COMMON.Func_VOID func_TurnInit;

    public SJ_COMMON.Func_Arg_BOOL func_BattleCommandInputWait;
    public SJ_COMMON.Func_Arg_BOOL func_SelectTarget;           // 스킬 및 아이템 대상 선택
    public SJ_COMMON.Func_VOID func_ANI_ATK;
    public SJ_COMMON.Func_VOID func_ANI_Damage;
    public SJ_COMMON.Func_VOID func_ANI_KO;

    public SJ_COMMON.Func_Arg func_RecvSkill;   // 스킬 받기 , 공격 맞기 , 힐받기 , 아이템 사용 대상 등등

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
        for( int i = 0 ; i < (int)_EQUIP_CHR_PART.MAX ; i++ )items_EQ.Add(null);
    }

    public SkillBase GetDefaultSkill()
    {
        return skillBase_Default;
    }

    static public CharBase InstCharBase_CSV( int csv_id , int level , _ARMY_FORCE _force )
    {
        CSV_CharBaseStat csv_load = GTF_CSV.csv_Char_ALL.Find_Int( csv_id ) as CSV_CharBaseStat;
        if( csv_load == null )
        {
            Debug.LogError( "InstCharBase_CSV  csv_load == null : " + csv_id );
            return null;            
        }

        CharBase charBase = new();
        charBase.Make( csv_load , level , _force );
        return charBase;
    }

    public void Make( CSV_CharBaseStat _csv , int level , _ARMY_FORCE _force )
    {
        SetCSV( _csv );
        LEVEL = level;
        armyForce = _force;
        cur_HP = csv.charPrcValue.HP;
        cur_MP = csv.charPrcValue.MP;
        InitSkill_Chr();
    }
    public void SetCSV( CSV_CharBaseStat _csv )
    {
        csv = _csv.Copy();
    }

    //=============================================================================================
    // 스킬
    public SkillBase AddSkill( CSV_Skill csv_skill , List<SkillBase> skills)
    {
        SkillBase skill = SkillBase.InstSkill( csv_skill );
        skill.charHave = this;

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
    
    public List<SkillBase> GetSkills_ALL()
    {
        List<SkillBase> lt = new();
        lt.AddRange( skills_Chr );
        lt.AddRange( skills_ADD );
        return lt;
    }

    public List<SkillBase> GetSkills( SKILL_ACTIVE_TYPE skill_type )
    {
        List<SkillBase> all = GetSkills_ALL();

        if( skill_type == SKILL_ACTIVE_TYPE.ALL )
        {
            return all;
        }

        List<SkillBase> lt = new();

        foreach( var s in lt )
        {
            if( s.csv.skill_type == skill_type ) lt.Add(s);
        }
        return lt;
    }

    //
    //=============================================================================================


    //=============================================================================================
    // 장비 아이템

    public ItemBase GetEquipItem( _EQUIP_CHR_PART part )
    {
        return items_EQ[(int)part];
    }

    // 장비하기 
    // 반환 : 기존 장비
    public ItemBase Add_EquipItem( ItemBase item_eq )
    {
        ItemBase item_recent = Remove_EquipItem( item_eq.csv.eq_part );
        item_eq.Add_EquipChar( this );
        items_EQ[(int)item_eq.csv.eq_part] = item_eq;
        return item_recent;
    }

    public ItemBase Remove_EquipItem( _EQUIP_CHR_PART part )
    {
        ItemBase item_recent = GetEquipItem( part );
        if( item_recent != null )
        {
            item_recent.Remove_EquipChar( this );
        }
        return item_recent;
    }

    //
    //=============================================================================================


    public void GetDamage( int damage )
    {
        cur_HP -= damage;
        if( cur_HP <= 0 )
        {
            cur_HP = 0;
            func_ANI_KO?.Invoke();
        }
        else
        {
            Call_ANI_Damage();
        }
         

        Debug.Log( csv.name + " : 데미지 : " + damage + "      hp : " + cur_HP );
    }

    // 커맨드를 입력가능한 캐릭터
    // KO , 수면 , 마비 등등 캐릭터는 제외
    public bool AbleBattleCommand_Ready()
    {
        if( cur_HP < 1 ) return false;
        return true;
    }

    public bool Check_ExistCommand()
    {
        if( command == null ) return false;
        return true;
    }

    public void Call_TurnInit()
    {
        func_TurnInit?.Invoke();
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

    public void Call_RecvSkill( SkillBase skill )
    {
        func_RecvSkill?.Invoke( skill );
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
        command.skill = skillBase_Default;
        command.sel_group = BattleTargetSelector.RandomTargetOpp_One( armyForce );
        
    }

    virtual public void OnMakeCommand()
    {
        Auto_Command();
    }


    public void TurnAction_Start()
    {
        // 행동불가.. ko , 수면 마비 등등
        if( AbleBattleCommand_Ready() == false )
        {
            OnEnd_TurnAction();
            return;
        }

        if( command == null )
        {
            Debug.LogError( "커멘드 없음!!!" );
            return;
        }

        Call_ANI_ATK();
        if( command.skill != null )
        {
            command.skill.Action( command.sel_group );
            return;
        }

        if( command.item != null )
        {
            command.item.Action( command.sel_group );
            return;
        }

        // 도망 처리 등등

    }

    // 객체 뷰어에서 호출한다.
    public void OnEnd_TurnAction()
    {
        BattleMain.NextCharAction();
    }
}
