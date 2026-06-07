using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_IKLoockPos : MonoBehaviour
{
	public	Animator	anit;
	public	Transform	tr_Target;
	public	SJ_Curve	sj_curve;

	public	GameObject	go_Coll_Event;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		sj_curve.UpdateCurve();
	}


	public	void	Start_LookAtPos( bool b )
	{
		sj_curve.StarTimeCurve_Cur( b );
	}

	private void OnAnimatorIK(int layerIndex)
	{
		if( sj_curve.val_cur < 0.01f  || tr_Target == null )return;
		anit.SetLookAtWeight( sj_curve.val_cur );
		anit.SetLookAtPosition( tr_Target.position );
	}

	private void OnTriggerEnter(Collider other)
	{
		if( go_Coll_Event == null ) return;

		if( other.gameObject == go_Coll_Event )
		{
			tr_Target = go_Coll_Event.transform;
			Start_LookAtPos( true );
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if( go_Coll_Event == null ) return;
		if( other.gameObject == go_Coll_Event )
		{
			Start_LookAtPos( false );
		}
	}

}
