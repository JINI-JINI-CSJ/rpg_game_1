using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_RandomObjBatch : MonoBehaviour 
{
	public	Transform			tr_par;
	public	List<GameObject>	list_obj;
	public	Vector3				pos_bb;
	public	int					count;

	public BoxCollider box_col;

	public	void		Random_CreateBatch()
	{
		SJ_Unity.Delete_Child(tr_par);
		SJ_Unity.Random_CreateBatch( tr_par , pos_bb , count , list_obj);
	}

	public void         Random_PosOnly_BoxColl()
	{
		if( box_col == null )
		{
			Debug.LogError( "박스 콜라이더 없음" );
		 	return;
		}

		foreach( var s in list_obj )
		{
		   s.transform.position =	SJ_Cood.Random_BoxBound( box_col );
		}

	}
}
