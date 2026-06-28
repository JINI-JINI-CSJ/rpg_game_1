using UnityEngine;

// 캐릭터 메이킹 성향 
// 

public class BiasMake_Char 
{
    // 대분류 : 일반 , 전사 , 마법사 , 지원가
    // 일반 확률은 낮게
    public _BIAS_JOB bias_job = new();

    // 직업군 
    public _BIAS_SKILL_MAIN_JOB bias_skill_job = new();

    // 마법 속성 , 플레이어는 없음? , 적군만 있게?
    public _BIAS_MAGIC_DEFINE bias_magic_define = new();

    // 무기 마스터리에 맞는 스킬
    // 직접 csv 에서 랜덤으로 몇개 가져온다.
    // 보너스 수치만 큼 더 가져온다.

    // 캐릭터 수치 , 추가 보너스로 몇개 수치 상승
    public _BIAS_CHAR_STAT bias_char_stat = new();

    // 성장 등급
    // 높을 수록 최대 레벨 증가
    // 확률 시트에서 가져온다.


    // 고등급 , 유니크에 대한 혜택은?
    // 성장 등급 + 최대 보너스 
    // 

    public void Init( Mng_X128SS _rd_make , Mng_X128SS _rd_inGame )
    {
        bias_job.SetRandom_Init( _rd_make , _rd_inGame );
        bias_skill_job.SetRandom_Init( _rd_make , _rd_inGame );
        bias_magic_define.SetRandom_Init( _rd_make , _rd_inGame );
        bias_char_stat.SetRandom_Init( _rd_make , _rd_inGame );
    }

    public CharBase Making( int max_grade )
    {
        // 1. 대 분류 직업
        // 2. 세부 직업 ( 검사 , 광전사 , 마법사 , 성직자 등등 )
        // 3. 추가 보너스 스킬 
        //  - 추가 갯수
        //  - 각 스킬마다 대분류 스킬 확률 (전,마,지)  , 본인 
        
        return null;
    }
}
