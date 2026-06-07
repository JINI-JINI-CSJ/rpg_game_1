using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJTrgActionPlayer_Mono : MonoBehaviour
{
	[Multiline]
	public	string	Desc;

	public	List<SJTrgAction_Mono>		lt_action		= new List<SJTrgAction_Mono>();

	public	bool	loop;
	public	int		loop_count;
	public	bool	syncAction = true;
	int				loop_count_remain;

	int				idx_cur_action = -1;

	public	SJTrgMode_Mono		par_mode;
	SJTrgAction_Mono	action_cur;
	SJTrgAction_Mono	action_UpdateExec;
	SJTrgAction_Mono	action_UpdateExec_Next;
	public	bool		play_MonoOnEnable = false;
	public	bool		play_StartFunc;
	public	GameObject	go_curSelect;
	public	GameObject	recv_end;			//  "OnEnd_ActionPlayer"
	public	bool		noAutoChildActFind;

	public	string		coll_StartAction;
	public	bool		noColl_Start;

	public	bool		debug_actionExec;

	bool	play;
	int		error_Loop_Prevention; // 무한루프 방지용

	public	List<SJTrgAction_Mono>	lt_noHideAct;	// 끝난 액션 비활성 안하기 객체들

	[HideInInspector]
	public	bool	loop_Stop;


	static	public	Dictionary< int , SJTrgActionPlayer_Mono >		dic_actPlayer = new Dictionary<int, SJTrgActionPlayer_Mono>();


	static	public	void	Reg_ActPlayer( SJTrgActionPlayer_Mono s )
	{
		dic_actPlayer[ s.gameObject.GetInstanceID() ] = s;
	}

	static	public	void	UnReg_ActPlayer( SJTrgActionPlayer_Mono s )
	{
		dic_actPlayer.Remove( s.gameObject.GetInstanceID() );
	}

	static	public	void OnEventRecv_All(string tag, int arg_i = 0, string arg_s = "", object obj = null)
	{
		foreach( KeyValuePair< int , SJTrgActionPlayer_Mono > k in dic_actPlayer )
		{
			k.Value.OnEventRecv( tag, arg_i, arg_s , obj );
		}
	}

	public	void	OnAddEvent( SJTrgPlayer_Mono player )
	{
		foreach( SJTrgAction_Mono s in lt_action )
		{
			s.self_Player = player;
			s.OnAdd();
		}
	}

	public	void	OnRemoveEvent( SJTrgPlayer_Mono player )
	{
		foreach( SJTrgAction_Mono s in lt_action )
		{
			s.self_Player = player;
			s.OnRemove();
		}
	}

	private void Start()
	{
		if( play_StartFunc )Start_Action();
	}

	void	OnEnable()
	{
		//if( play_MonoOnEnable )
		//{
		//	Start_Action();
		//}
	}

	virtual	public	void	OnInit() { }
	public	void	Init()
	{
		Align_ChildAction();

	}
	public	void	Align_ChildAction()
	{
		Debug.Log( "SJTrgActionPlayer_Mono 정리 : " + name );

		if (noAutoChildActFind == false)
		{
			lt_action.Clear();
			for (int i = 0; i < transform.childCount; i++)
			{
				SJTrgAction_Mono act_mono = transform.GetChild(i).GetComponent<SJTrgAction_Mono>();
				if (act_mono != null)
				{
					if( par_mode != null )
					{
						act_mono.self_Player = par_mode.par_layer.par_player;
					}
					act_mono.Init_Editer();
					act_mono.par_actPlayer = this;
					lt_action.Add(act_mono);
					act_mono.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			foreach( SJTrgAction_Mono s in lt_action )
			{
				s.par_actPlayer = this;
				s.gameObject.SetActive(false);
			}
		}

		gameObject.SetActive(false);

	}

	public	SJTrgAction_Mono	GetCurAction() {return action_cur; }

	//
	public	SJTrgAction_Mono	GetNextAction() 
	{
		int next_idx = idx_cur_action;
		if( next_idx >= lt_action.Count ) return null;
		return lt_action[next_idx]; 
	}

	virtual public	void	OnEnd_AllAction_1( SJTrgActionPlayer_Mono act_player ){}
	virtual public	void	OnEnd_AllAction( SJTrgActionPlayer_Mono act_player ){}

	public	void	Start_Action()
	{
		if( play ) return;

		if( debug_actionExec ) Debug.Log( "Start_Action : " + gameObject.name );

		lt_noHideAct.Clear();

		gameObject.SetActive(true);
		enabled = true;

		Reg_ActPlayer( this );
		error_Loop_Prevention = 0;
		loop_Stop = false;
		idx_cur_action = 0;
		action_cur = null;
		action_UpdateExec = null;
		action_UpdateExec_Next = null;
		loop_count_remain = loop_count;
		foreach( SJTrgAction_Mono s in lt_action )
		{
			s.played_end = false;
			s.gameObject.SetActive(false);
		}
		play = true;

		Next_Action(null);
	}

	public	void	End_AllAction( bool noSendEnd = false )
	{
		if( play == false ) return;
		foreach( SJTrgAction_Mono s in lt_action )
		{
			s.StopAllCoroutines();
			s.OnEndFinal();
			s.gameObject.SetActive(false);
		}

		OnEnd_AllAction_1( this );
		OnEnd_AllAction( this );

		action_cur = null;
		UnReg_ActPlayer( this );
		play = false;
		
		if( noSendEnd == false )
			SJ_Unity.SendMsg( recv_end , "OnEnd_ActionPlayer" );

		gameObject.SetActive(false);
	}

	public	void	Add_ActionObj( SJTrgAction_Mono act )
	{
		act.par_actPlayer = this;
		act.gameObject.SetActive(false);
		lt_action.Add( act );
	}

	public	void	Action_Obj( SJTrgAction_Mono action )
	{
		bool find = false;
		int i = 0;
		foreach( SJTrgAction_Mono s in lt_action )
		{
			if( s == action )
			{
				find = true;
				break; 
			}
			i++;
		}
		if( find == false ) return;

		idx_cur_action = i; 

		Next_Action(null);
	}

	public	bool	Next_Action( SJTrgAction_Mono end_act )
	{
		if( end_act != null && end_act.noSync )
		{
			// 노싱크인건 그냥 리턴..
			return false;
		}

		error_Loop_Prevention++;

		if( error_Loop_Prevention > 100 )
		{
			Debug.LogError( "ERROR!!!!!!! trigger infinity loop error!!!!!!" );
			loop_Stop = true;
			End_AllAction();
			return false;
		}

		if( end_act != null )
		{
			if( end_act.endAction_ActPlayerBreak )
			{
				// 액션 실행 중단. 
				gameObject.SetActive(false);
				return false;
			}
		}

		int debug_loop = 0;
		while(true)
		{
			if( idx_cur_action >= lt_action.Count )
			{
				if( loop == false )
				{
					End_AllAction();
				}
				else
				{
					if( loop_count == -1 )
					{
						// restart
						play = false;
						SJ_Unity.SendMsg( recv_end , "OnEnd_ActionPlayer" );
						Start_Action();
					}
					else
					{
						loop_count_remain--;
						if( loop_count_remain > 0 )
						{
							//restart
							play = false;
							SJ_Unity.SendMsg( recv_end , "OnEnd_ActionPlayer" );
							Start_Action();
						}
					}
				}
				return false;
			}

			SJTrgAction_Mono ac_new = lt_action[ idx_cur_action ];
			idx_cur_action++;

			action_cur = ac_new.OnGetPlayAction();

			
			if( ac_new.execDirect == false )
			{
				action_UpdateExec_Next = ac_new;
				break;
			}
			if( debug_actionExec )Debug.Log( "액션 : action_cur : " + gameObject.name + " : " + action_cur.name );
			ac_new.Action();
			if( syncAction && action_cur.noSync == false )
				break;

			ac_new.OnEnd();
			ac_new.deAction_Object();

			debug_loop++;
			if( debug_loop > 100 )
			{
				Debug.LogError( "액션 너무 많이 루프 돌았당~" );
				break;
			}
		}
		return true;
	}

	public	void	Update()
	{
		if( action_UpdateExec_Next != null && action_UpdateExec == null )
		{
			action_UpdateExec = action_UpdateExec_Next;
			action_UpdateExec_Next = null;
		}

		if( action_UpdateExec != null && action_cur != null )
		{
			if( debug_actionExec ) Debug.Log( "action_UpdateExec : " + gameObject.name + " : "  + action_UpdateExec.name );
			action_UpdateExec.Action();
			if( syncAction && action_cur != null && action_cur.noSync == false )
			{
				action_UpdateExec = null;
			}else{
				SJTrgAction_Mono act = action_UpdateExec;
				action_UpdateExec = null;
				act.EndAction( true );
				Next_Action(null);
			}
			return;
		}

		error_Loop_Prevention = 0;
		if( action_cur != null && action_cur.IsPlayed_end() == false ) action_cur.OnUpdate();

		foreach( SJTrgAction_Mono s in lt_noHideAct )s.OnUpdate();
	}

	public	void	FixedUpdate()
	{
		if( action_cur != null && action_cur.IsPlayed_end() == false ) action_cur.OnFixUpdate();
		foreach( SJTrgAction_Mono s in lt_noHideAct )s.OnFixUpdate();
	}

	public	void	Add_NoHideAct(SJTrgAction_Mono act)
	{
		//Debug.Log("Add_NoHideAct : " + act.name);

		foreach(SJTrgAction_Mono s in lt_noHideAct)
		{
			if( s == act )
				return;
		}

		lt_noHideAct.Add(act);
	}

	public void Remove_NoHideAct(SJTrgAction_Mono act)
	{
		//Debug.Log("Remove_NoHideAct : " + act.name);
		lt_noHideAct.Remove(act);
	}

	public bool OnEventRecv(string tag, int arg_i = 0, string arg_s = "", object obj = null)
	{
		SJTrgAction_Mono[]	act_mono	= GetComponentsInChildren<SJTrgAction_Mono>();
		foreach( SJTrgAction_Mono s in act_mono )
		{
			s.OnEventRecv( tag, arg_i , arg_s , obj );
		}
		return true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if( noColl_Start ) return;

		if( play == false )
		{
			if (string.IsNullOrEmpty(coll_StartAction))
			{
				Start_Action();
			}
			else
			{
				if (other.name == coll_StartAction) Start_Action();
			}
		}
	}

}
