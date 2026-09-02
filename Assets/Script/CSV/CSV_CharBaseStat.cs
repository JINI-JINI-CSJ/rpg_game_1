using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ``ID	이름	설명	리소스	등급	직업 대 분류	무기 정의	방어구 정의	HP	행동속도	물공	물방	물공명중	물공회피	마공	마방													
public class CSV_CharBaseStat : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string res;
    public string res3D;
    public string tag;
    public int grade;
    public int rarityGrade;
    public float pow_ratio; // 적군 전용 , 강함 가중치 , 경험치 계산등에 사용한다. 예) 하급고블린 -> 1 , 오우거 -> 2
    public JOB_BASE jOB_BASE;
    public int Weapon_Skill_ID;
    public int Armor_Skill_ID;
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
        tag = Next();
        grade = Next_Int();
        rarityGrade = Next_Int();
        pow_ratio = Next_Float();
        Enum.TryParse( Next() , out jOB_BASE );
        Weapon_Skill_ID = Next_Int();
        Armor_Skill_ID = Next_Int();
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
        s.Weapon_Skill_ID = Weapon_Skill_ID;
        s.Armor_Skill_ID = Armor_Skill_ID;
        s.csv_EqItem_Weapon = csv_EqItem_Weapon;
        s.csv_EqItem_Armor = csv_EqItem_Armor;

        s.charPrcValue.Copy( charPrcValue );
        
        return s;
    }

    public CSV_Skill GetWeaponSkill(){return GTF_CSV.csv_SkillPage_NORMAL.Find_Int( Weapon_Skill_ID ) as CSV_Skill;}
    public CSV_Skill GetArmorSkill(){return GTF_CSV.csv_SkillPage_NORMAL.Find_Int( Armor_Skill_ID ) as CSV_Skill;}

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

    public List<CSV_CharBaseStat> GetTag_Contain( string tag , int rarityGrade = -1 )
    {
        List<CSV_CharBaseStat> lt = new();
        foreach( var s in dic_int.Values.Cast<CSV_CharBaseStat>() )
        {
            if( s.tag.Contains(tag) )
            {
                if( (rarityGrade > -1 && rarityGrade == s.rarityGrade) ||
                    rarityGrade == -1 )
                {
                    lt.Add(s);
                }
            }
        }
        return lt;
    }

    public CSV_CharBaseStat GetTag_Contain_Random( Mng_X128SS rd , string tag , int rarityGrade = -1)
    {
        List<CSV_CharBaseStat> lt = GetTag_Contain( tag , rarityGrade );
        return rd.RandomList(lt);
    }
}
