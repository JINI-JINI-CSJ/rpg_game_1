using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 행사 , 이벤트
/// 무투회 , 인공던전 , 대항전 , 경매장 , 암시장 등등.
/// </summary>

public class GatheringsBase 
{
    // 전용 생성 랜덤
    public Mng_X128SS rd_self;

    // 
    public City city;


    // 행사 주기 : 대기 턴 , 지속 턴
    public int waitTurn = 10;
    public int keepUpTurn = 1;
    public bool NotRegular;  // 비 정기 행사 , 행사 종료 시  다음 예약 만들기




    // 참여 횟수 
    public int cur_participate;
    // 완료 횟수
    public int cur_complete;

    // 최초 완료 보상 , 이후 완료 보상
    public RewardInf rewardInf_First;
    public RewardInf rewardInf_After;

    // 확률 보상 , 여러개 일수 있으니 목록으로 하자.
    public List<RewardInf> lt_rewardInf_Per = new();

    public void Make( uint random_seed , City _city  )
    {
        rd_self = new Mng_X128SS( random_seed );
        city = _city;

        // 턴 정하기
        // 일단 랜덤으로 하자.
        // 행사 개최 주기 
        MakeWaitTurn();

        // 랜덤 행사인지.
        // 각자 행사에서 하자.
        //NotRegular = rd_self.RandomFloat_Per( GTF_CSV.csv_Config.gatherings_NotRegular_per );

        OnMake();
    }

    public void MakeWaitTurn()
    {
        // 현재는 도시 등급으로만 하자.
        waitTurn = GTF_CSV.csv_Config.Random_gatherings_ListTier_waitTurn( rd_self , city.cityData.Tier );
        keepUpTurn = GTF_CSV.csv_Config.Random_gatherings_ListTier_KeepUpTurn( rd_self , city.cityData.Tier );
    }

    // 월드 턴 증가후 호출 
    public void TurnPrc()
    {
        int r_turn = WorldMain.TURN % waitTurn;

        if( r_turn == 0 )
        {
            OnStartTurn();
        }

        if( r_turn < keepUpTurn )
        {
            OnKeepUpTurn();
        }  

        if( r_turn == keepUpTurn )
        {
            OnStopTurn();

            // 비 정규 행사이면 다음 행사 기간 다시 계산
            if( NotRegular ) MakeWaitTurn();
        }  
    }


    virtual public void OnMake(){}



    // 행사 시작
    virtual public void OnStartTurn(){}

    // 행사 유지
    virtual public void OnKeepUpTurn(){}

    // 행사 종료
    virtual public void OnStopTurn(){}

    
    // 도시 입장 , 메뉴 추가
    virtual public void OnEnter_City(){}

    // 주인공 행사 입장 , 클릭
    virtual public void OnEnter_Gatherings(){}
}
