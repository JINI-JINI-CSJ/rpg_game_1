using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_DampingTrans_Call_Update : MonoBehaviour
{
	public	SJ_DampingTrans		sJ_DampingTrans;

	private void LateUpdate()
	{
		sJ_DampingTrans.Update_Pos();
	}
}
