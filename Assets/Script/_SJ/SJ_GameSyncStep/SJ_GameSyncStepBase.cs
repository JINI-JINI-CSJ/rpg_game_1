using UnityEngine;

// SJ_SimpleSyncMono 클래스 기준으로 구현


public class SJ_GameSyncStepBase : MonoBehaviour
{
    [Multiline]
    public string DESC;
    

    public SJ_COMMON.Func_VOID func_endStep;

    public void OnEndFunc()
    {
        if( func_endStep != null ) func_endStep.Invoke();
        else SJ_SimpleSyncMono.NextPlay();  
    }
}
