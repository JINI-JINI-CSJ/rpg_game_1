using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 장편 스토리 타입
// - 일반
// - 메인 스토리
public enum CONTE_STROY_TYPE
{
    None = - 1,
    Normal , 
    City ,      // 도시 퀘스트?
    MainStroy , // 메인 스토리

}

/// == 미션 형태 ==
/// - 적군 토벌
/// - 던전 답사
/// - 아이템 입수 
/// - 정보 입수 
/// - 호감도 달성
/// 


public enum MISSION_TYPE
{
    None = -1 ,
    DefeatEnemyNormal ,
    DefeatEnemyBoss ,
    GetItem ,    
    Delivery , 
    Escort ,
    DungeonObjCheckUp ,
    DungeonConquer , 
    GetRumor ,
    Affection ,
    ManyBattle ,
    MAX , 
}


// 임무 기본 클래스
// 단서완료도 포함
public class MissionBase 
{
    public uint ID;

    // 연퀘일 경우 가장 처음 임무
    public MissionBase first_mission;

    // 루트에는 거의 무조건 존재하게 하자.
    // 연퀘나 다중퀘에는 루트에 한개가 기준이지만 특별한 경우 임무 객체마다 있을수 있다.
    public CSV_StoryTemplate csv_StoryTemplate;

    // 연퀘가 있을 경우 다음 퀘스트
    // 다음 항목 하위 항목과 별개이다.
    public MissionBase next_mission;


    // 하위 임무가 있을때만.    
    public int depth;
    // 다중퀘 일경우 루트 노드
    public MissionBase root_mission;    
    public MissionBase par_mission;
    // 충족해야할 미션 완료들 , 하위 미션들 , 단일 미션이면 없다.
    public List<MissionBase> child_missions = new();
    // 보상
    public RewardInf rewardInf = new();
    public int LEVEL;


    // 대표 단어들 
    // 고정 인덱스 
    // 0 : 가장 대표 단어 ( 듀랜달 )
    // 1 : 구체적 종류 ( 검 , 창 )
    // 2 : 대략 좁은 범위 ( 유물 , 소비아이템  )
    // 3 : 대략 넓은 범위
    // 4 : 늦은
    // 5 ~ 10 : 잠정 예약 
    // 11 ~ 최대 : 각 미션마다 정의


    public const int MAX_MISSION_WORD = 30;
    public string[] strs_word = new string[MAX_MISSION_WORD];

    // 임무 내용을 대표단어들과 조합해서 전달
    // 제목이나 , 개요 내용을 받고 strs_word 단어들로 대체
    public string GetTextLang( string strs )
    {
        // 일단 그냥 간단하게 MAX_MISSION_WORD 만큼 한다.
        return SJ_Language.STR_ReplaceParams( strs , strs_word );
    }

    virtual public void OnEvent( string evt_name , string obj , SJ_DIC<string> _dic ){}

    virtual public void OnComplete(){}

    // 현재 임무의 완료 비율 
    // 하위 미션이 없다면 현재 미션만 체크 0 또는 1 이다.
    // 하위 미션이 있다면 본인 빼고 하위 미션만 체크한다.
    public float CurCompletePer_Local()
    {
        if( child_missions.Count < 1 )
        {
            if( cur_state == 2 ) return 1;
            return 0;
        }
        else
        {
            float child_total = 0;
            foreach( var s in child_missions )
            {
                child_total += s.CurCompletePer_Local();
            }
            return child_total / (float)child_missions.Count;
        }
        return 0;
    }

    // 대표 임무의 총 완료 비율
    // 무조건 first_mission 만 한다.
    public float CurCompletePer_TotalALL()
    {
        // first_mission 

        float total_per = 0;
        MissionBase ms = first_mission;
        int total_num = 1;
        
        while( true )
        {
            total_per += ms.CurCompletePer_Local();
            if( ms.next_mission == null ) break;
            ms = ms.next_mission;
            total_num++;
        }
        return total_per / (float)total_num;
    }

    // 상태 : 0 미수주 , 1 수주 , 2 완료
    public int cur_state;

    // 난이도 , 기타 등등 정보
    public int diff;
    Mng_X128SS rd;
    public int rumor_diff;
    public SJ_DIC<string> dic;

    // 메이킹 함수
    // 인자 : 던전 난이도 , 소문 범위 넓을 수록 수색 난이도 상승 , 기타 인자 등등
    public void CreateMission( 
        SJ_ID_INT_Mng id_make , Mng_X128SS _rd , int child_num  , _BIAS_MISSION bias_type , int LV_start , int LV_end , 
        int _diff , int max_depth, int _depth , MissionBase _par_mission , MissionBase _root_mission  , SJ_DIC<string> _dic )
    {
        ID = id_make.Make_UID( typeof(MissionBase) );
        rd = _rd;
        par_mission = _par_mission;
        root_mission = _root_mission;
        depth = _depth;
        diff = _diff;
        dic = _dic;

        // 레벨 
        // 루트 -> 최대 레벨
        // 차일드 -> 최소 레벨
        LEVEL = (int)Mathf.Lerp( LV_end , LV_start , (float)depth / (float)max_depth );

        OnCreateMission();

        int depth_child = _depth+1;
        if( depth_child <= max_depth )
        {
            int child_count = rd.NextInt( 1 , child_num );
            for( int i = 0 ; i < child_count ; i++ )
            {
                MISSION_TYPE type = bias_type.RandomMission();
                MissionBase mission = InstMission( type );
                mission.CreateMission( id_make , _rd , child_num , bias_type , LV_start , LV_end , _diff , max_depth, _depth , _par_mission , _root_mission  ,_dic );
                child_missions.Add(mission);                
            }
        }
    }

    static public MissionBase InstMission( MISSION_TYPE type )
    {
        switch( type )
        {
            case MISSION_TYPE.DefeatEnemyNormal:return new Mission_DefeatEnemy();
            case MISSION_TYPE.DefeatEnemyBoss:  return new Mission_DefeatEnemy();       // 보스도 일단 같게
            case MISSION_TYPE.DungeonConquer:   return new Mission_DungeonConquer();
            case MISSION_TYPE.GetItem:          return new Mission_GetItem();
            case MISSION_TYPE.GetRumor:         return new Mission_GetRumor();
            case MISSION_TYPE.Affection:        return new Mission_Affection();
        }
        return null;
    }

    // 한개만 만들기
    static public MissionBase InstMissionCreateOne( SJ_ID_INT_Mng id_make , Mng_X128SS _rd , _BIAS_MISSION bias_type , int level , int diff )
    {
        MISSION_TYPE type = bias_type.RandomMission();
        MissionBase mission = InstMission( type );
        mission.LEVEL = level;
        mission.diff = diff;
        mission.rd = _rd;
        return mission;
    }       


    virtual public void OnCreateMission(){}
    virtual public void OnLoad( BinaryReader br ){}
    virtual public void OnSave( BinaryWriter bw ){}
    virtual public void OnAfterLoad(){}

    

}
