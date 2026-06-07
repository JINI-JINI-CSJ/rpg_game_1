using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_ChangeActionPlayer : SJTrgAction_Default_Mono 
{
	public	SJTrgActionPlayer_Mono	actPlayer;

	public override string OnChange_Name(){return "ChangeActionPlayer";}

	override	public	void	OnAction()
	{
		par_actPlayer.End_AllAction();

		actPlayer.Start_Action();
	}
}
