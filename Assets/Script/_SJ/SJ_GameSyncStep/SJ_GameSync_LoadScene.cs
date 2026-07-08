using UnityEngine;

public class SJ_GameSync_LoadScene : SJ_GameSyncStepBase
{
    public string SceneName;
    public void PlayStep()
    {
        SJ_UILoadingScene.LoadScene( SceneName );
    }
}
