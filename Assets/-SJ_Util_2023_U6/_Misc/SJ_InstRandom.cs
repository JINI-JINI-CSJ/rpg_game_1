using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_InstRandom : MonoBehaviour
{
	public	float			radius = 1.0f;
	public	float			time_total = 5.0f;
	public	float			time_term = 1.0f;
	public	GameObject		go_PoolObj;

	public	_SJ_GO_FUNC		sjFunc;



	public	void	Start_Random()
	{
		//Debug.Log( "SJ_InstRandom  Start_Random" );
		gameObject.SetActive(true);
		StartCoroutine( CO_End() );
		InvokeRepeating( "Repeat_Inst" , 0,time_term );
	}

	void	Repeat_Inst()
	{
		Vector3		pos		=	SJ_Cood.Random_SphereBound(radius);
		GameObject	inst	=	SJPool.GetNewInst( go_PoolObj );
		inst.transform.position = transform.position + pos;
	}

	IEnumerator CO_End()
	{
		yield return new WaitForSeconds(time_total);
		gameObject.SetActive(false);
		CancelInvoke();
		sjFunc.Func();
	}

}
