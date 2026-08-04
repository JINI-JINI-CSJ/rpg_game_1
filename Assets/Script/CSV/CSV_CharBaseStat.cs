using System;
using UnityEngine;

// ``ID	이름	설명	리소스	등급	직업 대 분류	무기 정의	방어구 정의	HP	행동속도	물공	물방	물공명중	물공회피	마공	마방													
public class CSV_CharBaseStat : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string res;
    public string res3D;
    public int grade;
    public float pow_ratio; // 적군 전용 , 강함 가중치 , 경험치 계산등에 사용한다. 예) 하급고블린 -> 1 , 오우거 -> 2
    public JOB_BASE jOB_BASE;
    public int Weapon_ID;
    public int Armor_ID;
    public CharPrcValue charPrcValue = new();

    // 아이템 정의 객체들
    public CSV_EqItemDefine csv_EqItem_Weapon;
    public CSV_EqItemDefine csv_EqItem_Armor;


    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);
        name = Next();
        desc = Next();
        res = Next();
        res3D = Next();
        grade = Next_Int();
        pow_ratio = Next_Float();
        Enum.TryParse( Next() , out jOB_BASE );
        Weapon_ID = Next_Int();
        Armor_ID = Next_Int();
        charPrcValue.ReadCSV(this);
    }

    public CSV_CharBaseStat Copy()
    {
        CSV_CharBaseStat s = new();
        s.name = name;
        s.desc = desc;
        s.res = res;
        s.res3D = res3D;
        s.grade = grade;
        s.pow_ratio = pow_ratio;
        s.jOB_BASE = jOB_BASE;
        s.Weapon_ID = Weapon_ID;
        s.Armor_ID = Armor_ID;
        s.csv_EqItem_Weapon = csv_EqItem_Weapon;
        s.csv_EqItem_Armor = csv_EqItem_Armor;

        s.charPrcValue.Copy( charPrcValue );
        
        return s;
    }
}



public class CSV_CharBaseStatPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_CharBaseStat();
    }

    // 로드 애프터 
    // 아이템 디파인 연결
    public override void LoadAfter()
    {
        
    }
}
