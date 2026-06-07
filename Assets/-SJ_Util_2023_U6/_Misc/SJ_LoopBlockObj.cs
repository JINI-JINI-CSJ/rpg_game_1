using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_LoopBlockObj : MonoBehaviour
{
    public  Transform   tr_nextLink;

    public  bool    enter_Collider_Default;

    public  bool    entered;

    public  Transform   TEST_linkBlock;

    virtual     public  void OnLoadRes_Block() { }

    virtual     public  void OnInst_Block() 
    {
        entered = false;
    }

    public  void TryEnterBlock()
    {
        if( entered ) return;
        entered = true;

        SJ_LoopBlockSys.Enter_Next(this);
    }

    public  void    Link_Next( Transform block_next )
    {
        block_next.transform.position = tr_nextLink.position;
        block_next.transform.rotation = tr_nextLink.rotation;
    }

    public  void    TEST_Link_Next()
    {
        if( TEST_linkBlock == null )
        {
            Debug.LogError("Error!!! : TEST_linkBlock == null ");
            return;
        }
        Link_Next(TEST_linkBlock);
    }


    private void OnTriggerEnter(Collider other)
    {
        if( enter_Collider_Default )
        {
            TryEnterBlock();
        }
    }

}
