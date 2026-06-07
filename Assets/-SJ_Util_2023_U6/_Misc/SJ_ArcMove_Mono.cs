using UnityEngine;
using System.Collections;

[System.Serializable]
public class SJ_ArcMove
{

	public Transform transform;

	public	bool	isLocal;

	public	bool	moving = false;

	public	float time=1.0f;
	float	time_cur;
	public	Vector3 src;
	public	Vector3 tar;

	public	Transform tr_Target;

	public	float	height = 5;
	public	AnimationCurve	height_curve;

	public	MonoBehaviour	recv;
	public	string			func;

	float	d_start = -1;
	float	d_end	= -1;
	bool	pos_goal = false;


	public	bool		noUpdate_Pos;

	public	void Start_Arc
		( float arg_time = -1.0f , float? arg_height = null , Vector3? arg_tar = null , Vector3? arg_src = null , 
		float delay_per_start = -1 , float delay_per_end = -1 )
	{
		pos_goal = false;
		moving = true;
		time_cur = 0;
		if(arg_time >= 0.0f) time = arg_time;

		float time_src = time;

		d_start = -1;
		if( delay_per_start > 0 )
		{
			d_start = time_src * delay_per_start;
			time -= d_start;
		}

		d_end = -1;
		if( delay_per_end > 0 )
		{
			d_end = time_src * (1.0f - delay_per_end);
			time -= d_end;
		}

		if( arg_height != null ) height = (float)arg_height;


		if(arg_tar != null)
		{
			tar = (Vector3)arg_tar;
		}
		else
		{
			if ( tr_Target != null ) tar = tr_Target.position;
		}

		if(arg_src != null)
		{
			src = (Vector3)arg_src;

			if( noUpdate_Pos == false )
			{
				if( isLocal )
					transform.localPosition = src;
				else
					transform.position = src;
			}
		}
		else
		{
			if( isLocal )
				src = transform.localPosition;
			else
				src = transform.position;
		}
	}

	public	Vector3?	Update_Arc()
	{
		Vector3 v = Vector3.zero;
		if( moving == false ) return null;

		if( d_start > 0 )
		{
			d_start -= Time.deltaTime;
			return null;
		}

		time_cur += Time.deltaTime;
		float time_r = time_cur / time;
		
		if(time_r >= 1.0f )
		{
			if( pos_goal == false )
			{
				if( noUpdate_Pos == false )
				{
					if(isLocal)
						transform.localPosition = tar;
					else
						transform.position = tar;
				}

				pos_goal = true;

				if( d_end < 0 )
				{
					moving = false;
					SJ_Unity.SendMsg( recv , func );
				}
                return null;
			}

			d_end -= Time.deltaTime;
			if( d_end < 0 )
			{
				moving = false;
				SJ_Unity.SendMsg( recv , func );
			}
            return null;
		}
		v = Vector3.Lerp(src , tar , time_r);


		float hei_r =	height_curve.Evaluate(time_r);
		v.y = height * hei_r;

		//Debug.Log( " Arc : v : " + src + " : " + tar + " : " + v + " : " + time_r );

		if( noUpdate_Pos == false )
		{
			if( isLocal )
				transform.localPosition = v;
			else
				transform.position = v;
		}
		return v;
	}

}


public class SJ_ArcMove_Mono : SJBase_Move
{
	public	bool	mono_Start;
	public SJ_ArcMove sj_ArcMove = new SJ_ArcMove();

	void Start()
	{
		if( mono_Start )
		{
			Start_Arc();
		}
	}

	public override void OnSetBase_Inf(Transform _tr, bool _noUpdate_pos , bool _localPos)
	{
		noUpdate_Pos = _noUpdate_pos;
		sj_ArcMove.transform = _tr;
		sj_ArcMove.noUpdate_Pos = _noUpdate_pos;
	}

	public	void SetFunc( MonoBehaviour mono , string func )
	{
		sj_ArcMove.recv = mono;
		sj_ArcMove.func = func;
	}

	public	void Start_Arc( float arg_time = -1.0f , float? arg_height = null , Vector3? arg_tar = null , Vector3? arg_src = null , 
		float delay_per_start = -1 , float delay_per_end = -1 )
	{
		if( sj_ArcMove.transform == null ) sj_ArcMove.transform = transform;
		this.enabled = true;
		sj_ArcMove.Start_Arc( arg_time , arg_height , arg_tar , arg_src , delay_per_start , delay_per_end );
	}

	void	Update()
	{
		//sj_ArcMove.Update_Arc();
	}

	public	void	Stop()
	{
		sj_ArcMove.moving = false;
		this.enabled = false;
	}

	public override Vector3? Update_BasePos()
	{
		return sj_ArcMove.Update_Arc();
	}
}
