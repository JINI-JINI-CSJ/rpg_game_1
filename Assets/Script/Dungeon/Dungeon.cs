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
    public int                  line_front_back;  
    public string               tagEnemy;       // 종족 태그    
    public int                  csv_id;         // 지정 csv  
    public List<string>         tagChrAttrStrong = new();  // 강약 속성 태그
    public List<string>         tagChrAttrWeak = new();    // 강약 속성 태그

    // 특수 스킬 등등

    // 외모 , 일단은 색깔로 하자. 
    // 각 캐릭터 모델링마다 색깔 바꾸는 메터리얼을 넣자.
    public Color32              color32;
    public DropItemInfo         dropItemInfo = new();

    // 메이킹
    // csv , 컬러 등등 외모
    // 강약점 , 아이템 
    // 아이템은 레어 등급값으로 연동하자.

    // 적군 태그 파트 , 
    public void Make( Mng_X128SS rd , string tagPart , EnemyRarityGrade _rarityGrade , int _line_front_back , int num_tagChrAttrStrong , int num_tagChrAttrWeak )
    {
        // 적군 태그 , 등급 등으로 csv 고르기
        // csv 태그 정의에서 종족 태그

        string tag_enemy = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( tagPart , rd );
        CSV_CharBaseStat csv = GTF_CSV.csv_CharEnemy.GetTag_Contain_Random( rd , tag_enemy , -1 , _rarityGrade , _line_front_back );

        csv_id = csv.ID_int;

        color32.r = (byte)rd.NextInt(0,255);
        color32.g = (byte)rd.NextInt(0,255);
        color32.b = (byte)rd.NextInt(0,255);
        color32.a = 255;

        List<string> tag_chrAttr = GTF_CSV.csv_TagDefinePage.GetTagPart_Str( "CHR_DEF_ATTR" );

        // 강약점 점수만큼 계속 뽑아내기 , 속성개수를 넘치지 않게 인자로 넘긴다.
        for( int i = 0 ; i < num_tagChrAttrStrong ; i++ )   tagChrAttrStrong.Add( rd.RandomList( tag_chrAttr , true ) );
        for( int i = 0 ; i < num_tagChrAttrWeak ; i++ )   tagChrAttrWeak.Add( rd.RandomList( tag_chrAttr , true ) );

        // 아이템 드랍
        // EnemyRarityGrade 으로 랜덤 가져오기 , 태그도 할까?
        // .... 

    }

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
    public int idx_layer;

    public string res_dungeon_layer;

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


    public void Make( int _idx_layer , Mng_X128SS rd , string tagPart_enemy , bool isBoss )
    {
        // 적군들 , 보스 

        // 적군 종류개수는 csv 전역에서 하자.

        // 일반 전열 , 후열
        int num_enemy_Normal_Front = 1;


        // 아이템 

    }

    public void Save( BinaryWriter bw )
    {
        bw.Write( idx_layer );
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
        idx_layer = br.ReadInt32();

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
    public DUNGEON_TYPE dUNGEON_TYPE;
    public SpotData spotData;
    public uint randomSeed = 12345;
    public int grade;
    public int LEVEL;
    public string res_map;

    // 던전 레이어들 정보
    public List<DungeonLayerInfo> layerInfos = new();


    // 초회 기본 탐사 완료 보상
    // 드랍 클래스로 일단 정의
    //public DropItemInfo dropItemInfo_BaseComplete = new();
    // 초회 심연 탐사 완료 보상
    //public DropItemInfo dropItemInfo_DeepComplete = new();
    // 보상은 각 층에서 할까?

    // 메이킹
    // 그룹 레이어 메이킹 정보 , 레벨을 제외하고 메이킹정보

}

public class MAKE_DUNGEON_LAYER_GROUP_INF
{
    // 리소스 태그
    public string tagPart_res_Dungeon;      // 메인 던전 리소스의 대분류 태그 "DUNGEON_RES_NORMAL" 등등
    public string tagPart_res_Enemy;        // 적군 리소스
    public string tagPart_res_FieldItem;    // 필드 아이템 리소스

    // 레이어 층수 랜덤 , 같거나 min 만 있으면 그것
    public int layer_num_random_max;


    // 위의 인자들은 미리 세팅하고 함수 호출하자.
    public List<DungeonLayerInfo> MakingLayers( Mng_X128SS rd )
    {
        List<DungeonLayerInfo> layerInfos = new();

        // 레이어 던전 리소스는 같게
        // csv 태그 정의에서 랜덤
        string res_dungeon = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( tagPart_res_Dungeon , rd );

        int total_layer = 1;
        if( layer_num_random_max > 1 )
        {
            total_layer = rd.NextInt(1,layer_num_random_max+1);
        }

        // 레이어 수만큼 만들기
        // 보스는 마지막 레이어만
        for( int i = 0 ; i < total_layer ; i++ )
        {
            
        }

        return layerInfos;   
    }
    
}


// 던전내의 특정 위치 좌표
public class COORD_IN_DUNGEON
{
    // 던전 아이디
    public uint ID;
    // 던전 레이어 인덱스
    public int idx_layer;
    // 좌표
    public Vector2Int pos;
}