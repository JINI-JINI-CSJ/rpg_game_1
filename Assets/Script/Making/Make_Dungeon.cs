using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WorldForge;

// 던전 설정들..

// 일반던전 : 도시 소속
// 특수던전 : 지도상 독립 위치


// 던전 성향

// [적군 설정]
// 월드 메이킹 시점에서 확정
// 던전 적군 파티 조우 확률 및 개체수
// 1안 : 고정값 , 일반:80 , 정예: 15 , 희소 : 5 (확률 csv)
// 희소등급별 적군 개체수 확률
// 일반 : 2~6 개체의 단순 확률 , 정예 : 1~2 개체 , 희소 : 1~2개체 
// 대열 : 일반 등급만 해당. 무작위로 전후방 개체 나누기 , 전열이 없으면 최소 전열 1체 이상

// 적군의 종류 갯수
// 현재는 던전의 규모(0~n) 에 따라 적군 종류가 달라지도록 한다.( 등급 , 난이도하고는 별개 , 순수 사이즈만 관련 )
// 전역 csv 에서 참조

// [아이템 설정]
// 보이는 보물상자 , 숨겨진 아이템 
// 월드 메이킹 시점에서 태그 또는 확률만 정의? , 던전 월드 업데이트시에 정하기
// 획득 지점은 던전의 방에서만 가능 , 통로에는 배치 안함
// 일정 확률로 방마다 보물 상자를 배치 (각 방마다 독립확률이니 전체 방에 다 있을수도 , 한개도 없을수도 있다.) 
// 보물 박스 아이템 비율 : 소비 70 , 돈 20 , 장비 10 (확률 csv)
// 숨김 아이템 : 일단 무조건 수집 , 그 외 퀘스트에서 유니크 아이템?

// [던전 형태 , 규모]
// 일반 , 특수 구분
// 일단 도시 던전은 모두 일반으로 분류
// 특수 던전은 독립 좌표로 지정된 던전 , 현재는 클로드 지도 메이킹에서 만든 던전은 모두 해당

// 
// 일반
// 일반 던전 계층 개수는 도시 규모에 비례 , 심연은 대도시 이상만?

// 특수 , 잠정 보류 , 도시 던전 행사에만 하는거로
// 다양한 형식이 있다
// -일반 + 입장 조건 : 직업 , 인원수 등등 제한
// -일반 + 심연      : 심연 존재
// -무작위           : 입장 할때 마다 새로 생성
// -무한 계층        : 도전용 
// 일단 나열된 형식들은 확률로 생성 , 또는 무한 계층은 소수이거나 후반에만...?

// 타입
// -일반 도시
// -필드 일반 던전 : 처음부터 보인다. 
// -필드 숨김 던전 : 소문이나 임무 보상으로 알려짐
// -인스턴스 던전 : 특정 기간등에만 보임 , 비정규로 보이기
// -무한던전 : 메인 시나리오 클리어 후 등장 , 층수가 무한하다.

// 일단 심연은 도시 던전에는 없는걸로 
// 필드 던전에서만 있게 하자.

public enum DUNGEON_TYPE
{
    // 외부던전 인덱스를 위해서 "필드 보임" 던전부터 시작
    None = -1 ,
    Field_Show = 0 ,    
    Field_Hide = 1,
    Instance = 2, 
    Infinity = 3, 
    City ,     
}

public class Make_Dungeon : MakeBase
{

