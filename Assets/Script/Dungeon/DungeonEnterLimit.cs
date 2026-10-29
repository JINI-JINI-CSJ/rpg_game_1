using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;

/// <summary>
/// 던전 입장 제한 조건 , 여러 가지 조건이 있을수 있다.
/// 조건 유형이 여러가지가 있을수 있고 , 유형들을 중첩해서 체크한다.
/// 
/// - 인원수
/// - 직업 대 카테고리 
/// - 세부직업
/// - 기타 등등
/// 
/// ==제한의 강도 , 점수==
/// 
/// 
/// </summary>
public class DungeonEnterLimit
{
    public int          numChars = 6;           // 최대 인원수
    public JOB_BASE     job_base;           // 허용 직업 대 카테고리 
    public List<string> job_tag_no = new(); // 금지 직업 태그 

    // 여기서 명령어 등등도 제한 할까나?
    // 일반 공격 , 스킬 , 아이템 등등
    // ...

    // 무작위로 만들기
    // 일단 대 분류 파트중에 한가지 씩만 하자.
    // 현재는 입장 조건만
    public void MakeRandom( Mng_X128SS rd )
    {
        switch( rd.NextInt( 0,3 ) )
        {
            case 0:
                {
                    numChars = rd.NextInt( 1 , 7 );
                }
                break;
            case 1:
                {
                    List<JOB_BASE> lt_job_base = new List<JOB_BASE> { JOB_BASE.FIGHTER , JOB_BASE.WIZARD , JOB_BASE.SUPPORTER };
                    job_base = rd.RandomList( lt_job_base );
                }
                break;
            case 2:
                {
                    // csv 말고 일단 여기서 기본 설정
                    float ratio_min = 0.1f;
                    float ratio_max = 0.5f;

                    List<string> tag_all = GTF_CSV.csv_TagDefinePage.GetTagPart_Str("CHAR_JOB_TAG_DETAIL");
                    int total_job_detail = tag_all.Count;
                    int min = (int)((float)total_job_detail * ratio_min);
                    int max = (int)((float)total_job_detail * ratio_max);
                    int cur = rd.NextInt( min , max );

                    for( int i = 0 ; i < cur ; i++ )
                    {
                        job_tag_no.Add( rd.RandomList( tag_all , true ) );
                    }
                }
                break;
        }
    }


    public bool Checkbox( BattleParty party )
    {
        if( numChars < party.CountLiveALL() ) return false;
        foreach( var s in party.GetALL() )
        {
            if( job_base != JOB_BASE.Common && s.Check_JOB_BASE( job_base ) == false ) return false;
            if( s.Check_JOB_TAG_List( job_tag_no ) ) return false;
        }
        return true;
    }

    // 각 파트별 점수 : 대략 전체 직업들중에 어느정도 비율로 제한하는지...
    // numChars : n/6
    // job_base : n/3 
    // job_tag_no : n/ 직업 총 태그수

    // 일단 총 점수 , 각 파트를 일점 점수(100) 를 만점으로 다 더하기 , 
    // 파트당 올 제한일경우 100 이다 , 이론상 전부 제한 할수 없으니 100은 안나온다.
    // 파트 개수 만큼 더한다.
    // 현재는 3파트

    public const int MAX_SCORE = 300;

    // 높을 수록 제한이 많고 , 난이도 상승
    public int Score()
    {
        int total = 0;
        // 인원수 , 적을 수록 큰 점수
        total += (int)(100.0f * ((float)( 6 - numChars ) / 6.0f) );
        // JOB_BASE 파트 한개만 지정되면 2/3 , 대략 66 정도
        if( job_base != JOB_BASE.Common ) total += 66;
        // 총 직업 태그
        total += (int)(100.0f * ((float)job_tag_no.Count /(float)GTF_CSV.csv_TagDefinePage.GetTagPart_Total( "CHAR_JOB_TAG_DETAIL" ) ));
        return total;
    }

    public float Score_TotalRatio()
    {
        return (float)Score() / (float)MAX_SCORE;
    }
}
