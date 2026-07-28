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

    static public bool FirstPlay()
    {
        return true;
    }
}
