using System.Collections.Generic;
using UnityEngine;



// 던전에서 생성되는 적군
// EnemyDungeonInfo 만든다.

public class Making_Enemy 
{
    // 아이템 드랍 정보
    // 일단 csv 에 정의된 아이템만 한다.
    // 메이킹 아이템은 특정 상황에서만 한다.(보스 격파 , 퀘스트 완료 등등)

    // 1안
    // 고정 값으로 랜덤
    // 적군 희소등급별 드랍 종류 -> 일반 : 1~2 , 강함 1~3 , 희소 1~4 , 보스 1~5 + 보너스 스탯

    // 2안
    // 희소 등급에 상관없이 n 개 할당 , 대신 아이템 등급과 확률로 배분
    // 이렇게 하면 일반 적군도 아주 낮은 확률로 좋은 아이템이 나올수 있다.
    // 적군 희소등급별로 확률표 , 최소 최대 수치에서 랜덤
    // 아니면 그래도 희소 등급에 따라서 종류 및 아이템 등급은 나누기

    // 최대 갯수 별로 추가 점수 아이템
    // 아이템은 같은데 강화 등급만 다르게?
    // 양손검 + 0 ~ +5
    // 전역으로 풀 강화 (+5) 기준으로 상승률 (예:30%)
    // 희소 일반 : +0 , +1
    // 강함 : +0 , +1 , +2
    // ....

    // 대분류 아이템 종류 비율 
    // 일반 , 수집 , 장비의 비율 및 고정 장비 갯수
    // 각각 희소 등급별로 확률표로 계산 , 장비 갯수 
    // 확률도 일단 고정 , 확률표 csv 에서 읽어오기
    // 확률은 그냥 소비품만 고정?
    // 일반 : 소비품 2
    // 정예 : 소비품 2 + 장비 1
    // 희소 : 소비품 2 + 수집 1 + 장비 1 
    // 보스 : 소비품 2 + 수집 1 + 장비 1 , 보너스(보너스 스탯 + 이팩트 스탯) 장비 1
    
    // 장비별 품질 보너스 
    // 정예 : 0~2
    // 희소 : 0~3
    // 보스 일반 장비 : 0~4
    // 보스 보너스 장비 : 0~5


    /// <summary>
    /// 던전에서 생성될 적군 정보 만들기
    /// - 적군 csv 정하기
    /// - 적군 속성 정하기
    /// - 적군 드랍 아이템 정하기
    /// - (잠정) 적군 패턴
    /// - (잠정) 보스라면 메이킹 스킬 장착
    /// </summary>
    /// <param name="rd"></param>
    /// <param name="enemyRarityGrade"></param>
    /// <param name="tag"></param>
    /// <param name="grade"></param>    
    /// <returns></returns>
    static public EnemyDungeonInfo Make_EnemyDungeonInfo( Mng_X128SS rd , DungeonInfo dungeonInfo , string tag = "" , int grade = -1 , EnemyRarityGrade enemyRarityGrade = EnemyRarityGrade.None )
    {
        EnemyDungeonInfo enemy_inf = new();

        // 적군 csv 에서 1개 고르기
        CSV_CharBaseStat csv_enemy = GTF_CSV.csv_CharEnemy.GetTag_Contain_Random( rd , tag , grade , enemyRarityGrade );

        if( csv_enemy == null )
        {
            Debug.LogError( "적군 csv 가 없습니다. tag : " + tag + " , grade : " + grade + " , enemyRarityGrade : " + enemyRarityGrade );
            return null;
        }

        enemyRarityGrade = enemy_inf.rarityGrade;

        enemy_inf.csv_id = csv_enemy.ID_int;

        // 강약 속성 정보
        // 태그정의 csv 에서 무기 마법 속성들 가져와서 0~3 개 정도 랜덤으로 가져오기
        // 강약 속성이 각각 1쌍씩 가져오기
        // 최대 3가지 정도
        // 0~3 은 확률csv 참조

        int attr_count = GTF_CSV.GetPerIdx( rd , "CHR_DEF_ATTR_COUNT" );
        List<string> attr_tags = GTF_CSV.csv_TagDefinePage.GetTagPart_Str( "CHR_DEF_ATTR" );
        for( int i = 0 ; i < attr_count ; i++ )
        {
            // 1쌍 2개씩 가져오기 , 강약
            string tag_attr_strong = rd.RandomList( attr_tags , true );
            string tag_attr_weak = rd.RandomList( attr_tags , true );
            enemy_inf.tagChrAttrStrong.Add( tag_attr_strong );
            enemy_inf.tagChrAttrWeak.Add( tag_attr_weak );
        }

        // 드랍 아이템 정보 만들기
        // 일반 : 소비품 2
        // 정예 : 소비품 2 + 장비 1
        // 희소 : 소비품 2 + 수집 1 + 장비 1 
        // 보스 : 소비품 2 + 수집 1 + 장비 1 , 보너스(보너스 스탯 + 이팩트 스탯) 장비 1

        // 희소 등급별 장비 품질 보너스 
        // 정예 : 0~2
        // 희소 : 0~3
        // 보스 일반 장비 : 0~4
        // 보스 보너스 장비 : 0~5
        // 인덱스 0 : 정예 , 1 : 희소 , 2 : 보스 일반 , 3 : 보스 보너스
        // 전역 csv 에서 참조

        int bonus_eq_grade_normal = 0;
        int bonus_eq_grade_boss_bonus = GTF_CSV.csv_Config.GetEnemyRarityGrade_EquipBonusMax(3);
        switch( enemyRarityGrade )
        {
            case EnemyRarityGrade.Normal:
                bonus_eq_grade_normal = 0;
                break;
            case EnemyRarityGrade.Elite:
                bonus_eq_grade_normal = GTF_CSV.csv_Config.GetEnemyRarityGrade_EquipBonusMax(0);
                break;
            case EnemyRarityGrade.Rare:
                bonus_eq_grade_normal = GTF_CSV.csv_Config.GetEnemyRarityGrade_EquipBonusMax(1);
                break;
            case EnemyRarityGrade.Boss:
                bonus_eq_grade_normal = GTF_CSV.csv_Config.GetEnemyRarityGrade_EquipBonusMax(2);
                break;
        }

        // 아이템 종류별 확률
        // 확률표 csv 에서 가져오기
        // 적군 희소 등급별로 확률표 , 희소 등급별로 값이 다르다. 다음 수치는 예시
        // 0 : 소비품1(20%) , 1 : 소비품2(10%) , 2 : 장비(3%) , 3 : 수집품(1%) , 4 : 보너스 장비 (100%)

        string tag_enemyRarityGrade = "DROP_ITEM_PER_ENEMY_RARITY_GRADE_" + enemyRarityGrade.ToString().ToUpper();

        float MIN_BASE_PER = 0.00001f;
        float per_item_drop = 0;
        // 소비품1 확률 
        // 소비품 태그중에 무작위 1개  
        per_item_drop = GTF_CSV.csv_PercentInfPage.GetCSV( tag_enemyRarityGrade ).GetPer( 0 );
        if( per_item_drop >= MIN_BASE_PER )
        {
            string tagItem = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "ITEM_CONSUME" , rd );
            enemy_inf.dropItemInfo.AddDropPer( -1 , 0 , per_item_drop , tagItem , 0 );
        }

