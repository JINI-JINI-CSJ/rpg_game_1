using System.IO;
using UnityEngine;

public class Mission_DefeatEnemy : MissionBase
{
    public int csv_Enemy;
    public uint DUNGEON_ID; 

    override public void OnCreateMission()
    {
        // 캐릭터(보스) 정하기
        // 종족 (주) , 성향?
        
    }

    override public void OnLoad( BinaryReader br ){}
    override public void OnSave( BinaryWriter bw ){}

    override public void OnAfterLoad()
    {
        
    }
}
