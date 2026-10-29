using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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

//=================================================================================================
/// <summary>
/// 드롭 확률 
/// </summary>
public class DropItemInfo
{
    // 100 기준으로 계산한다.
    public class DropPer
    {
        public float    per;   


        public ItemBase itemBase;

        public int      eq_sc_add;      // 장비 아이템일때 추가 점수 
        public int      eq_sc_minus;    // 장비 아이템일때 감점 


        // 돈 보상일때만 
        public int      gold;       // 돈 보상        

        // 소문일때만 
        public uint     rumor_ID;

        public void Set( float _per , ItemBase item , int _eq_sc_add , int _eq_sc_minus , int _gold , uint _rumor_ID )
        {
            per = _per;
            itemBase = item;
            eq_sc_add = _eq_sc_add;
            eq_sc_minus = _eq_sc_minus;
            gold = _gold;
            rumor_ID = _rumor_ID;
        }
     

        public void Save( BinaryWriter bw )
        {
            bw.Write( per );
            ItemBase.SaveItemBase( bw , itemBase );

            bw.Write( eq_sc_add );
            bw.Write( eq_sc_minus );
            bw.Write( gold );
            bw.Write( rumor_ID );
        }

        public void Load( BinaryReader br )
        {
            per = br.ReadSingle();
            byte item_data = br.ReadByte();
            itemBase = ItemBase.LoadItemBase( br );

            eq_sc_add = br.ReadInt32();
            eq_sc_minus = br.ReadInt32();
            gold = br.ReadInt32();
            rumor_ID = br.ReadUInt32();
        }


    }
    public List<DropPer> dropPers = new();

    public void LoadCSV( Mng_X128SS rd , CSV_DropRewardPer csvDropPer , int level , EnemyRarityGrade rarityGrade , string tagItem = "" )
    {
        int grade_item = GTF_CSV.LevelToGrade( level );

        // csv 의 목록대로 만들어 본다.
        // 확률을 랜덤한다.
        foreach( var s in csvDropPer._DROPs )
        {
            // 소문은 드랍csv 가 아니고, 메이킹에서 직접 넣는다.

            float per = rd.NextFloat( s.per_min , s.per_max );
            if( s.Check_Gold() )
            {
                int gold = GTF_CSV.LevelTo_EnemyRewardGold( level , rarityGrade );
                AddDropPer( per , null , 0 , 0 , gold , 0 );
                continue;
            }

            
            // 여기서는 소모품 , 장비품만 가져오자.
            // csv 드랍에서는 소모품 장비품만 기록하자.
            // 수량정해진 아이템은 나중에 배분 클래스에서 한다.
            // 장비품일 경우 EnemyRarityGrade 에 따라서 보너스 점수를 주자
            _ITEM_TYPE item_type = s.Get_ITEM_TYPE();
            if( item_type != _ITEM_TYPE.None )
            {
                CSV_Item csv_item = GTF_CSV.csv_ItemPage_ALL.GetItemTypeList_TagGrade_One( rd , item_type , tagItem , grade_item );
                ItemBase item = ItemBase.InstItemBase( csv_item );

                // 보너스는 일단 적군 등급을 숫자로 변환 
                AddDropPer( per , item , (int)rarityGrade , 0 , 0 , 0 );
                continue;
            }
        }
    }

    public void AddDropPer(float per , ItemBase item ,int eq_sc_add, int  eq_sc_minus , int gold , uint rumor_ID )
    {
        DropPer s = new();

        s.Set( per , item ,eq_sc_add,  eq_sc_minus , gold , rumor_ID );

        dropPers.Add( s );
    }


    // 여러개가 나올수도 있다.
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

//=================================================================================================
/// <summary>
/// 적군 
/// </summary>
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

    // 적군 csv 태그 , 밖에서 적군 태그는 정해서 넘긴다. 중복방지

