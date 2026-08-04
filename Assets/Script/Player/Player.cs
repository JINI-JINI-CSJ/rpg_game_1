using UnityEngine;
using UnityEngine.Video;

public class Player
{
    static public PlayerSaveFile saveFile = new();

    static public bool loaded;

    static public BattleParty battleParty = new();

    static public void LoadUserFile()
    {
        if( loaded )return;
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
        CharBase charBase = new();
        

        return true;
    }
}
