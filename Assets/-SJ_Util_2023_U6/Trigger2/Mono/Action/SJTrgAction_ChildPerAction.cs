using UnityEngine;
using System.Collections;

public class SJTrgAction_ChildPerAction : SJTrgAction_Default_Mono
{
	// 자식 액션들에 대한 발동 퍼센트
	public	int[]	child_action_per_list;

	public	SJTrgAction_Mono	action_cur;

	override	public	void	OnAction()
	{
		int idx =	SJ_Unity.Random_RangeStepList( child_action_per_list );
		if( child_action.Count <= idx )
		{
			EndAction();
			return;
		}
		action_cur = child_action[idx];

		if( par_actPlayer != null && par_actPlayer.debug_actionExec )
		{
			Debug.Log( "ChildPerAction : " + action_cur.name );
		}

		action_cur.Action();
	}

	//override	public	SJTrgAction_Mono	OnGetPlayAction() {return action_cur; }

	override	public	void	OnEndFinal(){if( action_cur != null ) action_cur.OnEndFinal();}

}
