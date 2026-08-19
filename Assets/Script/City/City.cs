using System.Collections.Generic;
using UnityEngine;
using WorldForge;

public enum CITY_ACTIVE_TYPE
{
    FIND_FIRST ,   // 처음부터 알려짐
    FIND_NEAR ,     // 근처 도착하면 알려짐 , 이웃 도시
    NO_FIND ,       // 힌트등이 없으면 알수 없음
}

public class City 
{
    public uint ID;
    public CityData cityData;
    public CITY_ACTIVE_TYPE city_active_type;
    // 연결된 도시
    public List<City> cities_Neighbor = new();

    // 특수도시 일 경우 해당 태그
    public string tag_SpcCity;      

    // 도시 규모에 따른 시설들
    public List<CityPartBase> cityParts = new();

    // 의뢰
    public _BIAS_MISSION_TYPE bias_mission;

    // 용병 : 직업 경향  , 마법경향 
    // 더 세부적이라면 각 직업들 가중치
    public _BIAS_SKILL_MAIN_JOB bias_job;
    public _BIAS_MAGIC_DEFINE bias_job_magic_prop;

    // 도시 퀘스트
    public MissionBase mission_Unique;

    // 도시 인물? 등등


    public bool find_cur;
    
}
