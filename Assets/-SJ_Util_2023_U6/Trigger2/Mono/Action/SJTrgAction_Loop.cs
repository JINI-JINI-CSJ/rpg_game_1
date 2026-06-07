using UnityEngine;
using System.Collections;

public class SJTrgAction_Loop : SJTrgAction_Default_Mono
{
	public	SJTrgAction_Mono	goto_action;

	//public	bool	loop;
	public	int		loop_count;
	int				loop_count_cur;

	override	public	void	OnInit()
	{
		loop_count_cur = 0;
	}

	override	public	void	OnAction()
	{
		if( loop_count > 0 )
		{
			loop_count_cur--;
			if( loop_count_cur < 0 )
			{
				EndAction();
				return;
			}
		}

		par_actPlayer.Action_Obj( goto_action );
	}

	override	public	bool	OnCheck_OnStartInst() {return true; }
	override	public	void	OnStartInst()
	{
		loop_count_cur = loop_count;
	}

}