        // 소비품2 확률
        per_item_drop = GTF_CSV.csv_PercentInfPage.GetCSV( tag_enemyRarityGrade ).GetPer( 1 );
        if( per_item_drop >= MIN_BASE_PER )
        {
            string tagItem = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "ITEM_CONSUME" , rd );
            enemy_inf.dropItemInfo.AddDropPer( -1 , 0 , per_item_drop , tagItem , 0 );
        }

        // 장비 확률
        per_item_drop = GTF_CSV.csv_PercentInfPage.GetCSV( tag_enemyRarityGrade ).GetPer( 2 );
        if( per_item_drop >= MIN_BASE_PER )
        {
            string tagItem = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "ITEM_EQUIP" , rd );
            enemy_inf.dropItemInfo.AddDropPer( -1 , bonus_eq_grade_normal , per_item_drop , tagItem , 0 );
        }
        
        // 수집품 확률
        // 직접 csv_id 로 가져오기
        per_item_drop = GTF_CSV.csv_PercentInfPage.GetCSV( tag_enemyRarityGrade ).GetPer( 3 );
        if( per_item_drop >= MIN_BASE_PER )
        {
            CSV_Item csv_item = Make_ItemUnique.Get_CollectItem( rd );
            if( csv_item == null )
            {
                // 더이상 없다. 수집품 csv 를 전부 소진했다. 게임 컨텐츠 에러
                // csv 에 더 채워 넣자.
                Debug.LogError( "수집품 아이템 csv 가 없습니다." );
                return null;
            }
            enemy_inf.dropItemInfo.AddDropPer( csv_item.ID_int , 0 , per_item_drop , "" , 0 );
        }

        // 보너스 장비 확률
        per_item_drop = GTF_CSV.csv_PercentInfPage.GetCSV( tag_enemyRarityGrade ).GetPer( 4 );
        if( per_item_drop >= MIN_BASE_PER )
        {
            string tagItem = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "ITEM_EQUIP" , rd );

            // 보너스 수치는 던전 등급에서 가져오기
            enemy_inf.dropItemInfo.AddDropPer( -1 , bonus_eq_grade_boss_bonus , per_item_drop , tagItem , dungeonInfo.grade ); 
        }

        // (잠정) 적군 패턴

        // (잠정) 보스라면 메이킹 스킬 장착
        if( enemyRarityGrade == EnemyRarityGrade.Boss )
        {
            
        }

        return enemy_inf;
    }
}
