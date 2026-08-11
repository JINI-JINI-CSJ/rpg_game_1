using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 배틀 파티 구성
// 전열 후열 구성
public class BattleParty
{
    public CharBase[] chars_Front;
    public CharBase[] chars_Back;

    public const int BATTLE_LINE_NUM = 3;

    public BattleParty()
    {
        chars_Front = new CharBase[BATTLE_LINE_NUM];
        chars_Back = new CharBase[BATTLE_LINE_NUM];

        for( int i = 0 ; i < BATTLE_LINE_NUM ; i++ )
        {
            chars_Front[i] = null;
            chars_Back[i] = null;
        }
    }

    public bool Add( int csv_id , int level , _ARMY_FORCE fORCE )
    {
        CharBase charBase = CharBase.InstCharBase_CSV( csv_id , level , fORCE );
        if( charBase == null ) return false;
        return Add(charBase);
    }

    public bool Add( CharBase charBase )
    {
        CharBase[] lt = GetLine( 0 );
        if(SJ_CSharpUtil.Add_Array( lt , charBase ) == false)
        {
            lt = GetLine( 1 );
            return SJ_CSharpUtil.Add_Array( lt , charBase );
        }
        return false;
    }

    // 0 : 앞 , 1 : 뒤
    public bool Add( int front_back , CharBase charBase )
    {
        CharBase[] lt = GetLine( front_back );
        return SJ_CSharpUtil.Add_Array( lt , charBase );
    }

    public CharBase[] FindLine( CharBase charBase )
    {
        for( int i = 0 ; i < 2 ; i++ )
        {
            CharBase[] lt = GetLine( i );
            if( lt.Contains( charBase ) ) return lt;
        }
        return null;
    }

    public bool Remove( CharBase charBase )
    {
        CharBase[] lt = FindLine( charBase );
        if( lt != null )
        {
            return SJ_CSharpUtil.Remove_Array( lt , charBase );
        }
        return false;
    }

    public CharBase[] GetLine( int front_back )
    {
        if( front_back == 0 )return chars_Front;
        return chars_Back;
    }


    public List<CharBase> GetALL()
    {
        List<CharBase> chars = new();
        chars.AddRange( chars_Front );
        chars.AddRange( chars_Back );
        return chars;
    }

    public List<CharBase> GetBattleLive( bool front = true , bool back = true , bool live = true )
    {
        List<CharBase> chars = new();
        if( front )
        {
            foreach( var s in chars_Front )
            {
                if( s.IsLive() || live == false ) chars.Add(s);
            }            
        }

        if( back )
        {
            foreach( var s in chars_Back )
            {
                if( s.IsLive() || live == false  ) chars.Add(s);
            }
            
        }
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

    public bool CheckLiveALL()
    {
        foreach( var s in GetALL() )
        {
            if( s.IsLive() == false ) return false;
        }
        return true;
    }
    
}