    public override void OnMake()
    {
        WorldForgeManager worldForge = Make_WorldMap.G.worldForge;

        // 던전 태그에 따라서 객체 만들기

        // 도시 던전 , 던전 객체 만들어서 도시 객체에 넣는다.
        foreach( var s in Make_WorldMap.G.dic_world_idx_city.Values )
        {
            DungeonInfo dungeonInfo = new DungeonInfo();

            // 도시 던전이면 스폿데이터는 없다.
            dungeonInfo.ID = MakingMain.Make_UID( typeof(DungeonInfo) );
            // 시드는 메이킹 시드
            dungeonInfo.randomSeed = GTF_Random.rd_make_common.NextUInt();
            // 도시 던전 레벨은 티어 랜덤 레벨
            dungeonInfo.LEVEL = GTF_CSV.RandomLevel_CityTire( GTF_Random.rd_make_common , s.cityData.Tier );

            s.dungeonInfo = dungeonInfo;
        }
        


        // 추가 층수 레이어

        // 3안
        // 레이어 리소스 형태( 잠정 ) : 현시대 일반 , 현시대 심연 , 백룸 또는 혼돈 세계 , 현 지구 디스토피아
        // 각 해당되는 태그 리소스중에 랜덤
        // 던전 맵 리소스 , 적군 리소스  csv 에서 태그별로 받아오기

        // 메이킹함수는 DungeonInfo 에서 직접하기
        // 던전 리소스는 태그 정의csv 에서 가져오자.
        // 일단 태그 1개만 가져와서  던전 , 적군 다하자.


        // == 도시 던전 == 
        // 시작 도시 레벨은 1 고정
        // 1~9 레벨 (1등급) 던전은 보스가 없다. 1층만 있다.
        // 2등급부터는 보스가 있다.
        // 던전 레이어 그룹은 무조건 1개 , 심연 같은거 없다.
        // 적군태그 : 일반 랜덤
        // 아이템 태그 : 일반 랜덤

        // 시작도시만 레벨 다시
        City city_Start = Make_City.G.city_Start;
        city_Start.dungeonInfo.LEVEL = 1;   

        foreach( var s in Make_WorldMap.G.dic_world_idx_city.Values )
        {
            DungeonInfo dungeonInfo = s.dungeonInfo;

            string res_dungeon = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_RES_NORMAL" , GTF_Random.rd_make_common );
            string tagEnemy = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_ENEMY_NORMAL" , GTF_Random.rd_make_common );
            string tagItem = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_DROP_PER_NORMAL" , GTF_Random.rd_make_common );

            bool makeBoss = true;
            if( dungeonInfo.LEVEL < 10 ) makeBoss = false;

            int grade = GTF_CSV.LevelToGrade( dungeonInfo.LEVEL );

            // 층수 , 일단 등급 / 2 정도 하자.
            int total_layer = grade / 2 + 1;

            dungeonInfo.AddMakeLayerGroup( total_layer , res_dungeon , dungeonInfo.LEVEL , dungeonInfo.LEVEL + 5 , GTF_Random.rd_make_common , 
            tagEnemy , makeBoss , "DUNGEON_NORMAL_" , tagItem );
        }

     

        // == 외부 던전 ==

        // -외부 던전 타입별로 레벨링
        //  Field_Show : 20 ~ 60
        //  Field_Hide : 30 ~ 70
        //  Instance : 40 ~ 80
        //  Infinity : 60 ~ 무한층
        //  무한을 제외한 외부 던전의 레벨은 
        //  인덱스로 계산하여 증가하게 하면 된다.      

        // -레이어 그룹 및 층수
        //  Field_Show : 등급 / 2 , 일반 -> 심연
        //  Field_Hide : 등급 , 일반 -> 심 -> 혼
        //  Instance : 등급 , 매 입장마다 랜덤 , 실시간 랜덤시드로 입장마다 Make 한다.
        //  Infinity : 무한 , 현재 디스 또는 올 랜덤 , 10 층마다 보스 같은 컨텐츠 추가하자.

        // 타입별로 정리
        foreach( var s in Make_WorldMap.G.dic_type_dungeon )
        {
            DUNGEON_TYPE dUNGEON_TYPE = s.Key;
            List<DungeonInfo> dungeonInfos = s.Value;

            int level_start = 0;
            int level_end = 0;       
                      

            switch( dUNGEON_TYPE )
            {
                case DUNGEON_TYPE.Field_Show:
                    {
                        level_start = GTF_CSV.csv_Config.makeDungeon_Field_Show_LevelStart;
                        level_end   = GTF_CSV.csv_Config.makeDungeon_Field_Show_LevelEnd;

                        for( int i = 0 ; i < dungeonInfos.Count ; i++ )
                        {
                            DungeonInfo dungeon = dungeonInfos[i];
                            dungeon.LEVEL = (int)(Mathf.Lerp( level_start , level_end , (float)i / (float)dungeonInfos.Count ));

                            int grade =  GTF_CSV.LevelToGrade( dungeon.LEVEL );
                            int total_layer =  grade / 2 + 1;                               


                            // 일반 
                            string res_dungeon_normal = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_RES_NORMAL" , GTF_Random.rd_make_common );
                            string tagEnemy_normal = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_ENEMY_NORMAL" , GTF_Random.rd_make_common );
                            string tagItem_normal = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_DROP_PER_NORMAL" , GTF_Random.rd_make_common );      

                            dungeon.AddMakeLayerGroup( total_layer , res_dungeon_normal , dungeon.LEVEL , dungeon.LEVEL + 5 , GTF_Random.rd_make_common , 
                            tagEnemy_normal , true , "DUNGEON_NORMAL_" , tagItem_normal );

                            // 심연
                            string res_dungeon_deep = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_RES_DEEP" , GTF_Random.rd_make_common );
                            string tagEnemy_deep = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_ENEMY_DEEP" , GTF_Random.rd_make_common );
                            string tagItem_deep = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_DROP_PER_DEEP" , GTF_Random.rd_make_common );  

                            dungeon.AddMakeLayerGroup( total_layer , res_dungeon_deep , dungeon.LEVEL , dungeon.LEVEL + 5 , GTF_Random.rd_make_common , 
                            tagEnemy_deep , true , "DUNGEON_DEEP_" , tagItem_deep );
                        }
                    }
                    break;
                case DUNGEON_TYPE.Field_Hide:
                    {
                        level_start = GTF_CSV.csv_Config.makeDungeon_Field_Hide_LevelStart;
                        level_end   = GTF_CSV.csv_Config.makeDungeon_Field_Hide_LevelEnd;

                        for( int i = 0 ; i < dungeonInfos.Count ; i++ )
                        {
                            DungeonInfo dungeon = dungeonInfos[i];
                            dungeon.LEVEL = (int)(Mathf.Lerp( level_start , level_end , (float)i / (float)dungeonInfos.Count ));

                            int grade =  GTF_CSV.LevelToGrade( dungeon.LEVEL );
                            int total_layer =  grade / 2 + 1;                               


                            // 일반 
                            string res_dungeon_normal = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_RES_NORMAL" , GTF_Random.rd_make_common );
                            string tagEnemy_normal = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_ENEMY_NORMAL" , GTF_Random.rd_make_common );
                            string tagItem_normal = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_DROP_PER_NORMAL" , GTF_Random.rd_make_common );      

                            dungeon.AddMakeLayerGroup( total_layer , res_dungeon_normal , dungeon.LEVEL , dungeon.LEVEL + 5 , GTF_Random.rd_make_common , 
                            tagEnemy_normal , true , "DUNGEON_NORMAL_" , tagItem_normal );

                            // 심연
                            string res_dungeon_deep = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_RES_DEEP" , GTF_Random.rd_make_common );
                            string tagEnemy_deep = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_ENEMY_DEEP" , GTF_Random.rd_make_common );
                            string tagItem_deep = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_DROP_PER_DEEP" , GTF_Random.rd_make_common );  

                            dungeon.AddMakeLayerGroup( total_layer , res_dungeon_deep , dungeon.LEVEL , dungeon.LEVEL + 5 , GTF_Random.rd_make_common , 
                            tagEnemy_deep , true , "DUNGEON_DEEP_" , tagItem_deep );

                            // 혼돈
                            string res_dungeon_chaos = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_RES_CHAOS" , GTF_Random.rd_make_common );
                            string tagEnemy_chaos = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_ENEMY_CHAOS" , GTF_Random.rd_make_common );
                            string tagItem_chaos = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "DUNGEON_DROP_PER_CHAOS" , GTF_Random.rd_make_common );  

                            dungeon.AddMakeLayerGroup( total_layer , res_dungeon_chaos , dungeon.LEVEL , dungeon.LEVEL + 5 , GTF_Random.rd_make_common , 
                            tagEnemy_chaos , true , "DUNGEON_CHAOS_" , tagItem_chaos );
                        }

                    }
                    break;
                case DUNGEON_TYPE.Instance:
                    {
                        level_start = GTF_CSV.csv_Config.makeDungeon_Instance_LevelStart;
                        level_end   = GTF_CSV.csv_Config.makeDungeon_Instance_LevelEnd;

                        
                    }
                    break;
                case DUNGEON_TYPE.Infinity:
                    {
                        // 무한은 똑같음
                        level_start = GTF_CSV.csv_Config.makeDungeon_Infinity_LevelStart;
                        level_end   = GTF_CSV.csv_Config.makeDungeon_Infinity_LevelStart;
                    }
                    break;
            }




        }        
    }
}
