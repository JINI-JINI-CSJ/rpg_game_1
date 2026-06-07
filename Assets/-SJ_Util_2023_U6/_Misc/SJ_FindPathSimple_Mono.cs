using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_FindPathSimple_Mono : MonoBehaviour
{
	public	List<SJ_FindPathSimple_Mono> lt_Neighb;


	public List<SJ_FindPathSimple_Mono>	FindPath( SJ_FindPathSimple_Mono end , int max_depth = 30 )
	{
		List<SJ_FindPathSimple_Mono> lt_path = new List<SJ_FindPathSimple_Mono>();
		return	this.FindPath( lt_path , end , 0 , max_depth);
	}

	public List<SJ_FindPathSimple_Mono> FindPath(List<SJ_FindPathSimple_Mono> lt_path , SJ_FindPathSimple_Mono end , int cur_depth , int max_depth )
	{
		if(cur_depth >= max_depth)
		{
			Debug.LogError("Error !!!! SJ_FindPathSimple_Mono MaxDepth!!!!!" );
			return lt_path;
		}
		lt_path.Add(this);
		if ( this == end ) return lt_path;

		float near_len = float.MaxValue;
		SJ_FindPathSimple_Mono near = null;
		foreach (SJ_FindPathSimple_Mono s in lt_Neighb)
		{
			float	len = Vector3.SqrMagnitude( end.transform.position - transform.position );
			if(len < near_len)
			{
				near_len = len;
				near = s;
			}
		}
		return near.FindPath(lt_path , end , ++cur_depth, max_depth);
	}

	private void OnDrawGizmos()
	{
		foreach (SJ_FindPathSimple_Mono s in lt_Neighb)
		{
			Gizmos.DrawLine( transform.position , s.transform.position );
		} 
	}

}
