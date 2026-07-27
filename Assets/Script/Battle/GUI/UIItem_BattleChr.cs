using UnityEngine;
using UnityEngine.UI;

public class UIItem_BattleChr : MonoBehaviour
{
    public CharBase         charBase;
    public Image            image;
    public Text             text_name;
    public SJ_GageBarText   gage_HP;
    public SJ_GageBarText   gage_MP;

    public Cld_ShakeEffect  shakeEffect;

    public void SetChar( CharBase _chr )
    {
        charBase = _chr;
        SJ_UnityUI_Util.Image_Load( image , "2D/PLAYER/FACE/" + charBase.csv.res );
        SJ_UnityUI_Util.TextString( text_name , charBase.csv.name );
        UpdateUI();
    }

    public void UpdateUI()
    {
        gage_HP.SetValue( charBase.cur_HP , charBase.csv.charPrcValue.HP );
        gage_HP.SetValue( charBase.cur_MP , charBase.csv.charPrcValue.MP );
    }

    // 커맨드 입력 알림
    // 발광 애니
    public void Active_CommandInput( bool b )
    {
        
    }

    // 행동 애니 
    public void ANI_Action()
    {
        
    }

    // 피격 애니
    // 빨강게 반짝이면서 흔들림
    public void ANI_GetDamage()
    {
        
    }
}
