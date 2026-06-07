using UnityEngine;
using System.Collections;

public class SJ_LookAt : MonoBehaviour
{
	public	bool		play;

	public	Transform		tr_tar;
	public	Vector3			pos_Tar;
	public	_SJ_CurveTime	sjcv;

	public	bool			play_Once;
	public	bool			byMoving;	// 이동했던 방향으로 바라보기 
	public	float			byMoving_Timing = 0.1f;
	float					byMoving_Timing_cur;
	public	bool			Y_Zero;
	public	bool			followMode;

	public	bool			noUpdate;


	bool				updated;
	Vector3				pos_Before = Vector3.zero;

	Quaternion			rot_cur = Quaternion.identity;
	Quaternion			rot_next = Quaternion.identity;

	public	float		followMode_slerp = 5;

	// 룩엣 Z 축  롤
	// 이 객체의 하위로 있어야 한다.
	public	Transform	tr_Roll;
	public	float		roll_Angle;

	// 한번만 기본 바라보기
	public	bool		order_One;

	public	void	Start_Common( float arg_time )
	{
		//Debug.Log( "SJ 룩엣 Start_Common ~~~~~~~~~~~~~~~~~~~~" );

		enabled = true;
		play = true;
		if( arg_time > 0.0001f )
			sjcv.time = arg_time;
		sjcv.Start();
	}

	public	void	Set_Target( Transform tr )
	{
		tr_tar = tr;
	}


	// 다른 세팅 유지하면서 타겟 한번만 바라보기
	public	void	LookAt_OneOrder( float arg_time  , Vector3 _tar_w )
	{
		Start_Common( arg_time );
		order_One = true;

		TargetLookAt(_tar_w);
	}

	public	void	Start_LookAt( float arg_time  , Vector3 _tar_w , bool _followMode = false , bool noTargetNull = false )
	{
		Start_Common( arg_time );
		pos_Tar = transform.InverseTransformPoint( _tar_w );

		if( noTargetNull == false )
			tr_tar = null;

		followMode = _followMode;

		TargetLookAt(_tar_w);
	}


	void	TargetLookAt(Vector3 _tar_w)
	{
		rot_cur = transform.rotation;
		Vector3 dir_target = _tar_w - transform.position;
		if( Y_Zero ) dir_target.y = 0;
		rot_next.SetFromToRotation( Vector3.forward , dir_target );
	}


	public	void	Start_LookAt( float arg_time  , Transform _tr_tar , bool _followMode = false )
	{
		Vector3 pos = Vector3.zero;

		tr_tar = _tr_tar;
		if( _tr_tar != null )
		{
			pos = _tr_tar.position;

		}
		else
		{
			// 로컬회전 기본값의 앞바라보는 방향구하기
			// 1. 현재 월드기준 바라보는 방향
			// 2. 위값을 로컬회전의 역행렬

			Quaternion	inv_local_rot =	Quaternion.Inverse(	transform.localRotation );
			Matrix4x4	mt = Matrix4x4.Rotate( inv_local_rot );
			Vector3		dir_w =	mt.MultiplyPoint3x4( transform.forward );
			pos = transform.position + dir_w;
		}

		Start_LookAt( arg_time ,pos , _followMode , true );
	}



	public	void	UpdateMovingLookAt()
	{
		if( updated )
		{
			byMoving_Timing_cur += Time.deltaTime;
			if( byMoving_Timing_cur >= byMoving_Timing )
			{
				byMoving_Timing_cur -= byMoving_Timing;
			
				Vector3 pos_dir = transform.position - pos_Before;
				if (Y_Zero) pos_dir.y = 0;
				if (pos_dir.magnitude > 0.01f)
				{
					pos_dir.Normalize();
					rot_next.SetLookRotation(pos_dir);
				}
				else
				{
					//Debug.Log( "Look at No : " + pos_dir );
				}
				pos_Before = transform.position;
			}
		}
		else
		{
			rot_next = transform.rotation;
		}
		transform.rotation = Quaternion.Slerp(transform.rotation, rot_next, followMode_slerp * Time.deltaTime);


		updated = true;
	}

	public	void Update_LookAt()
	{
		if( play == false )return;

		if( order_One )
		{
			if( LookAt_Default() )order_One = false;
			return;
		}

		if( byMoving )
		{
			UpdateMovingLookAt();
			return;
		}

		if( followMode )
		{
			Vector3 dir_target = tr_tar.position - transform.position;
			if( Y_Zero ) dir_target.y = 0;
			rot_next.SetLookRotation( dir_target , Vector3.up );
			transform.rotation = Quaternion.Slerp( transform.rotation , rot_next , followMode_slerp * Time.deltaTime );
			return;
		}

		LookAt_Default();
	}

	public	bool	LookAt_Default()
	{
		float r =	sjcv.Update();
		transform.rotation = Quaternion.Slerp( rot_cur , rot_next , r );
		if( Y_Zero ) 
		{
			Vector3 angle = transform.rotation.eulerAngles;
			angle.z = 0;
			transform.rotation = Quaternion.Euler( angle );
		}
		if( tr_Roll != null )
		{
			tr_Roll.localRotation = Quaternion.Slerp( tr_Roll.localRotation , Quaternion.Euler(0,0,roll_Angle) , followMode_slerp * Time.deltaTime );
		}
		if( sjcv.Check_End() )
		{
			if( play_Once )
			{
				play = false;				
				enabled = false;
			}
			return true;
		}

		return false;
	}

	public	void	Set_RollAngle( float ang ) { roll_Angle = ang; }

	// Update is called once per frame
	void Update ()
	{
		if( noUpdate ) return;

		if( byMoving == false )
			Update_LookAt();
	}

	//private void FixedUpdate()
	//{
	//	Debug.Log( "FixedUpdate : transform.position : " + transform.position );

	//	//if( byMoving == false )
		

	//	//if( updated )
	//	//{


	//	//	Vector3 pos_dir = transform.position - pos_Before;

	//	//	Debug.Log( "transform.position : " + transform.position + " : " + pos_Before );

	//	//	if( Y_Zero ) pos_dir.y = 0;
	//	//	if( pos_dir.magnitude > 0.01f )
	//	//	{
	//	//		pos_dir.Normalize();
	//	//		rot_next.SetFromToRotation( Vector3.forward , pos_dir );
	//	//		//transform.rotation = Quaternion.Slerp( transform.rotation , rot_next , followMode_slerp * Time.deltaTime );

	//	//		Debug.Log( "Look at pos_dir : " + pos_dir );
	//	//	}
	//	//	else
	//	//	{
	//	//		Debug.Log( "Look at No : " + pos_dir );
	//	//	}
	//	//}
	//	//else
	//	//{
	//	//	rot_next = transform.rotation;
	//	//}
	//	//pos_Before = transform.position;
	//	//updated = true;
	//}
}
