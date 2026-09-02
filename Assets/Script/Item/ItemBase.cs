using System.Collections.Generic;
using UnityEngine;

public class ItemBase
{
    public CSV_Item csv;

    // 

    public int count = 1;

    // 장비 캐릭터 , 장비 아이템일때만
    public CharBase eq_chr;



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
    }


    public void Add_EquipChar( CharBase charBase )
    {
        this.eq_chr = charBase;

        if( csv.charPrcValue.HP > 0 )               charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.HP            , this , csv.charPrcValue.HP );
        if( csv.charPrcValue.MP > 0 )               charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.MP            , this , csv.charPrcValue.MP );
        if( csv.charPrcValue.ACTION_SPEED > 0 )     charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.ACTION_SPEED  , this , csv.charPrcValue.ACTION_SPEED );
        if( csv.charPrcValue.ATK_P > 0 )            charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.ATK_P         , this , csv.charPrcValue.ATK_P );
        if( csv.charPrcValue.DEF_P > 0 )            charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.DEF_P         , this , csv.charPrcValue.DEF_P );
        if( csv.charPrcValue.HIT_RATE_P > 0 )       charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.HIT_RATE_P    , this , csv.charPrcValue.HIT_RATE_P );
        if( csv.charPrcValue.EVASION_RATE_P > 0 )   charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.EVASION_RATE_P, this , csv.charPrcValue.EVASION_RATE_P );
        if( csv.charPrcValue.ATK_M > 0 )            charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.ATK_M         , this , csv.charPrcValue.ATK_M );
        if( csv.charPrcValue.DEF_M > 0 )            charBase.csv.charPrcValue.ADD_VAL_INF( (int)CHAR_STAT.DEF_M         , this , csv.charPrcValue.DEF_M );
    }

    public void Remove_EquipChar( CharBase charBase )
    {
        this.eq_chr = null;
        charBase.csv.charPrcValue.REMOVE_VAL_INF_RefClass( this );
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
