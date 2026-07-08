using System.Collections.Generic;
using UnityEngine;

// ObjActive
public class SJ_GameSync_ObjActive : SJ_GameSyncStepBase
{
    public bool show;
    public List<GameObject> obj_show;

    public bool hide;
    public List<GameObject> obj_hide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayStep()
    {
        foreach( var s in obj_show ) s.SetActive( show );
        foreach( var s in obj_hide ) s.SetActive( hide );
        SJ_SimpleSyncMono.NextPlaySelf();
    }
}
