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
    One , 
    Line_Front ,
    Line_Back , 
    ALL ,
}


public class BattleCommand
{
    // 커맨드 분류
    public BATTLE_COMMAND_CATE cmd_cate;
    public SkillBase    skill; // 스킬 선택 했을때
    public ItemBase     item;

    public List<CharBase> targetChars = new();

    
}
