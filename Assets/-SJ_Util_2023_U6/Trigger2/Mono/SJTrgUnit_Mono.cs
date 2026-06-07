using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJTrgUnit_Mono : SJTrgActionPlayer_Mono
{
	// 조건 리스트
	public	List<SJTrgCondition>	lt_condition	= new List<SJTrgCondition>();

	public	SJTrgMode_Mono	par_trgMode;

	public	void	Ini_Unit()
	{
	}

	override public	void	OnEnd_AllAction_1(SJTrgActionPlayer_Mono act_player)
	{
		par_trgMode.OnEndActionPlayer( act_player );
	}


	public	bool	Play()
	{
		foreach( SJTrgCondition c in lt_condition )
		{
			if(	c.OnCheck() == false ) return false;
		}
		Start_Action();
		return true;
	}


	public	void	OnAdd()
	{
		foreach( SJTrgAction_Mono s in lt_action ) s.OnAdd();
	}

	public	void	OnRemove()
	{
		foreach( SJTrgAction_Mono s in lt_action ) s.OnRemove();
	}


}
