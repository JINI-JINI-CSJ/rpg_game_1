using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Converters;
using UnityEngine;

// 배틀 파티 구성
// 전열 후열 구성
public class BattleParty
{
    public CharBase[] chars_Front;
    public CharBase[] chars_Back;
    public List<ENEMY_INFO> enemy_make_inf = new();

    public BattleParty()
    {
        chars_Front = new CharBase[GTF_Define.BATTLE_LINE_NUM];
        chars_Back = new CharBase[GTF_Define.BATTLE_LINE_NUM];

        for( int i = 0 ; i < GTF_Define.BATTLE_LINE_NUM ; i++ )
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

    public List<CharBase> GetLineLive( int front_back )
    {
        List<CharBase> lt = new();
        foreach( var s in GetLine(front_back) )
        {
            if( s != null && s.IsLive() ) lt.Add(s);
        }
        return lt;
    }


    public List<CharBase> GetALL( bool live = true)
    {
        List<CharBase> chars = new();
        foreach( var s in chars_Front ) if( s != null && (s.IsLive() || live == false) ) chars.Add(s);
        foreach( var s in chars_Back ) if( s != null && (s.IsLive() || live == false) ) chars.Add(s);
        return chars;
    }

    public List<CharBase> GetBattleLive( bool front = true , bool back = true , bool live = true )
    {
        List<CharBase> chars = new();
        if( front )
        {
            foreach( var s in chars_Front )
            {
                if( s != null && (s.IsLive() || live == false) ) chars.Add(s);
            }            
        }

        if( back )
        {
            foreach( var s in chars_Back )
            {
                if( s != null && (s.IsLive() || live == false) ) chars.Add(s);
            }
            
        }
        return chars;
    }

    // 
    public List<CharBase> GetBattleCommandAble_Ready()
    {
        List<CharBase> chars = new();

        foreach( var s in chars_Front ) if( s != null && s.AbleBattleCommand_Ready() ) chars.Add(s);
        foreach( var s in chars_Back )  if( s != null && s.AbleBattleCommand_Ready() ) chars.Add(s);
        return chars;
    }

    public bool CheckLiveALL()
    {
        if( GetALL().Count > 0 ) return true;
        return false;
    }

    public int CountLiveALL()
    {
        return GetALL().Count;
    }

    // 랜덤으로 전열 , 후열에서 1캐릭 선택
    public CharBase GetRandomLive()
    {
        for( int i = 0 ; i < 2 ; i++ )
        {
            List<CharBase> lt = GetLineLive(i);
            if( lt.Count < 1 ) continue;
            if( lt.Count == 1 ) return lt[0];
            return GTF_TileMap.Random().RandomList( lt );
        }
        return null;
    }
    

    // 
    public void Add_MakeEnemyInfo_One( Mng_X128SS rd , int _level , string _tagEnemy , CSV_DropRewardPer csv_Drop, EnemyRarityGrade _rarityGrade , int _line_front_back , int num_tagChrAttrStrong , int num_tagChrAttrWeak ,string tagItem )
    {
        ENEMY_INFO enemy_inf = new();
        enemy_inf.Make( rd , _level , _tagEnemy , csv_Drop, _rarityGrade , _line_front_back , num_tagChrAttrStrong , num_tagChrAttrWeak ,tagItem );
    }

    public void Add_MakeEnemyInfo_One( Mng_X128SS rd , int _level , string _tagEnemy , string _tagDropPer , EnemyRarityGrade _rarityGrade , int _line_front_back , int num_tagChrAttrStrong , int num_tagChrAttrWeak ,string tagItem )
    {
        ENEMY_INFO enemy_inf = new();
        enemy_inf.Make( rd , _level , _tagEnemy , _tagDropPer, _rarityGrade , _line_front_back , num_tagChrAttrStrong , num_tagChrAttrWeak ,tagItem );
    }

    public void Add_MakeEnemyInfo_One_RdStrongWeek( Mng_X128SS rd , int _level , string _tagEnemy , string _tagDropPer , EnemyRarityGrade _rarityGrade , int _line_front_back , int max_rd_tagChrAttrStrong , int max_rd_tagChrAttrWeak ,string tagItem )
    {
        ENEMY_INFO enemy_inf = new();
        enemy_inf.Make( rd , _level , _tagEnemy , _tagDropPer, _rarityGrade , _line_front_back , max_rd_tagChrAttrStrong , max_rd_tagChrAttrWeak ,tagItem );
    }

    // 간단 만들기 , 테마 문자열 1개 (적군 , 드랍 , 아이템 전부 한개 태마) , 
    // 인원수도 "일반 등급" 이면 대략 정한다. 
    public void MakeEnemyList_Simple( Mng_X128SS rd , int _level  , string tagTheme , EnemyRarityGrade _rarityGrade )
    {
        switch( _rarityGrade )
        {
            case EnemyRarityGrade.Normal:
            case EnemyRarityGrade.Rare:
                {
                    // 일반과 레어는 전후열 나누어서 만들기 , 각 라인 최대 3 개체
                    // 강약 개수 최대 2가지
                    int front = rd.NextInt(1 , GTF_Define.BATTLE_LINE_NUM+1);
                    for( int i = 0 ; i < front ; i++ )
                    {
                        Add_MakeEnemyInfo_One_RdStrongWeek( rd , _level , tagTheme , tagTheme , _rarityGrade , 1 , 2 , 2 , tagTheme );
                    }

                    int back = rd.NextInt(1 , GTF_Define.BATTLE_LINE_NUM+1);
                    for( int i = 0 ; i < back ; i++ )
                    {
                        Add_MakeEnemyInfo_One_RdStrongWeek( rd , _level , tagTheme , tagTheme , _rarityGrade , 2 , 2 , 2 , tagTheme );
                    }
                }
                break;
            case EnemyRarityGrade.Elite:
                {
                    // 엘리트 최대 3
                    // 강점 최대  3 , 약점 최대 2
                    int elite = rd.NextInt(1 , GTF_Define.BATTLE_LINE_NUM+1);
                    for( int i = 0 ; i < elite ; i++ )
                    {
                        Add_MakeEnemyInfo_One_RdStrongWeek( rd , _level , tagTheme , tagTheme , _rarityGrade , 0 , 3 , 2 , tagTheme );
                    }
                }
                break;

            case EnemyRarityGrade.Boss:
                {
                    // 보스급 무조건 1개체
                    // 강점 최대 5 개 , 약점 0
                    Add_MakeEnemyInfo_One_RdStrongWeek( rd , _level , tagTheme , tagTheme , _rarityGrade , 0 , 5 , 0 , tagTheme );
                }
                break;
        }


    }


    public void Save( BinaryWriter bw )
    {
        // 직접 할당이면 캐릭터 객체 전부 저장
        // 아니면 유니크 아이디만 저장
    }

    public void Load( BinaryReader br )
    {
        
    }
}
