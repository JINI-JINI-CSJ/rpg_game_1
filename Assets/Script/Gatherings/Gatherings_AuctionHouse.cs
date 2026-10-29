using UnityEngine;

public class Gatherings_AuctionHouse : GatheringsBase
{

    public bool BuyAble;

    public ItemBase itemBase_make;

    // 암시장에서 소문?
    public RumorBase rumorBase_BlackMarket;

    // 아이템의 가격
    public int price;

    override public void OnStartTurn()
    {
        if( BuyAble == false )
        {
            // 메이킹 아이템

            // 레벨
            int level = GTF_CSV.RandomLevel_CityTire( rd_self , city.cityData.Tier );

            // 경매장 , 암시장에 따라서 점수 
            int sc_good = 0;
            int sc_bad = 0;
            //if( AuctionHouse_BlackMarket == 0 )
            {
                // 고정 보너스 점수
                sc_good = GTF_CSV.csv_Config.gatherings_AuctionHouse_GoodScore;
                sc_bad = 0;

                itemBase_make = Making_Item.MakeEqItem( rd_self , level , "" , sc_good , sc_bad );

                // 가격 ,  배수 랜덤
                float price_ratio = rd_self.NextFloat( 1.0f , GTF_CSV.csv_Config.gatherings_AuctionHouse_PriceRatio_Max );
                price = (int)(price_ratio * GTF_CSV.IncRatioLevel_Resource( level ));
            }

            // else
            // {
            //     // 확률적으로 소문 인지.
            //     if( rd_self.RandomFloat_Per( GTF_CSV.csv_Config.gatherings_BlackMarket_Rumor_Sell_Per ) )
            //     {
            //         // 소문 
            //         // 확률 진실 , 거짓
            //         rumorBase_BlackMarket = null;
                    
            //         if( rd_self.RandomFloat_Per( GTF_CSV.csv_Config.gatherings_BlackMarket_Rumor_True_Per ) )
            //         {
            //             // 진실.
            //             // 아이템 획득 임무중에 힌트 1개 가져오기
            //             rumorBase_BlackMarket = Make_QuestRumor.Get_Mission_GetItem_NoComplete_OneRumor( rd_self );
            //         }

            //         if( rumorBase_BlackMarket == null )
            //         {
            //             // 위에서 진실이 아니거나 , 모든 아이템 임무가 끝났다면
            //             // 거짓 소문
            //             rumorBase_BlackMarket = new();
            //             rumorBase_BlackMarket.truth = false;
            //         }

            //     }
            //     else
            //     {
            //         // 아이템 랜덤 점수
            //         sc_good = rd_self.NextInt( 0 , GTF_CSV.csv_Config.gatherings_BlackMarket_GoodScore + 1 );
            //         sc_bad = rd_self.NextInt( 0 , GTF_CSV.csv_Config.gatherings_BlackMarket_BadScore + 1 );

            //         itemBase_make = Making_Item.MakeEqItem( rd_self , level , "" , sc_good , sc_bad );
            //     }
            //     float price_ratio = rd_self.NextFloat( 1.0f , GTF_CSV.csv_Config.gatherings_BlackMarket_PriceRatio_Max );
            //     price = (int)(price_ratio * GTF_CSV.IncRatioLevel_Resource( level ));  
            // }

            BuyAble = true;
            // 구매 완료 하면   BuyAble = false
        }
    }

}
