using System.Collections.Generic;
using UnityEngine;

public class ItemBase
{
    public CSV_Item csv;

    public int count = 1;

    // 장비 캐릭터 , 장비 아이템일때만
    public CharBase eq_chr;

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

    virtual public BATTLE_ACTION_TARGET GetTargetType()
    {
        return BATTLE_ACTION_TARGET.One_Self_ALL;
    }

    // 인자 : 사용자
    virtual public void SelectTarget( CharBase chr )
    { 
        List<BATTLE_SEL_GROUP> lt = BattleTargetSelector.MakeSelectGroup( chr.armyForce , GetTargetType() );
        OnSelectTargetDefault( lt );

        BattleTargetSelector.Show( true );
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
