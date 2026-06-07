using UnityEngine;
using System.Collections;

[AddComponentMenu("SJTrgActionPlayer_Mono")]
public class SJTrgAction_ActionPlayer : SJTrgAction_Default_Mono
{
	public	SJTrgActionPlayer_Mono		actPlayer;

	public override string OnChange_Name()
	{
		return "ActionPlayer";
	}

	override	public	 void	OnInit_Editer()
	{
	}

	override	public	void	OnAction()
	{
		actPlayer.Init();
		actPlayer.recv_end = gameObject;
		actPlayer.Start_Action();
	}

	void OnEnd_ActionPlayer()
	{
		EndAction();
	}

	public override void OnEndFinal()
	{
		base.OnEndFinal();
		actPlayer.End_AllAction();
	}

}
