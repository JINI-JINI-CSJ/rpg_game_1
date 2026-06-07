using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SJ_CubicSpline_Mono))]
public class JumpCublic_StepObj : SJ_StepMode_Obj
{
    public  SJ_CubicSpline_Mono cubicSpline;
    public  SJ_ChrControl_Move  scc;

    public  float               height;

    public  List<Vector3>       lt_JumpList;

    Vector3 vStart;

    Vector3 vRecentTar;

    int     cur_idx;

    override public    void    OnEditor_Init()
    {
        base.OnEditor_Init();
        cubicSpline = GetComponent<SJ_CubicSpline_Mono>();
        scc = GetComponentInParent<SJ_ChrControl_Move>();
    }

    public override void OnStart_Mode()
    {
        if(lt_JumpList.Count < 1 )
        {
            Debug.LogError( "lt_JumpList.Count < 1" );
            return;
        }

        scc.Set_Gravity(false);

        cur_idx = 0;
        cubicSpline.dlgFunc_onEndTime = End_Cubic;
        cubicSpline.Init( scc.transform.position );
        vStart = scc.transform.position;
        vRecentTar = vStart;

        Add_JumpPos();
        cubicSpline.StartCubic();
    }

    public  bool    Add_JumpPos()
    {
        if( cur_idx >= lt_JumpList.Count )
        {
            return false;
        }

        Vector3 tar = lt_JumpList[cur_idx];
        Vector3 mid = (tar + vRecentTar) * 0.5f;
        mid.y += height;
        cubicSpline.AddPos(mid);
        cubicSpline.AddPos(tar);
        return true;
    }

    public  void    End_Cubic()
    {
        if( Add_JumpPos() == false )
        {
            NextStep();
        }
    }

    public override void OnUpdate_Mode()
    {
        Vector3? pos = cubicSpline.Update_BasePos();
        if( pos.HasValue )
        {
            scc.AddPos(pos.Value);
        }
    }

}
