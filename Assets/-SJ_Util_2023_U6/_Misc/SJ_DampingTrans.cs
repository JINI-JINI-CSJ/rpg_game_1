using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_DampingTrans : MonoBehaviour 
{
	public	Transform		tr_target;
	public	float			damping_lerp = 0.5f;
	public	Vector3			offset;
	public	bool			UpdatePrc;
	//public	float			max_radius;

	// Use this for initialization
	void Start () {
		
	}
	


	public	void	SetTrans_Active( Transform tr )
	{
		transform.position = tr.position;
		transform.rotation = tr.rotation;
		enabled = true;
	}

	public	void	Update_Pos()
	{
		Vector3 pos_tar = tr_target.position + offset;
		Vector3 v		= Vector3.Lerp( transform.position , pos_tar , damping_lerp );
		Vector3 vc = pos_tar - v;

		transform.position		= v;
		transform.rotation		= Quaternion.Slerp( transform.rotation , tr_target.rotation , damping_lerp );
		return;
	}


	// Update is called once per frame
	void Update () 
	{
		if( UpdatePrc )Update_Pos();
	}
}
