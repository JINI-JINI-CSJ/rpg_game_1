using System.Collections.Generic;
using UnityEngine;

public enum BATTLE_COMMAND_CATE
{
    None = -1 ,  // 아무것도 안하기
    Attack ,    // 기본 공격
    Skill ,     // 스킬 , 마법 
    Guard ,     // 방어
    Item ,      // 아이템 사용
    Escape ,    // 도망 시도
    BACK_MENU , // 뒤로 가기 , 이전 캐릭으로 롤백
}


public enum BATTLE_ACTION_TARGET
{
    None  = 0 ,

    // 상대편
    One_Opp_Front , // 상대 전열라인중 1 인
    One_Opp_Back , // 상대 후열라인중 1인 
    One_Opp_ALL , // 상대전체중 1인
    Line_Opp_Front , // 상대 전열 라인 
    Line_Opp_Back ,  // 상대 후열 라인
    Line_Opp_ALL ,  // 상대 전후열중 1개 라인
    ALL_Opp     ,   // 상대 전체


    // 아군
    One_Self_Front , // 
    One_Self_Back , // 
    One_Self_ALL , // 
    Line_Self_Front , // 
    Line_Self_Back ,  // 
    Line_Self_ALL ,  // 
    ALL_Self     ,   // 
}




public class BattleCommand
{
    // 커맨드 분류
    public BATTLE_COMMAND_CATE cmd_cate;
    public SkillBase    skill; // 스킬 선택 했을때
    public ItemBase     item;

    public BATTLE_SEL_GROUP sel_group;

    
}
