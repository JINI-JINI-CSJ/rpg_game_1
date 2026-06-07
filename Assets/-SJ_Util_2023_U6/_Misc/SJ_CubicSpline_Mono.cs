using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_CubicSpline_Mono : SJBase_Move
{
	public	bool		play;
	SJ_CubicSpline		cubicSp = new SJ_CubicSpline();
	public	_SJ_CurveTime	sJ_CurveTime = new _SJ_CurveTime();
	public	float			term_speed = -1f;

	public	List<Vector3>	pos_next;
	public	bool			lookAt = true;

	public	delegate	void	DlgFunc_OnEndTime();
	public	DlgFunc_OnEndTime	dlgFunc_onEndTime;

	// Use this for initialization
	void Start ()
	{
	}

	public	void	Init( Vector3 pos_start , float _term_time = -1 )
	{
		if( tr_self == null )tr_self = transform;

		term_speed = _term_time;

		cubicSp.Clear();
		cubicSp.Add_Pos(pos_start + Vector3.left );
		cubicSp.Add_Pos(pos_start + Vector3.right );
		cubicSp.Add_Pos(pos_start);

		pos_next.Clear();

		if( noUpdate_Pos == false )
			tr_self.localPosition = pos_start;

		play = false;
	}

	public	void	StartCubic()
	{
		play = true;
		nextPos();
	}

	public	void	Stop()
	{
		play = false;
	}

	public	void	AddPos( Vector3 pos )
	{
		pos_next.Add(pos);
	}

	
	// Update is called once per frame
	void Update ()
	{
		//Update_Pos();
	}

	void	nextPos()
	{
		if( pos_next.Count < 1 )
			return;
		
		Vector3	pos = pos_next[0];

		if( term_speed > 0.01f )
		{
			Vector3 pos_b = cubicSp.list_pos[ cubicSp.list_pos.Count - 1 ];
			float len = Vector3.Distance( pos , pos_b );
			sJ_CurveTime.time = len / term_speed;
		}
		sJ_CurveTime.Start();
		cubicSp.Add_Pos( pos );
		pos_next.RemoveAt(0);
	}

	public override Vector3? Update_BasePos()
	{
		Vector3 pos = Vector3.zero;
		if (play == false)
		{
			return null;
		}
		float r = sJ_CurveTime.Update();
		cubicSp.Get_LastPosLerp(r, ref pos);

		if (lookAt)
		{
			Vector3 pos_w = tr_self.TransformPoint(pos);
			tr_self.LookAt(pos_w);
		}

		if (noUpdate_Pos == false)
			tr_self.localPosition = pos;

		if (r >= 1.0f)
		{
			if (dlgFunc_onEndTime != null) dlgFunc_onEndTime();
			nextPos();
		}

		return pos;
	}


	public	void Update_Pos( int idx , Vector3 p )
	{
		if(idx >= cubicSp.list_pos.Count)
		{
			return;
		}
		cubicSp.list_pos[idx] = p;
	}


	public override void LastPos_Offset(Vector3 offset)
	{

	}

}
