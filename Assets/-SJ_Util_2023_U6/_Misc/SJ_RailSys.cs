using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_RailSys : MonoBehaviour 
{
	// static	public	SJ_RailSys	g_SJ_RailSys;

	// // 현재 있는 중심 1개 빼고 앞뒤로 몇개 여유 있는지.
	// // 그냥 앞에 보이는거.
	// public	int					margin_Count = 2;
	// public	List<SJ_RailObj>	list_default_SJ_RailObj;
	// public	float				speed = 0.1f; // 레일당 시간 속도.. 

	// public	List<SJ_RailObj>	list_SJ_RailObj_next;

	// public	List<SJ_RailObj>	list_SJ_RailObj_cur; // 초기에 배치 가능.. 위치는 자동으로 연결..
	// public	SJ_RailObj			SJ_RailObj_cur;
	// public	float				progress_cur = 0.5f;

	// public	bool				start = false;
	// public	bool				rail_move;
	// public	int					Total_Rail_View;

	// public	GameObject			go_Rot;
	// public	SJ_Anicurve_Time	sj_curve_rot;

	// // 샘플 무빙 모드 
	// public	_SJ_CurveTime		curveTime_SampleMove_C;


	// public	Vector3				rail_Pos;


	// Quaternion					rot_recent;

	// virtual	public	void		Pre_Start_Rail(){}
	// virtual	public	void		OnAdd_Rail(){}
	// virtual	public	GameObject	OnNext_Rail(){return null;}

	// private void Awake()
	// {

	// }

	// public	void	OnAwake()
	// {
	// 	g_SJ_RailSys = this;
	// 	sj_curve_rot.val_start	= 0;
	// 	sj_curve_rot.val_target = 1;
	// 	sj_curve_rot.val_cur	= 1;
	// }
	
	// virtual	public	void	Load_InstPool()
	// {
	// 	// 레일들 인스턴트풀.. 
	// 	foreach( SJ_RailObj s in list_default_SJ_RailObj )SJPool.Init_PoolObj( s.gameObject );

	// }
	
	// public	void	RailBlock_StartLink()
	// {
	// 	SJ_RailObj rail_before = null;
	// 	foreach (SJ_RailObj s in list_SJ_RailObj_cur)
	// 	{
	// 		if (rail_before != null) 
	// 			rail_before.Link(s);
	// 		rail_before = s;
	// 	}
		
	// 	// 중심에서 앞을 보게..
	// 	SJ_RailObj rail_center = null;

	// 	if( list_SJ_RailObj_cur.Count > 1 )
	// 		rail_center = list_SJ_RailObj_cur[ 1 ];
	// 	else if( list_SJ_RailObj_cur.Count > 0 )
	// 		rail_center = list_SJ_RailObj_cur[ 0 ];

	// 	if( rail_center == null ) 
	// 	{
	// 		Debug.Log( "레일블럭 없음~~~" );
	// 		return;
	// 	}

	// 	rail_center.OnProgress( progress_cur  , gameObject );
	// 	rail_center.OnProgress( progress_cur + 0.001f , gameObject );
	// 	SetCurRail( rail_center);
	// }

	// public	void	SetCurRail( SJ_RailObj rail )
	// {
	// 	SJ_RailObj_cur = rail;
	// 	OnSetCurRail();
	// }
	// // 새로운 레일이 현재레일(중심)로 세팅되었다.
	// virtual	public	void	OnSetCurRail(){}

	// public	void	Start_Rail()
	// {
	// 	start = true;
	// 	rail_move = true;
	// }

	// static	public	void	SetRailMove( bool b )
	// {
	// 	g_SJ_RailSys.rail_move = b;
	// }

	// public	void	Update_Moving_Prog()
	// {
	// 	if( progress_cur >= 1.0f )
	// 	{
	// 		progress_cur = progress_cur - (int)progress_cur;
	// 		NextRail_InstLink();
	// 	}
	// 	SJ_RailObj_cur.OnProgress( progress_cur , gameObject );
	// }

	// public	void	Rot_Update()
	// {
	// 	//SJ_Unity.SetEqTrans( go_Rot.transform , transform );
	// 	//sj_curve_rot.Update();
	// 	//go_Rot.transform.rotation = Quaternion.Slerp( rot_recent , transform.rotation , sj_curve_rot.val_cur );
	// }

	// void	NextRail_InstLink()
	// {
	// 	SJ_RailObj	rail_new_next =  Next_Rail();
	// 	SJ_RailObj	rail_cur_last = list_SJ_RailObj_cur[ list_SJ_RailObj_cur.Count-1 ];
	// 	SJ_RailObj	rail_cur_fst  = list_SJ_RailObj_cur[ 0 ];
	// 	SJ_RailObj_cur.On_RemoveRail();
	// 	rail_cur_last.Link( rail_new_next );
	// 	list_SJ_RailObj_cur.Add(rail_new_next);

	// 	SetCurRail( SJ_RailObj_cur.next_rail);

	// 	list_SJ_RailObj_cur.RemoveAt(0);
	// 	SJPool.ReturnInst(rail_cur_fst.gameObject);
	// 	OnAdd_Rail();
	// 	SJ_RailObj_cur.On_AddRail();

	// 	//rot_recent = go_Rot.transform.rotation;
	// 	//sj_curve_rot.StartTar_FromCur(1 , true);
	// }


	
	// public	Vector3	GetPosOffset_TweenLink( float offset_len )
	// {
	// 	return SJ_RailObj_cur.GetPosCur_Offset(offset_len);
	// }

	// /// <summary>
	// /// 현재트윈링크 이동거리로부터 다음 이동거리까지
	// /// 
	// /// </summary>
	// public	void	_Start_Move_TweenLink_TargetMove( float target_move , float time , GameObject recv , string func )
	// {
	// 	curveTime_SampleMove_C.val_s = SJ_RailObj_cur.itweenLink.moved_cur;
	// 	curveTime_SampleMove_C.val_e = target_move;
	// 	curveTime_SampleMove_C.func.Set( recv , func );
	// 	curveTime_SampleMove_C.time = time;
	// 	curveTime_SampleMove_C.Start();
	// }

	// static public void	Start_Move_TweenLink_TargetMove(float target_move , float time , GameObject recv , string func )
	// {
	// 	g_SJ_RailSys._Start_Move_TweenLink_TargetMove( target_move , time , recv , func );
	// }

	// public	void	Move_TweenLink( float	_speed = -1 )
	// {
	// 	if( curveTime_SampleMove_C.CheckPlay() )
	// 	{
	// 		curveTime_SampleMove_C.Update();
	// 		float len_move = curveTime_SampleMove_C.ValueLerp();
	// 		//Debug.Log( "curveTime_SampleMove_C :" + len_move );
	// 		SJ_RailObj_cur.Move_TweenLink_SetDist( ref rail_Pos , len_move );
	// 	}else{
	// 		if( rail_move == false ) return;

	// 		if( _speed > -1 ) speed = _speed;
	// 		if( SJ_RailObj_cur.Move_TweenLink( ref rail_Pos  , speed ) )
	// 		{
	// 			NextRail_InstLink();
	// 		}
	// 	}
	// }

	// static	public	void	Add_Next_Rail( SJ_RailObj rail )
	// {
	// 	g_SJ_RailSys.list_SJ_RailObj_next.Add(rail);
	// }

	// public	SJ_RailObj	Next_Rail()
	// {
	// 	GameObject go = OnNext_Rail();
	// 	if( go != null ) return Inst_Rail( go );
		
	// 	SJ_RailObj rail = SJ_Unity.GetArray_Random<SJ_RailObj>( list_default_SJ_RailObj.ToArray() );
	// 	if( list_SJ_RailObj_next.Count > 0 )
	// 	{
	// 		rail = list_SJ_RailObj_next[0];
	// 		list_SJ_RailObj_next.RemoveAt(0);
	// 	}
	// 	return Inst_Rail( rail.gameObject );
	// }
	
	// public	SJ_RailObj	Inst_Rail( GameObject go )
	// {
	// 	return SJPool.GetNewInst( go ).GetComponent<SJ_RailObj>();
	// }

	// public	SJ_RailObj	Inst_Rail_Default()
	// {
	// 	return Inst_Rail( SJ_Unity.GetArray_Random<SJ_RailObj>( list_default_SJ_RailObj.ToArray() ).gameObject );
	// }

	// public	SJ_RailObj GetRailObj_OffsetLen( float offset_length , ref float _prog )
	// {
	// 	float	f = (offset_length / SJ_RailObj_cur.length ) + progress_cur;
	// 	int		idx = (int)f;
	// 	float	prog = f - idx;
	// 	if( f < 0 )
	// 	{
	// 		idx--;
	// 		prog = 1.0f - prog;
	// 	}

	// 	SJ_RailObj rail = list_SJ_RailObj_cur[ 1 + idx ]; // 1은 무조건 가운데 레일..
	// 	return rail;
	// }

	// public	Vector3	_GetPos_FromCurPos( float offset_length )
	// {
	// 	float _prog = 0;
	// 	SJ_RailObj rail = GetRailObj_OffsetLen(offset_length , ref _prog);
	// 	return rail.GetPos( _prog );
	// }

	// static	public	Vector3	GetPos_FromCurPos( float offset_length )
	// {
	// 	return g_SJ_RailSys._GetPos_FromCurPos( offset_length );
	// }

}
