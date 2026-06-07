using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public	enum _SJARG
{
	String = 0 ,
	Int ,
	Float ,
	Double
}

[System.Serializable]
public	class _SJ_HASH_VALUE
{
	public	string	Name;
	public	_SJARG	Type;
	public	string  Value;
}


public enum SJFuncInf_Default
{
	None = 0,
	CreateObj			= 1000 ,
	SetPos				= 2000 ,
	AnimationClip		= 3000 ,
	AnimationSpeed		= 4000 ,
	Delay				= 5000 ,
	ChangeSJTrgPlayer	= 6000
}

[System.Serializable]
public class _SJFuncInf_Default 
{
	[System.Serializable]
	public class _CreateObj
	{
		public GameObject go;
	}
	public _CreateObj _createObj;

	[System.Serializable]
	public class _Cood
	{
		public	Vector3		vec3;
		public	GameObject	go_pos;
	}
	public	_Cood	_cood;

	[System.Serializable]
	public class _Animation 
	{
		public AnimationClip ani_clip;
		public float ani_speed = 1.0f;
		public float ani_play_range = 1.0f;
	}
	public _Animation _animation;

	[System.Serializable]
	public class _Delay 
	{
		public float time;
	}
	public _Delay _delay;

	[System.Serializable]
	public class _ChangeSJTrgPlayer 
	{
		//[System.NonSerialized]
		public SJTriggerPlayer sjTriggerPlayer;
	}
	public _ChangeSJTrgPlayer _changeSJTrgPlayer;
}


public	enum SJ_EXECACT_PLAY_MODE
{
	Once ,
	Sequence , 
	TimeLine
}

//[System.Serializable]
public	class _SJFuncInf 
{ 
	public	string	name;

	[HideInInspector]
	public	SJTriggerExecAction		sjTriggerExecAction;

	public	MonoBehaviour	mono_SendMsg;
	public	string			func_SendMsg = "OnSJTrgAction";
	public	bool			no_sync;

	public	List<_SJ_HASH_VALUE>	list_arg = new List<_SJ_HASH_VALUE>();
	public	Hashtable				hash_arg = new Hashtable();

	public	SJFuncInf_Default	func_default;
	public	_SJFuncInf_Default	sjFuncInf_Default_arg;

	public	void	Init()
	{
		hash_arg.Clear();
		foreach(_SJ_HASH_VALUE s in list_arg)
		{
			switch( s.Type )
			{
				case _SJARG.String:	hash_arg[s.Name] = s.Value;					break;
				case _SJARG.Int:	hash_arg[s.Name] = int.Parse( s.Value );	break;
				case _SJARG.Float:	hash_arg[s.Name] = float.Parse(s.Value);	break;
				case _SJARG.Double:	hash_arg[s.Name] = double.Parse(s.Value);	break;
			}
		}
	}

	public	string	GetStr(string Name)		{ return (string)hash_arg[Name];}
	public	int		GetInt(string Name)		{ return (int)hash_arg[Name];	}
	public	float	GetFloat(string Name)	{ return (float)hash_arg[Name]; }
	public	double	GetDouble(string Name)	{ return (double)hash_arg[Name];}

	public	void	Exec( SJTriggerExecAction	_sjTriggerExecAction )
	{
		sjTriggerExecAction = _sjTriggerExecAction;
		SJTriggerPlayer cur_player;

		// player
		if( _sjTriggerExecAction.sjTriggerPlayer != null )
		{
			cur_player = _sjTriggerExecAction.sjTriggerPlayer;
		}
		else
		{ 
			if( sjTriggerExecAction.sjTriggerExec == null )
			{
				Debug.LogError( "error!!!! _SJFuncInf:Exec -->>> sjTriggerExecAction.sjTriggerExec == null " );
				return;
			}
			cur_player =	sjTriggerExecAction.sjTriggerExec.sjTriggerPlayer;
		}
		cur_player._sjFuncInf_cur = this;

		// send msg
		if( string.IsNullOrEmpty( func_SendMsg ) == false )
		{
			if( mono_SendMsg == null )	SJ_Unity.SendMsg( cur_player , func_SendMsg );
			else						SJ_Unity.SendMsg( mono_SendMsg , func_SendMsg );
			return;
		}

		// sj default func
		switch (func_default)
		{
			case SJFuncInf_Default.CreateObj:			cur_player.OnDefault_CreateObj(this);		return;
			case SJFuncInf_Default.SetPos:				cur_player.OnDefault_SetPos(this);			return;
			case SJFuncInf_Default.AnimationClip:		cur_player.OnDefault_AnimationClip(this);	return;
			case SJFuncInf_Default.AnimationSpeed:		cur_player.OnDefault_AnimationSpeed(this);	return;
			case SJFuncInf_Default.Delay:				cur_player.OnDefault_Delay(this);			return;
			case SJFuncInf_Default.ChangeSJTrgPlayer:	cur_player.OnDefault_CreateObj(this);		return;
		}

		// user action
		cur_player.OnUserAction(this);
	}
}

