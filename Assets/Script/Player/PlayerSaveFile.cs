using UnityEngine;


public class PlayerSaveFile
{
    // 0 : 완전 처음 후원자 선택
    // 1 : 처음 숲 던전
    // 2 : 처음 도시 시작
    public int firstStep;

    public void Save()
    {
        
    }

    public void Load()
    {
        
    }

    public void SetFirstPlay_Step( int n )
    {
        firstStep = n;
    }

    public int GetFirstPlay_Step(){return firstStep;}

}
