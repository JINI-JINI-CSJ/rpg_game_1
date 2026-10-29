using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모임 , 행사 
/// 도시에 배정
/// - 무투회 :  중도시 이상
/// - 인공던전: 대도시 이상
/// - 경매장 :  대도시 이상
/// - 암시장 :  중도시 이상
/// 
/// 기타 행사는 계속 아이디어 내자.
/// 
/// 
/// </summary>

public class Make_Gatherings : MakeBase
{
    static public Make_Gatherings G;

    Make_Gatherings()
    {
        G = this;
    }

    public override void OnMake()
    {
        // 수도 : 거의 모든 행사 다 만들기
        List<City> cities_nations = Make_WorldMap.GetCities_Tire( 0 );

        foreach( var s in cities_nations )
        {
            
        }
    }
}
