using UnityEngine;
/// <summary>
/// 문 선택 파트 : 적군 전투
/// 
/// 좀 강한 적군으로만 구성? 
/// 엘리트 급으로 일단 만들자.
/// 
/// </summary>
public class Gatherings_SelectDoorObj_Enemy : Gatherings_SelectDoorObjBase
{
    public BattleParty battleParty = new();

    public override void OnMake(Mng_X128SS rd)
    {
        // 적군 테마 : 일반 
        // 등급 : 엘리트
        
    }
}
