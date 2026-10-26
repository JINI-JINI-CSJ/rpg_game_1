using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 아이템 대분류
public enum _ITEM_TYPE
{
    None = 0 ,
    Consume ,       // 소비품
    Equip ,         // 장비품
    Collections ,   // 수집
    Unique ,        // 유니크 이벤트 등등
}


// 장비 아이템일 경우만 장착 부위
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


public class CSV_Item : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string res;
    public string tag;
    public _ITEM_TYPE item_type;
    public int useLobby;    // 마을 도시에서 사용가능
    public int useDungeon;  // 던전에서 사용가능
    public int useBattle;   // 전투중에 사용가능
    public int instanceBattleUse; // 전투중 인스턴스 사용 가능 여부 
    public int level;       // 기본 레벨
    public int grade;       // 등급
    public string class_name;
    public _EQUIP_CHR_PART eq_part;
    public int need_skill;

    public  CharPrcValue charPrcValue = new();

    public List<string> args;

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);

        name = Next();
        desc = Next();
        res = Next();
        tag = Next();
        Enum.TryParse( Next() , out item_type );
        useLobby = Next_Int();
        useDungeon = Next_Int();
        useBattle = Next_Int();
        instanceBattleUse = Next_Int();
        level = Next_Int();
        grade = Next_Int();
        class_name = Next();
        Enum.TryParse( Next() , out eq_part );
        need_skill = Next_Int();
        charPrcValue.ReadCSV( this );
        Remain_Data( args );
    }

    public string GetName()
    {
        return SJ_Language.Str( "ITEM_NAME" , ID_int );
    }
    public string GetDesc()
    {
        return SJ_Language.Str( "ITEM_DESC" , ID_int );
    }

    // 
    public bool UseCheckUSE_OR( int useLobby,int useDungeon,int useBattle )
    {
        if( this.useLobby == useLobby || this.useDungeon == useDungeon || this.useBattle == useBattle ) return true;
        return false;
    }

    // 월드 메이킹 때 설정한 등급
    public int matter_grade = -1;

    public int ScoreMatter()
    {
        int score = matter_grade * GTF_CSV.csv_Config.making_recipe_matter_grade_score;
        if( score < 1 ) score = 1;
        return score;
    }
}

public class CSV_ItemPage : SJ_CSV_BasePage
{
    public Dictionary<_ITEM_TYPE , List<CSV_Item>> dic_itemType = new();

    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_Item();
    }

    public override void LoadAfter()
    {
        foreach( var s in dic_int.Values.Cast<CSV_Item>() )
        {
            List<CSV_Item> lt_type = null;
            if( dic_itemType.TryGetValue( s.item_type , out lt_type ) == false )
            {
                lt_type = new();
                dic_itemType[s.item_type] = lt_type;
            }
            lt_type.Add(s);
        }
    }

    public List<CSV_Item> GetRangeLevel( int lv_s , int lv_e )
    {
        List<CSV_Item> lt = new();
        foreach( var s in dic_int.Values.Cast<CSV_Item>() )
        {
            if( s.level >= lv_s && s.level <= lv_e ) lt.Add(s);
        }
        return lt;
    }

    public CSV_Item GetRangeLevel_One( Mng_X128SS rd , int lv_s , int lv_e )
    {
        List<CSV_Item> lt = GetRangeLevel( lv_s , lv_e );
        return rd.RandomList( lt );
    }

    public List<CSV_Item> GetTag( string tag )
    {
        List<CSV_Item> lt = new();
        foreach( var s in dic_int.Values.Cast<CSV_Item>() )
        {
            if( s.tag.Contains( tag ) ) lt.Add(s);
        }
        return lt;
    }

    public CSV_Item GetTag_One( Mng_X128SS rd , string tag )
    {
        List<CSV_Item> lt = GetTag(tag);
        return rd.RandomList(lt);
    }

    public List<CSV_Item> GetItemTypeList( _ITEM_TYPE tYPE )
    {
        List<CSV_Item> items = null;
        dic_itemType.TryGetValue( tYPE , out items );
        return items;
    }

    public List<CSV_Item> GetItemTypeList_Tag( _ITEM_TYPE tYPE , string tag )
    {
        List<CSV_Item> items_tag = new();
        List<CSV_Item> items = GetItemTypeList( tYPE );

        if( string.IsNullOrEmpty( tag ) )
        {
            return items;
        }

        if( items != null )
        {
            foreach( var s in items )
            {
                if( s.tag.Contains( tag ) ) items_tag.Add( s );
            }
        }
        return items_tag;
    } 

    public List<CSV_Item> GetItemTypeList_TagGrade( _ITEM_TYPE tYPE , string tag , int grade )
    {
        List<CSV_Item> items_tag = GetItemTypeList_Tag(tYPE , tag );
        List<CSV_Item> items = new();

        foreach( var s in items_tag )
        {
            if( grade == s.grade ) items.Add( s );
        }
        return items;
    }

    public CSV_Item GetItemTypeList_TagGrade_One( Mng_X128SS rd , _ITEM_TYPE tYPE , string tag , int grade )
    {
        List<CSV_Item> items = GetItemTypeList_TagGrade( tYPE , tag , grade );
        return rd.RandomList( items );
    }
}