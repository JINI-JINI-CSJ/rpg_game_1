using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

// 보상 정보
public class RewardInf 
{
    // 골드
    // 경험치
    // 아이템 
    // 캐릭터
    // 소문

    public int gold;
    public int exp;
    public List<ItemBase> items = new();
    public List<CharBase> charBases = new();
    public List<RumorBase> rumors = new();

    // 확률 , 0 ~ 1 값으로 한다.
    public float per_get;
    
    // 이미 보상 받은 것인지.
    // 일반 보상에는 사용하지 않는다.
    public bool getState;

    public void Clear()
    {
        gold = 0;
        exp = 0;
        items.Clear();
        charBases.Clear();
        rumors.Clear();
    }

    public bool CheckPerGet( Mng_X128SS rd )
    {
        if( rd.NextFloat() <= per_get ) return true;
        return false;
    }

    public string ToMsg( string add_last = "" )
    {
        string str = "";        
        List<string> lt = new();

        foreach( var s in items )lt.Add(s.GetName());
        foreach( var s in charBases )lt.Add(s.GetName());
        foreach( var s in rumors )lt.Add(s.GetName());
        lt.Add( exp + " EXP" );
        lt.Add( gold + " GOLD" );

        foreach( var s in lt )
        {
            if( string.IsNullOrEmpty(str ) )
            {
                str  = s;
            }
            else
            {
                str += "\n" + s; 
            }
        }

        if( string.IsNullOrEmpty( add_last ) == false )
        {
            str += "\n" + add_last; 
        }
        return str;
    }

    public void Write( BinaryWriter bw )
    {
        
    }

    public void Read( BinaryReader br )
    {
        
    }
}
