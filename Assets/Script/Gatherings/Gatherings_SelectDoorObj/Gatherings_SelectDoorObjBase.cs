using UnityEngine;

/// <summary>
/// 문 선택 이벤트용 클래스의 베이스
/// 
/// </summary>
public class Gatherings_SelectDoorObjBase
{
    public GatheringsBase gatheringsBase;

    public void Make( Mng_X128SS rd , GatheringsBase gatheringsBase_par )
    {
        gatheringsBase = gatheringsBase_par;
        OnMake(rd);
    }

    virtual public void OnMake( Mng_X128SS rd ){}

    // 선택 및 실행

    virtual public void OnExec()
    {
        // 기본은 그냥 통과

        // 
    }
}
