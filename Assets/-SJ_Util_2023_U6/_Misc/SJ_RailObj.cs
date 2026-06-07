using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_RailObj : MonoBehaviour 
{
	// public	SJ_Itweens_Link		itweenLink;

	// //
	// public	Transform	tr_Link_Head;
	// public	Transform	tr_Link_Tail;

	// public	float		length;
	// public	SJ_RailObj	next_rail;
	// public	List<SJ_RailEventObj>	list_ReadyEvent;

	// public	Terrain		terrain;

	// [System.Serializable]
	// public	class _ITWEEN_RANDOM_CREATE
	// {
	// 	public	string		Desc;

	// 	[System.Serializable]
	// 	public	class _ITWEEN_RANDOM_CREATE_OBJ
	// 	{
	// 		public	int				per;
	// 		public	GameObject		prf;
	// 	}
	// 	public	List<_ITWEEN_RANDOM_CREATE_OBJ>	list_ITWEEN_RANDOM_CREATE_OBJ;
	// 	public	Transform		tr_Par_itween_RandObj;
	// 	public	int				total_Create_Obj;
	// 	public	float			radius_Pos_itween_RandObj;

	// 	public	float			tween_ratio_min = 0;
	// 	public	float			tween_ratio_max = 1;

	// 	public	bool			only_Side;			// 양쪽에만 배치
	// 	public	float			side_min = 1;
	// 	public	float			side_max = 5;

	// 	public	void	Create_ITweenRandObj( SJ_RailObj sj_RailObj )
	// 	{
	// 		List<int>	list_per = new List<int>();
	// 		foreach( _ITWEEN_RANDOM_CREATE_OBJ s in list_ITWEEN_RANDOM_CREATE_OBJ )
	// 		{
	// 			list_per.Add(s.per);
	// 		}
	// 		SJ_Unity.Delete_Child( tr_Par_itween_RandObj );
	// 		for( int i = 0 ; i < total_Create_Obj ; i++ )
	// 		{
	// 			Create_ITweenRandObj_Prc( sj_RailObj , list_per );
	// 		}
	// 	}

	// 	public	void	Create_ITweenRandObj_Prc( SJ_RailObj sj_RailObj , List<int>	list_per )
	// 	{
	// 		float f_tween = UnityEngine.Random.Range( tween_ratio_min , tween_ratio_max );
	// 		Vector3 pos = sj_RailObj.itweenLink.Sample( f_tween );

	// 		if( only_Side == false )
	// 		{
	// 			// 중앙에서 자유롭게 랜덤
	// 			Vector3 pos_offset = SJ_Cood.Random_SphereBound( radius_Pos_itween_RandObj );
	// 			pos += pos_offset;
	// 		}
	// 		else
	// 		{
	// 			// 양측 끝쪽만 랜덤
	// 			Vector3 pos_front = sj_RailObj.itweenLink.Sample( f_tween + 0.05f );
	// 			Vector3 dir = pos_front - pos;
	// 			dir.Normalize();

	// 			int left_right = UnityEngine.Random.Range(0,2);
	// 			if( left_right == 0 ) left_right = -1;
	// 			dir = Quaternion.Euler( 0, 90 * left_right , 0 ) * dir;

	// 			Vector3 pos_offset = dir * UnityEngine.Random.Range(side_min,side_max);
	// 			pos += pos_offset;
	// 		}

	// 		pos.y = 0;

	// 		if( sj_RailObj.terrain != null )
	// 		{
	// 			pos.y =	sj_RailObj.terrain.SampleHeight( pos );
	// 		}

	// 		int idx = SJ_Unity.Random_RangeStepList( list_per.ToArray() );
	// 		GameObject inst_go = SJPool.GetNewInst_Or_Create(list_ITWEEN_RANDOM_CREATE_OBJ[idx].prf);
	// 		inst_go.transform.parent = tr_Par_itween_RandObj;
	// 		inst_go.transform.localPosition = pos;
	// 	}
	// }

	// public	List<_ITWEEN_RANDOM_CREATE>	list_ITWEEN_RANDOM_CREATE;


	// // Use this for initialization
	// void Start () {
		
	// }
	
	// // Update is called once per frame
	// void Update () {
		
	// }

	// private void Awake()
	// {
	// 	SJ_RailEventObj[]	re = GetComponentsInChildren<SJ_RailEventObj>();
	// 	list_ReadyEvent.AddRange(re);
	// }

	// public	void	Editor_PreWork_SJ()
	// {
	// 	itweenLink.Init();
	// 	length = itweenLink.total_length;
	// }

	// public	void	AddRail()
	// {
	// 	foreach( SJ_RailEventObj s in list_ReadyEvent )s.Ready();
	// 	On_AddRail();
	// }

	// public	void	RemoveRail()
	// {
	// 	foreach( SJ_RailEventObj s in list_ReadyEvent )s.End();
	// 	On_RemoveRail();
	// }

	// virtual	public	void	On_AddRail(){}
	// virtual	public	void	On_RemoveRail(){}


	// public	void	OnProgress( float progress , GameObject go_play )
	// {
	// 	Vector3		cur_pos = GetPos( progress );
	// 	go_play.transform.LookAt( cur_pos );
	// 	go_play.transform.position = cur_pos;

	// 	foreach( SJ_RailEventObj s in list_ReadyEvent )
	// 	{
	// 		s.Check_Active( progress );
	// 	}
	// }

	// public	Vector3	GetPos( float progress )
	// {
	// 	Vector3		cur_pos = itweenLink.Sample( progress );

	// 	if( tr_Link_Head == null )
	// 		cur_pos = transform.TransformPoint( cur_pos );
	// 	else
	// 		cur_pos = tr_Link_Head.TransformPoint( cur_pos );
	// 	return cur_pos;
	// }

	// public	void	Link( SJ_RailObj rail_tail )
	// {
	// 	if( tr_Link_Tail == null ) return;

	// 	if( tr_Link_Head == null )
	// 		rail_tail.transform.SetPositionAndRotation( tr_Link_Tail.position , tr_Link_Tail.rotation );
	// 	else
	// 		rail_tail.tr_Link_Head.SetPositionAndRotation( tr_Link_Tail.position , tr_Link_Tail.rotation );

	// 	next_rail = rail_tail;

	// 	itweenLink.SetNextLink( rail_tail.itweenLink );
	// }
	
	// public	void	Create_ITweenRandObj()
	// {
	// 	UnityEngine.Random.InitState( (int)System.DateTime.Now.Ticks );
	// 	foreach( _ITWEEN_RANDOM_CREATE s in list_ITWEEN_RANDOM_CREATE )
	// 	{
	// 		s.Create_ITweenRandObj(this);
	// 	}
	// 	OnEnd_Create_ITweenRandObj();
	// }

	// virtual public	void	OnEnd_Create_ITweenRandObj(){}
	


	// public	bool	Move_TweenLink( ref Vector3 ref_pos ,  float	speed )
	// {
	// 	return itweenLink.Move( ref ref_pos , speed );
	// }


	// public	bool	Move_TweenLink_SetDist( ref Vector3 ref_pos ,  float	move )
	// {
	// 	return itweenLink.Move_SetDist( ref ref_pos , move );
	// }

	// public	Vector3 GetPosCur_Offset( float offset_len )
	// {
	// 	Vector3 pos = Vector3.zero;
	// 	itweenLink.GetPos( itweenLink.moved_cur + offset_len , ref pos );
	// 	return pos;
	// }

	// public	void	FitHeight_Terrain_Itween()
	// {
	// 	itweenLink.FitHeight_Terrain( terrain );

	// 	SetLinkPos_ByTween();
	// }

	// // 위치 보정 작업은 이 객체가 원점에 있을때 해야 한다.
	// public	void	SetLinkPos_ByTween()
	// {
	// 	Vector3 pos_fst  = Vector3.zero;
	// 	Vector3 pos_last = Vector3.zero;

	// 	itweenLink.GetPos_FstLast(ref pos_fst , ref pos_last);
	// 	if( tr_Link_Head != null ) tr_Link_Head.position = pos_fst;
	// 	if( tr_Link_Tail != null ) tr_Link_Tail.position = pos_last;
	// }

}

