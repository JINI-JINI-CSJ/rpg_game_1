using System.Collections.Generic;
using UnityEngine;



// 임무 기본 클래스
public class MissionBase 
{
    // 템플릿 1 종류 
    public int title_Template_1;
    // 템플릿 2 내용
    public int title_Template_2;
    virtual public string GetTitle(){return "제목";}


    // 소문이 있을경우 소문 목록
    public List<RumorBase> rumors = new();

    public int diff;
    public int rumor_diff;
    public SJ_DIC<string> dic;

    // 메이킹 함수
    // 인자 : 던전 난이도 , 소문 범위 넓을 수록 수색 난이도 상승 , 기타 인자 등등
    virtual public void CreateMission( Mng_X128SS rd , int _diff , int _rumor_diff , SJ_DIC<string> _dic )
    {
        diff = _diff;
        rumor_diff = _rumor_diff;
        dic = _dic;
    }
    
}
