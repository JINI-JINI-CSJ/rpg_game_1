using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class SJTriggerPlayer : MonoBehaviour

{
	public	SJHierarchy	sjHierarchy = new SJHierarchy();
	public	Dictionary<int,HashSet<SJTriggerExec>>		dic = new Dictionary<int,HashSet<SJTriggerExec>>();

	public	SJTriggerPlayer								sjTriggerPlayer_cur;
	public	Dictionary<string,SJTriggerPlayer>			dic_TriggerPlayer = new Dictionary<string, SJTriggerPlayer>();

	[HideInInspector]
	public	_SJFuncInf		_sjFuncInf_cur;

	[HideInInspector]
	public	SJTriggerExecAction	_sjTrgAct_cur;

	virtual public	void	OnSelectPlayer(){}
	virtual	public	void	OnActionEnd( SJTriggerExecAction action ) { }

	public	void	Init()
	{

	}

	public	bool	OnNotice_FuncEnd()
	{
		return	_sjTrgAct_cur.NextAction();
	}

	public	void	SelectPlayer( SJTriggerPlayer _sjTriggerPlayer )
	{
		sjTriggerPlayer_cur = _sjTriggerPlayer;
		OnSelectPlayer();
	}

	public	bool	SelectPlayer( string name )
	{
		if( string.IsNullOrEmpty(name) )
		{
			sjTriggerPlayer_cur = this;
			return true;
		}

		SJTriggerPlayer find = null;
		if(	dic_TriggerPlayer.TryGetValue( name , out find) == false )
		{
			return false;
		}
		sjTriggerPlayer_cur = find;
		OnSelectPlayer();
		return true;
	}



	public Dictionary<int, HashSet<SJTriggerExec>>	GetTriggerDic()
	{
		if( sjTriggerPlayer_cur != null ) return sjTriggerPlayer_cur.dic;
		return dic;
	}

	static	List<int>		temp_event_list = new List<int>();
	public	bool	FindEqTriggerExec( SJTriggerExec sjTriggerExec )
	{
		Dictionary<int,HashSet<SJTriggerExec>>  dic_hash = GetTriggerDic();
		temp_event_list.Clear();
		sjTriggerExec.GetEventList( temp_event_list );
		HashSet<SJTriggerExec> hash_find;
		foreach( int event_id in temp_event_list )
		{
			if(	dic_hash.TryGetValue( event_id , out hash_find ) )
			{
				if(	hash_find.Contains( sjTriggerExec ) ) return true;
			}
		}
		return false;
	}


	public	void	AddTriggerExec( SJTriggerExec sjTriggerExec_Prf , bool setPlayer_callFunc = true )
	{
		if( sjTriggerExec_Prf.add_DuplableAble == false )
		{
			if( FindEqTriggerExec( sjTriggerExec_Prf ) )
			{
				sjTriggerExec_Prf.OnDuplicated();
				return;
			}
		}
		SJTriggerExec new_exec = SJTriggerSys.GetNewInst_Exec( sjTriggerExec_Prf );

		AddTriggerExec( new_exec );

		Dictionary<int,HashSet<SJTriggerExec>>  dic_hash = GetTriggerDic();
		temp_event_list.Clear();
		sjTriggerExec_Prf.GetEventList( temp_event_list );
		HashSet<SJTriggerExec> hash_find;
		foreach( int event_id in temp_event_list )
		{
			if(	dic_hash.TryGetValue( event_id , out hash_find ) )
			{
				hash_find.Add( sjTriggerExec_Prf );
			}
		}
		if( setPlayer_callFunc )
		{
			sjTriggerExec_Prf.sjTriggerPlayer = this;
			sjTriggerExec_Prf.OnAdd();
		}
	}


	public	void	RemoveTriggerExec( SJTriggerExec sjTriggerExec , bool callFunc = true)
	{
		Dictionary<int,HashSet<SJTriggerExec>>  dic_hash = GetTriggerDic();
		if( callFunc )
			sjTriggerExec.OnRemove();
		temp_event_list.Clear();
		sjTriggerExec.GetEventList( temp_event_list );
		HashSet<SJTriggerExec> hash_find;
		foreach( int event_id in temp_event_list )
		{
			if(	dic_hash.TryGetValue( event_id , out hash_find ) )
			{
				hash_find.Remove(sjTriggerExec);
			}
		}
		SJTriggerSys.DestoryExecObj( sjTriggerExec );

	}


	public	int		TriggerMessage_Play			( int event_id , object arg_obj = null  ,int val_int=0 , float val_float=0, string val_str=null )
	{
		foreach( KeyValuePair<int,HashSet<SJTriggerExec>> kv in GetTriggerDic())
		{
			HashSet<SJTriggerExec> hash = kv.Value;
			foreach( SJTriggerExec exec in hash )
			{
				exec.OnTriggerMessage_Play( event_id , arg_obj  ,val_int , val_float, val_str );
			}
		}
		return 0;
	}


	public	bool	TriggerMessage_Quary_Bool	( int event_id , ref bool	ref_bool	,object arg_obj = null , int val_int=0 , float val_float=0, string val_str=null )
	{
		bool r = false;
		foreach( KeyValuePair<int,HashSet<SJTriggerExec>> kv in GetTriggerDic())
		{
			HashSet<SJTriggerExec> hash = kv.Value;
			foreach( SJTriggerExec exec in hash )
			{
				if(	exec.OnTriggerMessage_Quary_Bool( event_id ,ref ref_bool	, arg_obj  ,  val_int ,  val_float, val_str ) ) r = true;
			}
		}
		return r;
	}

	public	bool 	TriggerMessage_Quary_Int	( int event_id , ref int	ref_int		,object arg_obj = null , int val_int=0 , float val_float=0, string val_str=null )
	{
		bool r = false;
		foreach( KeyValuePair<int,HashSet<SJTriggerExec>> kv in GetTriggerDic())
		{
			HashSet<SJTriggerExec> hash = kv.Value;
			foreach( SJTriggerExec exec in hash )
			{
				if(	exec.OnTriggerMessage_Quary_Int( event_id ,ref ref_int	, arg_obj  ,  val_int ,  val_float, val_str ) ) r = true;
			}
		}
		return r;
	}

	public	bool	TriggerMessage_Quary_Float	( int event_id , ref float	ref_float	,object arg_obj = null , int val_int=0 , float val_float=0, string val_str=null )
	{
		bool r = false;
		foreach( KeyValuePair<int,HashSet<SJTriggerExec>> kv in GetTriggerDic())
		{
			HashSet<SJTriggerExec> hash = kv.Value;
			foreach( SJTriggerExec exec in hash )
			{
				if(	exec.OnTriggerMessage_Quary_Float( event_id ,ref ref_float	, arg_obj  ,  val_int ,  val_float, val_str ) ) r = true;
			}
		}
		return r;
	}

	public	bool	TriggerMessage_Quary_String	( int event_id , ref string	ref_str		,object arg_obj = null , int val_int=0 , float val_float=0, string val_str=null )
	{
		bool r = false;
		foreach( KeyValuePair<int,HashSet<SJTriggerExec>> kv in GetTriggerDic())
		{
			HashSet<SJTriggerExec> hash = kv.Value;
			foreach( SJTriggerExec exec in hash )
			{
				if(	exec.OnTriggerMessage_Quary_String( event_id ,ref ref_str	, arg_obj  ,  val_int ,  val_float, val_str ) ) r = true;
			}
		}
		return r;
	}

	//====================================================================================================================
	//====================================================================================================================
	//====================================================================================================================

	virtual public	void	OnUserAction( _SJFuncInf _sjFuncInf ){}

	// 기본 액션
	// 객체 생성
	virtual public void OnDefault_CreateObj(_SJFuncInf arg)
	{
		SJPool.GetNewInst(arg.sjFuncInf_Default_arg._createObj.go);
		arg.sjTriggerExecAction.NextAction();
	}

	virtual public void OnDefault_SetPos(_SJFuncInf arg)
	{
		if( arg.sjFuncInf_Default_arg._cood.go_pos != null )
		{
			gameObject.transform.position = arg.sjFuncInf_Default_arg._cood.go_pos.transform.position;
		}
		else
		{
			gameObject.transform.localPosition = arg.sjFuncInf_Default_arg._cood.vec3;
		}
	}

	// 애니 모션
	virtual public void OnDefault_AnimationClip(_SJFuncInf arg, bool wait_next = false)
	{
		float total_time = OnDefault_AnimationClip(arg.sjFuncInf_Default_arg);
		if (wait_next) Wait_NextAction(total_time, arg);
	}

	public float OnDefault_AnimationClip(_SJFuncInf_Default arg)
	{
		Animation ani = GetComponent<Animation>();
		AnimationClip clip = arg._animation.ani_clip;
		float total_time = SJ_Unity.AnimationClip_CrossPlay(ani, clip, arg._animation.ani_speed, arg._animation.ani_play_range);
		return total_time;
	}

	// 애니 모션 스피드
	virtual public void OnDefault_AnimationSpeed(_SJFuncInf arg)
	{
		Animation ani = GetComponent<Animation>();
		AnimationClip clip = arg.sjFuncInf_Default_arg._animation.ani_clip;
		AnimationState ani_st = ani[clip.name];
		ani_st.speed = arg.sjFuncInf_Default_arg._animation.ani_speed;
	}

	// 지연 시간
	virtual public void OnDefault_Delay(_SJFuncInf arg)
	{
		Wait_NextAction(arg.sjFuncInf_Default_arg._delay.time, arg);
	}


	//이벤트 바꾸기
	virtual public	void OnDefault_ChangeSJTrgPlayer( _SJFuncInf arg )
	{
		SelectPlayer( arg.sjFuncInf_Default_arg._changeSJTrgPlayer.sjTriggerPlayer );
	}


	public void Wait_NextAction(float wait ,_SJFuncInf arg)
	{
		StartCoroutine(CO_Wait_NextAction(wait , arg));
	}

	IEnumerator CO_Wait_NextAction(float wait , _SJFuncInf arg)
	{
		yield return new WaitForSeconds(wait);
		arg.sjTriggerExecAction.NextAction();
	}
}
