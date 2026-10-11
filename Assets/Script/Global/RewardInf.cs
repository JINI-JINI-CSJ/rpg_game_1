using System.Collections.Generic;
using System.IO;
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

    public void Write( BinaryWriter bw )
    {
        
    }

    public void Read( BinaryReader br )
    {
        
    }
}
