using System.Collections.Generic;
using UnityEngine;

//
public class _BIAS_TWO_VAL
{
    public float val_1;
    public float val_2;
    public float Random( Mng_X128SS rd ){return rd.NextFloat( val_1 , val_2 );}
}

// 범용 성향
public class _BIAS_COMMON
{
    public class OBJ_PER_VAL
    {
        public object   obj;
        public float    per;
    }
    public List<OBJ_PER_VAL> objs = new List<OBJ_PER_VAL>();

    public Mng_X128SS rd_init;
    public Mng_X128SS rd_random;

    public void SetRandom_Init( Mng_X128SS _rd_make , Mng_X128SS _rd_inGame = null )
    {
        rd_init = _rd_make;
        rd_random = _rd_inGame;
        if( rd_random == null ) rd_random = _rd_make;
        OnSetRandom_Init();
    }

    public Mng_X128SS GetRandomClass()
    {
        return rd_random;
    }

    virtual public void OnSetRandom_Init(){}

    virtual public void AddObj( object obj , float min_per = 0.01f )
    {
        OBJ_PER_VAL s = new OBJ_PER_VAL();
        s.obj = obj;
        s.per = rd_init.NextFloat( min_per , 1 );
        objs.Add(s);
    }

    virtual public object Random()
    {
        rd_random.Step_Clear();
        foreach( var s in objs )
        {
            rd_random.Step_Add( s.per , s.obj );
        }
        return rd_random.Step_Random();
    }
}


//=================================================================================================
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
            CSV_Skill sk = rd_init.RandomList( cSVs_all , true );
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

//=================================================================================================
// 마법 속성들
public class _BIAS_MAGIC_DEFINE : _BIAS_COMMON
{
    public void FillInit( int init_num = 3  )
    {
        List<CSV_MagicPropDefine> cSVs = GTF_CSV.csv_MagicPropDefinePage.CopyData_INT<CSV_MagicPropDefine>();
        List<CSV_MagicPropDefine> csv_sell = new();
        for( int i = 0 ; i < init_num ; i++ )
        {
            CSV_MagicPropDefine sk = rd_init.RandomList( cSVs , true );
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


//=================================================================================================
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
            CSV_CharBaseStat sk = rd_init.RandomList( cSVs , true );
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

// //=================================================================================================
// // 임무 성향 
// // 타입별로 확률
// public class _BIAS_MISSION_TYPE : _BIAS_COMMON
// {
//     // DefeatEnemy ,
//     // DungeonConquer , 
//     // GetItem ,
//     // GetRumor ,
//     // Affection ,

//     public float per_DefeatEnemy = 1;
//     public float per_DungeonConquer = 1;
//     public float per_GetItem = 1;
//     public float per_GetRumor = 1;
//     public float per_Affection = 1;

//     override public void OnSetRandom_Init()
//     {
//         AddObj( MISSION_TYPE.DefeatEnemyNormal    , per_DefeatEnemy );
//         AddObj( MISSION_TYPE.DungeonConquer , per_DungeonConquer );
//         AddObj( MISSION_TYPE.GetItem        , per_GetItem );
//         AddObj( MISSION_TYPE.GetRumor       , per_GetRumor );
//         AddObj( MISSION_TYPE.Affection      , per_Affection );
//     }

//     public MISSION_TYPE Random_MissionType()
//     {
//         return (MISSION_TYPE)Random();
//     }
// }

//=================================================================================================
// 던전 성향
// - 종족 , 마법 성향
// - 아이템 



//=================================================================================================
// 



// public class BiasMake_Common : _BIAS_COMMON
// {

// }
