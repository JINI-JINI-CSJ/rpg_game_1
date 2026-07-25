using UnityEngine;
using UnityEngine.UI;

public class SyncStep_FirstUIBlack : SJ_GameSyncStepBase
{
    public Color    color;
    public bool     show;
    public void PlayStep()
    {
        Image img = GTF_Global.G.img_FirstBlack;
        img.color = color;
        img.gameObject.SetActive(show);
    }
}
