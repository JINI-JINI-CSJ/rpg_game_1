using UnityEngine;
using System.Collections;

public class SJMiscUtil_1 
{
	//
	// 태그된 오브젝트들이 다 보일때까지 
	// 캠 뒤로 이동
	//
	public static	void 	CameraFitInAllTagObj( 	Camera cam_main = null , string tag_obj = "FIT_IN_CAM" ,
													float fCamBackStep = -0.1f , float fMaxBackStep = -1000.0f )
	{
		float fBackAcc = 0.0f;
		GameObject[] go_obj = GameObject.FindGameObjectsWithTag( "tag_obj" );
		
		if( go_obj.Length < 1 )
			return;
		
		if( cam_main == null )
			cam_main = Camera.main;
		
		while(true)
		{
			bool bAllIn = true;
			foreach( GameObject obj in go_obj )
			{
				Vector3 vpos =	cam_main.WorldToViewportPoint( obj.transform.position );
				if( vpos.x < 0.0f || vpos.x > 1.0f || vpos.y < 0.0f || vpos.y > 1.0f )
				{
					bAllIn = false;
					break;
				}
			}
			
			if( bAllIn )
				break;
			
			Vector3	vBack =	cam_main.transform.forward * fCamBackStep;
			cam_main.transform.Translate( vBack );
			fBackAcc += fCamBackStep;
			if( fBackAcc < fMaxBackStep )
				break;
		}
	}
	
	public static Vector3 Interp_Catmull_Rom(Vector3[] pts, float t)
	{
		if( pts.Length < 3 ) return Vector3.zero;

		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}	
	
	static	public	void 	Float_Idx_lerp( int total , float _ratio , ref int ref_idx , ref float ref_lerp )
	{
		float f_idx = (float)total * _ratio;
		ref_idx = (int)f_idx;
		ref_lerp = f_idx - (float)ref_idx;
	}
}

[System.Serializable]
public	class _SJ_CurveTime
{
	public	AnimationCurve	cv;
	public	float			time = 1;

	public	float			val_s;
	public	float			val_e;

	public	bool			reverse;
	public	_SJ_GO_FUNC		func;

	bool	play;
	bool	end;
	float	elapse;
	float	normal_time;

	//[HideInInspector]

	public	float	cv_ratio;

	bool	end_func;
	float	time_start;

	public	bool	CheckPlay(){return play;}

	public	_SJ_CurveTime()
	{
		Init();
	}



	public	void	Init()
	{
		play = false;
		end = false;
		elapse = 0;
		normal_time = 0;
		cv_ratio = 0;
		end_func = false;
	}

	public	void	Start()
	{
		Init();
		play = true;
		time_start = Time.time;
	}

	public	float	Update()
	{
		if( play == false ) return cv_ratio;

		float cur_t = Time.time - time_start;
		normal_time = cur_t / time;

		float nt = normal_time;
		if( reverse )
		{
			nt = 1.0f - normal_time;
		}
		cv_ratio = cv.Evaluate( nt );
		
		if( normal_time >= 1.0f )
		{
			play = false;
			end = true;

			//cv_ratio = 1;
			//if( reverse ) cv_ratio = 0;

			func.Func();
		}

		//Debug.Log( "_SJ_CurveTime : " + normal_time + " : " + cv_ratio);

		return cv_ratio;
	}

	public	float	ValueLerp()
	{
		return Mathf.Lerp( val_s , val_e , cv_ratio );
	}

	public	bool	Check_End()
	{
		if( end == false ) return false;

		if( end_func == false )
		{
			// 한번만 호출되게..
			end_func = true;
			return true;
		}

		return false;
	}

	public	bool	End_Stop()
	{
		if( play == false ) return false;
		play = false;
		return true;
	}

}

[System.Serializable]
public	class SJ_Anicurve_Time
{
	public	AnimationCurve	ac;
	public	float			time;
	public	float			time_cur;
	public	float			val_start;
	public	float			val_target;
	public	float			val_cur;
	public	float			evaluate_cur;

