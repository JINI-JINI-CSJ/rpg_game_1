using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CSV_ITEM_PopRefill<CSV_Item> : SJ_ListPopRefill<CSV_Item>
{
    public Mng_X128SS rd;
    public override int OnRandom(int max)
    {
        return rd.NextInt(0,max);
    }
}

public class ITEMBase_PopRefill<ItemBase> : SJ_ListPopRefill<ItemBase>
{
    public Mng_X128SS rd;
    public override int OnRandom(int max)
    {
        return rd.NextInt(0,max);
    }
}

// 기본 메이킹 월드 배분

// 1. 유니크 아이템 월드 배분 , 동급 대비 가장 좋은 성능 1개 장비 (보너스 점수 최고로...)
// 2. 수집품 등급 정의 및 던전 도시 배분
// 레시피는 다른 메이킹 클래스에서 하자.

public class Make_ItemUnique : MakeBase
{
    static public Make_ItemUnique G;

    void Awake()
    {
        G = this;
    }

    // 유니크 아이템
    // 등급 , 리필 
    public Dictionary<int,ITEMBase_PopRefill<ItemBase>> dic_ItemUnique_PopRefill = new();    

    // 배분용 컬랙션 아이템
    // 초기화 시점에 csv 에서 읽어온다.
    // 던전이나 적군에서 드랍 아이템을 만들때 배분한다.

    //MATTER_COOKING
    public List<CSV_Item> csv_Items_MATTER_all = new();
    public List<CSV_Item> csv_Items_MATTER_COOKING = new();
    public List<CSV_Item> csv_Items_MATTER_DISPENSE = new();


    // 재료 희소 등급별로 정리
    public Dictionary<int,List<CSV_Item>> dic_grade_Items_MATTER_COOKING = new();
    public Dictionary<int,List<CSV_Item>> dic_grade_Items_MATTER_DISPENSE = new();


    public void Make_EqItem()
    {
        // 동급 듭급에서 가장 좋은 장비 아이템
        // 일단 각 등급당 고정 개수 만들기
        // 아니면 최종 등급에 가까운 전설 무기를 많이 만들까? (옛날부터 내려오는 전설이 있다.)
        // 그 후 던전 , 이벤트 등으로 배분하자.
        // 숨김 던전 , 경매 , 암시장 , 투자 , 기연 등으로 배분하자.
        // 레벨링은 고려 안해도 된다. 낮은 레벨에서 전설 무기 1개로는 사기가 아니다.

        // 대략 n단계 정도.. 전설급 , 이세계 == 이계 == 다원(多元)급 == 이경(異境)급
        // 노멀 엔딩 스토리까지는 전설급
        // 그 후 진 엔딩까지 이경급
        // 아니면 이벤트나 던전 별로 나누기
        // 전설8 : 이경2 같이 비율
        // 단건 사건(던전 , 경매 , 암시장)을 전설급으로
        // 많은 단서 입수 ->  이세계 급

        // 일단 [전설] -> [신화] -> [이경] 정도로 하자.
        // 각 등급당 개수는 전역으로 정하자.

        for( int i = 0 ; i < GTF_CSV.csv_Config.max_grade; i++ )
        {
            int num = GTF_CSV.csv_Config.GetMaking_unique_item_grade_num( i );
            int grade = i + 1;
            if( num > 0 )
            {
                // MAKE_ITEM_EQ_BASE 기본 베이스 아이템으로 만들기
                // 좋음 점수만 추가 
                ItemBase item = Making_Item.MakeEqItem( GTF_Random.rd_make_common , GTF_CSV.csv_Config.GradeToLevel( grade ) , 
                                        "MAKE_ITEM_EQ_BASE" , GTF_CSV.csv_Config.making_unique_item_good_score , 0 );

                ITEMBase_PopRefill<ItemBase> popRefill = null;
                if( dic_ItemUnique_PopRefill.TryGetValue( grade , out popRefill ) == false )
                {
                    popRefill = new();
                    dic_ItemUnique_PopRefill[grade] = popRefill;
                }
                popRefill.AddSrcOne( item );
            }
        }
    }


    public void Load_CollectItem()
    {
        // 수집 아이템 분류
        csv_Items_MATTER_all.Clear();
        csv_Items_MATTER_COOKING.Clear();
        csv_Items_MATTER_DISPENSE.Clear();
        foreach( var s in GTF_CSV.csv_ItemPage_ALL.dic_int.Values.Cast<CSV_Item>() )
        {
            if( s.tag.Contains( "MATTER_" ) )
            {
                csv_Items_MATTER_all.Add(s);
                if( s.tag.Contains( "MATTER_COOKING" ) )csv_Items_MATTER_COOKING.Add(s);
                if( s.tag.Contains( "MATTER_DISPENSE" ) )csv_Items_MATTER_DISPENSE.Add(s);
            }
        }

        // 등급당 개수 계산
        // 누적해서 계산하자.
        List<int> item_grade_num = new();
        int recent_num = 0;
        foreach( var s in GTF_CSV.csv_Config.making_item_collect_per )
        {
            recent_num = recent_num + (int)((float)csv_Items_MATTER_all.Count * (0.01f * (float)s));
            item_grade_num.Add(recent_num);
        }

        // 랜덤으로 팝업하면서 등급만 정의 
        CSV_ITEM_PopRefill<CSV_Item> popRefill = new();
        popRefill.rd = GTF_Random.rd_make_item;
        popRefill.InitList( csv_Items_MATTER_all );

        int num_prc = 0;
        while( true )
        {
            CSV_Item csv = popRefill.Popup( true );
            if( csv == null ) break;

            // 재료 등급 0 ~ n ( GTF_CSV.csv_Config.making_item_collect_per )
            for( int grade = 0; grade < item_grade_num.Count ; grade++ )
            {
                if( num_prc <= item_grade_num[grade] )
                {
                    csv.making_grade = grade;

                    Dictionary<int,List<CSV_Item>> dic_grade = null;
                    if( csv.tag.Contains( "MATTER_COOKING" ) )  dic_grade = dic_grade_Items_MATTER_COOKING;
                    else                                        dic_grade = dic_grade_Items_MATTER_DISPENSE;

                    List<CSV_Item> grade_lt = null;
                    if( dic_grade.TryGetValue( grade , out grade_lt ) == false )
                    {
                        grade_lt = new();
                        dic_grade[grade] = grade_lt;
                    }
                    grade_lt.Add( csv );
                }
            }
            num_prc++;
        }
    }

    public void MakeRecipe()
    {
        
    }

    public CSV_Item _Get_CollectItem( Mng_X128SS rd )
    {
        //return rd.RandomList( csv_Items_Collect , true );
        return null;
    }

    static public CSV_Item Get_CollectItem( Mng_X128SS rd )
    {
        return G._Get_CollectItem( rd );
    }
}
