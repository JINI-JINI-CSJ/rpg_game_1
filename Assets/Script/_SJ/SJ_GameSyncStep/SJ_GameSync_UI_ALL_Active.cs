using UnityEngine;

public class SJ_GameSync_UI_ALL_Active : SJ_GameSyncStepBase
{
    public bool active;

    public void PlayStep()
    {
        SJ_UnityUIMng_Curve.ALL_Active( active );
        SJ_SimpleSyncMono.NextPlaySelf();
    }
}
