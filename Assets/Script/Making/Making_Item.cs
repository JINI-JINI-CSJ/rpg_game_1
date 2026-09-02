using UnityEngine;

public class _BIAS_ITEM : _BIAS_COMMON
{
    // 1. 장비로만 통일
    // 2. 무기 , 방어구 , 장신구
    // 3. 각 파트별 태그
    // 다음부턴 메이킹 추가
    // 4. 추가 상승 파라미터
    // 5. 추가 효과    

    // 각 태그들
    _BIAS_COMMON bias_weapon=new();
    _BIAS_COMMON bias_armor=new();
    _BIAS_COMMON bias_acc=new();

    public override void OnSetRandom_Init()
    {
        AddObj( _EQUIP_CHR_PART.Weapon );
        AddObj( _EQUIP_CHR_PART.Armor );
        AddObj( _EQUIP_CHR_PART.Acc_1 );

        foreach( var s in GTF_CSV.csv_TagDefinePage.GetTagPart_Str( "ITEM_WEAPON" ) )
        {
            bias_weapon.AddObj( s );
        }

        foreach( var s in GTF_CSV.csv_TagDefinePage.GetTagPart_Str( "ITEM_ARMOR" ) )
        {
            bias_armor.AddObj( s );
        }
        foreach( var s in GTF_CSV.csv_TagDefinePage.GetTagPart_Str( "ITEM_ACC" ) )
        {
            bias_acc.AddObj( s );
        }
    }
}

public class Making_Item
{
    
    static public ItemBase MakeEqItem( SJ_ID_INT_Mng idMng , Mng_X128SS _rd , _BIAS_ITEM bias_item , int sc_params , int sc_addEff )
    {
        ItemBase item = new();
        return item;
    }

    // 이미 있는 아이템에 보너스 스탯 
    // 아이템의 파라미터 보너스 점수는 그냥 추가 강화 정도로... 
    // 이미 품질 강화가 있으니..
    static public void MakeBonusScore( ItemBase item , int sc_params , int sc_addEff )
    {
        
    }
}
