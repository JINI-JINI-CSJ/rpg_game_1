using System.Collections.Generic;
using UnityEngine;

public class SJ_GameSync_FuncExec : SJ_GameSyncStepBase
{

    [System.Serializable]
    public class FUNC
    {
        public MonoBehaviour    go;
        public string           strFunc;        
        public void Exec()
        {
            SJ_CSharpUtil.CallStrFunc( go , strFunc );
        }
    }

    public List<FUNC> fUNCs;

    public void PlayStep()
    {
        foreach( var s in fUNCs )
        {
            s.Exec();
        }

        SJ_SimpleSyncMono.NextPlaySelf();
    }
}
