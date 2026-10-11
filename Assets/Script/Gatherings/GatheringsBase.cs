using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 행사 , 이벤트
/// 무투회 , 인공던전 , 대항전 , 경매장 , 암시장 등등.
/// </summary>

public class GatheringsBase 
{
    // 
    public City city;

    // 등급
    public int class_grade = 1;
    // 행사 주기 : 대기 턴 , 지속 턴
    public int waitTurn = 10;
    public int elapseTurn = 1;

    // 참여 횟수 
    public int participate;
    // 완료 횟수
    public int complete;

    // 최초 완료 보상 , 이후 완료 보상
    public RewardInf rewardInf_First;
    public RewardInf rewardInf_After;

    // 확률 보상 , 여러개 일수 있으니 목록으로 하자.
    public List<RewardInf> lt_rewardInf_Per = new();


    public void TurnPrc()
    {
        if( CheckPlay() )   OnStart();
        else                OnStop();
    }

    public bool CheckPlay()
    {
        // 월드턴으로 계산
        int r_turn = WorldMain.TURN % waitTurn;
        if( r_turn < elapseTurn )
        {
            return true;
        }        
        return false;
    }

    virtual public void OnMake(){}

    // 행사 시작
    virtual public void OnStart(){}
    // 행사 종료
    virtual public void OnStop(){}

    virtual public void OnPlayerEnter(){}
}
