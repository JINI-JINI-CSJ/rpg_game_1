using UnityEngine;

public class SJ_GameSync_OpenPanel : SJ_GameSyncStepBase
{
    public string strPanel;

    public bool use_SJ_CurveToggle;

    public bool open;

    public bool Start_Next;

    public void PlayStep()
    {
        if( use_SJ_CurveToggle )
        {
            SJ_UnityUIMng_Curve.Open( strPanel );
        }
        else
        {
            SJ_UnityUIMng.OpenPopup( strPanel );
        }

        if( Start_Next ) SJ_SimpleSyncMono.NextPlaySelf();
    }


}
