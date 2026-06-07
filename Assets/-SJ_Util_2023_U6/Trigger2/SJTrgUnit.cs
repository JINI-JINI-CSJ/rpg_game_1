using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 효과 단위 또는 묶음
// 한 아이템에 여러 버프가 있다면 여러개의 액션을 등록한다.
public class SJTrgUnit : SJTrgActionPlayer
{
	// 이벤트 리스트
	public	List<int>		lt_EventInt = new List<int>();
	public	List<string>	lt_EventStr = new List<string>();

	// 조건 리스트
	public	List<SJTrgCondition>	lt_condition	= new List<SJTrgCondition>();


	public	SJTrgMode	par_trgMode;

	override public	void	OnEnd_AllAction_1(SJTrgActionPlayer act_player)
	{
		//par_trgMode.OnEndActionPlayer( act_player );
	}

	public	void	InitEventTag()
	{
		AddTag( lt_EventInt.ToArray() );
		AddTag( lt_EventStr.ToArray() );
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
		foreach( SJTrgAction s in lt_action ) s.OnAdd();
	}

	public	void	OnRemove()
	{
		foreach( SJTrgAction s in lt_action ) s.OnRemove();
	}



}
