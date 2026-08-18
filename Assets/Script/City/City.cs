using System.Collections.Generic;
using UnityEngine;
using WorldForge;


public class City 
{
    public CityData cityData;

    public bool active;

    // 특수도시 일 경우 해당 태그
    public string tag_SpcCity;  

    // 도시 규모에 따른 시설들
    public List<CityPartBase> cityParts = new();

    // 의뢰
    public _BIAS_MISSION_TYPE bias_mission;

    // 용병 
    // 직업 경향  , 마법경향 
    // 더 세부적이라면 각 직업들 가중치
    public _BIAS_SKILL_MAIN_JOB bias_job;
    public _BIAS_MAGIC_DEFINE bias_job_magic_prop;

    // 도시 퀘스트
    public MissionBase mission_Unique;

    // 도시 인물? 등등
}
