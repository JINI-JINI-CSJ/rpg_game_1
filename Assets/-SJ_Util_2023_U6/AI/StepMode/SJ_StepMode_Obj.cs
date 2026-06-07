using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_StepMode_Obj : MonoBehaviour
{
    public  SJ_StepMode_Mono        par_mng;
    public  SJ_StepMode_Mono._MODE  step_mode;


    public  void    NextStep()
    {
        par_mng.Next_ListModePlay();
    }
    
    virtual    public  void    OnEditor_Init()
    {
        par_mng = GetComponentInParent<SJ_StepMode_Mono>();
    }

    virtual    public  void    OnStart_Mode(){}

    virtual    public  void    OnUpdate_Mode(){}

    virtual    public  void    OnEnd_Mode()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    virtual    public  void    OnCancel_Mode()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

}
