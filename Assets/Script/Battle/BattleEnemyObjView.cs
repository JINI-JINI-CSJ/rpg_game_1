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
        charBase = null;
        if( enemyMono != null )GameObject.DestroyImmediate(enemyMono.gameObject);
        gameObject.SetActive(false);
    }

    public void InitCharBase( CharBase chr )
    {
        if( image_targetMark == null )
        {
            image_targetMark = Panel_BattleMain.G.enemy_target_marks[IDX];
            SJ_UnityUIMng.WorldToUI( image_targetMark.transform , tr_TargetMarkPos );
        }

        charBase = chr;
        // 2D , 3D 
        if( string.IsNullOrEmpty( chr.csv.res ) == false )
        {
            spriteRenderer.sprite = SJ_ResPoolSys.GetResObjs_PathName_Sprite( chr.csv.res );
        } 
        else if( string.IsNullOrEmpty( chr.csv.res3D ) == false )
        {
            GameObject load_3D = SJ_ResPoolSys.Inst_Obj( chr.csv.res3D );
            SJ_Unity.SetEqTrans( load_3D.transform , null , transform );
            anit = load_3D.GetComponentInChildren<Animator>();
            enemyMono = load_3D.GetComponent<EnemyMono>();
        }
        gameObject.SetActive(true);
    }

}
