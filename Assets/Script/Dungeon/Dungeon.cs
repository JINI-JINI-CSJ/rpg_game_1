using System.Collections.Generic;
using UnityEngine;

// 던전 정보
// 도시 외곽 필드 등도 포함

[System.Serializable]
public class DungeonInfo
{
    public int ID;
    public int randomSeed;
    public bool NO_INCOUNT_BATTLE;

    public int LEVEL;

    public string res_map;

    public DUNGEON_INCOUNT_INF iNCOUNT_INF = new();

    // 적군 등장
    // 일단 5가지 
    // 일반 , 약간 강함 , 강함 , 보스 , 희귀

    // 적군 등장 구분
    public enum _ENEMY_STRONG_TYPE
    {
        None = -1 ,
        Normal , 
        Strong_1 , 
        Strong_2 , 
        Epic ,        
        Boss ,
    }

    // 위의 확률은 전역으로 일단 고정 , 보스는 확률 제외

    // 아이템 드랍
    public class _ITEM_DROP_PER
    {
        public float per;
        public CSV_Item csv_item;
    }


    // 적군 인카운트 구조체 
    public class _ENEMY_INCOUNT_INF
    {
        public _ENEMY_STRONG_TYPE enemy_strong_type;
        public List<CSV_CharBaseStat> csv_enemy = new();
        public List<_ITEM_DROP_PER>   drop_item = new();

    }
    
}
