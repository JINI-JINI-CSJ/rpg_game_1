using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_LerpKeyObj : MonoBehaviour
{
	[Space]
	[Header ("[기본정보]") ]
	public	bool	use_Pos = true;
	public	bool	use_Rot;
	public	bool	use_LookAt;
	public	bool	use_RollAngle;
	public	float	blend_Time=-1;
	[Header ("[우선순위]") ]
	public	int		Priority;	

	public	AnimationCurve	curve;
	public	_SJ_GO_FUNC		endFunc;

	public	bool		use_sample_Pos;
	public	Vector3		sample_Pos;
	public	bool		use_sample_Rot;
	public	Quaternion	sample_Rot;
	public	bool		use_sample_LookAt;
	public	Vector3		sample_LookAt;
	public	bool		use_sample_RollAngle;
	public	float		sample_RollAngle;

	public	SJ_LerpKeySys	par_sys;

	public	float		StartKeyTime;

	// 이번 프레임에 호출 한번만 되게..
	[HideInInspector]
	public	bool		update_Framed;

	[HideInInspector]
	public	bool		plaing;

	private void Awake()
	{
	}

	public	void	Func_End()
	{
		plaing = false;
		endFunc.Func(this);
	}


	public	float	ElapseTime(){return Time.time - StartKeyTime;}

	public	bool	Enter_Key( SJ_LerpKeySys tar_sys = null , bool resume = false , MonoBehaviour mono = null , string func = "" )
	{
		Debug.Log( "SJ_LerpKeyObj 엔터키 : " + name );

		if( tar_sys == null ) tar_sys = SJ_LerpKeySys.g_default;
		if( tar_sys == null ) return false;
		tar_sys.AddKey(this , blend_Time , resume , mono , func );
		par_sys = tar_sys;

		if( resume == false )
			OnStartLerp();
		return true;
	}

	virtual	public	void	OnStartLerp(){}

	virtual	public	void OnEndLerp() { }

	public	void	Blend( _SJ_LERP_KEY_LAYER par_layer , float ratio , SJ_LERP_KEY_ACTION t )
	{
		float cr = curve.Evaluate( ratio );

		//if( use_Pos )
		if( t == SJ_LERP_KEY_ACTION.Pos )
		{ 
			//Debug.Log( "Blend : " + cr );
			par_layer.sys.pos_cur				=	Vector3.Lerp( par_layer.default_Key.keyobj.Update_Pos()		, Update_Pos() , cr );
		}
		//if( use_Rot ) 
		if( t == SJ_LERP_KEY_ACTION.Rot )
			par_layer.sys.rot_cur				=	Quaternion.Lerp( par_layer.default_Key.keyobj.Update_Rot()	, Update_Rot() , cr );
		//if( use_LookAt ) 
		if( t == SJ_LERP_KEY_ACTION.LookAt )
			par_layer.sys.lookAt_cur		=	Vector3.Lerp( par_layer.default_Key.keyobj.Update_LookAt()	, Update_LookAt() , cr );
		//if( use_RollAngle ) 
		if( t == SJ_LERP_KEY_ACTION.RollAngle )
			par_layer.sys.rollAnlge_cur =	Mathf.Lerp(par_layer.default_Key.keyobj.Update_RollAngle()	, Update_RollAngle() , cr );
	}

	public	void	Blend_Value( _SJ_LERP_KEY_LAYER par_layer , float ratio , SJ_LERP_KEY_ACTION t )
	{
		float cr = curve.Evaluate( ratio );
		//if( use_Pos )
		if( t == SJ_LERP_KEY_ACTION.Pos )
		{
			//Debug.Log( "Blend_Value : " + cr );
			par_layer.sys.pos_cur				=	Vector3.Lerp( par_layer.sys.pos_first		, Update_Pos() , cr );
		}
		//if( use_Rot ) 
		if( t == SJ_LERP_KEY_ACTION.Rot )
			par_layer.sys.rot_cur				=	Quaternion.Lerp( par_layer.sys.rot_first	, Update_Rot() , cr );
		//if( use_LookAt ) 
		if( t == SJ_LERP_KEY_ACTION.LookAt )
			par_layer.sys.lookAt_cur		=	Vector3.Lerp( par_layer.sys.lookAt_first	, Update_LookAt() , cr );
		//if( use_RollAngle ) 
		if( t == SJ_LERP_KEY_ACTION.RollAngle )
			par_layer.sys.rollAnlge_cur =	Mathf.Lerp(par_layer.sys.rollAnlge_first	, Update_RollAngle() , cr );
	}



	// 프레임당 한번 호출
	virtual	public	void Update_CurFrame() { }

	virtual	public	Vector3	Update_Pos()
	{
		if( use_sample_Pos ) return sample_Pos;
		return transform.position;
	}

	virtual	public	Quaternion Update_Rot() 
	{ 
		if( use_sample_Rot ) return sample_Rot;
		return transform.rotation; 
	}

	virtual	public	Vector3	Update_LookAt()
	{ 
		if( use_sample_LookAt ) return sample_LookAt;
		return transform.position;
	}

	virtual	public	float	Update_RollAngle()
	{ 
		if( use_sample_RollAngle ) return sample_RollAngle;
		return 0;
	}


	public	Quaternion	LookAtRot( Vector3 self , Vector3 tar , bool roll_zero = true )
	{
		return	Quaternion.LookRotation( tar - self );
	}

}
