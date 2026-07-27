using UnityEngine;

// 캐릭터 객체 기본 , 플레이어 파티 , 적군 등등
public class CharBase 
{
    public CSV_CharBaseStat csv;

    public int cur_HP;
    public int cur_MP;

    public void SetCSV( CSV_CharBaseStat _csv )
    {
        csv = _csv;
        
    }
    
}
