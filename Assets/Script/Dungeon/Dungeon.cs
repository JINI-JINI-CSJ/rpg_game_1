using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WorldForge;

// 적군 등급 enum : 일반 , 정예 , 희소 , 보스
public enum EnemyRarityGrade
{
    None  = 0,
    Normal = 1,
    Elite = 2,
    Rare = 3,
    Boss = 4,
}

// csv 에서 아이템 태그별로 등급을 해놓자.
// 돈이나 exp 보상은 따로 일괄 계산 , 적군 등급 , 던전 등급 등등
public class DropItemInfo
{
    // 100 기준으로 계산한다.
    public class DropPer
    {
        public int      csv_id;     // 지정 csv
        public int      power_step; // 강화 단계 , 밑의 보너스 스탯과 별개
        public float    per;
        public string   tagItem;    // 태그 아이템 중에 한개

        // 등급은 일단 던전 등급을 참조 

        // 보너스 스탯이 있을경우
        public int      sc_bonus; // 나중에 이걸 다시 추가 강화와 추가 이펙트 수치로 나누기 , 초과 하는건 무시
    }
    public List<DropPer> dropPers = new();

    public void AddDropPer( int csv_id , int power_step , float per , string tagItem , int sc_bonus )
    {
        DropPer s = new();
        s.csv_id = csv_id;
        s.power_step = power_step;
        s.per = per;
        s.tagItem = tagItem;
        s.sc_bonus = sc_bonus;

        dropPers.Add( s );
    }

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

    public void Save( BinaryWriter bw )
    {
        bw.Write( dropPers.Count );
        foreach( var s in dropPers )
        {
            bw.Write( s.csv_id );
            bw.Write( s.power_step );
            bw.Write( s.per );
            bw.Write( s.tagItem );
            bw.Write( s.sc_bonus );
        }
    }

    public void Load( BinaryReader br )
    {
        int count = br.ReadInt32();
        for( int i = 0 ; i < count ; i++ )
        {
            DropPer s = new();
            s.csv_id = br.ReadInt32();
            s.power_step = br.ReadInt32();
            s.per = br.ReadSingle();
            s.tagItem = br.ReadString();
            s.sc_bonus = br.ReadInt32();

            dropPers.Add( s );
        }
    }
}

// 던전의 개별적군 정의
public class EnemyDungeonInfo
{
    public EnemyRarityGrade     rarityGrade;    // 
    public int                  csv_id;         // 지정 csv  
    public string               tagEnemy;       // 종족 태그
    public List<string>         tagChrAttrStrong = new();  // 강약 속성 태그
    public List<string>         tagChrAttrWeak = new();    // 강약 속성 태그
    public DropItemInfo         dropItemInfo = new();

    public void Save( BinaryWriter bw )
    {
        bw.Write( (int)rarityGrade );
        bw.Write( csv_id );
        bw.Write( tagEnemy );
        bw.Write( tagChrAttrStrong.Count );
        foreach( var s in tagChrAttrStrong )
        {
            bw.Write( s );
        }
        bw.Write( tagChrAttrWeak.Count );
        foreach( var s in tagChrAttrWeak )
        {
            bw.Write( s );
        }

        dropItemInfo.Save( bw );
    }

    public void Load( BinaryReader br )
    {
        rarityGrade = (EnemyRarityGrade)br.ReadInt32();
        csv_id = br.ReadInt32();
        tagEnemy = br.ReadString();
        int count = br.ReadInt32();
        for( int i = 0 ; i < count ; i++ )
        {
            tagChrAttrStrong.Add( br.ReadString() );
        }
        count = br.ReadInt32();
        for( int i = 0 ; i < count ; i++ )
        {
            tagChrAttrWeak.Add( br.ReadString() );
        }

        dropItemInfo.Load( br );
    }
}

// 던전 한개의 계층 정보
// - 적군 정의 및 등장 확률 
// - 계층 탐색 완성도
public class DungeonLayerInfo
{
    public int layer;
    public List<EnemyDungeonInfo> enemyDungeonInfos = new();

    // 계층의 규모 정도 , 0~5 정도
    public int dungeonSize;

    public float completePer; // 계층 탐색 완성도

    public void Save( BinaryWriter bw )
    {
        bw.Write( layer );
        bw.Write( completePer );
        bw.Write( dungeonSize );
        bw.Write( enemyDungeonInfos.Count );
        foreach( var s in enemyDungeonInfos )
        {
            s.Save( bw );
        }
    }

    public void Load( BinaryReader br )
    {
        layer = br.ReadInt32();
        completePer = br.ReadSingle();
        dungeonSize = br.ReadInt32();
        int count = br.ReadInt32();
        for( int i = 0 ; i < count ; i++ )
        {
            EnemyDungeonInfo s = new();
            s.Load( br );
            enemyDungeonInfos.Add( s );
        }
    }
}


// 던전 전체 정보

[System.Serializable]
public class DungeonInfo
{
    public uint ID;

    public SpotData spotData;

    public int randomSeed = 12345;
    public int grade;
    public int LEVEL;
    public string res_map;

    // 던전 레이어들 정보
    public List<DungeonLayerInfo> layerInfos = new();

    // 초회 기본 탐사 완료 보상
    // 드랍 클래스로 일단 정의
    public DropItemInfo dropItemInfo_BaseComplete = new();

    // 초회 심연 탐사 완료 보상
    public DropItemInfo dropItemInfo_DeepComplete = new();
}
