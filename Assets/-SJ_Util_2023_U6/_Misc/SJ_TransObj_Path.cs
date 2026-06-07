using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_TransObj_Path : MonoBehaviour 
{
	public	string	Name_TR_Obj = "TR_Path_Obj";

	[System.Serializable]
	public	class _TR_OBJ
	{
		public	Transform	tr;
		public  float		len; // 현재부터 다음노드까지의 거리
	}
	public	List<_TR_OBJ>		list_TR_OBJ;
	public  float				total_length;	// 계산된 총거리

	public	SJ_TransObj_Path	next_path;

	public	float			cur_moved;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public	void	Calc_Length()
	{
		list_TR_OBJ.Clear();
		for( int i =0 ; i < transform.childCount ; i++ )
		{
			Transform t = transform.GetChild(i);
			if( t.name.Contains( Name_TR_Obj ) )
			{
				_TR_OBJ s = new _TR_OBJ();
				s.tr = t;
				list_TR_OBJ.Add(s);
			}
		}

		if( list_TR_OBJ.Count < 2 ) return;

		total_length = 0;
		for( int i = 0 ; i < list_TR_OBJ.Count-1 ; i++ )
		{
			Transform tr_1 = list_TR_OBJ[i].tr;
			Transform tr_2 = list_TR_OBJ[i+1].tr;

			float len = Vector3.Magnitude( tr_1.localPosition - tr_2.localPosition );
			list_TR_OBJ[i].len = len;
			total_length += len;
		}
	}

	public	void	ShowMark( bool b )
	{
		for( int i = 0 ; i < list_TR_OBJ.Count ; i++ )
		{
			Transform tr_1 = list_TR_OBJ[i].tr;

			for( int o = 0; o < tr_1.childCount ; o ++ )
			{
				Transform t = tr_1.GetChild(o);
				t.gameObject.SetActive(b);
			}
		}
	}

	public	bool	Move_DeltaTime( ref Transform ref_tr , float speed )
	{
		// 첨부터 길이를 다 체크 , 어차피 한번만 하는거라.. 
		cur_moved += speed * Time.deltaTime;

		float	 step_len = 0;
		_TR_OBJ	 cur_s = null;
		int i = 0;
		foreach( _TR_OBJ s in list_TR_OBJ )
		{
			cur_s = s;
			if( cur_moved < step_len + s.len )break;
			step_len += s.len;
			i++;
		}

		if( i >= list_TR_OBJ.Count-2 )
		{
			float remain = cur_moved - total_length;
			if( next_path != null )
			{
				next_path.Move_DeltaTime( ref ref_tr , speed );
			}
			else
			{
				_TR_OBJ last = list_TR_OBJ[list_TR_OBJ.Count-1];

				ref_tr.position = last.tr.position;
				ref_tr.rotation = last.tr.rotation;
			}
			return true;
		}

		float remain_len = cur_moved - step_len;
		float r = remain_len / cur_s.len;

		_TR_OBJ	 cur_n = list_TR_OBJ[i+1];
		ref_tr.position = Vector3.Slerp( cur_s.tr.position , cur_n.tr.position , r );
		ref_tr.rotation = Quaternion.Slerp( cur_s.tr.rotation , cur_n.tr.rotation , r );

		return false;
	}


}
