using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SJ_CubicSpline_Mono))]
public class MoveCubic_StepObj : SJ_StepMode_Obj
{
    public  SJ_CubicSpline_Mono cubicSpline;
    public  SJ_ChrControl_Move  scc;

    [System.Serializable]
    public  class _RANDOM_POS
    {
        public  bool    use = true;

        public  BoxCollider         col_box;
        public  SphereCollider      col_sph;

        public  Vector3 GetPos()
        {
            if( col_box != null )return SJ_Cood.Random_BoxBound( col_box );
            if( col_sph != null )return SJ_Cood.Random_SphereBound( col_sph );
            Debug.Log( "GetPos : 세팅 없음!!!" );
            return Vector3.zero;
        } 
    }
    public    _RANDOM_POS   random_pos;


    override public    void    OnEditor_Init()
    {
        base.OnEditor_Init();
        cubicSpline = GetComponent<SJ_CubicSpline_Mono>();
        scc = GetComponentInParent<SJ_ChrControl_Move>();
    }


    public override void OnStart_Mode()
    {
        cubicSpline.dlgFunc_onEndTime = End_Cubic;
        cubicSpline.Init( scc.transform.position );
        if( random_pos.use )
        {
            cubicSpline.AddPos( random_pos.GetPos() );
        }
        cubicSpline.StartCubic();

    }

    public  void    End_Cubic()
    {
        if( random_pos.use )
        {
            cubicSpline.AddPos( random_pos.GetPos() );
        }else{
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
