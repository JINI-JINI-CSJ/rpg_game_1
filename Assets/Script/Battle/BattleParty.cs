using System.Collections.Generic;
using UnityEngine;

// 배틀 파티 구성
// 전열 후열 구성
public class BattleParty
{
    public List<CharBase> chars_Front = new();
    public List<CharBase> chars_Back = new();

    public List<CharBase> GetALL()
    {
        List<CharBase> chars = new();
        chars.AddRange( chars_Front );
        chars.AddRange( chars_Back );
        return chars;
    }

    public List<CharBase> GetBattleLive()
    {
        List<CharBase> chars = new();
        foreach( var s in chars_Front ) if( s.IsLive() ) chars.Add(s);
        foreach( var s in chars_Back ) if( s.IsLive() ) chars.Add(s);
        return chars;
    }

    // 
    public List<CharBase> GetBattleCommandAble()
    {
        List<CharBase> chars = new();

        foreach( var s in chars_Front ) if( s.AbleBattleCommand() ) chars.Add(s);
        foreach( var s in chars_Back ) if( s.AbleBattleCommand() ) chars.Add(s);
        return chars;
    }
    
}
