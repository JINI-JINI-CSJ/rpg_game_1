using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

// 객체 지정
public	class _SJTRG_PLAYER_ID_Mono
{
	public	int		tag_int;
	public	string	tag_str;
	
	public	virtual	SJTrgPlayer_Mono	OnGetPlayer( SJTrgAction_Mono action )
	{
		return null;
	}

	public	JSONBinaryTag	value_Type_Get;
	public	string			value_Name_Get;

	public	int		GetValue_Int( SJTrgAction_Mono action  )
	{
		SJTrgPlayer_Mono player = OnGetPlayer(action);
		if( player != null ) return	player.json[value_Name_Get].AsInt;
		return 0;
	}

	public	float		SJTrgAction_Mono(SJTrgAction_Mono action)
	{
		SJTrgPlayer_Mono player = OnGetPlayer(action);
		if( player != null ) return	player.json[value_Name_Get].AsFloat;
		return 0;
	}
}


[System.Serializable]
public	class _SJArgValue_INT		{ public	SJTrgAction_Mono	act; public	string	arg_name; public	int		val; }
[System.Serializable]
public	class _SJArgValue_FLOAT		{ public	SJTrgAction_Mono	act; public	string	arg_name; public	float	val; }
[System.Serializable]
public	class _SJArgValue_STRING	{ public	SJTrgAction_Mono	act; public	string	arg_name; public	string	val; }
[System.Serializable]
public	class _SJArgValue_OBJ		{ public	SJTrgAction_Mono	act; public	string	arg_name; public	object	val; }

[System.Serializable]
public	class _SJArgVal
{
	public	List<_SJArgValue_INT>		int_list = new List<_SJArgValue_INT>();
	public	List<_SJArgValue_FLOAT>		float_list = new List<_SJArgValue_FLOAT>();
	public	List<_SJArgValue_STRING>	str_list = new List<_SJArgValue_STRING>();
	public	List<_SJArgValue_OBJ>		obj_list = new List<_SJArgValue_OBJ>();

	public	void	SetValue()
	{
		foreach( _SJArgValue_INT s in  int_list)	s.act.SetValue( s.arg_name , s.val );
		foreach( _SJArgValue_FLOAT s in  float_list)s.act.SetValue( s.arg_name , s.val );
		foreach( _SJArgValue_STRING s in  str_list)	s.act.SetValue( s.arg_name , s.val );
		foreach( _SJArgValue_OBJ s in  obj_list)	s.act.SetValue( s.arg_name , s.val );
	}
}


public class SJTrgAction_Mono : MonoBehaviour
{
	[Multiline]
	public	string					Desc;
	public	string					subObjName;

	public	bool					noSync;		// 바로 리턴 , 다음 액션
	public	bool					endAction_ActPlayerBreak; // 액션 종료후 액션 플레이어 정지
	public	_SJTRG_PLAYER_ID_Mono	playerTag;
	public	SJTrgActionPlayer_Mono	par_actPlayer;

	public	_SJ_SelObjName			executeObj_Name; // 실행객체 이름 , 없으면 밑에꺼..

	public	SJTrgPlayer_Mono		self_Player;
	public	bool					noEnd_Hide_GameObj;

	public	List<SJTrgAction_Mono>	child_action = new List<SJTrgAction_Mono>();
	
	public	string[]				Tag_recvEvent;

	public	bool					noPlay;			// 아무것도 안하고 바로 다음 꺼 , 시스템전용
	public	bool					noPlay_User;	// " , 유저 조정
	public	bool					stop_User;		// 정지
	public	bool					execDirect;		// 바로 실행할지 , 다음 update에서 실행할지
	public	bool					noNextAction;

	public	bool						saveGlobal_CurAct;
	static	public	SJTrgAction_Mono	g_saveGlobal_CurAct;
	
	public	SJTagObj_Mono			sjtagobj_mono;

	public	bool	played_end = false;


