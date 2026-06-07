using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_FSMLayer : MonoBehaviour
{
	public	List<SJ_FSMNode>	lt_fsmRoot;
	public	SJ_FSMNode			cur_fsm;


	public	void	Editor_Init()
	{
		foreach( SJ_FSMNode s in lt_fsmRoot ) s.Editor_Init();
		if( lt_fsmRoot.Count > 0 ) cur_fsm = lt_fsmRoot[0];
	}

	public	void	ChangeLayer( string str_layer )
	{
		foreach( SJ_FSMNode s in lt_fsmRoot )
		{
			if( s.name == str_layer )
			{
				if( cur_fsm!= null ) cur_fsm.Stop_AllState();
				cur_fsm = s;
				break;
			}
		}
	}

	public	void	Stop_AllState()
	{
		if( cur_fsm!= null ) cur_fsm.Stop_AllState();
	}

	public	HashSet<SJ_FSMNode>		GetCurNode()
	{
		if( cur_fsm == null )return null;
		return 	cur_fsm.hs_curExec;
	}

	public	HashSet<SJ_FSMAction>		GetCurAction_All()
	{
		if( cur_fsm == null )return null;
		return 	cur_fsm.hs_curAction;
	}

	public	void	Exec_Start( object obj = null )
	{
		if( cur_fsm == null )
		{
			Debug.LogError("Error!! : SJ_FSMLayer : Exec_Start : cur_fsm == null ");
			return;
		}
		cur_fsm.ExecStart( obj );
	}


	public	void	Exec_StateRoot( string _name , object obj = null )
	{
		if( cur_fsm == null )
		{
			Debug.LogError("Error!! : SJ_FSMLayer : Exec_StateRoot : cur_fsm == null ");
			return;
		}
		cur_fsm.Exec_StateRoot( _name , obj );
	}


}
