using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_TimeMaterial : SJTrgAction_Default_Mono 
{
	public	SJ_TimeMaterial		sj_tm;

	override	public	void	OnAction()
	{
		sj_tm.endFunc.Set(gameObject , "OnEnd_TimeMat");
		sj_tm.Start_Mat();
	}

	public	void	OnEnd_TimeMat()
	{
		EndAction();
	}

}
