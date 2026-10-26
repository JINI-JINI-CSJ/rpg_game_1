using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 드랍 전용
/// 금액 , 소모품 , 수집품 , 장비 아이템 등의 확률을 정의한다.
/// 세부 아이템은 정의하지 않는다
/// </summary>
/// 

// 드랍 정보 기본 단위
public class _DROP_PER_UNIT
{
    // ###특이 사항###
    // 아이템 테이블의 순서 
    // 일단 이거로 장비 아이템의 보너스를 정하자.
    // 뒤로 갈수록 , 많을수록 장비 아이템 보너스 점수가 높아진다.
    // 강한 보스일 수록 아이템을 더 많이 넣자. 운이 좋아서 여러개 나와도 된다.
    // 구지 정확하게 하려면 필드 추가하자.
    public int idx;

    // 종류 : 아이템 , 골드 , 정보소문
    public string kind_drop;

    // 드랍 확률 ( 100 기준 독립 계산 )
    public float per_min;
    public float per_max;

    public void ReadCSV( SJ_CSV_BaseObj csv )
    {
        kind_drop = csv.Next();
        per_min = csv.Next_Float();
        per_max = csv.Next_Float();
    }

    public bool Check_Gold(){if( kind_drop == "GOLD" ) return true;return false;}
    public bool Check_Rumor(){if( kind_drop == "RUMOR" ) return true;return false;}

    public _ITEM_TYPE Get_ITEM_TYPE()
    {
        _ITEM_TYPE t = _ITEM_TYPE.None;
        Enum.TryParse( kind_drop , out t );
        return t;
    }

    public float PerRandom( Mng_X128SS rd )
    {
        return rd.NextFloat( per_min , per_max );
    }
}

// ``ID_STR	이름	설명	태그	아이템1-파트	아이템1-최소	아이템1-최대	아이템2-파트	아이템2-최소	아이템2-최대	아이템3-파트	아이템3-최소	아이템3-최대	아이템4-파트	아이템4-최소	아이템4-최대	아이템5-파트	아이템5-최소	아이템5-최대	아이템6-파트	아이템6-최소	아이템6-최대				
/// <summary>
/// **확률표 처럼 문자열 아이디**
/// </summary>
public class CSV_DropRewardPer : SJ_CSV_BaseObj
{
    // 이름 
    // 설명
    // tag 
    // 목록 _DROP_PER_UNIT

    public string tag;

    public List<_DROP_PER_UNIT> _DROPs = new();

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);

        tag = Next();

        while( true )
        {
            if( Check_AbleNext() == false ) break;
            _DROP_PER_UNIT s = new();
            s.ReadCSV( this );
            s.idx = _DROPs.Count;
            _DROPs.Add(s);
        }
    }
}

public class CSV_DropRewardPerPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_DropRewardPer();
    }

}