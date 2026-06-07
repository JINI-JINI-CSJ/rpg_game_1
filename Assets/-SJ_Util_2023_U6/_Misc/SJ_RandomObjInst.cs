using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SJ_RandomObjInst : MonoBehaviour
{
    public  List<GameObject>        lt_prfObj;
    public  Transform               tr_BoxRandom;

    public  bool            playAnit;
    public  string          playAnit_Name;
    public  float           randomTime_min;
    public  float           randomTime_max;

    public  bool            play_awake;

    // [System.Serializable] public class Event_RandomInst_Start : UnityEvent<GameObject> { }
    // public   bool        use_func;
    // public   Event_RandomInst_Start func_inst;

    public  _SJ_GO_FUNC     func_start;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        if( play_awake ) Play_RandomTimeCreate();
    }

    private void OnDisable() {
        //if( play_awake ) ReturnAll();
    }

    public  void    ReturnAll()
    {
        foreach( GameObject prf in lt_prfObj )
           SJPool.ReturnInst_All( prf );
    }

    public  void    Play_RandomTimeCreate()
    {
        RandomTineCo();
    }

    public  void    RandomTineCo()
    {
        float wait = UnityEngine.Random.Range( randomTime_min , randomTime_max );
        StartCoroutine(CO_RandomCreate( wait ));
    }

    IEnumerator CO_RandomCreate( float wait )
    {
        yield return new WaitForSeconds(wait);
        Create_Inst();
        RandomTineCo();
    }

    public  void    Create_Inst()
    {
        Vector3 pos = SJ_Cood.Random_ScaleBound( tr_BoxRandom );
        GameObject inst = SJPool.GetNewInst( SJ_Unity.GetArray_Random( lt_prfObj.ToArray() ) );
        SJ_Unity.SetEqTrans( inst.transform , null , transform );
        inst.transform.position = pos;

        if( playAnit )
        {
            Animator anit = inst.GetComponentInChildren<Animator>();
            if( anit != null ) anit.Play(playAnit_Name);
        }
        func_start.Func( inst );
    }

}
