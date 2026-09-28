using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// ``ID	이름	설명	리소스  리소스3D	등급	스킬 대 분류	아이템 태그	  속성 태그 	클래스이름	인자1	2	3	4	5	6	7
public class CSV_Skill : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string res;
    public string res3d;
    public string tag;
    public int grade;
    public SKILL_ACTIVE_TYPE skill_type;
    public string TAG_ITEM;
    public string TAG_MAGIC_PROP;
    public string class_name;

    public BATTLE_ACTION_TARGET act_target;

    public List<string> args;
    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);
        name = Next();
        desc = Next();
        res = Next();
        res3d = Next();
        tag = Next();
        grade = Next_Int();
        Enum.TryParse( Next() , out skill_type );
        TAG_ITEM = Next();
        TAG_MAGIC_PROP = Next();
        class_name = Next();
        Enum.TryParse( Next() , out act_target );
        Remain_Data( args );
    }

    public string GetName(){return SJ_Language.Str( "SKILL_NAME" , ID_int );}
    public string GetDesc(){return SJ_Language.Str( "SKILL_DESC" , ID_int );}
}

public class CSV_SkillPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_Skill();
    }

    public List<CSV_Skill> GetTag_Contain( string tag )
    {
        List<CSV_Skill> lt = new();
        foreach( var s in dic_int.Values.Cast<CSV_Skill>() )
        {
            if( s.tag.Contains( tag ) ) lt.Add(s);
        }
        return lt;
    }

    public CSV_Skill GetTag_Contain_One( Mng_X128SS rd , string tag )
    {
        List<CSV_Skill> lt = GetTag_Contain(tag);
        return rd.RandomList( lt );
    }

    // 개수만큼 중복되지 않게 여러개 가져오기
    public List<CSV_Skill> GetTag_Contain_Count( Mng_X128SS rd , string tag , int count )
    {
        List<CSV_Skill> lt = GetTag_Contain( tag );
        List<CSV_Skill> lt_add = new();
        for( int i = 0 ; i < count ; i++ )
        {
            CSV_Skill csv = rd.RandomList(lt);
            if( csv == null ) return lt_add;
            lt_add.Add(csv);
        }
        return lt_add;
    }
}
