using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_UnityTime : SJTrgAction_Mono
{
	public	float	time_Scale;

	override public	void	OnAction()
	{
		Time.timeScale = time_Scale;
	}
}
