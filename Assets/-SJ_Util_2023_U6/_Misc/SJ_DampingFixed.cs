using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// lerp 로 따라가다가 종료시작하면 lerp 1까지 러프타임으로 ..
/// </summary>

public class SJ_DampingFixed : SJBase_Move
{
	public	float			Lerp = 0.1f;
	public	float			Last_Lerp_Time = 0.5f;
	float					last_Lerp_Time_cur = -1;

	public	bool			TransUpdate;

	public	bool 			end_disable;

	public	Vector3			tar_pos;
	public	Vector3			cur_pos;

	public	_SJ_GO_FUNC		sj_func;

	public	void	StartDamping( Vector3 start_pos ,  _SJ_GO_FUNC go_func = null , float _lerp = -1 ,  float _last_time = -1 )
	{
		cur_pos = start_pos;		

		enabled = true;
		if(_lerp > 0)			Lerp = _lerp;
		if(_last_time > 0 )		Last_Lerp_Time = _last_time;
		if(go_func != null)		sj_func = go_func;
	}

	public	void	SetTargetPos( Vector3 v , bool start_lerpTime = true )
	{
		enabled = true;

		if( tr_self != null )
			cur_pos = tr_self.position;
		tar_pos = v;
		if(start_lerpTime)Start_LerpTime();
	}

	public	void	Start_LerpTime()
	{
		last_Lerp_Time_cur = Last_Lerp_Time;
	}



	public	bool 	IsMoving()
	{
		if( last_Lerp_Time_cur > 0 ) return true;
		return false;
	}

	override public Vector3? Update_BasePos()
	{
		float lerp_cur = Lerp;
		if(last_Lerp_Time_cur > 0)
		{
			last_Lerp_Time_cur -= Time.deltaTime;

			if(last_Lerp_Time_cur <= 0)
			{
				cur_pos = tar_pos;
				if( end_disable )enabled = false;
				sj_func.Func();
				return cur_pos;
			}

			float r = last_Lerp_Time_cur / Last_Lerp_Time;
			lerp_cur = Mathf.Lerp(Lerp , 1.0f , r );
		}

		cur_pos = Vector3.Lerp(tar_pos , cur_pos , lerp_cur);
		return cur_pos;
	}

	private void Update() {
		if( noUpdate_Pos == false )
		{
			Vector3? v = Update_BasePos();
			tr_self.position = v.Value;
		}
	}

}
