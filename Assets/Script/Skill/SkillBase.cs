using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SKILL_NORMAL_INF
{
    public BATTLE_ACTION_TARGET  target;
    public int      base_val;
    public int      mp;
    public float    add_pow;
}

/// <summary>
/// 스킬 베이스
/// </summary>

public class SkillBase 
{
    public CSV_Skill csv;    

    // 기본 공격력 이외에 기타 지원 스킬도 참조 할수 있다.
    // 탐문 , 함정 해체 등등  메이킹으로 만들었을 경우 참조하자.
    public SKILL_NORMAL_INF skill_normal_inf;

    // 부가 효과 있을때만.
    public SkillBase skill_addEff; 

    public CharBase charHave;
    public int LEVEL;

    static public SkillBase InstSkill( int csv_id , int level = 1 )
    {
        CSV_Skill csv = GTF_CSV.csv_SkillPage_ALL.Find_Int( csv_id , true ) as CSV_Skill;
        if( csv == null )
        {
            return null;
        }
        return InstSkill( csv , level );
    }

    static public SkillBase InstSkill( CSV_Skill csv , int level = 1 )
    {
        SkillBase inst_skill = null;
        if( string.IsNullOrEmpty( csv.class_name ) == false )
        {
            inst_skill = SJ_CSharpUtil.NewClass_Str( csv.class_name ) as SkillBase;
            if( inst_skill == null )
            {
                Debug.LogError( "에러!!! csv.class_name : " + csv.ID_int );
                return null;
            }
        }
        else
        {
            inst_skill = new();
        }
        inst_skill.SetCSV(csv);
        inst_skill.LEVEL = level;
        return inst_skill;
    }

    public void SetCSV( CSV_Skill _csv )
    {
        csv = _csv;
    }

    virtual public BATTLE_ACTION_TARGET GetTargetType()
    {
        if( skill_normal_inf != null ) return skill_normal_inf.target;
        return BATTLE_ACTION_TARGET.One_Opp_Front;
    }

    virtual public void SelectTarget( SJ_COMMON.Func_Arg func_ok = null , SJ_COMMON.Func_VOID func_cancel = null )
    {
        List<BATTLE_SEL_GROUP> lt = BattleTargetSelector.MakeSelectGroup( charHave.armyForce , GetTargetType() );
        OnSelectTargetDefault( lt );

        BattleTargetSelector.Show( true , func_ok , func_cancel );
    }

    virtual public void OnSelectTargetDefault( List<BATTLE_SEL_GROUP> lt )
    {
        if( lt.Count > 0 )
        {
            BattleTargetSelector.SetCursor( lt[0] );
        }
    }

    virtual public void Action( BATTLE_SEL_GROUP sel_group )
    {
        OnAction( sel_group );
        foreach( var s in sel_group.chars )
        {
            OnActionChar(s);
            s.Call_RecvSkill( this );
        }
    }
    virtual public void OnAction( BATTLE_SEL_GROUP sel_group ){}
    virtual public void OnActionChar( CharBase chr ){}



    // 플레이어 (바닥 유아이) 에  스킬 효과 
    virtual public void OnViewEffect_Player( GameObject go ){}

    // 적군에  스킬 효과 
    virtual public void OnViewEffect_Enemy( GameObject go ){}
}


// 스킬객체 그룹
// 일단 중복 스킬은 제외하자.
public class SkillBaseGroup
{
    public HashSet<int> hs_csv_id = new();
    public List<SkillBase> skills = new();

    public void AddCSV( int csv_id )
    {
        hs_csv_id.Add(csv_id);
    }

    public void AddCSV( List<CSV_Skill> csvs )
    {
        foreach( var s in csvs )
        {
            AddCSV( s.ID_int );
        }   
    }

    public void UpdateSkillBase()
    {
        foreach( var s in hs_csv_id )
        {
            SkillBase skill = skills.Find( x=> x.csv.ID_int == s);
            if( skill == null )
            {
                skill = SkillBase.InstSkill( s );
                skills.Add(skill);
            }
        }
    }

    public void Read( BinaryReader br )
    {
        hs_csv_id.Clear();
        skills.Clear();
        int c = br.ReadInt32();
        for( int i = 0 ; i < c ; i++ )
        {
            int csv_id = br.ReadInt32();
            AddCSV(csv_id);
        }
        UpdateSkillBase();
    }

    public void Write( BinaryWriter bw )
    {
        bw.Write( hs_csv_id.Count );
        foreach( var s in hs_csv_id )
        {
            bw.Write( s );
        }
    }
}