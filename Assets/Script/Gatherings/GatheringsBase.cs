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
    public City city;
    // 행사 주기 : 대기 턴 , 지속 턴
    public int waitTurn = 10;
    public int keepUpTurn = 1;

    public bool NotRegular;             // 비 정기 행사 , 행사 종료 시  다음 예약 만들기

    // 비정기 일경우 턴 대기 랜덤 범위
    // 유지는 일단 위의 변수로 하자.
    public int NotRegular_waitTurn_Min;     
    public int NotRegular_waitTurn_Max;      


    // 최초 완료 보상 , 이후 완료 보상
    public RewardInf rewardInf_First;
    public RewardInf rewardInf_After;

    // 확률 보상 , 여러개 일수 있으니 목록으로 하자.
    public List<RewardInf> lt_rewardInf_Per = new();



    //=========================================================================
    // 상태 저장

    // 남은 턴
    public int cur_waitTurn;
    public int cur_keepTurn;

    // 참여 횟수 
    public int cur_participate;
    // 완료 횟수
    public int cur_complete;



    public void Make( uint random_seed , City _city  )
    {
        cur_keepTurn = 0;
        rd_self = new Mng_X128SS( random_seed );
        city = _city;

        city.gatherings.Add( this );

        Make_NotRegular_Turn();

        OnMake();
    }


    // 비정기일경우
    public void Make_NotRegular_Turn()
    {
        if( NotRegular == false ) return;
        cur_waitTurn = rd_self.NextInt( NotRegular_waitTurn_Min , NotRegular_waitTurn_Max );
    }

    // 월드 턴 증가후 호출 
    public void TurnPrc()
    {
        int r_turn = cur_waitTurn--;

        if( r_turn < 1 )
        {
            cur_keepTurn = keepUpTurn;
            OnStartTurn();
        }

        if( cur_keepTurn > 0 )
        {
            OnKeepUpTurn();
            cur_keepTurn--;
            if( cur_keepTurn < 1 )
            {
                OnStopTurn();

                // 비 정규 행사이면 다음 행사 기간 다시 계산
                Make_NotRegular_Turn();
            }              
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
