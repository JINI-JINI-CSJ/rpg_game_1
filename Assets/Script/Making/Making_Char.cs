using System.Collections.Generic;
using UnityEngine;


//=================================================================================================
// 전사 , 마법사 , 지원가
// 직업 태그를 다시 분류

// 직업 태그 분류
// 전사 : 일반 , 방어형 , 공격형 등등
// 마법사 : 공격형 , 힐 , 버프 , 디버프 등등
// 지원가 : 탐색 , 생존 , 지식 , 생산 등등 

// 1. 각 도시마다 경향
// 2. 처음 생성시 유니크 캐릭터

public class _BIAS_JOB : _BIAS_COMMON
{
    // 2 단계로 진행
    // 1 . 대 분류      (현 클래스)
    // 2 . 태그 분류    (맴버 클래스)

    public _BIAS_COMMON bias_tagFIGHTER = new();
    public _BIAS_COMMON bias_tagWIZARD = new();
    public _BIAS_COMMON bias_tagSUPPORTER = new();

    override public void OnSetRandom_Init()
    {
        bias_tagFIGHTER.SetRandom_Init( rd_init , rd_random ); 
        bias_tagWIZARD.SetRandom_Init( rd_init , rd_random ); 
        bias_tagSUPPORTER.SetRandom_Init( rd_init , rd_random ); 

        // 대분류
        AddObj( JOB_BASE.FIGHTER );
        AddObj( JOB_BASE.WIZARD );
        AddObj( JOB_BASE.SUPPORTER );

        // 태그
        for( int i = 1 ; i < (int)JOB_BASE.MAX; i++ )
        {
            string tag = ((JOB_BASE)i).ToString();
            List<string> lt_csv = GTF_CSV.csv_TagDefinePage.GetTagPart_Str(tag);

            _BIAS_COMMON bias_tag = null;
            switch( i )
            {
                case 1: bias_tag = bias_tagFIGHTER;break;
                case 2: bias_tag = bias_tagWIZARD;break;
                case 3: bias_tag = bias_tagSUPPORTER;break;
            }
            foreach( var s in lt_csv )
            {
                bias_tag.AddObj( s );
            }
        }
    }

    public string RandomTAG()
    {
        JOB_BASE job = (JOB_BASE)Random();
        _BIAS_COMMON bias_tag = null;
        switch( job)
        {
            case JOB_BASE.FIGHTER:      bias_tag = bias_tagFIGHTER;break;
            case JOB_BASE.WIZARD:       bias_tag = bias_tagWIZARD;break;
            case JOB_BASE.SUPPORTER:    bias_tag = bias_tagSUPPORTER;break;
        }
        return bias_tag.Random() as string;
    }
}


// 정해진 태그나 직업등으로 메이킹
// ==일반==
//  - 기본 수치에서 낮은 추가 점수
//  - 등급에 해당하는 기본 스킬 
// 
// ==유니크==
//  - 기본 수치에서 높은 추가 점수
//  - 점수 높은 메이킹 스킬을 여러개 붙이기
public class Making_Char
{
    public _BIAS_JOB bias_job;

    public void BiasInit( Mng_X128SS rd_make ,Mng_X128SS rd_inGame = null )
    {
        bias_job = new();
        bias_job.SetRandom_Init( rd_make , rd_inGame );
    }

    public CharBase Make_Player( int sc_stat = 0 , int add_skill = 0 , int sc_skill = 0 , Mng_X128SS rd = null )
    {
        if( rd == null ) rd = bias_job.GetRandomClass();
        return MakingChr( rd , bias_job.RandomTAG() , 1 , sc_stat , add_skill , sc_skill );
    }

    public CharBase Make_Enemy( int sc_stat = 0 , int add_skill = 0 , int sc_skill = 0 , Mng_X128SS rd = null )
    {
        if( rd == null ) rd = bias_job.GetRandomClass();
        return MakingChr( rd , bias_job.RandomTAG() , 2 , sc_stat , add_skill , sc_skill );
    }
    

    // prop_type -> 0 : 방어 속성 안함 , 1 : 마법일 경우 마법 속성 따라감 , 2 : 랜덤(주로 적군들)
    static public CharBase MakingChr( Mng_X128SS rd , string tag , int chr_prop_type , int sc_stat , int add_skill , int sc_skill )
    {
        // 기본 csv 
        CSV_CharBaseStat csv = GTF_CSV.csv_Char_ALL.GetTag_Contain_Random( rd , tag );
        CharBase charBase = CharBase.InstCharBase_CSV( csv , 1 , _ARMY_FORCE.Player );

        // 스탯 보너스
        charBase.csv.charPrcValue.RandomStatBonus( rd , sc_stat , GTF_CSV.csv_Config.makeChar_statAddFix );

        // 무작위 스킬 
        for( int i = 0 ; i < add_skill ; i++ )
        {
            SkillBase skill = Skill_MAKE_NormalGrade.Make( tag , sc_skill , rd );
            charBase.AddSkill_ADD( skill );
        }

        // 방어 마법 속성
        switch( chr_prop_type )
        {
            case 0:break;
            case 1:
                {
                    // 선택 케릭터의 웨폰 마스터리를 그대로 따라감 , 보통 마법사 계열 , 없으면 무시
                    CSV_Skill csv_skill = csv.GetWeaponSkill();
                    if( csv_skill != null )
                    {
                        CSV_MagicPropDefine csv_magic_prop = GTF_CSV.csv_MagicPropDefinePage.FindTag( csv_skill.TAG_MAGIC_PROP );
                        charBase.magic_prop = csv_magic_prop;
                    }
                }
                break;
            case 2:
                {
                    // 아무거나 붙이기
                    CSV_MagicPropDefine csv_magic_prop = GTF_CSV.csv_MagicPropDefinePage.GetRandom( rd );
                    charBase.magic_prop = csv_magic_prop;
                }
                break;
        }

        return charBase;
    }
}
