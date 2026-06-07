using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_LerpTransform : SJTrgAction_Default_Mono 
{
	public	_SJ_CurveTime	sjcv;
	public	_SJ_SelObjName	obj_End;

	// 실행객체가 시작했던 위치
	GameObject		go_play;
	Transform		tr_target;
	Vector3			pos_Start;
	Quaternion		rot_Start;

	float	elapse = 0;
	bool	start = false;

	public override void OnInit()
	{
		start = false;
	}

	override	public	void	OnAction()
	{
		start = true;
		elapse = 0;
		go_play = GetExecuteObj();
		pos_Start = go_play.transform.position;
		rot_Start = go_play.transform.rotation;

		tr_target = obj_End.Get().transform;

		sjcv.Start();
	}


	public override void OnUpdate()
	{
		float r = sjcv.Update();
		Vector3		pos = Vector3.Lerp( pos_Start , tr_target.position , r );
		Quaternion	rot = Quaternion.Slerp( rot_Start , tr_target.rotation , r );

		go_play.transform.position = pos;
		go_play.transform.rotation = rot;

		if( IsSyncMode() )
		{
			if( sjcv.Check_End() )
			{
				EndAction();
			}
		}
	}


}
