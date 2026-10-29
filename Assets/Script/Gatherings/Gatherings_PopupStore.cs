// using System.Collections.Generic;
// using UnityEngine;

// 별도 클래스 2개를 만들었다.

// 
/// <summary>
/// 팝업 스토어
/// 경매장 , 암시장
/// - 경매장 : 좋은 품질의 확인된 아이템 , 가격이 당연히 비싸다. 성실하게 플레이 
/// - 암시장 : 무작위 품질의 확인 안된 아이템 , 가격 편차가 크다. 도박성 플레이
/// 
/// 행사 시작 전에 미리 아이템을 만들어 놓는다. 
/// 이전 행사의 아이템이 팔리지 않았다면 이월된다.
/// 
/// </summary>


// public class Gatherings_PopupStore : GatheringsBase
// {
//     // 0 : 경매장  , 1 : 암시장
//     public int AuctionHouse_BlackMarket;

//     // 아이템이 존재하면 객체 할당
//     // 팔렸으면 null 

//     // 현재 판매 물품이 있는지 , 아이템 또는 소문
//     public bool BuyAble;

//     public ItemBase itemBase_make;

//     // 암시장에서 소문?
//     public RumorBase rumorBase_BlackMarket;

//     // 아이템의 가격
//     public int price;

//     // public override void OnMake()
//     // {
//     //     // 경매장 , 암시장 확률
//     //     if( rd_self.RandomFloat_Per( GTF_CSV.csv_Config.gatherings_AuctionHouse_BlackMarket_per ) )
//     //     {
//     //         AuctionHouse_BlackMarket = 0;
//     //     }
//     //     else
//     //     {
//     //         AuctionHouse_BlackMarket = 1;
//     //     }
//     //     // 경매장은 정기 행사
//     //     // 암시장은 비정기 행사
//     // }

//     override public void OnStartTurn()
//     {
//         if( BuyAble == false )
//         {
//             // 메이킹 아이템

//             // 레벨
//             int level = GTF_CSV.RandomLevel_CityTire( rd_self , city.cityData.Tier );

//             // 경매장 , 암시장에 따라서 점수 
//             int sc_good = 0;
//             int sc_bad = 0;
//             if( AuctionHouse_BlackMarket == 0 )
//             {
//                 // 고정 보너스 점수
//                 sc_good = GTF_CSV.csv_Config.gatherings_AuctionHouse_GoodScore;
//                 sc_bad = 0;

//                 itemBase_make = Making_Item.MakeEqItem( rd_self , level , "" , sc_good , sc_bad );

//                 // 가격 ,  배수 랜덤
//                 float price_ratio = rd_self.NextFloat( 1.0f , GTF_CSV.csv_Config.gatherings_AuctionHouse_PriceRatio_Max );
//                 price = (int)(price_ratio * GTF_CSV.IncRatioLevel_Resource( level ));
//             }
//             else
//             {
//                 // 확률적으로 소문 인지.
//                 if( rd_self.RandomFloat_Per( GTF_CSV.csv_Config.gatherings_BlackMarket_Rumor_Sell_Per ) )
//                 {
//                     // 소문 
//                     // 확률 진실 , 거짓
//                     rumorBase_BlackMarket = null;
                    
//                     if( rd_self.RandomFloat_Per( GTF_CSV.csv_Config.gatherings_BlackMarket_Rumor_True_Per ) )
//                     {
//                         // 진실.
//                         // 아이템 획득 임무중에 힌트 1개 가져오기
//                         rumorBase_BlackMarket = Make_QuestRumor.Get_Mission_GetItem_NoComplete_OneRumor( rd_self );
//                     }

//                     if( rumorBase_BlackMarket == null )
//                     {
//                         // 위에서 진실이 아니거나 , 모든 아이템 임무가 끝났다면
//                         // 거짓 소문
//                         rumorBase_BlackMarket = new();
//                         rumorBase_BlackMarket.truth = false;
//                     }

//                 }
//                 else
//                 {
//                     // 아이템 랜덤 점수
//                     sc_good = rd_self.NextInt( 0 , GTF_CSV.csv_Config.gatherings_BlackMarket_GoodScore + 1 );
//                     sc_bad = rd_self.NextInt( 0 , GTF_CSV.csv_Config.gatherings_BlackMarket_BadScore + 1 );

//                     itemBase_make = Making_Item.MakeEqItem( rd_self , level , "" , sc_good , sc_bad );
//                 }
//                 float price_ratio = rd_self.NextFloat( 1.0f , GTF_CSV.csv_Config.gatherings_BlackMarket_PriceRatio_Max );
//                 price = (int)(price_ratio * GTF_CSV.IncRatioLevel_Resource( level ));  
//             }

//             BuyAble = true;
//             // 구매 완료 하면   BuyAble = false
//         }
//     }

//     // // 도시 등급에 따른 레벨 랜덤 
//     // int RandomLevel_CityTire()
//     // {
//     //     // 최소 : 도시 등급     
//     //     // 최대 : 도시 등급+1   
//     //     // int min_lv = GTF_CSV.CityTier_To_Level( city.cityData.Tier );
//     //     // int max_lv = GTF_CSV.CityTier_To_Level( city.cityData.Tier + 1 );
//     //     int min_lv = 0; 
//     //     int max_lv = 0;
//     //     GTF_CSV.CityTier_To_Level( city.cityData.Tier , ref min_lv , ref max_lv );

//     //     return rd_self.NextInt( min_lv , max_lv );
//     // }

//     // 도시 진입 이벤트 받고  특별 메뉴 보이기

//     // 판매 물품이 있으면 보여주고 , 없으면 도시 메뉴에서 삭제
//     // 경매장 일 경우 아이템 상세 보여주기
//     // 암시장 일 경우 아이템인지 , 정보인지만 공개 


// }
