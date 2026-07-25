using UnityEngine;

public class SyncStep_InputAble : SJ_GameSyncStepBase
{
    public bool Input_TileMapMove;

    public bool Input_UI;


    public void PlayStep()
    {
        GTF_Global.PlayerInputAble( Input_TileMapMove , Input_UI );
        SJ_SimpleSyncMono.NextPlay();
    }
}
