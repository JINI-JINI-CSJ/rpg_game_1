using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 적군 캐릭터 액션 배치 포지션
public class BattleEnemyObjView : MonoBehaviour
{
    public int IDX; // 본인 인덱스 , 타겟 마크 같은거 찾을때 

    public Transform tr_TargetMarkPos;

    public CharBase charBase;

    // 2디 이미지 일경우
    // 3디 캐릭이면 스프라이트 감춘다.
    public SpriteRenderer spriteRenderer;

    public SJ_Curve_Color_Mono  spr_curve_atk;
    public SJ_Curve_Color_Mono  spr_curve_damage;
    public SJ_Curve_Color_Mono  spr_curve_ko;
    public Cld_ShakeEffect      shakeEffect_damage;
    

    public float Time_ATK = 0.5f;

    Image       image_targetMark;

    Animator    anit;
    EnemyMono   enemyMono;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        spriteRenderer.color = Color.white;
        charBase = null;
        if( enemyMono != null )GameObject.DestroyImmediate(enemyMono.gameObject);
        anit = null;
        gameObject.SetActive(false);
    }

    public void InitCharBase( CharBase chr )
    {
        if( chr == null ) return;

        if( image_targetMark == null )
        {
            image_targetMark = Panel_BattleMain.G.enemy_target_marks[IDX];
            SJ_UnityUIMng.WorldToUI( image_targetMark.transform , tr_TargetMarkPos );
            image_targetMark.gameObject.SetActive(false);
        }

        charBase = chr;
        charBase.func_ANI_ATK = ANI_ATK;
        charBase.func_ANI_Damage = ANI_GetDamage;
        charBase.func_RecvSkill = Call_RecvSkill;
        charBase.func_SelectTarget = Call_SelectTarget;


        // 2D , 3D 
        if( string.IsNullOrEmpty( chr.csv.res ) == false )
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = SJ_ResPoolSys.GetResObjs_PathName_Sprite( chr.csv.res );
        } 
        else if( string.IsNullOrEmpty( chr.csv.res3D ) == false )
        {
            spriteRenderer.enabled = false;
            GameObject load_3D = SJ_ResPoolSys.Inst_Obj( chr.csv.res3D );
            SJ_Unity.SetEqTrans( load_3D.transform , null , transform );
            anit = load_3D.GetComponentInChildren<Animator>();
            enemyMono = load_3D.GetComponent<EnemyMono>();
        }
        gameObject.SetActive(true);
    }

    public void ANI_ATK()
    {
        if( anit != null )
        {
            anit.Play( "Attack" );
        }
        else
        {
            spr_curve_atk.StartPlay();
        }

        StartCoroutine( CO_WaitATK() );
    }

    IEnumerator CO_WaitATK()
    {
        yield return new WaitForSeconds(Time_ATK);
        ANI_ATK_End();
    }

    public void ANI_ATK_End()
    {
        charBase.OnEnd_TurnAction();
    }

    public void ANI_GetDamage()
    {
        if( anit != null )
        {
            anit.Play( "Damage" );
        }
        else
        {
            spr_curve_damage.StartPlay();
            shakeEffect_damage.Shake();            
        }
    }

    public void ANI_KO()
    {
        if( anit != null )
        {
            anit.Play( "KO" );
        }
        else
        {
            spr_curve_ko.StartPlay();
        }
    }

    public void Call_RecvSkill( object skl_obj )
    {
        SkillBase skill = skl_obj as SkillBase;
        skill.OnViewEffect_Enemy( gameObject );
    }

    public void Call_SelectTarget( bool b )
    {
        image_targetMark.gameObject.SetActive(b);
    }

}
