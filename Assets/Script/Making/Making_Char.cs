using UnityEngine;

// 정해진 태그나 직업등으로 메이킹
// ==일반==
//  - 기본 수치에서 낮은 추가 점수
//  - 등급에 해당하는 기본 스킬 
// 
// ==유니크==
//  - 기본 수치에서 높은 추가 점수
//  - 점수 높은 메이킹 스킬을 여러개 붙이기
public class Making_Char
{
    static public CharBase Making( Mng_X128SS rd , string tag , int sc_stat , int add_skill = 0 , int sc_skill = 0 )
    {
        // 기본 csv 
        CSV_CharBaseStat csv = GTF_CSV.csv_Char_ALL.GetTag_Contain_Random( rd , tag );
        CharBase charBase = CharBase.InstCharBase_CSV( csv , 1 , _ARMY_FORCE.Player );

        // 스탯 보너스
        charBase.csv.charPrcValue.RandomStatBonus( rd , sc_stat , GTF_CSV.csv_Config.makeChar_statAddFix );
        

        return charBase;
    }
}
