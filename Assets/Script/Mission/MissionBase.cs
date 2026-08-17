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
    DefeatEnemy ,
    DungeonConquer , 
    GetItem ,
    GetRumor ,
    Affection ,
    MAX , 
}


// 임무 기본 클래스
// 단서완료도 포함
public class MissionBase 
{
    public uint ID;

    public MissionBase root_mission;
    public MissionBase par_mission;
    // 충족해야할 미션 완료들 , 하위 미션들 , 단일 미션이면 없다.
    public List<MissionBase> child_missions = new();

    // 
    public int depth;

    public int LEVEL;

    // 템플릿 1 종류 
    public int title_Template_1;
    // 템플릿 2 내용
    public int title_Template_2;
    virtual public string GetTitle(){return "제목";}

    virtual public void OnEvent( string evt_name , string obj , SJ_DIC<string> _dic ){}

    virtual public void OnComplete(){}

    // 상태 
    public int state;

    // 난이도 , 기타 등등 정보
    public int diff;
    Mng_X128SS rd;
    public int rumor_diff;
    public SJ_DIC<string> dic;

    // 메이킹 함수
    // 인자 : 던전 난이도 , 소문 범위 넓을 수록 수색 난이도 상승 , 기타 인자 등등
    public void CreateMission( 
        SJ_ID_INT_Mng id_make , Mng_X128SS _rd , _BIAS_MISSION_TYPE bias_type , int LV_start , int LV_end , int _diff , int max_depth, int _depth 
         , MissionBase _par_mission , MissionBase _root_mission  , SJ_DIC<string> _dic )
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
            MISSION_TYPE type = bias_type.Random_MissionType();
            MissionBase mission = InstMission( type );
            mission.CreateMission( id_make , _rd , bias_type , LV_start , LV_end , _diff , max_depth, _depth , _par_mission , _root_mission  ,_dic );
            child_missions.Add(mission);
        }

    }

    static public MissionBase InstMission( MISSION_TYPE type )
    {
        switch( type )
        {
            case MISSION_TYPE.DefeatEnemy:      return new Mission_DefeatEnemy();
            case MISSION_TYPE.DungeonConquer:   return new Mission_DungeonConquer();
            case MISSION_TYPE.GetItem:          return new Mission_GetItem();
            case MISSION_TYPE.GetRumor:         return new Mission_GetRumor();
            case MISSION_TYPE.Affection:        return new Mission_Affection();
        }
        return null;
    }
    virtual public void OnCreateMission(){}
    virtual public void OnLoad( BinaryReader br ){}
    virtual public void OnSave( BinaryWriter bw ){}

    virtual public void OnAfterLoad(){}

    
}
