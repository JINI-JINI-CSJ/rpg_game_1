using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 획득 
/// 소문을 모아서 특정 위치를 지정하기
/// 특정 던전의 위치를 특정하게 하자
/// 만약 유저가 아무 정보 없이 최종 위치를 발굴하면 확률적으로 발견 가능하게 하자. 물론 아주 희박한 확률
/// </summary>

public class Mission_GetItem : MissionBase
{
    // 최종 아이템
    public ItemBase item;

    // 최종 위치
    public COORD_IN_DUNGEON coord_in_dungeon;

    // 소문 정보
    public List<RumorBase> rumors = new();


    // 무작위로 아직 입수하지 않은 소문 가져오기
    public RumorBase GetRumorNoHave( Mng_X128SS rd )
    {
        if( rumors.Count < 1 ) return null;
        List<RumorBase> no_have = new();
        foreach( var s in rumors )
        {
            if( s.cur_Have == false ) no_have.Add(s);
        }
        if( no_have.Count < 1 ) return null;
        return rd.RandomList( no_have );
    }
}
