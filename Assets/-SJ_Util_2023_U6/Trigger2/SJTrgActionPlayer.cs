using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]

public class SJTrgActionPlayer : SJTagObj
{

	public	List<SJTrgAction>		lt_action		= new List<SJTrgAction>();

	public	bool	loop;
	public	int		loop_count;
	public	bool	syncAction = true;

	int		loop_count_remain;

	int		idx_cur_action = -1;

	SJTrgAction	action_cur;

	public	SJTrgAction	GetCurAction() {return action_cur; }

//#if UNITY_STANDALONE
//	[HideInInspector]
//	public	SJTrgActionPlayer_Mono	mono;
//#endif

	virtual public	void	OnEnd_AllAction_1( SJTrgActionPlayer act_player ){}

	virtual public	void	OnEnd_AllAction( SJTrgActionPlayer act_player )
	{
//#if UNITY_STANDALONE
//	mono.OnEnd_AllAction(act_player);
//#endif
	}

	public	void	Start_Action()
	{
		idx_cur_action = 0;
		action_cur = null;

		loop_count_remain = loop_count;

		Next_Action();
	}

	void	End_AllAction()
	{
		OnEnd_AllAction_1( this );
		OnEnd_AllAction( this );
	}

	public	bool	Next_Action()
	{
		while(true)
		{
			if( idx_cur_action >= lt_action.Count )
			{
				if( action_cur != null ) action_cur.OnEnd();

				if( loop == false )
				{
					End_AllAction();
				}
				else
				{
					if( loop_count == -1 )
					{
						Start_Action();
					}
					else
					{
						loop_count_remain--;
						if( loop_count_remain > 0 )
						{
							Start_Action();
						}
					}
				}
				return false;
			}

			SJTrgAction ac_new = lt_action[ idx_cur_action ];

			ac_new.OnAction();
			action_cur = ac_new;
			idx_cur_action++;

			if( syncAction && action_cur.noSync == false )
				break;
		}
		return true;
	}

	public	void	Update()
	{
		if( action_cur != null ) action_cur.OnUpdate();
	}

	public	void	FixedUpdate()
	{
		if( action_cur != null ) action_cur.OnFixUpdate();
	}


}
