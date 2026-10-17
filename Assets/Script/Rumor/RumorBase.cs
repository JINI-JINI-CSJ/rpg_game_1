using UnityEngine;

/// <summary>
/// 소문 베이스
/// </summary>

public class RumorBase
{
    // // 템플릿 1 종류 
    // public int title_Template_1;
    // // 템플릿 2 내용
    // public int title_Template_2;
    // virtual public string GetTitle(){return "제목";}
    // virtual public string GetDesc(){return "내용";}

    public uint ID;

    // 진실 여부 , 현재는 획득 후 바로 확인 및 삭제
    public bool truth;

    // 연관된 임무
    public MissionBase mission;


    // 현재 상황 , 입수 여부
    public bool cur_Have;
}