	public	void	Init_Editer()
	{
		//par_action = null;
		child_action.Clear();
		for(int i = 0 ; i < transform.childCount ; i++ )
		{
			SJTrgAction_Mono child_act = transform.GetChild(i).GetComponent<SJTrgAction_Mono>();
			if( child_act != null )
			{
				//child_act.par_action = this;
				child_action.Add( child_act );
				child_act.gameObject.SetActive(false);
			}
		}

		if( string.IsNullOrEmpty( OnChange_Name() ) == false )
		{
			gameObject.name = OnChange_Name() + subObjName;
		}

		sjtagobj_mono = GetComponent<SJTagObj_Mono>();
		if( sjtagobj_mono != null ) sjtagobj_mono.Init_Editer();
		OnInit_Editer();
	}

	virtual	public	string		OnChange_Name()
	{
		return "";
	}

	public	void	AddTagComp()
	{
		sjtagobj_mono = gameObject.AddComponent<SJTagObj_Mono>();
	}

	virtual	public	 void	OnInit_Editer(){}

	void	Update()
	{
		//OnUpdate();
	}

	public	void	Init_Inst()
	{
		noPlay = false;
		played_end = false;
		OnPreInit();
		OnInit();
		gameObject.SetActive(false);
	}
	virtual	public	void	OnPreInit() { }
	virtual	public	void	OnInit() { }

	virtual	public	SJTrgAction			GetAction() {return null; }

	virtual	public	SJTrgAction_Mono	OnGetPlayAction() {return this; }

	public	void	Stop_EndAction_Wait()
	{
		StopCoroutine( "CO_EndAction" );
	}

	float	wait_endAction;
	public	void	EndAction_Wait( float wait )
	{
		if( wait < 0.0f ) return;

		//Debug.Log( gameObject.name + "EndAction_Wait : " + wait );

		wait_endAction = wait;
		StartCoroutine( "CO_EndAction" );		
	}

	IEnumerator	CO_EndAction()
	{
		yield return new WaitForSeconds(wait_endAction);
		//Debug.Log( "Wait_EndAction : " + gameObject.name );
		EndAction();
	}

	public	SJTrgPlayer_Mono GetPlayer()
	{
		SJTrgPlayer_Mono player =	playerTag.OnGetPlayer(this);
		return player;
	}

	// 지정된 이름이 있으면 그 객체
	// 없으면 셀프 플레이어
	public	GameObject		GetExecuteObj()
	{
		GameObject go =  executeObj_Name.Get();
		if( go != null ) return go;
		return self_Player.gameObject;
	}

	public	bool	IsSyncMode()
	{
		if(	par_actPlayer.syncAction && noSync == false )
		{
			return true;
		}
		return false;
	}

	public	void	Action()
	{
		if( par_actPlayer != null && par_actPlayer.loop_Stop )
		{
			return;
		}

		if( stop_User )
		{
			return;
		}

		if( noPlay || noPlay_User )
		{
			endAction_Next();
			return;
		}

		if( saveGlobal_CurAct ) g_saveGlobal_CurAct = this;

		if( par_actPlayer != null && par_actPlayer.debug_actionExec )
		{
			// if( self_Player != null )
			// 	Debug.Log( "액션 : action_cur : " +  self_Player.name + " : " + gameObject.name );
			// else
			// 	Debug.Log( "액션 : action_cur : " + gameObject.name );
		}

		//Debug.Log( "액션 : " +	this.name );
		gameObject.SetActive(true);
		OnSetTagRecvEvent();
		OnAction();
	}

	virtual	public	void	OnSetTagRecvEvent()
	{

	}

	public	void	SetTagRecvEvent( string tag )
	{

	}

	public	void	RemoveTagRecvEvent(  )
	{

	}

	public	void			SetTempObjID( GameObject go , string tag_name )
	{
		if( self_Player != null )
			self_Player.sjDicStr_RegObj.Add(tag_name , go);
	}

	public	GameObject		GetTempObjID( string tag_name )
	{
		List<GameObject> list_obj =	self_Player.sjDicStr_RegObj.Find_List(tag_name);
		if( list_obj.Count > 0 )return list_obj[0];
		return null;
	}

