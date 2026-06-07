using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_StepMode
{
	public delegate void DlgFunc_Void();
	public delegate void DlgFunc_Event( int evt_i , string evt_s , object obj );

	public	class _STEP_FUNC
	{
		public _STEP_FUNC
			(
				DlgFunc_Void _func_update,
				float _wait_time = -1 ,
				string _Name = "",
				DlgFunc_Event _func_event_recv = null,
				DlgFunc_Void _func_enter = null,
				DlgFunc_Void _func_exit = null,
				DlgFunc_Void _func_fixedupdate = null
			)
		{
			func_update			= _func_update;
			wait_time 			= _wait_time;
			Name				= _Name;
			func_event_recv		= _func_event_recv;
			func_enter			= _func_enter;
			func_exit			= _func_exit;
			func_fixedupdate	= _func_fixedupdate;
		}
	

		public SJ_StepMode		stepMode;

		// 젤 많이 쓰는 함수일듯해서 맨위에
		public	DlgFunc_Void	func_update;
		public	string			Name;
		public	DlgFunc_Event	func_event_recv;
		public	DlgFunc_Void	func_enter;
		public	DlgFunc_Void	func_exit;
		public	DlgFunc_Void	func_fixedupdate;

		public	float 			wait_time = -1;

		public	float 			wait_time_cur;

		public	void Enter()
		{
			// "스텝진입 :" +  Name );

			wait_time_cur = 0;
			if( func_enter != null ) func_enter();
		}
		public	void Update()
		{
			if( func_update != null ) func_update();
			if( wait_time > 0 )
			{
				wait_time_cur += Time.deltaTime;
				if( wait_time_cur > wait_time )
				{
					if( func_update != null ) func_update();
					stepMode.Next_Step();
					return;
				}
			}
		}

		public	void FixUpdate()
		{
			if( func_fixedupdate != null ) func_fixedupdate();
		}

		public	void Exit()
		{
			//Debug.Log( "스텝나감 :" +  Name );
			if( func_exit != null ) func_exit();
		}

		public	void Event( int evt_i , string evt_s , object obj )
		{
			if( func_event_recv != null ) func_event_recv(evt_i , evt_s , obj);
		}

	}

	public	List<_STEP_FUNC> lt_STEP_FUNC = new List<_STEP_FUNC>();



	public	bool 	loop;

	int 			cur_step_idx;
	_STEP_FUNC 		cur_step;

	bool 			play;

	public	GameObject 	go_self;


	public	DlgFunc_Void	func_all_end;

	public	void	AddFunc
		(
			DlgFunc_Void	_func_update,
			float  			_wait_time = -1 ,
			string			_Name = "",
			DlgFunc_Event	_func_event_recv = null,
			DlgFunc_Void	_func_enter = null,
			DlgFunc_Void	_func_exit = null,
			DlgFunc_Void	_func_fixedupdate = null
		)
	{
		_STEP_FUNC sf = new _STEP_FUNC
			(
				_func_update,
				_wait_time ,
				_Name ,
				_func_event_recv ,
				_func_enter ,
				_func_exit ,
				_func_fixedupdate
			);
		sf.stepMode = this;
		lt_STEP_FUNC.Add(sf);

//		Debug.Log( "lt_STEP_FUNC : " + lt_STEP_FUNC.Count );
	}

	public	int 	CountRegStep()
	{
		return lt_STEP_FUNC.Count;
	}

	public	void Start_Step()
	{
		if( lt_STEP_FUNC.Count < 1 )
		{
			Debug.Log( "주의!!! lt_STEP_FUNC.Count < 1" );
			return;
		}
		play = true;
		cur_step_idx = 0;
		cur_step = null;
		Next_Step();
	}

	public	_STEP_FUNC GetCurStep(){return cur_step;}

	public	bool 	Check_CurMode(string _name)
	{
		int temp_idx = cur_step_idx;
		if( temp_idx >= lt_STEP_FUNC.Count )
		{
			temp_idx = 0;
		}
		_STEP_FUNC temp = lt_STEP_FUNC[temp_idx];
		if(temp.Name != _name)
		{
			return false;
		}
		return true;
	}

	public	bool 	Next_Step_CheckName( string _name )
	{
		if( Check_CurMode(_name) == false )
		{
			Debug.LogError("Next_Step_CheckName : 인자 : " + _name);	
			if( cur_step != null )		
			Debug.LogError("Next_Step_CheckName : 현재 : " + cur_step.Name);	
			return false;
		}


 		return Next_Step();
	}

	public	bool 	Next_Step()
	{
		if( play == false ) return false;
		if( cur_step != null )cur_step.Exit();
		if( cur_step_idx >= lt_STEP_FUNC.Count )
		{
			if( loop )
			{
				cur_step_idx = 0;
				cur_step = null;
				Next_Step();
				return false;
			}else{

				All_End();
				return true;
			}
		}
		cur_step = lt_STEP_FUNC[cur_step_idx];
		cur_step_idx++;		
		cur_step.Enter();
		return false;
	} 

	public	void All_End()
	{
		Stop();		
		if( func_all_end != null ) func_all_end.Invoke();
	}

	public	void 	Stop()
	{
		play = false;
	}

	public	void 	Resume()
	{
		play = true;
	}

	public	bool 	Check_Play(){return play;}

	public	void 	Update()
	{
		if( play == false ) return;
		if( cur_step != null )cur_step.Update();
	}

	public	void 	FixedUpdate() 
	{
		if( play == false ) return;
		if( cur_step != null )cur_step.FixUpdate();
	}

	
	public	void Func_Event( int evt_i , string evt_s , object obj )
	{
		if( play == false ) return;
		if( cur_step != null )cur_step.Event(evt_i , evt_s , obj);
	}
}
