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
        if( _chr == null ) return;

        gameObject.SetActive(true);
        charBase = _chr;
        
        charBase.func_ANI_ATK = ANI_ATK;
        charBase.func_ANI_Damage = ANI_GetDamage;
        charBase.func_RecvSkill = Call_RecvSkill;

        SJ_UnityUI_Util.Image_Load( image , "2D/PLAYER/FACE/" + charBase.csv.res );
        SJ_UnityUI_Util.TextString( text_name , charBase.csv.name );
        UpdateUI();
    }

    public void UpdateUI()
    {
        gage_HP.SetValue( charBase.cur_HP , charBase.cur_HP );
        gage_MP.SetValue( charBase.cur_MP , charBase.cur_MP );
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
        charBase.OnEnd_TurnAction();
    }

    // 피격 애니
    // 빨강게 반짝이면서 흔들림
    public void ANI_GetDamage()
    {
        curve_ani_Damage.StartFunc_FWD();
        shakeEffect.Shake();
    }

    public void Call_RecvSkill( object skl_obj )
    {
        SkillBase skill = skl_obj as SkillBase;
        skill.OnViewEffect_Player( gameObject );
    }
}
