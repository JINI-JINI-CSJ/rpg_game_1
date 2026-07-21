using UnityEngine;
using UnityEngine.UI;

public class SJ_GameSync_UIImage : SJ_GameSyncStepBase
{
    public Image    image;
    public Text     text;
    public Color    color;
    public bool     show;


    public void PlayStep()
    {
        if( image != null )
        {
            image.color = color;            
            image.gameObject.SetActive(show);
        }

        if( text != null )
        {
            text.color = color;
            text.gameObject.SetActive(show);
        } 

        SJ_SimpleSyncMono.NextPlaySelf();
    }
}
