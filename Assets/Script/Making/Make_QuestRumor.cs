using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 만들어진 유니크 객체들을 퀘스트로 분배하기
/// 배분 대상 : 던전 , 도시 호감도 , 정기 행사
/// 
/// 
/// </summary>

public class Make_QuestRumor : MakeBase
{
    static public Make_QuestRumor G;

    // 임무 중에 "아이템 획득" 클래스만 정리 해놓은것 , 메이크 아이템 파트로 옮길수도... 
    public List<Mission_GetItem> mission_GetItems = new();


    public override void OnMake()
    {
        
    }


    // 완료되지 않은 "소문 아이템" 퀘스트 , 또는 소문 입수 조건이 있는 아이템 퀘스트
    static public List<Mission_GetItem> Get_Mission_GetItem_NoComplete()
    {
        List<Mission_GetItem> lt = new();
        foreach( var s in G.mission_GetItems )
        {
            if( s.cur_state != 2 ) lt.Add(s);
        }
        return lt;
    }

    // 위의 반환목록중에서 1개의 임무중에 1개의 소문
    static public RumorBase Get_Mission_GetItem_NoComplete_OneRumor( Mng_X128SS rd )
    {
        List<Mission_GetItem> missions = Get_Mission_GetItem_NoComplete();
        if( missions.Count < 1 ) return null;
        Mission_GetItem getItem =  rd.RandomList( missions );
        return getItem.GetRumorNoHave( rd );
    }
}
