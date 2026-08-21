using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPopup_ItemSkill : SJ_UIGridMove_PlayerInput
{
    public GameObject   go_DESC;
    public Text         text_Desc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 캐릭터 스킬
    static public void Open_CharSkill( CharBase charBase , SKILL_ACTIVE_TYPE skill_type , SJ_COMMON.Func_Arg func_ok )
    {
        List<_GRID_MOVE_DEFAULT_DATA> lt_grid = new();
        foreach( var s in charBase.GetSkills( skill_type ) ) _GRID_MOVE_DEFAULT_DATA.AddList( lt_grid , s.csv.GetName() , s );
        GameObject go_panel = SJ_UnityUIMng_Curve.Open( "PanelPopup_ItemSkill" );
        go_panel.GetComponent<PanelPopup_ItemSkill>().Listing_SetFunc_OK( lt_grid , func_ok );
    }

    // 아이템 
    static public void Open_Item( int useLobby,int useDungeon,int useBattle , SJ_COMMON.Func_Arg func_ok )
    {
        List<_GRID_MOVE_DEFAULT_DATA> lt_grid = new();
        foreach( var s in Player.inventory.GetItemUse( useLobby,useDungeon,useBattle ) ) _GRID_MOVE_DEFAULT_DATA.AddList( lt_grid , s.csv.GetName() , s );
        GameObject go_panel = SJ_UnityUIMng_Curve.Open( "PanelPopup_ItemSkill" );
        go_panel.GetComponent<PanelPopup_ItemSkill>().Listing_SetFunc_OK( lt_grid , func_ok );
    }


    override public void OnFunc_Move( GameObject obj )
    {
        if( go_DESC == null ) return;
        SkillBase skill = SJ_UIGridMove_PrcActiveObj.GetUserValue<SkillBase>( obj );
        SJ_UnityUI_Util.TextString( text_Desc , skill.csv.GetDesc() );
    }
}
