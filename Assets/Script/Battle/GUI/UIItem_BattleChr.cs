using UnityEngine;
using UnityEngine.UI;

public class UIItem_BattleChr : MonoBehaviour
{
    public CharBase         charBase;
    public Image            image;
    public Text             text_name;
    public SJ_GageBarText   gage_HP;
    public SJ_GageBarText   gage_MP;



    // 현재 명령어 입력 대기
    public GameObject               go_AniCursor;
    // 공격 애니
    public SJ_Curve_TransObjToggle  curve_ani_ATK;
    // 피격 애니
    public SJ_Curve_TransObjToggle  curve_ani_Damage;
    public Cld_ShakeEffect          shakeEffect;    

    public void Clear()
    {
        charBase = null;
        gameObject.SetActive(false);
    }

    public void SetChar( CharBase _chr )
    {
        gameObject.SetActive(true);
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
        go_AniCursor.SetActive(b);
    }

    // 행동 애니 
    public void ANI_ATK()
    {
        curve_ani_ATK.func_OnEnd = ANI_ATK_End;
        curve_ani_ATK.StartFunc_FWD();
    }

    public void ANI_ATK_End()
    {
        SJ_SimpleSync.Next( BattleTurn.BATTLE_SYNC_ACTION );
    }

    // 피격 애니
    // 빨강게 반짝이면서 흔들림
    public void ANI_GetDamage()
    {
        curve_ani_Damage.StartFunc_FWD();
        shakeEffect.Shake();
    }
}
