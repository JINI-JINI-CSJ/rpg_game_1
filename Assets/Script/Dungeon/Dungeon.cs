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
// 등급은 일단 던전 등급을 참조 
public class DropItemInfo
{
    // 100 기준으로 계산한다.
    public class DropPer
    {
        public int      csv_id;     // 지정 csv
        public int      power_step; // 강화 단계 , 밑의 보너스 스탯과 별개
        public float    per;
        public string   tagItem;    // 태그 아이템 중에 한개
        public int      gold;       // 돈 보상
        // 보너스 스탯이 있을경우
        public int      sc_bonus; // 나중에 이걸 다시 추가 강화와 추가 이펙트 수치로 나누기 , 초과 하는건 무시

        public void Save( BinaryWriter bw )
        {
            bw.Write( csv_id );
            bw.Write( power_step );
            bw.Write( per );
            bw.Write( tagItem );
            bw.Write( gold );
            bw.Write( sc_bonus );
        }

        public void Load( BinaryReader br )
        {
            csv_id = br.ReadInt32();
            power_step = br.ReadInt32();
            per = br.ReadSingle();
            tagItem = br.ReadString();
            gold = br.ReadInt32();
            sc_bonus = br.ReadInt32();
        }

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
            s.Save(bw);
        }
    }

    public void Load( BinaryReader br )
    {
        int count = br.ReadInt32();
        for( int i = 0 ; i < count ; i++ )
        {
            DropPer s = new();
            s.Load( br );
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

    // 적군 정보 있는지 , 둘다 없으면 적군 정보 없음
    public bool CheckEnemyInf()
    {
        if( csv_id != 0 || string.IsNullOrEmpty( tagEnemy ) ) return true;
        return false;
    }

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

// 필드 아이템 
// DropItemInfo 를 하나 가지고 있고 , 무조건 이 아이템 획득하기
public class FieldItem
{
    public int field_type; // 일반 보물 상자 , 숨김
    public int Room_Idx;
    DropItemInfo.DropPer dropItem = new();
    public int OpenState;

    public void Save( BinaryWriter bw )
    {
        bw.Write(field_type);
        bw.Write(Room_Idx);
        dropItem.Save(bw);
        bw.Write(OpenState);
    }

    public void Load( BinaryReader br )
    {
        field_type  = br.ReadInt32();
        Room_Idx    = br.ReadInt32();
        dropItem.Load(br);
        OpenState   = br.ReadInt32();
    }
}


// 던전 한개의 계층 정보
// - 적군 정의 및 등장 확률 
// - 계층 탐색 완성도
public class DungeonLayerInfo
{
    public int layer;

    // 계층의 규모 정도
    public int dungeonSize;
    // 적군
    public List<EnemyDungeonInfo> enemy_Normal_Front = new(); // 일반 등급 전열
    public List<EnemyDungeonInfo> enemy_Normal_Back = new(); // 일반 등급 후열
    public List<EnemyDungeonInfo> enemy_Elite = new(); // 정예 등급
    public List<EnemyDungeonInfo> enemy_Rare = new(); // 희소 등급
    public EnemyDungeonInfo enemy_Boss = new(); // 보스 등급
    // 필드 아이템
    public List<FieldItem>  fieldItems = new();



    public float completePer; // 계층 탐색 완성도

    public void Save( BinaryWriter bw )
    {
        bw.Write( layer );
        bw.Write( dungeonSize );        

        SaveEnemyList( bw, enemy_Normal_Front );
        SaveEnemyList( bw, enemy_Normal_Back );
        SaveEnemyList( bw, enemy_Elite );
        SaveEnemyList( bw, enemy_Rare );
        enemy_Boss.Save( bw );

        bw.Write( fieldItems.Count );
        foreach( var s in fieldItems )
        {
            s.Save( bw );
        }

        bw.Write( completePer );        
    }

    void SaveEnemyList( BinaryWriter bw , List<EnemyDungeonInfo> lt )
    {
        bw.Write( lt.Count );
        foreach( var s in lt )
        {
            s.Save( bw );
        }
    }

    public void Load( BinaryReader br )
    {
        layer = br.ReadInt32();

        dungeonSize = br.ReadInt32();
        LoadEnemyList( br, enemy_Normal_Front );
        LoadEnemyList( br, enemy_Normal_Back );
        LoadEnemyList( br, enemy_Elite );
        LoadEnemyList( br, enemy_Rare );
        enemy_Boss.Load( br );

        int count = br.ReadInt32();
        for( int i = 0 ; i < count ; i++ )
        {
            FieldItem s = new();
            s.Load( br );
            fieldItems.Add( s );
        }

        completePer = br.ReadSingle();        
    }

    void LoadEnemyList( BinaryReader br , List<EnemyDungeonInfo> lt )
    {
        int count = br.ReadInt32();
        for( int i = 0 ; i < count ; i++ )
        {
            EnemyDungeonInfo s = new();
            s.Load( br );
            lt.Add( s );
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
