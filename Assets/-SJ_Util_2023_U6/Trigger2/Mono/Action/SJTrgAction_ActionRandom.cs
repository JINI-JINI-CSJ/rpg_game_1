using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_ActionRandom : SJTrgAction_Default_Mono 
{
	public List<int>	percent_Acts;

	override	public	void	OnAction()
	{
		int idx =	SJ_Unity.Random_RangeStepList( percent_Acts.ToArray() );
		SJTrgAction_Mono ac = child_action[idx];
		ac.Action();
	}
}
