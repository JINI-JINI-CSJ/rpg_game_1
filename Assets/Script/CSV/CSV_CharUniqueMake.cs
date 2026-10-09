using UnityEngine;
using System.Collections.Generic;
using System.Linq;


// 캐릭터 유니크 메이킹 점수 설정

//``ID	이름	설명	클래스 등급	 ,수치 보너스 점수,	일반 스킬 개수,	궁극기 스킬 개수																			
public class CSV_CharUniqueMake : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string tag;
    public int class_grade;
    public int Score_chrVal;
    public int Score_SkillNormal;
    public int Score_SkillUltimate;

    //메이킹 생성 인원	
    public int making_total;
    // 소문 입수 개수
    public int rumor_num;

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);
        name = Next();
        desc = Next();
        tag = Next();
        class_grade = Next_Int();
        Score_chrVal = Next_Int();
        Score_SkillNormal = Next_Int();
        Score_SkillUltimate = Next_Int();
        making_total = Next_Int();
        rumor_num = Next_Int();
    }
}

public class CSV_CharUniqueMakePage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj(){return new CSV_CharUniqueMake();}

    public CSV_CharUniqueMake Find( string tag , int class_grade )
    {
        foreach( var s in dic_int.Values.Cast<CSV_CharUniqueMake>() )
        {
            if( s.tag == tag && s.class_grade == class_grade ) return s;
        }
        return null;
    }
}
