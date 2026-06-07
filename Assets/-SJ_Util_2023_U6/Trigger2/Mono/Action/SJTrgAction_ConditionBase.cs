using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_ConditionBase : SJTrgAction_Default_Mono
{
	public	enum	CONDITION_TYPE
	{
		AND , 
		OR
	}
	public	CONDITION_TYPE type;

	int complete_child = 0;

	public override string OnChange_Name()
	{
		return "ConditionBase";
	}

	public override void OnAction()
	{
		complete_child = 0;
		foreach( SJTrgAction_Mono s in child_action )
		{
			//SJTrgAction_ConditionBase cond = s as SJTrgAction_ConditionBase;
			//cond.OnAction_Condition();
			s.gameObject.SetActive(true);
			s.noNextAction = true;
			s.OnAction();
		}
	}

	//virtual	public void	OnAction_Condition(){}

	public	void	RecvChild_Complete( SJTrgAction_ConditionBase child )
	{
		complete_child++;
		if( type == CONDITION_TYPE.OR )
		{
			Complete_Condition();
		}

		if( complete_child == child_action.Count )
		{
			Complete_Condition();
		}
	}

	public	void	Complete_Condition( bool noEndAction = false )
	{
		if( noEndAction == false )
			EndAction();

		SJTrgAction_ConditionBase par_cond =	transform.parent.GetComponent<SJTrgAction_ConditionBase>();
		if( par_cond != null )
		{
			par_cond.RecvChild_Complete(this);
		}
		gameObject.SetActive(false);
	}
}
