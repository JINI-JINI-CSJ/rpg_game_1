using System.Collections.Generic;
using QuadTreeSystem;
using UnityEngine;
using WorldForge;

// 도시
// 스타팅 도시 , 등급 , 레벨 차등
// 월드 메이커에서 도시 등급이 정해져 있으니 , 대략 소도시 중 한개 랜덤해서 스타팅
// 이론상 모든 도시를 다 갈수 있으나 
// 도로 가도 이동 난이도가 있으니 힘들수 있다.
// 하지만 각종 스킬(은신,뇌물,협상등등) 로 갈수도 있다.
public class Make_City : MakeBase
{
    public CityData cityData_Start;

    // 저장 로드 할땐 위치값만 사용

    public override void OnMake()
    {
        WorldForgeManager worldForge = Make_WorldMap.G.worldForge;
        QuadTree quadTree = Make_WorldMap.G.quadTree;

        List<QTPoint> qTs = quadTree.GetAllPoints( Make_WorldMap.TAG_HASH_CITY_Village() );

        QTPoint qt_s = GTF_Random.rd_make_world.RandomList( qTs );
        cityData_Start = (CityData)qt_s.Data;

        MakingMain.NextMake();
    }
}
