using UnityEngine;



public class CSV_ConteStory : SJ_CSV_BaseObj
{
    // ``ID	이름	설명	하위 임무 평균 갯수(최소1)	소문 범위 최대	일반 적군 격파	보스 격파	수집	전달 배달	호위 경호	던전 조사	던전 답사	소문 입수	우호도		

    public int child_num;
    public int rumor_range;

    public float  per_DefeatEnemyNormal;
    public float  per_DefeatEnemyBoss;
    public float  per_GetItem;    
    public float  per_Delivery;
    public float  per_Escort;
    public float  per_DungeonObjCheckUp;
    public float  per_DungeonConquer;
    public float  per_GetRumor;
    public float  per_Affection;
    public float  per_ManyBattle;



    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);

        Next();
        Next();
        child_num = Next_Int();
        rumor_range = Next_Int();
        per_DefeatEnemyNormal   = Next_Float();
        per_DefeatEnemyBoss     = Next_Float();
        per_GetItem             = Next_Float();
        per_Delivery            = Next_Float();
        per_Escort              = Next_Float();
        per_DungeonObjCheckUp   = Next_Float();
        per_DungeonConquer      = Next_Float();
        per_GetRumor            = Next_Float();
        per_Affection           = Next_Float();
        per_ManyBattle          = Next_Float();
    }
}

public class CSV_ConteStoryPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_ConteStory();
    }
}