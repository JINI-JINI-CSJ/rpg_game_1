using System.Collections.Generic;
using UnityEngine;
using WorldForge;

// csv 에서 아이템 태그별로 등급을 해놓자.
public class DropItemInfo
{
    // 100 기준으로 계산한다.
    public class DropPer
    {
        public int      csv_id;     // 지정 csv

        public int      power_step; // 강화 단계 , 밑의 보너스 스탯과 별개

        public float    per;

        public string   tagItem;    // 태그 아이템 중에 한개

        
        // 보너스 스탯이 있을경우
        public int      sc_bonus; // 나중에 이걸 다시 추가 강화와 추가 이펙트 수치로 나누기 , 초과 하는건 무시
    }
    public List<DropPer> dropPers = new();

    public List<DropPer> PerItemDrop( Mng_X128SS rd )
    {
        // 100 기준 각자 계산한다.
        List<DropPer> lt = new();
        foreach( var s in lt )
        {
            if( rd.RandomFloat_Per( s.per , 100 ) ) lt.Add( s );
        }
        return lt;
    }
}

// 던전의 개별적군 정의
public class EnemyDungeonInfo
{
    public int          rarityGrade;    // 희소등급(강함 등급이 아님) : 0 일반 , 1 강함 , 2 희소 , 3 보스 
    public int          csv_id;         // 지정 csv  
    public string       tagEnemy;       // 종족 태그
    public string       tagMagicProp;   // 마법 속성
    public DropItemInfo dropItemInfo;
}

// 던전 정보
// 도시 외곽 필드 등도 포함
[System.Serializable]
public class DungeonInfo
{
    public uint ID;

    public SpotData spotData;

    public int randomSeed = 12345;
    public int LEVEL;
    public string res_map;
    public DUNGEON_INCOUNT_INF iNCOUNT_INF = new();

    // 던전 계층
    public int MAX_LAYER;

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
