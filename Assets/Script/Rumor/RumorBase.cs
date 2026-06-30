using UnityEngine;

/// <summary>
/// 소문 베이스
/// </summary>

public class RumorBase
{
    // 템플릿 1 종류 
    public int title_Template_1;
    
    // 템플릿 2 내용
    public int title_Template_2;

    virtual public string GetTitle(){return "제목";}

    virtual public string GetDesc(){return "내용";}

    // 연관된 임무
    public MissionBase mission;
}
