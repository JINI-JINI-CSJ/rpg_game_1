using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_LoopBlockSys : MonoBehaviour
{
    static  public  SJ_LoopBlockSys g;

    // 현재 보이는 블럭들
    public  SJ_LoopBlockObj     block_cur;
    public  SJ_LoopBlockObj     block_next;
    public  SJ_LoopBlockObj     block_back;

    public  SJ_LoopBlockObj     prf_block_Next;

    public  bool                noEnter;

    private void Awake()
    {
        g = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static  public  void    SetFirst( SJ_LoopBlockObj _cur , SJ_LoopBlockObj _next )
    {
        g.block_cur = _cur;
        g.block_next = _next;
    }

    public  void    _Enter_Next( SJ_LoopBlockObj enter_block )
    {
        if( noEnter ) return;

        OnEnterBlock_Pre( enter_block );
        if( block_back != null )
        {
            SJPool.ReturnInst( block_back.gameObject );
        }

        block_back = block_cur;
        block_cur = block_next;

        if( prf_block_Next != null )
        { 
            GameObject inst = SJPool.GetNewInst(prf_block_Next.gameObject);
            block_next = inst.GetComponent<SJ_LoopBlockObj>();

            block_cur.Link_Next( block_next.transform );
            block_next.OnInst_Block();
        }
    }

    virtual public  void    OnEnterBlock_Pre( SJ_LoopBlockObj enter_block ) { }
    static  public  void    Enter_Next( SJ_LoopBlockObj inst_block )
    {
        g._Enter_Next(inst_block);
    }

}
