using UnityEngine;

// 적군 캐릭터 액션
public class BattleEnemyObjView : MonoBehaviour
{
    public CharBase charBase;

    // 2디 이미지 일경우
    // 3디 캐릭이면 스프라이트 감춘다.
    public SpriteRenderer spriteRenderer;

    Animator anit;
    GameObject load_3D;

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
        if( load_3D != null )GameObject.DestroyImmediate(load_3D);
        gameObject.SetActive(false);
    }

    public void InitCharBase( CharBase chr )
    {
        charBase = chr;
        // 2D , 3D 
        if( string.IsNullOrEmpty( chr.csv.res ) == false )
        {
            spriteRenderer.sprite = SJ_ResPoolSys.GetResObjs_PathName_Sprite( chr.csv.res );
        } 
        else if( string.IsNullOrEmpty( chr.csv.res3D ) == false )
        {
            load_3D = SJ_ResPoolSys.Inst_Obj( chr.csv.res3D );
            SJ_Unity.SetEqTrans( load_3D.transform , null , transform );
            anit = load_3D.GetComponentInChildren<Animator>();
        }
        gameObject.SetActive(true);
    }

}