	public	float			delay = -1;
	float					delay_cur;

	public	_SJ_GO_FUNC		func;

	bool	play;
	bool	complete = false;

	public	void	SetCurValue( float v ) { val_cur = v; }
	public	bool	IsPlay() {return play; }
	public	void	Stop()
	{
		play = false;
	}

	public	void	StartTime()
	{
		time_cur = 0;
		play = true;
		complete = false;

		delay_cur = delay;
	}

	public	void	StartTar_FromCur( float _val_tar , bool noStartVal = false )
	{
		StartTime();

		if( noStartVal == false )
			val_start = val_cur;

		val_target = _val_tar;
	}


	public	float	Update()
	{
		if( play == false ) return  val_cur;

		//float delta_t = Time.unscaledDeltaTime;

		if( delay_cur > 0 )
		{
			delay_cur -= Time.unscaledDeltaTime;
			return val_start;
		}

		time_cur += Time.unscaledDeltaTime;
		float r = time_cur / time;

		if( r >= 1.0f )
		{
			r = 1.0f;
			play = false;
			complete = true;

			func.Func();
		}
		evaluate_cur =	ac.Evaluate( r );
		val_cur = Mathf.Lerp( val_start , val_target , evaluate_cur );

		OnUpdate();

		return val_cur;
	}

	virtual	public	void	OnUpdate() { }

	public	bool	Check_Complete()
	{
		if( complete )
		{
			complete = false;
			return true;
		}
		return false;
	}


	public	bool	End_Stop()
	{
		if( play == false ) return false;
		play = false;
		val_cur = val_target;

		func.Func();
		return true;
	}
}

[System.Serializable]
public	class SJ_Anicurve_Time_Vec3 : SJ_Anicurve_Time
{
	public	Vector3			vec3_start;
	public	Vector3			vec3_target;
	public	Vector3			vec3_cur;

	public	void	SetCurValue( Vector3 v ) { vec3_cur = v; }

	public	void	StartTar_FromCur( Vector3 _val_tar )
	{
		StartTime();

		vec3_start = vec3_cur;
		vec3_target = _val_tar;
	}

	override	public	void	OnUpdate()
	{
		vec3_cur = Vector3.Slerp( vec3_start , vec3_target , evaluate_cur );
	}
}

[System.Serializable]
public	class SJ_Anicurve_Time_Trans : SJ_Anicurve_Time
{
	public	Vector3			vec3_start;
	public	Vector3			vec3_target;
	public	Vector3			vec3_cur;

	public	Quaternion		rot_start;
	public	Quaternion		rot_target;
	public	Quaternion		rot_cur;

	public	Transform		tr_start;
	public	Transform		tr_target;

	public	bool			noUpdate_Pos;

	public	bool			check_target_update;

	public	void			Start( Transform _t1 = null , Transform _t2 = null )
	{
		if( _t1 != null )tr_start  = _t1;
		if( _t2 != null )tr_target = _t2;

		vec3_start	= tr_start.localPosition;
		rot_start	= tr_start.localRotation;

		if( tr_target != null )
		{
			vec3_target	= tr_target.localPosition;
			rot_target	= tr_target.localRotation;
		}

		vec3_cur = vec3_start;
		rot_cur = rot_start;

		StartTime();
	}

	override	public	void	OnUpdate()
	{
		if( check_target_update )
		{
			rot_target	= tr_target.localRotation;
			vec3_target	= tr_target.localPosition;
		}

		vec3_cur = Vector3.Slerp( vec3_start , vec3_target , evaluate_cur );
		rot_cur = Quaternion.Slerp( rot_start , rot_target ,evaluate_cur);

		if( noUpdate_Pos == false )
			tr_start.localPosition = vec3_cur;

		tr_start.localRotation = rot_cur;
	}

}