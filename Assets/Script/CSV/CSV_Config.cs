using System.Collections.Generic;
using UnityEngine;

// 

// 전체 레벨값 디자인
// 1~100 일반 최대 레벨 , 10 단위로 클래스 기준 , 클래스 강화 재료가 있어야 다음 레벨 클래스
// 60정도 까지가 일반 장편 스토리 최대 
// 100 정도 까지가 인간 초월 느낌
// 그 이상은 신적인 느낌 ( 여기는 플레이어가 정말 끝까지 하겠다는 의지로.. )
// 유니크 객체는 100 정도까지 하자. 
// 신급은 자동 생성

public class CSV_Config : SJ_CSV_BasePage
{
    //===========================================
    // 일반

    // 레벨 제한은 없고 , 대신 일정 레벨 이상이면 아주 느리게 레벨업
    // 일반 최대 레벨 
    public int level_max_normal = 100;

    // 메인 스토리 최대 레벨
    public int level_max_story = 60;

    // 유니크 스텝 레벨 , n 레벨클래스 마다 유니크 등급
    public int levelStep_Unique = 20;


    //===========================================
    // 월드 메이킹 

    // 실 사이즈
    public int size_world = 2000;

    // 총 국가 최소대
    public List<int> world_nation;
    // 총 도시 규모별 최소대
    public List<int> world_city_Major;
    public List<int> world_city_Minor;
    public List<int> world_city_Village;

    // 총 던전
    public List<int> world_dungeon;

    // 특수 도시 비율
    public List<float> world_spc_city_per;

    // 유니크 객체 총 비율 (도시 갯수에 비례)
    // 1 보다 클 수도 있다. 
    public List<float> uniqueObj_per;

    // 

    //===========================================

    // 캐릭터 메이킹
    // 보너스 점수당 10 퍼센트
    public float makeChar_statAddFix = 0.1f; 

    // 스킬 메이킹
    public int makeSkill_BaseVal_FIGHTER;       // 메이킹 스킬 기본 공격력
    public int makeSkill_BaseVal_WIZARD_ATK;    // 공격 마법 기본 공격력
    public int makeSkill_BaseVal_WIZARD_HEAL;   // 회복 마법 기본 수치
    public int makeSkill_BaseVal_SUPPORTER;     // 지원

    // 위력 강도 목록
    // 일단 다 같게..
    public List<float> makeSkill_addPow;
    public List<int>   makeSkill_mp;

    // 캐릭터 추카 스탯 보너스 레벨별  , 인덱스 첫칸 1 부터  , 각 칸은 해당 스탯 보너스의 최대 레벨 
    // 초과시 마지막 인덱스값 + 1
    // 예) ~20,~40,~60,~80,~100,~~ 

    // 캐릭터 추가 스킬
    // 예) ~20 , ~60 , ~100 ,~~ 

    public override void Read()
    {
        
    }

    public float GetMakeSkill_addPow( int grade ){return SJ_CSharpUtil.GetList_IndexSafe( makeSkill_addPow , grade );}
    public int GetMakeSkill_MP( int grade ){return SJ_CSharpUtil.GetList_IndexSafe( makeSkill_mp , grade );}

    public int Random_WorldNation( Mng_X128SS rd )      {return rd.NextInt( world_nation[0] , world_nation[1] );}
    public int Random_WorldCityMajor( Mng_X128SS rd )   {return rd.NextInt( world_city_Major[0] , world_city_Major[1] );}
    public int Random_WorldCityMinor( Mng_X128SS rd )   {return rd.NextInt( world_city_Minor[0] , world_city_Minor[1] );}
    public int Random_WorldCityVillage( Mng_X128SS rd ) {return rd.NextInt( world_city_Village[0] , world_city_Village[1] );}
    public int Random_WorldDungeon( Mng_X128SS rd )     {return rd.NextInt( world_dungeon[0] , world_dungeon[1] );}
    public float Random_WorldCitySpcPer( Mng_X128SS rd ){return rd.NextFloat( world_spc_city_per[0] , world_spc_city_per[1] );}
    public float Random_UniqueObjPer( Mng_X128SS rd )   {return rd.NextFloat( uniqueObj_per[0] , uniqueObj_per[1] );}


    // 레벨 스텝 : 총 기본 레벨 / 스텝 레벨 + 1
    // + 1 은 0 부터 시작하기 위해
    // 0,20,40,60,80,100
    //..


    public int LevelStepUnique_Total(){return level_max_normal / levelStep_Unique;}
    public int LevelStepUniqueCur( int i ){return i * levelStep_Unique;}
}
