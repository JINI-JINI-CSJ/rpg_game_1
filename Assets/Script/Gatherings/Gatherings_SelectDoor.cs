using UnityEngine;
/// <summary>
/// 연속 문 선택하기
/// 문 종류
/// - 통과
/// - 전투
/// - 함정
/// - 그 외 , 특정 스킬 필요 ( 예 : 고고학 7랭크 이상 , 관찰 5 랭크 이상 )
/// 
/// 
/// </summary>
public class Gatherings_SelectDoor : GatheringsBase
{
    // 항목 요소들은 미리 만들고 , 유저 시작할때 배치만 따로 한다.


    // 초회 우승 보상
    public RewardInf reward_First = new();

    // 반복 우승 보상
    public RewardInf reward_Loop = new();

    public bool isFirst_Reward;

    public override void OnMake()
    {
        // 도시 규모에 따라 문 개수
        
    }


}

// 각 요소들의 객체들 , 각각 클래스로 만들자.
