using UnityEngine;

public class SyncStep_DungeonUI_Show : SJ_GameSyncStepBase
{
    public bool show;

    public void PlayStep()
    {
        InGame.ShowUIDungeon( show );
    }
}
