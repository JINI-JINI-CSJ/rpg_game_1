using UnityEngine;
using UnityEngine.Video;

public class Player
{
    static public bool loaded;    
    static public PlayerSaveFile    saveFile = new();
    static public BattleParty       battleParty = new();
    static public CharBase          char_Hero; // 주인공 캐쉬
    static public PlayerInventory   inventory = new();


    static public int TURN_WORLD = 1;


    static public void LoadUserFile()
    {
        if( loaded )return;
        loaded = true;
        saveFile.Load();
    }

    static public void SaveUserFile()
    {
        saveFile.Save();
    }

    // 최초 인트로 씬에서 실행
    // csv 1 번으로 캐릭터 만들고 , 전열 1에 등록
    static public bool FirstPlay()
    {
        // 주인공 캐릭터
        char_Hero = CharBase.InstCharBase_CSV( 1 , 1 , _ARMY_FORCE.Player );
        battleParty.Add( 0 , char_Hero );

        // 기본 무기 장착
        ItemBase item_1 = inventory.Add_OneItem( 1 );
        char_Hero.Add_EquipItem( item_1 );

        return true;
    }


}
