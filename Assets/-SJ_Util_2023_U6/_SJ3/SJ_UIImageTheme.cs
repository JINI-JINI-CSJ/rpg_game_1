using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SJ_UIImageTheme : MonoBehaviour
{
    public bool root;

    public Sprite Reg_bgFrame;
    public Sprite Reg_Button;
    public Sprite Reg_ToggleBG;
    public Sprite Reg_ToggleOn;
    public Sprite Reg_TitleBar;


    public List<Image> img_bgFrame;
    public List<Image> img_Button;
    public List<Image> img_Toggle_BG;
    public List<Image> img_Toggle_On;
    public List<Image> img_TitleBar;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void Change_Unit( Sprite spr , List<Image> images )
    {
        foreach( var s in images )
        {
            s.sprite = spr;
        }
    }

    public void Change( SJ_UIImageTheme root )
    {
        Change_Unit( root.Reg_bgFrame , img_bgFrame );
        Change_Unit( root.Reg_Button  , img_Button );
        Change_Unit( root.Reg_ToggleBG, img_Toggle_BG );
        Change_Unit( root.Reg_ToggleOn, img_Toggle_On );
        Change_Unit( root.Reg_TitleBar, img_TitleBar );
    }

    [ContextMenu("ChangeUITheme")]
    public void ChangeUITheme()
    {
        SJ_UIImageTheme[] sJ_UIImages = GetComponentsInChildren<SJ_UIImageTheme>();
        foreach( var s in sJ_UIImages )
        {
            if( s != this )
            {
                s.Change( this );
            }
        }
    }
}
