using UnityEngine;

// 1안
// 퀘스트 깊이 , 넓이 비율
// 일단 메인 임무 (마지막 임무)는 1개 , 이 밑으로 어떻게 깊이 및 넓이가 될지 결정.
// 0 : 완전 수평 , 모든 퀘스트 종속 관계 없음
// 1 : 완전 수직 , 동시 퀘스트 없음 , 무조건 선후 관계 , 종속관계
// 0.5 : 정 삼각형에 가까운 종속 배치
// ShapeRatio

// 2안
// 단순하게 하위임무가 몇개가 될지 확률 및 깊이
// 깊이 0 , 하위 확률 갯수 10 : 동시 임무 가능 퀘스트가 1~10 개이고 종속 임무 없음
// 깊이 n , 하위 확률 갯수 m : n 차 연퀘, 각 퀘마다 종속 갯수 1~m
// 전역 설정 : 최대 깊이 , 최대 종속 임무 갯수 확률

// 레벨 설정 : 메인 임무 , 최하단 임무 에서 비례 , 래밸 변동 폭 인자 추가

// 스토리 
// 임무
// (보류)수행 가능 조건 : 레벨  , 우호도 , 단서 등등
// 특정 도시 국가 임무들은 하위 임무를 할당하면 된다.
// 제목 및 개요 : 미션 템플릿 참조
// 보상 : 자동 생성에서 할당
// 

public class _BIAS_MISSION : _BIAS_COMMON
{
    CSV_ConteStory csv;

    public override void OnSetRandom_Init()
    {
        if( csv != null )
        {
            AddObj( MISSION_TYPE.DefeatEnemyNormal , csv.per_DefeatEnemyNormal );
            AddObj( MISSION_TYPE.DefeatEnemyBoss , csv.per_DefeatEnemyBoss );
            AddObj( MISSION_TYPE.GetItem , csv.per_GetItem );
            AddObj( MISSION_TYPE.Delivery , csv.per_Delivery );
            AddObj( MISSION_TYPE.Escort , csv.per_Escort );
            AddObj( MISSION_TYPE.DungeonObjCheckUp , csv.per_DungeonObjCheckUp );
            AddObj( MISSION_TYPE.DungeonConquer , csv.per_DungeonConquer );
            AddObj( MISSION_TYPE.GetRumor , csv.per_GetRumor );
            AddObj( MISSION_TYPE.Affection , csv.per_Affection );
            AddObj( MISSION_TYPE.ManyBattle , csv.per_ManyBattle );
        }
        else
        {
            AddObj( MISSION_TYPE.DefeatEnemyNormal );
            AddObj( MISSION_TYPE.DefeatEnemyBoss );
            AddObj( MISSION_TYPE.GetItem  );
            AddObj( MISSION_TYPE.Delivery  );
            AddObj( MISSION_TYPE.Escort  );
            AddObj( MISSION_TYPE.DungeonObjCheckUp );
            AddObj( MISSION_TYPE.DungeonConquer );
            AddObj( MISSION_TYPE.GetRumor  );
            AddObj( MISSION_TYPE.Affection  );
            AddObj( MISSION_TYPE.ManyBattle );
        }
    }

    public MISSION_TYPE RandomMission()
    {
        return (MISSION_TYPE)Random();
    }
}


public class Making_ConteStroy
{
    public _BIAS_MISSION    bias_mission;

    static public MissionBase MakeConteStroy( SJ_ID_INT_Mng id_make , Mng_X128SS _rd ,  int depth_max , int child_num , _BIAS_MISSION _bias_mission , int start_level , int max_level , 
                                                int diff , MISSION_TYPE root_mission_type = MISSION_TYPE.DefeatEnemyBoss )
    {
        MissionBase mission_root = MissionBase.InstMission( root_mission_type );
        mission_root.CreateMission( id_make , _rd , child_num , _bias_mission , start_level , max_level , diff , depth_max , 0 , null , null , null );
        return mission_root;
    }
}
