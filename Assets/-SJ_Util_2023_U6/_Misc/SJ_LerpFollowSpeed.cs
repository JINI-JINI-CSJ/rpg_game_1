using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_LerpFollowSpeed : MonoBehaviour
{
	[Header ("목표객체") ]
	public	Transform	tr_Target;
	[Header ("러프시간 위치") ]
	public	float		LerpTime_Pos = 0.1f;
	[Header ("러프시간 각도") ]
	public	float		LerpTime_Ang = 0.1f;
	[Header ("최소속도") ]
	public	float		Speed_Min = 1f;

	[Header ("위치 최대거리") ]
	public	float		max_PosDist = 5;

	public	Vector3		offset;

	bool	play;
	public	Vector3		pos_tar_recent;
	public	float		speed_target;
	public	float		speed_cur;


	public	void	SetPos_Active( Transform tr = null )
	{
		if( tr != null ) tr_Target = tr;
		if( tr_Target == null )
		{
			Debug.LogError("Error! : SJ_LerpFollowSpeed : SetPos_Active : tr_Target == null");
			return;
		}

		transform.position = tr_Target.position;
		transform.rotation = tr_Target.rotation;
	}


	public	void	Update_LerpSpeed()
	{
		if( play == false )
		{ 
			play = true;
			pos_tar_recent = tr_Target.position;
		}

		Vector3 v_move = tr_Target.position - pos_tar_recent;
		speed_target = v_move.magnitude / Time.deltaTime;
		speed_cur = Mathf.Lerp( speed_cur , speed_target , LerpTime_Pos );
		if( speed_cur > speed_target )speed_cur = speed_target;
		pos_tar_recent = tr_Target.position;
		Vector3 dir = (tr_Target.position + offset) - transform.position;
		if (dir.magnitude < 0.001f)
		{
			transform.position = tr_Target.position + offset;
			return;
		}
		if( speed_cur < Speed_Min )speed_cur = Speed_Min;
		dir.Normalize();
		//transform.position += dir * speed_cur * Time.deltaTime;
		 Vector3 v_last = transform.position + (dir * speed_cur * Time.deltaTime);

		Vector3 v_len = pos_tar_recent - v_last;
		if( v_len.sqrMagnitude > max_PosDist*max_PosDist )
		{
			v_len.Normalize();
			v_last = pos_tar_recent + (v_len*max_PosDist);
		}

		transform.position = v_last;

		transform.rotation = Quaternion.Slerp( transform.rotation , tr_Target.rotation , LerpTime_Ang * Time.deltaTime );
	}

	private void LateUpdate()
	{
		Update_LerpSpeed();
	}

}
