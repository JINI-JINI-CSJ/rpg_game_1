using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase
{
    // 기본 데이터
    public uint ID;
    public CSV_Item csv;
    //public List<CSV_Skill> lt_csv_skill_addEff = new(); // 추가 효과가 있을경우    

    public SkillBaseGroup skillBaseGroup = new();

    // 인게임
    public CharPrcValue charPrcValue = new();
    // 레벨
    public int LEVEL;
    // 
    public int cur_count = 1;

    // 장비 캐릭터 , 장비 아이템일때만
    public CharBase cur_eq_chr;

    // 스킬객체
    //public List<SkillBase> lt_skill = new();

    static public ItemBase InstItemBase( int csv_id )
    {
        CSV_Item csv = GTF_CSV.csv_ItemPage_ALL.Find_Int( csv_id ) as CSV_Item;
        if( csv == null ) return null;
        return InstItemBase( csv );
    }

    static public ItemBase InstItemBase( CSV_Item csv )
    {
        ItemBase inst_item = null;
        if( string.IsNullOrEmpty( csv.class_name ) == false )
        {
            inst_item = SJ_CSharpUtil.NewClass_Str( csv.class_name ) as ItemBase;
        }
        else
        {
            inst_item = new();
        }
        inst_item.SetCSV(csv);
        return inst_item;
    }

    public void SetCSV( CSV_Item _csv )
    {
        csv = _csv;
        charPrcValue.Copy( csv.charPrcValue );
    }

    public void AddSkillCSV( List<CSV_Skill> cSV_s )
    {
        skillBaseGroup.AddCSV( cSV_s );
        skillBaseGroup.UpdateSkillBase();
    }


    public void Add_EquipChar( CharBase charBase )
    {
        this.cur_eq_chr = charBase;

        if( charPrcValue.HP > 0 )               charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.HP            , this , charPrcValue.HP );
        if( charPrcValue.MP > 0 )               charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.MP            , this , charPrcValue.MP );
        if( charPrcValue.ACTION_SPEED > 0 )     charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.ACTION_SPEED  , this , charPrcValue.ACTION_SPEED );
        if( charPrcValue.ATK_P > 0 )            charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.ATK_P         , this , charPrcValue.ATK_P );
        if( charPrcValue.DEF_P > 0 )            charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.DEF_P         , this , charPrcValue.DEF_P );
        if( charPrcValue.HIT_RATE_P > 0 )       charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.HIT_RATE_P    , this , charPrcValue.HIT_RATE_P );
        if( charPrcValue.EVASION_RATE_P > 0 )   charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.EVASION_RATE_P, this , charPrcValue.EVASION_RATE_P );
        if( charPrcValue.ATK_M > 0 )            charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.ATK_M         , this , charPrcValue.ATK_M );
        if( charPrcValue.DEF_M > 0 )            charBase.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.DEF_M         , this , charPrcValue.DEF_M );
    }

    public void Remove_EquipChar( CharBase charBase )
    {
        this.cur_eq_chr = null;
        charBase.charPrcValue.REMOVE_VAL_INF_RefClass( this );
    }


    virtual public BATTLE_ACTION_TARGET GetTargetType()
    {
        return BATTLE_ACTION_TARGET.One_Self_ALL;
    }

    // 인자 : 사용자
    virtual public void SelectTarget( CharBase chr , SJ_COMMON.Func_Arg func_ok = null , SJ_COMMON.Func_VOID func_cancel = null )
    { 
        List<BATTLE_SEL_GROUP> lt = BattleTargetSelector.MakeSelectGroup( chr.armyForce , GetTargetType() );
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

    virtual public void Action( BATTLE_SEL_GROUP sel_group ){}
}
