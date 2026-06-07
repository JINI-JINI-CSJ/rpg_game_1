using UnityEngine;
using System.Collections;

public class SJ_TransUpdate : SJBase_Move
{
	public	bool	update_Prc;

	public	bool	use_pos = true;
	public	Vector3	vec_pos;

	Vector3			vec_RecentMove_before;
	Vector3			vec_RecentMove_cur;	//

	public	bool	use_rot;
	public	bool	local_rot;
	public	Vector3	vec_rot;

	public	bool	use_scl;
	public	Vector3	vec_scl;


	public	bool		pos_targetMode;
	public	Vector3		pos_Target;
	public	float		velocity_Target = -1;
	public	Transform	tr_Target;
	public	Vector3		offset_Target;
	public	float		targetTime;

	[HideInInspector]
	public	float		targetMode_Time_ratio_cur;

	public	bool			use_curve_targetMode;
	public	AnimationCurve	curve_targetMode;

	public	Vector3	pos_Start;
	float	targetTime_cur;

	public	_SJ_GO_FUNC		func_targetMode = new _SJ_GO_FUNC();

	public	bool	useHoming;
	public	bool	stop;


	// Use this for initialization
	void Start () {
	
	}

	void	Check_Self()
	{
		if(	tr_self == null ) tr_self = transform;
	}


	public	void	SetVelocityMode( Vector3 targetPos , float speed )
	{
		enabled = true;
		Check_Self();
		pos_targetMode = false;
		vec_pos = targetPos - tr_self.position;
		vec_pos.Normalize();
		Debug.Log( "SetVelocityMode vec_pos : " + vec_pos );
		velocity_Target = speed;
	}
	
	public	void	SetTargetMode( bool _modeTarget = false , Transform _tr_tar = null , float _time_tar=0, Vector3 _pos_tar = default(Vector3) , GameObject recv = null , string func = "" , Vector3? _pvCustom = null )
	{
		enabled = true;
		Check_Self();
		pos_targetMode = _modeTarget;
		if( _modeTarget )
		{
			use_pos = true;
			if( localPos )
				pos_Start = tr_self.localPosition;
			else 
				pos_Start = tr_self.position;

			if( _pvCustom != null )pos_Start = _pvCustom.Value;

			pos_Target = _pos_tar;
			tr_Target = _tr_tar;
			targetTime = _time_tar;
			targetTime_cur = 0;

			if( recv != null )
				func_targetMode.Set(recv , func);
		}
		else
		{
			Vector3 pos_tar = _pos_tar;
			if( _tr_tar != null )
			{
				pos_tar = _tr_tar.position;
			}
			vec_pos = pos_tar - tr_self.position;
			vec_pos.Normalize();
		}
	}

	private void FixedUpdate()
	{
		Check_Self();

		if (stop) return;

		if (localPos)
		{
			vec_RecentMove_cur = tr_self.localPosition - vec_RecentMove_before;
			vec_RecentMove_before = tr_self.localPosition;
		}else{
			vec_RecentMove_cur = tr_self.position - vec_RecentMove_before;
			vec_RecentMove_before = tr_self.position;
		}
	}

	public	void	RecentTarget_ToMoveMode()
	{
		enabled = true;
		pos_targetMode = false;
		float f = 1.0f / Time.fixedDeltaTime;
		vec_pos = vec_RecentMove_cur * f;
	}

	public override Vector3? Update_BasePos()
	{
		Check_Self();

		if(stop)return null;

		if (use_pos)
		{
			if (pos_targetMode)
			{
				Vector3 _pos_target = pos_Target;
				if (tr_Target != null)
				{
					_pos_target = tr_Target.position;
				}
				_pos_target += offset_Target;

				if (velocity_Target < 0)
				{
					targetTime_cur += Time.deltaTime;
					targetMode_Time_ratio_cur = targetTime_cur / targetTime;

					if (use_curve_targetMode)
						targetMode_Time_ratio_cur = curve_targetMode.Evaluate(targetMode_Time_ratio_cur);

					Vector3 v = Vector3.Lerp(pos_Start, _pos_target, targetMode_Time_ratio_cur );
					if (targetTime_cur >= targetTime) v = _pos_target;

					if (noUpdate_Pos == false)
					{
						if (localPos) tr_self.localPosition = v;
						else tr_self.position = v;
					}

					//Debug.Log("targetTime_cur : " + targetTime_cur + "  : targetTime :" + targetTime);

					if (targetTime_cur >= targetTime)
					{
						enabled = false;
						func_targetMode.Func();
					}

					return v;
				}
				else
				{
					Vector3 pos_cur = tr_self.position;
					if (localPos) pos_cur = tr_self.localPosition;

					Vector3 v_dir = _pos_target - pos_cur;
					Vector3 v_move = v_dir.normalized * velocity_Target * Time.deltaTime;
					//Debug.Log( " v_move : " + v_move );

					if (v_dir.sqrMagnitude <= v_move.sqrMagnitude)
					{
						if (noUpdate_Pos == false)
						{
							if (localPos) tr_self.localPosition = _pos_target;
							else tr_self.position = _pos_target;
						}
						//Debug.Log( "SendMsg : Update_Pos : pos_targetMode " );
						func_targetMode.Func();
						enabled = false;
						return _pos_target;
					}

					if (noUpdate_Pos == false)
					{
						if (localPos) tr_self.localPosition += v_move;
						else tr_self.position += v_move;
					}
					else
					{
						Vector3 v = Vector3.zero;
						if (localPos) v = tr_self.localPosition += v_move;
						else v = tr_self.position += v_move;
						return v;
					}
				}


			}
			else
			{
				Vector3 v = vec_pos * Time.deltaTime;

				if (velocity_Target > 0)
				{
					v *= velocity_Target;
				}

				if (noUpdate_Pos == false)
				{
					if (localPos)
					{
						tr_self.localPosition += v;
					}
					else
					{
						tr_self.Translate(v, Space.World);
					}
				}
			}
		}

		if (use_rot)
		{
			Vector3 v = vec_rot * Time.deltaTime;
			if (local_rot)
			{
				tr_self.Rotate(v.x, v.y, v.z, Space.Self);
			}
			else
			{
				tr_self.Rotate(v.x, v.y, v.z, Space.World);
			}
		}

		if (use_scl)
		{
			Vector3 v = vec_scl * Time.deltaTime;
			tr_self.localScale += v;
		}

		return null;
	}


	// Update is called once per frame
	void Update ()
	{
		if( update_Prc ) Update_BasePos();
	}
}
