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
            {
                // 고정 보너스 점수
                sc_good = GTF_CSV.csv_Config.gatherings_AuctionHouse_GoodScore;
                sc_bad = 0;

                itemBase_make = Making_Item.MakeEqItem( rd_self , level , "" , sc_good , sc_bad );

                // 가격 ,  배수 랜덤
                float price_ratio = rd_self.NextFloat( 1.0f , GTF_CSV.csv_Config.gatherings_AuctionHouse_PriceRatio_Max );
                price = (int)(price_ratio * GTF_CSV.IncRatioLevel_Resource( level ));
            }

            BuyAble = true;
            // 구매 완료 하면   BuyAble = false
        }
    }

}
