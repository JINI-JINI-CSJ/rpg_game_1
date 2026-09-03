using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// 기본 메이킹 월드 배분
public class Make_ItemUnique : MakeBase
{
    static public Make_ItemUnique G;

    void Awake()
    {
        G = this;
    }

    // 배분용 컬랙션 아이템
    // 초기화 시점에 csv 에서 읽어온다.
    // 던전이나 적군에서 드랍 아이템을 만들때 배분한다.
    public List<CSV_Item> csv_Items_Collect = new();

    public void Load_CollectItem()
    {
        csv_Items_Collect.Clear();
        foreach( var s in GTF_CSV.csv_ItemPage_ALL.dic_int.Values.Cast<CSV_Item>() )
        {
            // 태그 "COLLECT" 가 있는 아이템만 배분용으로 등록한다.
            if( s.tag.Contains( "COLLECT" ) ) csv_Items_Collect.Add( s );
        }
    }

    public CSV_Item _Get_CollectItem( Mng_X128SS rd )
    {
        return rd.RandomList( csv_Items_Collect , true );
    }

    static public CSV_Item Get_CollectItem( Mng_X128SS rd )
    {
        return G._Get_CollectItem( rd );
    }
}