	public	List<GameObject>	GetTempObjIDs( string tag_name )
	{
		return	self_Player.sjDicStr_RegObj.Find_List(tag_name);
	}


	public	void	EndAction( bool noNextAct = false )
	{
		//Debug.Log( "EndAction : " +	this.name );
		played_end = true;
		OnEnd();
		deAction_Object();

		if( noEnd_Hide_GameObj && par_actPlayer != null )
		{
			par_actPlayer.Add_NoHideAct(this);
		}

		if( noNextAct == false )
			endAction_Next();
	}

	public	void Remove_noHideAct()
	{
		if ( par_actPlayer != null)
		{
			par_actPlayer.Remove_NoHideAct(this);
		}
	}


	public	bool	IsPlayed_end() {return played_end; }

	public	void	deAction_Object()
	{
		if( noEnd_Hide_GameObj == false ) gameObject.SetActive(false);
	}

	void	endAction_Next()
	{
		if( noNextAction ) return;

		SJTrgActionPlayer_Mono par_actp =	GetComponentInParent<SJTrgActionPlayer_Mono>();
		if( par_actp == null ) return;

		par_actp.Next_Action( this );
	}

	public	SJTrgPlayer_Mono	GetPlayerSelf()
	{
		return self_Player;
	}

	public	float	PlayAni( AnimationClip ani_clip )
	{
		return 	SJ_Unity.AnimationClip_CrossPlay( self_Player.ani , ani_clip );
	}

	public	void	StopAni( AnimationClip ani_clip = null )
	{
		if( ani_clip == null )	self_Player.ani.Stop();
		else					self_Player.ani.Stop(ani_clip.name);
	}

	public	void	AddEventRecv( bool link_parent = true )
	{
		if( sjtagobj_mono.sJTagSys_Mono_Inserted != null ) return;
		self_Player.Insert_TagObj(sjtagobj_mono , -1 , "" ,  link_parent);
	}


	public	void	RemoveEventRecv()
	{
		if( sjtagobj_mono.sJTagSys_Mono_Inserted == null ) return;
		self_Player.Remove_TagObj(sjtagobj_mono);
	}


	virtual	public	bool	OnCheck_OnStartInst() {return false; }
	virtual	public	void	OnStartInst(){ }

	virtual	public	void	OnAdd() { }
	virtual	public	void	OnRemove() { }
	virtual	public	void	OnRemove_After() { }

	virtual public	void	OnAction() { }

	virtual	public	void	OnStart() { }
	virtual	public	void	OnFixUpdate() { }

	virtual	public	void	OnUpdate() { }
	virtual	public	void	OnEnd() { }
	virtual	public	void	OnEndFinal(){}

	virtual	public	void	SetValue( string arg_name , int		val ) { }
	virtual	public	void	SetValue( string arg_name , float	val ) { }
	virtual	public	void	SetValue( string arg_name , string	val ) { }
	virtual	public	void	SetValue( string arg_name , object	val ) { }
	virtual	public	void	OnArgStep(int idx) { }

	// 
	virtual	public	bool	OnEventRecv(string tag , int arg_i = 0 , string arg_s = "" , object obj = null ){return false;}
	virtual	public	int 	FixValue(	string tag , string arg_name , int		val )		{ return val; }
	virtual	public	float 	FixValue(	string tag , string arg_name , float	val )		{ return val; }
	virtual	public	bool	QueryOne(	string tag , string arg_name , ref bool ref_val )	{return false; }

	virtual	public	bool	OnEventRecv(int tag , int arg_i = 0 , string arg_s = "" , object obj = null ){return false;}
	virtual	public	int 	FixValue(	int tag , string arg_name , int		val )			{ return val; }
	virtual	public	float 	FixValue(	int tag , string arg_name , float	val )			{ return val; }
	virtual	public	bool	QueryOne(	int tag , string arg_name , ref bool ref_val )		{return false; }

}

