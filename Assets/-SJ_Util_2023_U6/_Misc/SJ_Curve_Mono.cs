using UnityEngine;
using System.Collections;

[System.Serializable]
public	class SJ_Curve
{
	public	enum	LOOP_TYPE
	{
		None , 
		Restart ,
		Pingpong
	}

	public	bool		realTime = false;
	public	LOOP_TYPE	loop_type;
	public	bool	play;
	public	bool	play_fwd = true;

	public	AnimationCurve	Curve = new AnimationCurve();

	public	float	time = 1.0f;
	float			time_cur = -1;
	public	float	val_start = 0;
	public	float	val_end = 1;
	public	float	val_cur = 0f;

	public	float	realTime_Start;

	public  _SJ_GO_FUNC		end_recvMsg = new _SJ_GO_FUNC();

	public	float	Val() {return val_cur; }
	public	void	StartTime()
	{
		time_cur = 0;
		realTime_Start = Time.realtimeSinceStartup;
		play = true;
	}


	public	void	InitCurve( float _val )
	{
		time_cur = -1;
		val_start = val_end = val_cur = _val;
	}

	public	void	StarTimeCurve( float _start , float _end , bool _play_fwd = true )
	{
		play_fwd = _play_fwd;
		val_start = val_cur = _start;
		val_end = _end;
		StartTime();
	}

	public	void	StarTimeCurve_ValEnd( float _end , bool _play_fwd = true )
	{
		play_fwd = _play_fwd;
		val_start = val_cur;
		val_end = _end;
		StartTime();
	}

	public	void	StarTimeCurve_Cur(bool _play_fwd = true)
	{
		if( play_fwd != _play_fwd )
		{
			time_cur = time - time_cur;
		}
		play_fwd = _play_fwd;
		play = true;
	}

	public	float	UpdateCurve()
	{
		if( !play ) return val_cur;

		if(realTime == false)	time_cur += Time.deltaTime;
		else					time_cur = Time.realtimeSinceStartup - realTime_Start;

		float	r = time_cur / time;
		if( r >= 1.0f )
		{
			switch( loop_type )
			{
				case LOOP_TYPE.None:
				{
					r = 1;
					
					if( play_fwd )
						val_cur = val_end;
					else 
						val_cur = val_start;

					play = false;

					end_recvMsg.Func();
					return val_cur;
				}


				case LOOP_TYPE.Restart:
				case LOOP_TYPE.Pingpong:
				{
					r = r - 1.0f;
					time_cur -= time;
					if( loop_type == LOOP_TYPE.Pingpong )play_fwd = !play_fwd;
					if( realTime ) realTime_Start = Time.realtimeSinceStartup;
				}
				break;
			}
		}

		if( play_fwd == false )r = 1.0f - r;
		
		float r_c = Curve.Evaluate( r );
		val_cur = val_start + ((val_end - val_start) * r_c );

		return	val_cur;
	}
}

public class SJ_Curve_Mono : MonoBehaviour
{
	public	SJ_Curve	sj_curve;

	public	bool	prcUpdateFunc;

	public	float	Val() {return sj_curve.Val(); }
	public	void	StartTime()
	{
		sj_curve.StartTime();
	}


	public	void	InitCurve( float _val )
	{
		sj_curve.InitCurve(_val);
	}

	public	void	StarTimeCurve( float _start , float _end  )
	{
		sj_curve.StarTimeCurve( _start , _end  );
	}

	public	void	StarTimeCurve_ValEnd( float _end )
	{
		sj_curve.StarTimeCurve_ValEnd( _end );
	}

	public	float	UpdateCurve()
	{
		return	sj_curve.UpdateCurve();
	}

	// Update is called once per frame
	void Update ()
	{
		if(prcUpdateFunc) UpdateCurve();
	}
}
