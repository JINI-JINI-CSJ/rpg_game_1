using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 유니크 캐릭터
/// 인게임에서 만들수도 있다.
/// 아이템과 마찬가지로 동급 최대 점수로 만든다.
/// 
/// 여기서는 메이킹 인물들 만들기.
/// - 중대 도시급의 인물 : 도시 유니크 임무 , 또는 도시 호감도 완료 , 수도 -> 6클래스 , 대도시 -> 5클래스
/// -- csv : [수도 , 대도시 의 클래스 등급 ]
/// 
/// - 재야 숨은 인재 :  특정 도시에 관계 없이 정보 수집 , 대략 4~8 클래스 , 당연히 고 클래스일수록 정보 개수 많아짐, 최종적으로 아무 도시에 배치한다.
///                    정보가 다 모아지면 알림으로 특정 도시 알림 , 도시 진입하면 바로 영입
/// -- csv : [인원수 , 클래스 등급 , 소문 개수]
/// 																			
/// 
/// </summary>
/// 

public class CharBase_PopRefill<CharBase> : SJ_ListPopRefill<CharBase>
{
    public Mng_X128SS rd;
    public override int OnRandom(int max)
    {
        return rd.NextInt(0,max);
    }
}

public class Make_CharUnique : MakeBase
{

    // 유니크 캐릭터
    // 등급 , 리필 
    public Dictionary<int,CharBase_PopRefill<CharBase>> dic_ChrUnique_PopRefill = new();    

    public override void OnMake()
    {
        // csv 캐릭터유니크메이킹 
        foreach( var s in GTF_CSV.csv_CharUniqueMakePage.dic_int.Values.Cast<CSV_CharUniqueMake>() )
        {
            MakeChrCSV(s);
        }
    }

    void MakeChrCSV( CSV_CharUniqueMake csv )
    {
        int class_grade = csv.class_grade;
        for( int i = 0 ; i < csv.making_total ; i++ )
        {
            // 캐릭터 직업 태그로 만들기
            // 태크 정의 csv 에서 랜덤으로 가져오기
            string tag_job = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( "MAKE_CHAR_JOB_TAG" , GTF_Random.rd_make_common );
            CharBase charBase = Making_Char.Making( GTF_Random.rd_make_common , tag_job , csv.Score_chrVal , "" , csv.Score_SkillNormal , "" , csv.Score_SkillUltimate );
            CharBase_PopRefill<CharBase> popRefill = null;
            if( dic_ChrUnique_PopRefill.TryGetValue( class_grade , out popRefill ) == false )
            {
                popRefill = new();
                dic_ChrUnique_PopRefill[class_grade] = popRefill;
            }
            popRefill.AddSrcOne( charBase );
        }
    }
}
