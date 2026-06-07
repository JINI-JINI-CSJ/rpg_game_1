using UnityEngine;
using System.Collections;

public class SJTrgAction_EndAction : SJTrgAction_Default_Mono
{
	public	float	endTimer=1;
	public	float	endTimer_Random_max = -1;

	public	SJTrgAction_Mono[]	acts_deActive;

	public override string OnChange_Name()
	{
		return "EndAction";
	}

	override	public	void	OnAction()
	{
		float time = endTimer;
		if( endTimer_Random_max > endTimer )
		{
			time = UnityEngine.Random.Range( endTimer , endTimer_Random_max );
		}

		if( time >= 0.001f )
			EndAction_Wait( time );
	}

	override	public	void	OnEnd()
	{
		foreach( SJTrgAction_Mono s in acts_deActive )
		{
			s.OnEnd();
			s.Remove_noHideAct();
			s.gameObject.SetActive(false);
		}
	}

}