//[System.Serializable]
public class SJTriggerExecAction  
{
	public	string			name;
	public	bool			syncMode;

	List<_SJFuncInf>		list_play = new List<_SJFuncInf>();

	[HideInInspector]
	public	SJTriggerExec	sjTriggerExec;

	[HideInInspector]
	public	SJTriggerPlayer	sjTriggerPlayer;

	virtual public	int			OnGet_SJFuncInfCount(){return 0;}
	virtual	public	_SJFuncInf	OnGet_SJFuncInf(int idx){return null;}
	virtual public	void		OnAdd_SJFuncInf(_SJFuncInf func_inf) { }
	virtual	public	void		OnRemove_SJFuncInf(int idx) {}
	virtual public	void		OnClear_SJFuncInf() { }


	public	enum	ACTION_PLAY_Q_STATE
	{
		None = 0, 
		Start	,
		Playing , 
		End
	}


	public	SJTriggerPlayer	GetCurPlayer()
	{
		if( sjTriggerExec != null && sjTriggerExec.sjTriggerPlayer != null )
		{
			return	sjTriggerExec.sjTriggerPlayer;
		}
		else if( sjTriggerPlayer != null )
		{
			return	sjTriggerPlayer;
		}
		return null;
	}

	public ACTION_PLAY_Q_STATE Action( SJTriggerPlayer _sjTriggerPlayer = null , SJTriggerExec _sjTriggerExec = null )
	{
		sjTriggerExec = _sjTriggerExec;
		sjTriggerPlayer = _sjTriggerPlayer;

		GetCurPlayer()._sjTrgAct_cur = this;

		if(syncMode == false)
		{
			int c = OnGet_SJFuncInfCount();
			for(int i=0;i<c;i++)
			{
				_SJFuncInf s = OnGet_SJFuncInf(i);
				s.Exec( this );
			}
			return ACTION_PLAY_Q_STATE.End;
		}
		else
		{
			ACTION_PLAY_Q_STATE r = ACTION_PLAY_Q_STATE.None;

			if (OnGet_SJFuncInfCount() < 1 )
			{
				if(	ActionCopy(this,true) == false) return ACTION_PLAY_Q_STATE.None;
				r = ACTION_PLAY_Q_STATE.Start;
			}
			NextAction();
			return r;
		}
	}

	virtual public	void OnAction_AllEnd() { }
	public	bool	NextAction()
	{
		if( syncMode == false ) return false;

		while(true)
		{
			if(list_play.Count < 1)
			{
				OnAction_AllEnd();
				GetCurPlayer().OnActionEnd( this );

				return false;
			}
			_SJFuncInf s = list_play[0];
			s.Exec( this );
			list_play.RemoveAt(0);

			if( s.no_sync == false ) break;
		}

		return true;
	}

	public	bool	ActionCopy( SJTriggerExecAction other , bool clearQ )
	{
		if(other == null) other = this;
		if(clearQ) list_play.Clear();

		int c = other.OnGet_SJFuncInfCount();
		if (c < 1) return false;
		for (int i = 0; i < c; i++)
		{
			_SJFuncInf s = other.OnGet_SJFuncInf(i);
			list_play.Add( s );
		}
		return true;
	}




}
