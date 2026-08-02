using UnityEngine;
using System.Collections;
using Unity.Mathematics;

[System.Serializable]
public	class SJ_Curve
{
	public	enum	LOOP_TYPE
	{
		None , 
		Restart ,
		PingPong
	}

	public	bool		realTime = false;
	public	LOOP_TYPE	loop_type = LOOP_TYPE.None;
	public	bool	play;
	public	bool	play_fwd = true;

	public	AnimationCurve	Curve = AnimationCurve.Linear(0,0,1,1);

	public	float	time = 1.0f;
	float			time_cur = -1;
	public	float	val_start = 0;
	public	float	val_end = 1;
	public	float	val_cur = 0f;

	public float 	curve_cur;

	public	float	realTime_Start;

	public  _SJ_GO_FUNC		end_recvMsg = new _SJ_GO_FUNC();


	public  SJ_COMMON.Func_VOID func_Update;
	public  SJ_COMMON.Func_VOID func_End;

	public	float	Val() {return val_cur; }
	public	void	StartTime()
	{
		time_cur = 0;
		curve_cur = 0;
		realTime_Start = Time.realtimeSinceStartup;
		play = true;
	}

	public	void	StartTime_PlayDir( bool _play_fwd )
	{
		play_fwd = _play_fwd;
		StartTime();
	}

	public	void	InitCurve( float _val )
	{
		time_cur = -1;
		val_start = val_end = val_cur = _val;
	}

	public	void	StarTimeCurve( float _start , float _end , bool _play_fwd = true , float playTime = 1.0f )
	{
		play_fwd = _play_fwd;
		val_start = val_cur = _start;
		val_end = _end;
		time = playTime;
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
					UpdatePrc(r);

					play = false;

					end_recvMsg.Func();
					func_End?.Invoke();
					return val_cur;
				}


				case LOOP_TYPE.Restart:
				case LOOP_TYPE.PingPong:
				{
					r = r - 1.0f;
					time_cur -= time;
					if( loop_type == LOOP_TYPE.PingPong )play_fwd = !play_fwd;
					if( realTime ) realTime_Start = Time.realtimeSinceStartup;
				}
				break;
			}
		}
		UpdatePrc(r);
		return	val_cur;
	}

	void UpdatePrc(float r)
	{
		if( play_fwd == false )r = 1.0f - r;
		curve_cur = Curve.Evaluate( r );
//Debug.Log( "curve_cur : " + curve_cur );
		val_cur = val_start + ((val_end - val_start) * curve_cur );
		OnUpdate();
	}

	virtual public void OnUpdate()
	{
		func_Update?.Invoke();
	}
}

[System.Serializable]
public	class SJ_Curve_Vec3 : SJ_Curve
{
	public Vector3 pos_start;
	public Vector3 pos_end;
	public Vector3 pos_cur;
    public override void OnUpdate()
    {
        pos_cur = Vector3.Lerp( pos_start , pos_end , curve_cur );
		base.OnUpdate();
    }
}
[System.Serializable]
public	class SJ_Curve_Rot : SJ_Curve
{
	public Quaternion rot_start;
	public Quaternion rot_end;
	public Quaternion rot_cur;
    public override void OnUpdate()
    {
        rot_cur = Quaternion.Lerp( rot_start , rot_end , curve_cur );
		base.OnUpdate();
    }
}
[System.Serializable]
public	class SJ_Curve_Color : SJ_Curve
{
	public Color col_start;
	public Color col_end;
	public Color col_cur;
    public override void OnUpdate()
    {
        col_cur = Color.Lerp( col_start , col_end , curve_cur );

		//Debug.Log( "col_start : " + col_start + "    col_end : " + col_end + "        curve_cur : " + curve_cur +  "       col_cur : " + col_cur );

		base.OnUpdate();
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
		float f =	sj_curve.UpdateCurve();
		OnUpdateCurve();
		return f;
	}
	virtual public void OnUpdateCurve(){}

	// Update is called once per frame
	void Update ()
	{
		if(prcUpdateFunc) UpdateCurve();
	}
}