    // string tagEnemy : 적군 csv 태그 및 csv 
    public void Make( Mng_X128SS rd , int level , string tagEnemy , CSV_DropRewardPer csv_Drop, EnemyRarityGrade _rarityGrade , int _line_front_back , int num_tagChrAttrStrong , int num_tagChrAttrWeak ,string tagItem )
    {
        // 적군 태그 , 등급 등으로 csv 고르기
        // csv 태그 정의에서 종족 태그
        CSV_CharBaseStat csv = GTF_CSV.csv_CharEnemy.GetTag_Contain_Random( rd , tagEnemy , -1 , _rarityGrade , _line_front_back );

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
        // csv 드랍 테이블에서 가져온다. 
        //CSV_DropRewardPer csv_Drop = GTF_CSV.csv_DropRewardPer.Find_Str( csvIDDropPer ) as CSV_DropRewardPer;
        
        dropItemInfo.LoadCSV( rd , csv_Drop , level , _rarityGrade , tagItem );
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

//=================================================================================================
/// <summary>
/// 필드 아이템
/// </summary>
public class FieldItem
{
    public int field_type; // 일반 보물 상자 , 숨김
    public int Room_Idx;
    public DropItemInfo         dropItemInfo = new();
    public int OpenState;

    public void Save( BinaryWriter bw )
    {
        bw.Write(field_type);
        bw.Write(Room_Idx);
        dropItemInfo.Save(bw);
        bw.Write(OpenState);
    }

    public void Load( BinaryReader br )
    {
        field_type  = br.ReadInt32();
        Room_Idx    = br.ReadInt32();
        dropItemInfo.Load(br);
        OpenState   = br.ReadInt32();
    }
}


//=================================================================================================
/// <summary>
// 던전 한개의 계층 정보
// - 적군 정의 및 등장 확률 
// - 계층 탐색 완성도
/// </summary>
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
    // 개수를 정해야 하는데 던전 구조를 알아야 한다.
    // 일단 보류
    // 그냥 많이 만들고 , 던전 생성시 가져오자.. 남아도 괜찮다.
    public List<FieldItem>  fieldItems = new();


    public float completePer; // 계층 탐색 완성도


    public void Make( int _idx_layer , string _res_dungeon  ,int level , Mng_X128SS rd , string tagPartEnemy , bool isBoss , string strCSV_DropPer , string tagItem )
    {
        idx_layer = _idx_layer;

        res_dungeon_layer = _res_dungeon;

        CSV_DropRewardPer csv_DropPer = GTF_CSV.csv_DropRewardPer.Find_Str(  strCSV_DropPer) as CSV_DropRewardPer;

        // 적군들 , 보스 
        // 적군 종류개수는 csv 전역에서 하자.
        // 일반 전열 , 후열
        int num_enemy_Normal_Front = rd.NextInt( GTF_CSV.csv_Config.makeDungeon_Layer_EnemyNormalFront_Min , GTF_CSV.csv_Config.makeDungeon_Layer_EnemyNormalFront_Max + 1 );
        int num_enemy_Normal_Back  = rd.NextInt( GTF_CSV.csv_Config.makeDungeon_Layer_EnemyNormalBack_Min  , GTF_CSV.csv_Config.makeDungeon_Layer_EnemyNormalBack_Max + 1 );
        int num_enemy_Normal_Elite = rd.NextInt( GTF_CSV.csv_Config.makeDungeon_Layer_EnemyElite_Min , GTF_CSV.csv_Config.makeDungeon_Layer_EnemyElite_Max + 1 );
        int num_enemy_Normal_Rare  = rd.NextInt( GTF_CSV.csv_Config.makeDungeon_Layer_EnemyRare_Min , GTF_CSV.csv_Config.makeDungeon_Layer_EnemyRare_Max + 1 );

        MakeEnemyList( enemy_Normal_Front , rd , level , num_enemy_Normal_Front , tagPartEnemy , csv_DropPer , EnemyRarityGrade.Normal , 1 , 
        0 , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_Normal_Base ,
        0 , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_Normal_Base ,
        tagItem );

        MakeEnemyList( enemy_Normal_Back , rd , level, num_enemy_Normal_Back , tagPartEnemy , csv_DropPer , EnemyRarityGrade.Normal , 2 , 
        0 , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_Normal_Base ,
        0 , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_Normal_Base , 
        tagItem);

        MakeEnemyList( enemy_Elite , rd , level, num_enemy_Normal_Elite , tagPartEnemy , csv_DropPer , EnemyRarityGrade.Elite , 0 , 
        GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_StrongMin_Elite , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_StrongMax_Elite ,
        GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_WeekMin_Elite , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_WeekMax_Elite , tagItem);

        MakeEnemyList( enemy_Rare , rd , level, num_enemy_Normal_Rare , tagPartEnemy , csv_DropPer , EnemyRarityGrade.Rare , 0 , 
        0 , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_Normal_Base ,
        0 , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_Normal_Base , tagItem );

        if( isBoss )
        {
            string tagEnemy = GTF_CSV.csv_TagDefinePage.GetTagPart_StrRandom( tagPartEnemy , rd );
            enemy_Boss = MakeEnemy( rd , level , tagEnemy , csv_DropPer , EnemyRarityGrade.Boss , 0 , 
            GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_StrongMin_Boss , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_StrongMax_Boss , 
            GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_WeekMin_Boss , GTF_CSV.csv_Config.makeDungeon_EnemyChrAttr_WeekMax_Boss  ,tagItem );
        }

        // 필드 아이템
        // 그냥 왕창 만들자.
        for( int i = 0 ; i < 100 ; i++ )
        {
            FieldItem fieldItem = new();
            fieldItem.dropItemInfo.LoadCSV( rd ,csv_DropPer , level , EnemyRarityGrade.Normal , tagItem );
            fieldItems.Add( fieldItem );
        }
    }

    void MakeEnemyList( List<EnemyDungeonInfo> lt_enemy , Mng_X128SS rd  , int level , int total_enemy , string tagPartEnemy , CSV_DropRewardPer csv_DropPer , EnemyRarityGrade _rarityGrade , int _line_front_back , 
        int attr_strong_min , int attr_strong_max , int attr_week_min , int attr_week_max , string tagItem)
    {
        // 태그 파트 ->  [적군 파트] , [csv드랍확률]

        // 적군 대분류 태그에서 적군 태그 여러게 가져와서 여러개 만들기
        List<string> lt_tagEnemy = GTF_CSV.csv_TagDefinePage.GetTagPart_Str( tagPartEnemy );

        for( int i = 0 ; i < total_enemy ; i++ )
        {
            string tagEnemy = rd.RandomList( lt_tagEnemy , true );
            EnemyDungeonInfo enemy = MakeEnemy( rd , level , tagEnemy , csv_DropPer , _rarityGrade , _line_front_back , attr_strong_min , attr_strong_max , attr_week_min , attr_week_max , tagItem );
            lt_enemy.Add(enemy);
        }
    }

    EnemyDungeonInfo MakeEnemy( Mng_X128SS rd , int level , string tagEnemy , CSV_DropRewardPer csv_DropPer , EnemyRarityGrade _rarityGrade , int _line_front_back , 
        int attr_strong_min , int attr_strong_max , int attr_week_min , int attr_week_max , string tagItem )
    {
        EnemyDungeonInfo enemy = new();
        int attr_strong = rd.NextInt( attr_strong_min , attr_strong_max + 1 );
        int attr_week = rd.NextInt( attr_week_min , attr_week_max + 1 );
        enemy.Make( rd , level , tagEnemy , csv_DropPer , _rarityGrade , _line_front_back , attr_strong , attr_week , tagItem );
        return enemy;
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
        foreach( var s in lt )s.Save( bw );
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


//=================================================================================================
/// <summary>
// 레이어 그룹
// 기본 심연 등등 한개의 테마로 한다.
// 또는 기본인데 테마가 여러게 일수도 있다. 
/// </summary>

public class DungeonLayerInfo_Group
{
    public List<DungeonLayerInfo> dungeonLayers = new();

    public void Make( int total_layer , string _res_dungeon  ,int level_start , int level_end , Mng_X128SS rd , string tagPartEnemy , bool isBoss , string strCSV_DropPer , string tagItem)
    {
        for( int i = 0 ; i < total_layer ; i++ )
        {
            int lv = (int)(Mathf.Lerp( level_start , level_end , (float)i / (float)total_layer ));
            DungeonLayerInfo layerInfo = new();

            bool makeBoss = false;
            if( isBoss && i == (total_layer-1) )
            {
                // 보스 존재 및 마지막 레이어라면 보스 메이킹
                makeBoss = true;
            }

            layerInfo.Make( i , _res_dungeon , lv , rd , tagPartEnemy , makeBoss , strCSV_DropPer , tagItem);
            dungeonLayers.Add( layerInfo );
        }
    }

}


//=================================================================================================
/// <summary>
// 던전 전체 정보
/// </summary>
/// 

// 던전 입장 제한 조건 소스 파일을 따로 둔다.

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

    public List<DungeonLayerInfo_Group> layerInfo_Groups = new();


    // 입장 제한
    public DungeonEnterLimit enterLimit = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="total_layer">총 레이어</param>
    /// <param name="_res_dungeon">던전 리소스</param>
    /// <param name="level_start">레벨 시작</param>
    /// <param name="level_end">레벨 종료</param>
    /// <param name="rd">랜덤</param>
    /// <param name="tagPartEnemy">적군태그</param>
    /// <param name="isBoss">보스 여부</param>
    /// <param name="strCSV_DropPer">드랍 확률 정의 태그</param>
    /// <param name="tagItem">아이템 태그</param>
    public void AddMakeLayerGroup(  int total_layer , string _res_dungeon  ,int level_start , int level_end , Mng_X128SS rd , string tagPartEnemy , bool isBoss , string strCSV_DropPer , string tagItem )
    {
        DungeonLayerInfo_Group  layerInfo_Group = new();
        layerInfo_Group.Make( total_layer , _res_dungeon  ,level_start , level_end , rd , tagPartEnemy , isBoss , strCSV_DropPer , tagItem );
    }
}


//=================================================================================================
/// <summary>
// 기타 등등
/// </summary>
/// 
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