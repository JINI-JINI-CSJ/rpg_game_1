using UnityEngine;


public class PlayerSaveFile
{
    static public PlayerSaveFile G;

    // 0 : 최초 
    // 1 : 처음 인트로 여신 대화 끝
    // 2 : 촌락 대화 끝
    public int firstStep;

    public PlayerSaveFile()
    {
        G = this;
    }

    public void _Save()
    {
        
    }

    public void _Load()
    {
        
    }

    static public void Save()
    {
        G?._Save();
    }

    static public void Load()
    {
        G?._Load();
    }

    public void _SetFirstPlay_Step( int n )
    {
        firstStep = n;
    }

    static public void SetFirstPlay_Step( int n )
    {
        G?._SetFirstPlay_Step(n);
    }

    public int _GetFirstPlay_Step(){return firstStep;}

    static public int GetFirstPlay_Step(){return G._GetFirstPlay_Step();}
}
