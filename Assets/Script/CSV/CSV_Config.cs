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

    // 레벨 -> 비용 비율 계산인자.
    // 2차 함수 인자
    // 레벨 ^ level_PowArgGlobal
    public float level_ResourcesArgGlobal = 1.4f;

    // 레벨 -> 강함 비율
    public float level_PowArgGlobal = 1.1f;    

    // 레벨 제한은 없고 , 대신 일정 레벨 이상이면 아주 느리게 레벨업
    // 일반 최대 레벨 
    public int level_max_normal = 100;

    public int grade_by_level_step = 10;    // 클래스 당 레벨 

    // 메인 스토리 최대 레벨
    public int level_max_story = 60;

    // 유니크 스텝 레벨 , n 레벨클래스 마다 유니크 등급
    public int levelStep_Unique = 20;

    // 기본기준 최대레벨 강함 비율
    // 1 레벨 : 100 레벨의 강함비율
    public float level_MAX_Value_per = 50; // 50 배 강함

    // 등급(그레이드 최대)
    public int max_grade = 10;

    //===========================================
    // 월드 메이킹 

    // 실 사이즈
    public int size_world = 2000;


    // 총 도시개수의 비율로만 하기
    // 정해진 단계별로 총 개수
    // 그 후 도시등급개수 및 던전은 비율값으로만

    // 월드 세팅 단계별로 총 개수 : 1 ~ 5 단계 정도
    public List<int> total_city;

    // 도시의 등급별로 비율 , 현재는 5단계
    // 수도 제외 , 큰 도시부터 작은 도시 , 클로드 월드 메이킹이랑 순서 맞추기
    public List<float> cityGrade_ratio;

    // 외부 던전 기본 비율 : 일반 , 숨김 , 인스턴스 , 무한
    public List<float> dungeon_BaseType_ratio;



    // 특수 도시 비율
    public List<float> world_spc_city_per;

    // 유니크 객체 총 비율 (도시 갯수에 비례)
    // 1 보다 클 수도 있다. 
    public List<float> uniqueObj_per;

    //===========================================
    // 스킬 메이킹
    public float makeSkill_addPow = 0.2f;           // 점수당 추가 공격력

    public float makeSkill_UltimateAddPow = 10;     // 궁극기 추가 공격력

    public float makeSkill_addMP = 0.8f;            // 점수당 제곱하는 총 mp , 예) 3 점이리면  0.8 * 0.8 * 0.8 


    //===========================================
    // 아이템 메이킹

    // 메이킹 아이템 가격 최소 기준
    public int making_unique_item_Price = 1000;

    // 유니크 아이템
    // 0 클래스 ~ 10 클래스 ( 1 레벨 ~ 100 레벨 )
    public List<int> making_unique_item_grade_num;

    // 유니크 아이템 최대 점수
    // 만들때는 수치 점수랑 , 이펙트 점수를 랜덤으로 분배
    public int making_unique_item_good_score = 4;

    // 유니크 아이템 수치 점수 퍼센트
    public float making_item_statFix = 0.15f;


    // 수집품 등급별 퍼센트 , 100 기준으로 인덱스 별로
    // 80, 15 , 4 , 1 
    public List<int> making_item_collect_per;

    
    // 레시피 아이템 등급당 개수
    // 현재 : 총 레시피 등급 ( 4 * 2 - 1 ) = 7 
    // 한 파트당 : 7 * 20 -> 140 
    // 음식 , 소모 -> 140 * 2 = 280
    public int making_recipe_grade_num = 20;

    // 레시피의 재료 최대 개수
    public int making_recipe_matter_max = 5;

    // 레시피 재료 아이템의 등급당 점수
    // making_item_collect_per 에 연동
    // 현재 : 1 , 20 , 40 , 60
    public int making_recipe_matter_grade_score = 20;

    //===========================================
    // 캐릭터 메이킹
    // 보너스 점수당 10 퍼센트
    public float makeChar_statFix = 0.1f; 

    // 스킬 메이킹
    public int makeSkill_BaseVal_FIGHTER;       // 메이킹 스킬 기본 공격력
    public int makeSkill_BaseVal_WIZARD_ATK;    // 공격 마법 기본 공격력
    public int makeSkill_BaseVal_WIZARD_HEAL;   // 회복 마법 기본 수치
    public int makeSkill_BaseVal_SUPPORTER;     // 지원

    // 유니크 캐릭터 도시 소속 : 수도 , 대도시의 클래스 등급
    public List<int>   makeChar_City_Class; // 도시 역등급 순서 -> 수도 , 대도시 ....




    // 유니크 캐릭터 재야 인재 : [ 캐릭터 클래스 등급 , 인원수 , 소문 개수]
    // 재야 인사
    public class Chr_UniqueOutSide
    {
        public int class_chr;
        public int chr_num;
        public int rumor_num;

        // 배열 값을 pop 하면서 읽어본다. 
        // 모자라면 에러
        public void ReadArray( List<int> data )
        {
            if( PopData(data , ref class_chr ) == false ) return;
            if( PopData(data , ref chr_num ) == false ) return;
            if( PopData(data , ref rumor_num ) == false ) return;
        }

        bool PopData( List<int> data , ref int arg )
        {
            if( data.Count < 1 )
            {
                Debug.LogError( "Chr_UniqueOutSide : 데이터 부족 " );
                return false;
            }
             return true;
        }
    }

    public List<Chr_UniqueOutSide> chr_UniqueOutSides = new();


    // 캐릭터 추카 스탯 보너스 레벨별  , 인덱스 첫칸 1 부터  , 각 칸은 해당 스탯 보너스의 최대 레벨 
    // 초과시 마지막 인덱스값 + 1
    // 예) ~20,~40,~60,~80,~100,~~ 

    // 캐릭터 추가 스킬
    // 예) ~20 , ~40 , ~60 ,


    //===========================================
    // 던전 적군 메이킹

    // 희소 등급별 장비 품질 보너스 
    // 정예 : 0~2
    // 희소 : 0~3
    // 보스 일반 장비 : 0~4
    // 보스 보너스 장비 : 0~5

    // 인덱스 0 : 정예 , 1 : 희소 , 2 : 보스 일반 , 3 : 보스 보너스
    public List<int> enemyRarityGrade_EquipBonusMax;


    //===========================================
    // 행사 모임 관련
    
    // 행사 개최 주기 
    public int gatherings_waitTurn_Min = 20;
    public int gatherings_waitTurn_Max = 100;

    // 행사 개최 기간
    public int gatherings_keepUpTurn_Min = 3;
    public int gatherings_keepUpTurn_Max = 5;

    // 공통 비정규 행사 확률
    public float gatherings_NotRegular_per = 0.1f;


    // 경매장 , 암시장

    // 경매장 암시장 비율
    public float gatherings_AuctionHouse_BlackMarket_per = 0.8f;// 경매장 확률 

    // 경매장일경우 좋은 효과 점수 , 고정
    public int gatherings_AuctionHouse_GoodScore = 2;

    // 암시장 일경우 좋은 효과 점수 최대 랜덤 , 0 부터 시작
    public int gatherings_BlackMarket_GoodScore = 4;
    // 나쁜 점수
    public int gatherings_BlackMarket_BadScore = 4;

    // 경매장 가격 배수 최대 , 최소는 1
    public float gatherings_AuctionHouse_PriceRatio_Max = 2;

    // 암시장 가격 배수 최대
    public float gatherings_BlackMarket_PriceRatio_Max = 4;

    // 암시장일경우 소문 매물로 나올 확률
    public float gatherings_BlackMarket_Rumor_Sell_Per = 0.2f;

    // 위의 소문이 진실일 확률
    public float gatherings_BlackMarket_Rumor_True_Per = 0.3f;

    public override void Read()
    {
        
    }

    // public int Random_WorldNation( Mng_X128SS rd )      {return rd.NextInt( world_nation[0] , world_nation[1] );}
    // public int Random_WorldCityMajor( Mng_X128SS rd )   {return rd.NextInt( world_city_Major[0] , world_city_Major[1] );}
    // public int Random_WorldCityMinor( Mng_X128SS rd )   {return rd.NextInt( world_city_Minor[0] , world_city_Minor[1] );}
    // public int Random_WorldCityVillage( Mng_X128SS rd ) {return rd.NextInt( world_city_Village[0] , world_city_Village[1] );}
    // public int Random_WorldDungeon( Mng_X128SS rd )     {return rd.NextInt( world_dungeon[0] , world_dungeon[1] );}


    public int TotalCity( int idx ){return SJ_CSharpUtil.GetList_IndexSafe( total_city , idx );}



    public float Random_WorldCitySpcPer( Mng_X128SS rd ){return rd.NextFloat( world_spc_city_per[0] , world_spc_city_per[1] );}
    public float Random_UniqueObjPer( Mng_X128SS rd )   {return rd.NextFloat( uniqueObj_per[0] , uniqueObj_per[1] );}


    // 레벨 스텝 : 총 기본 레벨 / 스텝 레벨 + 1
    // + 1 은 0 부터 시작하기 위해
    // 0,20,40,60,80,100
    //..


    public int LevelStepUnique_Total(){return level_max_normal / levelStep_Unique;}
    public int LevelStepUniqueCur( int i ){return i * levelStep_Unique;}


    public int GetEnemyRarityGrade_EquipBonusMax( int rarityGradeIdx )
    {
        return SJ_CSharpUtil.GetList_IndexSafe( enemyRarityGrade_EquipBonusMax , rarityGradeIdx );
    }

    // 등급은 1부터 시작
    // 여기는 인덱스로 체크
    public int GetMaking_unique_item_grade_num( int idx )
    {
        return SJ_CSharpUtil.GetList_IndexSafe( making_unique_item_grade_num , idx );
    }

    // 현재 10 단위
    // 1~10 단위로 등급하자.
    // 0은 민간인이라는 설정
    public int GradeToLevel( int grade )
    {
        int level = grade * grade_by_level_step;
        if( level < 1 ) level = 1;
        return level;
    }

    public int MaxRecipeMatterScore()
    {
        // 총등급 * 등급당 점수 * 재료 최대 개수 * 재료 최대 종류
        // 4 * 20 * 5 * 2
        return making_item_collect_per.Count * making_recipe_matter_grade_score * making_recipe_matter_max * 2;
    }

}
