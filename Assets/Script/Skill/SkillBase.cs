using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬 베이스
/// </summary>

public class SkillBase 
{
    public CSV_Skill csv;

    public int LEVEL;

    

    virtual public void Action( List<CharBase> targets ){}
}
