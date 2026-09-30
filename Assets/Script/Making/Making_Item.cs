using System.Collections.Generic;
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

    
    // static public ItemBase MakeEqItem( SJ_ID_INT_Mng idMng , Mng_X128SS _rd , _BIAS_ITEM bias_item , int sc_params , int sc_addEff )
    // {
    //     ItemBase item = new();
    //     return item;
    // }

    // csv 장비 아이템기반의 레벨 범위로 만들기  
    static public ItemBase MakeEqItem( SJ_ID_INT_Mng idMng , Mng_X128SS _rd , int lv_s , int lv_e , int sc_params , int sc_addEff )
    {
        CSV_Item csv = GTF_CSV.csv_ItemPage_Equip.GetRangeLevel_One( _rd , lv_s , lv_e );
        if( csv == null )
        {
            Debug.LogError( "MakeEqItem 에러!! : " + lv_s + " : " + lv_e );
            return null;
        }

        ItemBase item = ItemBase.InstItemBase( csv );
        return item;
    }

    // 공통 csv 아이템 장비로 만들기
    // 예 ) 검 기본 , 창 기본 등등 기본 csv 기반으로 만든다. 
    // 레벨등으로 강함을 정한다.
    static public ItemBase MakeEqItem( Mng_X128SS _rd , int level , string tag , int sc_add , int sc_minus )
    {
        CSV_Item csv = GTF_CSV.csv_ItemPage_Equip.GetTag_One( _rd , tag );
        if( csv == null )
        {
            Debug.LogError( "MakeEqItem 에러!! : " + tag );
            return null;
        }
        ItemBase item = ItemBase.InstItemBase( csv );
        item.ID = MakingMain.Make_UID( typeof( ItemBase ) );

        // 아이템 수치 레벨링
        item.Leveling( level );

        // sc_add 만큼 좋은 효과들
        // sc_minus 만큼 나쁜 효과들
        PrcBonusScore( _rd , item , sc_add , 1 );
        PrcBonusScore( _rd , item , sc_minus , -1 );

        return item;
    }



    static public void PrcBonusScore(  Mng_X128SS _rd , ItemBase item , int score , int good_bad ) // good_bad : 1 좋음 , -1 나쁨
    {
        if( score < 1 ) return;

        // 점수  [수치] : [효과] 배분
        int sc_chrVal = 0 , sc_effSk = 0;
        GTF_Common.MakeAddScore_ChrVal_EffSk( _rd  , score , ref sc_chrVal , ref sc_effSk );

        // 추가 수치
        item.chrValue_ScoreFix.AddScore_RandomScore( _rd , sc_chrVal , good_bad );

        // 추가 효과 스킬
        item.skillBaseGroup.RandomAddEffSkill( _rd , sc_effSk , good_bad );
    }



}


// 수치 보정 점수
public class ChrValue_ScoreFix
{
    // 0 : 아이템  , 1 : 캐릭터 , 
    public int making_obj; 

    // 수치 정의 , 보정 점수 
    // 보정 점수는 + , - 해서 최종이 된다.
    public Dictionary<CHAR_STAT,int> dic_grade = new();


    // 0 : 아이템  , 1 : 캐릭터
    public void SetObjType( int obj )
    {
        making_obj = obj;
    }

    public void AddScore_RandomScore( Mng_X128SS rd , int total_score , int good_bad )
    {
        for( int i=0;i<total_score ; i++ )
        {
            AddScore( GTF_Common.Random_Char_Stat(rd) , good_bad );
        }
    }

    public void AddScore( CHAR_STAT stat , int score )
    {
        dic_grade[stat] += score;
    }

    public void AddFix_ChrValue( CharPrcValue charPrcValue )
    {
        float fix_common = 0;

        if( making_obj == 0 )   fix_common = GTF_CSV.csv_Config.making_item_statFix;
        else                    fix_common = GTF_CSV.csv_Config.makeChar_statFix;

        foreach( var s in dic_grade )
        {
            charPrcValue.ADD_VAL_INF( (int)s.Key , this , 0 , fix_common );
        }
    }

    public void RemoveFix_ChrValue( CharPrcValue charPrcValue  )
    {
        charPrcValue.REMOVE_VAL_INF_RefClass( this );
    }
}