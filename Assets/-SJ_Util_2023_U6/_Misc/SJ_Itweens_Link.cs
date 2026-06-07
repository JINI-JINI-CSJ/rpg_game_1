using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_Itweens_Link : MonoBehaviour 
{
	// [System.Serializable]
	// public	class _Tween_Obj
	// {
	// 	public	iTweenPath	tween;
	// 	public	List<float>	nodes_per_Length;  // 각 노드당 길이
	// 	public	List<float>	nodes_per_percent; // 각 노드당 비율
	// 	public	float		length_tween;

	// 	public	float	GetLengthRatio( float move )
	// 	{
	// 		int		idx = 0;
	// 		float	acc_per = 0;
	// 		foreach( float len in nodes_per_Length )
	// 		{
	// 			if( move < len ) break;
	// 			move -= len;
	// 			acc_per += nodes_per_percent[idx];
	// 			idx++;
	// 		}

	// 		// 찾은 구간에서 다시 비율계산
	// 		float cur_r = move / nodes_per_Length[idx];
	// 		cur_r = cur_r / nodes_per_percent[idx];

	// 		return acc_per += cur_r;
	// 	}
	// }
	// public List<_Tween_Obj>	list_iTween_Obj;
	// public	float	total_length;

	// public	SJ_Itweens_Link	next_Obj;

	// [HideInInspector]
	// public	float			moved_cur;


	// public	Quaternion	qt_CurMove;
	// public	bool		use_qt_1;
	// public	bool		use_qt_2;

	// // Use this for initialization
	// void Start () {
		
	// }
	
	// // Update is called once per frame
	// void Update () {
		
	// }

	// public	void	SetNextLink( SJ_Itweens_Link next )
	// {
	// 	next_Obj = next;
	// }

	// // 두개의 트윈을 부드럽게 연결
	// public	void	Link_Lerp( iTweenPath s1 , iTweenPath s2 )
	// {
	// 	if( s1.nodeCount < 2 || s2.nodeCount < 2 )return;

	// 	Vector3 pos_s1_before =	s1.nodes[s1.nodeCount-2];
	// 	Vector3 pos_s1_center =	s1.nodes[s1.nodeCount-1];

	// 	Vector3 pos_s2_next =	s2.nodes[1];

	// 	s2.nodes[0] = pos_s1_center;

	// 	Vector3 dir_s1_dir = pos_s1_center - pos_s1_before;
	// 	dir_s1_dir.Normalize();

	// 	float len_s2 = Vector3.Magnitude( pos_s2_next - pos_s1_center );
	// 	s2.nodes[1] = pos_s1_center + (dir_s1_dir*len_s2);
	// }


	// public	void	PreCalc()
	// {
	// 	total_length = 0;
	// 	for( int i = 0 ; i < list_iTween_Obj.Count ; i++ )
	// 	{
	// 		_Tween_Obj s1 = list_iTween_Obj[i];
	// 		if( i <  list_iTween_Obj.Count-1)
	// 		{
	// 			_Tween_Obj s2 = list_iTween_Obj[i+1];
	// 			Link_Lerp(s1.tween,s2.tween);
	// 		}

	// 		//PreCalc_Length(s1);
	// 		s1.length_tween = iTween.PathLength( s1.tween.nodes.ToArray() );
	// 		total_length += s1.length_tween;
	// 	}
	// }

	// // Itween 거리계산이 아니라
	// // 그냥 근사치
	// // Itween 은 노드를 다시 스무스 연계점 생성해서 길이를 계산하는거
	// public	void	PreCalc_Length( _Tween_Obj s )
	// {
	// 	s.nodes_per_Length.Clear();
	// 	s.nodes_per_percent.Clear();

	// 	float t = 0;
	// 	for( int i = 0 ; i < s.tween.nodes.Count -1 ; i++ )
	// 	{
	// 		Vector3 v1 = s.tween.nodes[i];
	// 		Vector3 v2 = s.tween.nodes[i+1];
	// 		float len = Vector3.Distance( v1 , v2 );
	// 		s.nodes_per_Length.Add(len);
	// 		t += len;
	// 	}

	// 	foreach( float l in s.nodes_per_Length )
	// 	{
	// 		s.nodes_per_percent.Add( l / t );
	// 	}

	// 	s.length_tween = t;
	// }

	// public	void	Init()
	// {
	// 	moved_cur = 0;
	// 	PreCalc();
	// }

	// public	Vector3	Sample( float r )
	// {
	// 	if( total_length < 0.001f ) PreCalc();

	// 	_Tween_Obj	cur_s = null;
	// 	float		step_c = 0;

	// 	float c_ratio = 1;
	// 	foreach( _Tween_Obj s in list_iTween_Obj )
	// 	{
	// 		cur_s = s;
	// 		c_ratio = s.length_tween / total_length;
	// 		if( r <= c_ratio + step_c ) break;
	// 		step_c += c_ratio;
	// 	}

	// 	// r 에서 전체 남은 비율
	// 	float remain_all_r = r - step_c;

	// 	// r 에서 현재 트윈기준 남은 비율
	// 	float remain_cur_r = remain_all_r / c_ratio;
	// 	Vector3 pos = iTween.PointOnPath( cur_s.tween.nodes.ToArray() , remain_cur_r );
	// 	pos = transform.TransformPoint( pos );
	// 	return pos;
	// }

	// public	bool	Move( ref Vector3 ref_pos , float speed , float remain_move = 0 )
	// {
	// 	moved_cur += speed * Time.deltaTime + remain_move;
	// 	Vector3 pos = Vector3.zero;
	// 	bool b = GetPos( moved_cur , ref pos );
	// 	ref_pos = pos;

	// 	return b;
	// }

	// public	bool	Move_SetDist( ref Vector3 ref_pos , float _move )
	// {
	// 	moved_cur = _move;
	// 	Vector3 pos = Vector3.zero;
	// 	bool b = GetPos( moved_cur , ref pos );
	// 	ref_pos = pos;

	// 	return b;
	// }

	// public	bool GetPos( float moved_len ,ref Vector3  ref_pos )
	// {
	// 	float cur_r = 0;
	// 	float move_len_r = moved_len;
	// 	SJ_Itweens_Link sj_tw_link = null;
	// 	_Tween_Obj		itween_find = null;
	// 	bool b = FindTween_Dist( ref move_len_r , ref itween_find , ref sj_tw_link );
	// 	if( itween_find == null ) return true;
	// 	cur_r = move_len_r / itween_find.length_tween;

	// 	ref_pos = iTween.PointOnPath(itween_find.tween.nodes.ToArray() , cur_r );
	// 	ref_pos = sj_tw_link.transform.TransformPoint( ref_pos );


	// 	qt_CurMove = itween_find.tween.Get_Rot( cur_r ,ref use_qt_1 , ref use_qt_2 );
	// 	qt_CurMove =sj_tw_link.transform.rotation * qt_CurMove;

	// 	return b;
	// }

	// //public	bool GetRot( float moved_len ,ref Quaternion  ref_rot , ref bool b1 , ref bool b2 )
	// //{
	// //	float cur_r = 0;
	// //	float move_len_r = moved_len;
	// //	SJ_Itweens_Link sj_tw_link = null;
	// //	_Tween_Obj		itween_find = null;
	// //	bool b = FindTween_Dist( ref move_len_r , ref itween_find , ref sj_tw_link );
	// //	if( itween_find == null ) return true;
	// //	cur_r = move_len_r / itween_find.length_tween;

	// //	ref_rot = itween_find.tween.Get_Rot( cur_r ,ref b1 , ref b2 );
	// //	ref_rot =sj_tw_link.transform.rotation * ref_rot;
	// //	return b;
	// //}

	// public	bool	SampleDist( float r , ref float ref_move , ref Vector3 ref_pos )
	// {
	// 	ref_move = total_length * r;
	// 	return GetPos( ref_move , ref ref_pos );
	// }



	// public	bool	FindTween_Dist( ref float dist , ref _Tween_Obj itween , ref SJ_Itweens_Link link )
	// {
	// 	itween = null;
	// 	foreach( _Tween_Obj s in list_iTween_Obj )
	// 	{
	// 		if( dist <= s.length_tween )
	// 		{
	// 			itween = s;
	// 			link = this;
	// 			break;
	// 		}
	// 		dist -= s.length_tween;
	// 	}

	// 	if( itween == null && next_Obj != null )
	// 	{
	// 		next_Obj.FindTween_Dist( ref dist , ref itween , ref link );
	// 		return true;
	// 	}

	// 	return false;
	// }


	// public	void	FitHeight_Terrain( Terrain terrain )
	// {
	// 	foreach( _Tween_Obj s in list_iTween_Obj )
	// 	{
	// 		FitHeight_Terrain_Prc( terrain , s.tween );
	// 	}
	// }


	// public	void	FitHeight_Terrain_Prc( Terrain terrain , iTweenPath tween )
	// {
	// 	for (int i = 0; i < tween.nodeCount; i++)
	// 	{
	// 		Vector3 pos = tween.nodes[i];
	// 		pos.y = terrain.SampleHeight( tween.nodes[i] );
	// 		tween.nodes[i] = pos;
	// 	}
	// }

	// public	void	GetPos_FstLast( ref Vector3 pos_fst , ref Vector3 pos_last )
	// {
	// 	if( list_iTween_Obj.Count < 1 ) return;

	// 	iTweenPath tw_1 = list_iTween_Obj[0].tween;
	// 	iTweenPath tw_2 = list_iTween_Obj[list_iTween_Obj.Count-1].tween;

	// 	pos_fst  = tw_1.nodes[0];
	// 	pos_last = tw_2.nodes[ tw_2.nodeCount - 1 ];
	// }

}
