using UnityEngine;

// SJ_SimpleSyncMono 클래스 기준으로 구현


public class SJ_GameSyncStepBase : MonoBehaviour
{
    [Multiline]
    public string DESC;
    

    public SJ_COMMON.Func_VOID func_end;

    public void OnEndFunc()
    {
        if( func_end != null ) func_end.Invoke();
        else SJ_SimpleSyncMono.NextPlay();  
    }
}
