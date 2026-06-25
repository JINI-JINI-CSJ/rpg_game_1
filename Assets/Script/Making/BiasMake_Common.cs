using System.Collections.Generic;
using UnityEngine;

// 범용 성향
public class _BIAS_COMMON
{
    public class OBJ_PER_VAL
    {
        public object   obj;
        public float    per;
    }
    public List<OBJ_PER_VAL> objs = new List<OBJ_PER_VAL>();

    public Mng_X128SS rd_make;
    public Mng_X128SS rd_inGame;

    public void SetRandom_Init( Mng_X128SS _rd_make , Mng_X128SS _rd_inGame )
    {
        rd_make = _rd_make;
        rd_inGame = _rd_inGame;
        OnSetRandom_Init();
    }

    virtual public void OnSetRandom_Init(){}

    public void AddObj( object obj , float min_per = 0.01f )
    {
        OBJ_PER_VAL s = new OBJ_PER_VAL();
        s.obj = obj;
        s.per = rd_make.NextFloat( min_per , 1 );
        objs.Add(s);
    }
    public object Random()
    {
        rd_inGame.Step_Clear();
        foreach( var s in objs )
        {
            rd_inGame.Step_Add( s.per , s.obj );
        }
        return rd_inGame.Step_Random();
    }
}


// 전사 , 마법사 , 지원가

public class _BIAS_JOB
{
    public float WARRIOR;
    public float WIZARD;
    public float SUPPORTER;

    Mng_X128SS rd_make;
    Mng_X128SS rd_inGame;

    public void SetRandom_Init( Mng_X128SS _rd_make , Mng_X128SS _rd_inGame )
    {
        rd_make = _rd_make;
        rd_inGame = _rd_inGame;

        WARRIOR     = rd_make.NextFloat(0.1f , 1);
        WIZARD      = rd_make.NextFloat(0.1f , 1);
        SUPPORTER   = rd_make.NextFloat(0.1f , 1);
    }


    public JOB_BASE Random()
    {
        rd_inGame.Step_Start_Add( WARRIOR  , JOB_BASE.WARRIOR );
        rd_inGame.Step_Add( WIZARD         , JOB_BASE.WIZARD );
        rd_inGame.Step_Add( SUPPORTER      , JOB_BASE.SUPPORTER );
        return (JOB_BASE)rd_inGame.Step_Random();
    }   
}


// 직업 메이킹 스킬들
// 직업 대분류 갯수만큼
// 무기 종류 , 마법장착 종류
public class _BIAS_SKILL_MAIN_JOB : _BIAS_COMMON
{
    public void FillInit( JOB_BASE jOB_BASE , int init_num = 3 )
    {
        List<CSV_Skill> cSVs_all = GTF_CSV.GetSkill_JobMake( jOB_BASE );
        List<CSV_Skill> csv_sell = new();

        for( int i = 0 ; i < init_num ; i++ )
        {
            CSV_Skill sk = rd_make.RandomList( cSVs_all , true );
            if( sk != null )csv_sell.Add( sk );
        }
        foreach( var s in csv_sell )
        {
            AddObj( s );
        }
    }

    public CSV_Skill RandomSkill()
    {
        return Random() as CSV_Skill;
    }
}

// 마법 속성들
public class _BIAS_MAGIC_DEFINE : _BIAS_COMMON
{
    public void FillInit( int init_num = 3  )
    {
        List<CSV_MagicPropDefine> cSVs = GTF_CSV.csv_MagicPropDefinePage.CopyData_INT<CSV_MagicPropDefine>();
        List<CSV_MagicPropDefine> csv_sell = new();
        for( int i = 0 ; i < init_num ; i++ )
        {
            CSV_MagicPropDefine sk = rd_make.RandomList( cSVs , true );
            if( sk != null )csv_sell.Add( sk );
        }
        foreach( var s in csv_sell )
        {
            AddObj( s );
        }
    }

    public CSV_MagicPropDefine RandomMagicDefine()
    {
        return Random() as CSV_MagicPropDefine;
    }
}



// 캐릭터 기본 수치들
// 직업 대분류 갯수만큼
public class _BIAS_CHAR_STAT : _BIAS_COMMON
{
    public void FillInit(  JOB_BASE jOB_BASE , int init_num = 3  )
    {
        List<CSV_CharBaseStat> cSVs = GTF_CSV.GetCharStat_JobMake( jOB_BASE );
        List<CSV_CharBaseStat> csv_sell = new();
        for( int i = 0 ; i < init_num ; i++ )
        {
            CSV_CharBaseStat sk = rd_make.RandomList( cSVs , true );
            if( sk != null )csv_sell.Add( sk );
        }
        foreach( var s in csv_sell )
        {
            AddObj( s );
        }
    }

    public CSV_CharBaseStat RandomMagicDefine()
    {
        return Random() as CSV_CharBaseStat;
    }
}

public class BiasMake_Common 
{

}
