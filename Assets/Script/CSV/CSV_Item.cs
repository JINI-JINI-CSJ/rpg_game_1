using System;
using System.Collections.Generic;
using UnityEngine;

// 아이템 대분류
public enum _ITEM_TYPE
{
    None = 0 ,
    Consume ,   // 소비품
    Equip ,     // 장비품
    Unique ,    // 유니크 이벤트 등등
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


// ``ID	이름	설명	리소스	등급	클래스 장비-공격력	장비-방어력	인자1	2	3	4	5	
public class CSV_Item : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string res;
    public _ITEM_TYPE item_type;
    public int grade;
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
        Enum.TryParse( Next() , out item_type );
        grade = Next_Int();
        class_name = Next();
        Enum.TryParse( Next() , out eq_part );
        need_skill = Next_Int();
        charPrcValue.ReadCSV( this );
        Remain_Data( args );
    }

    public string GetName(){return name;}
    public string GetDesc(){return desc;}
}

public class CSV_ItemPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_Item();
    }
}