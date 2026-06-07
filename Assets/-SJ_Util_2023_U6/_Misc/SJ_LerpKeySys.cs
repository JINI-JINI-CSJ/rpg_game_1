using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public	enum	SJ_LERP_KEY_ACTION
{
	Pos = 0 ,
	Rot = 1,
	LookAt = 2,
	RollAngle = 3 , 
}

[System.Serializable]
public	class _SJ_LERP_KEY
{
	public	float				time_Play;
	public	float				time_Play_cur;
	public	SJ_LerpKeyObj		keyobj;

	public	_SJ_GO_FUNC recvFunc = new _SJ_GO_FUNC();

	public	void	Set( SJ_LerpKeyObj _key_obj , float _blend  )
	{
		keyobj = _key_obj;
		time_Play = _blend;
		time_Play_cur = 0;
	}

	public	bool	Update( ref float nor_blend )
	{
		time_Play_cur += Time.deltaTime;
		float r = time_Play_cur / time_Play;
		bool end = false;
		if( time_Play_cur >= time_Play )
		{ 
			r = 1;
			end = true;
		}
		nor_blend = r;
		return end;
	}


	public	void	End()
	{
		keyobj.Func_End();
		keyobj.OnEndLerp();
	}

	public	void	Start_RecvFunc()
	{
		recvFunc.Func();
		recvFunc.Init();
	}

}

[System.Serializable]
public class _SJ_LERP_KEY_LAYER_LOCK_ADD_ORDER
{
	public	SJ_LERP_KEY_ACTION	type;
	//public	bool				locking;

	public	void	Start_Locking()
	{
		SJ_LerpKeySys.LockAddKey(type , true);
	}

	public	void	End_Locking()
	{
		SJ_LerpKeySys.LockAddKey(type , false);
	}
}

[System.Serializable]
public	class _SJ_LERP_KEY_LAYER
{	
	public	bool	Lock_Add;

	public	SJ_LerpKeySys		sys;
	public	SJ_LERP_KEY_ACTION	type;
	public	_SJ_LERP_KEY		default_Key;
	public	List<_SJ_LERP_KEY>	lt_Q_key = new List<_SJ_LERP_KEY>();
	public	int					Priority_cur;	

	public	List<SJ_LerpKeyObj>	debug_lt_objkey;

	_SJ_LERP_KEY recent_default_key;

	public	void	Init(SJ_LerpKeySys _s , SJ_LERP_KEY_ACTION _t )
	{
		sys = _s;
		type = _t;
		Priority_cur = 0;
	}

	public	void	InitPriority()
	{
		Priority_cur = 0;
	}

	public	void	AddKey( SJ_LerpKeyObj _key_obj , float _blend , bool resume = false , MonoBehaviour mono = null , string func = "")
	{
		if( Lock_Add ) return;
		if( _key_obj.Priority < Priority_cur )
		{
			Debug.LogError( "주의!!! : 우선순위 낮은키 제외 : " + _key_obj.name + " : " + _key_obj.Priority + " < " + Priority_cur );
			return;
		}
		if( resume == false )
			_key_obj.StartKeyTime = Time.time;
		sys.OnAdd_PreKey( _key_obj , ref _blend , type );
		_SJ_LERP_KEY s  = new _SJ_LERP_KEY();
		s.Set( _key_obj , _blend );
		if( default_Key.keyobj == null )
		{
			default_Key = s;
			_key_obj.plaing = true;
			_key_obj.enabled = true;
		}
		else 
		{ 
			// 같은키가 있으면 안함.
			foreach( _SJ_LERP_KEY k in lt_Q_key )
			{ 
				if( k.keyobj == _key_obj )
				{
					//Debug.Log( "주위! 같은키1 : " + _key_obj.name );
					return;
				}
			}
			if( default_Key.keyobj == _key_obj )
			{
				//Debug.Log( "주위! 같은키2 : " + _key_obj.name );
				return;
			}
			lt_Q_key.Add(s);
			if( lt_Q_key.Count == 1 )
			{
				sys.OnAddKey( type , default_Key , lt_Q_key[0] );
			}
			else
			{
				sys.OnAddKey( type , lt_Q_key[lt_Q_key.Count-2] , lt_Q_key[lt_Q_key.Count-1] );
			}
			_key_obj.plaing = true;
			_key_obj.enabled = true;
			recent_default_key = default_Key;
		}
		Priority_cur = _key_obj.Priority;
		s.recvFunc.SetMono( mono , func );
		if( sys.deubg )
		{
			debug_lt_objkey.Add( _key_obj );
		}
	}

	public	bool	AddKey_Recent()
	{
		if( recent_default_key != null )
		{
			//Debug.Log( "AddKey_Recent : 주위! 최근 키 없음~ " );
			return false;
		}
		AddKey( recent_default_key.keyobj , recent_default_key.time_Play );
		return true;
	}

	public	void	PreUpdate()
	{
		if( default_Key.keyobj != null )default_Key.keyobj.update_Framed = false;
		foreach( _SJ_LERP_KEY s in lt_Q_key ) s.keyobj.update_Framed = false;
	}

	void	Update_CurFrame( SJ_LerpKeyObj s )
	{ 
		if( s != null && s.update_Framed == false )
		{ 
			s.Update_CurFrame();
			s.update_Framed = true;
		}
	}

	public	void	Update()
	{
		if( default_Key.keyobj == null ) return;

		Update_CurFrame( default_Key.keyobj );
		if( lt_Q_key.Count> 0 )
		{
			_SJ_LERP_KEY k = lt_Q_key[0];
			Update_CurFrame( k.keyobj );
			float blend = 0;
			bool end = k.Update(ref blend);
			k.keyobj.Blend( this , blend , type );
			if( end )
			{
				default_Key.End();
				default_Key = k;
				default_Key.Start_RecvFunc();
				lt_Q_key.RemoveAt(0);
			}
		}
		else
		{
			float blend = 0;
			bool end = default_Key.Update(ref blend);
			default_Key.keyobj.Blend_Value( this , blend , type );
		}
	}
}


public class SJ_LerpKeySys : MonoBehaviour
{
	static	public	SJ_LerpKeySys	g_default;

	public	float	blend_default = 0.5f;

	public	_SJ_LERP_KEY_LAYER	layer_Pos;
	public	_SJ_LERP_KEY_LAYER	layer_Rot;
	public	_SJ_LERP_KEY_LAYER	layer_LockAt;
	public	_SJ_LERP_KEY_LAYER	layer_RollAngle;
	public	Transform			tr_Pos;
	public	Transform			tr_Rot;
	public	Transform			tr_LookAt;
	public	Transform			tr_RollAngle;

	public	Vector3			pos_cur;
	public	Quaternion		rot_cur;
	public	Vector3			lookAt_cur;
	public	float			rollAnlge_cur;

	public	Vector3			pos_first;
	public	Quaternion		rot_first;
	public	Vector3			lookAt_first;
	public	float			rollAnlge_first;

	public	bool			deubg;


	private void Awake()
	{
		Debug.Log( "SJ_LerpKeySys : Awake" );
		layer_Pos.Init(this			,  SJ_LERP_KEY_ACTION.Pos );
		layer_Rot.Init(this			,  SJ_LERP_KEY_ACTION.Rot );
		layer_LockAt.Init(this		,  SJ_LERP_KEY_ACTION.LookAt );
		layer_RollAngle.Init(this	,  SJ_LERP_KEY_ACTION.RollAngle );
		OnAwake();
	}

	virtual	public	void	OnAddKey(SJ_LERP_KEY_ACTION t , _SJ_LERP_KEY k1 , _SJ_LERP_KEY k2 ){}

	virtual	public	void OnAwake() { }

	static	public	Transform Trans_Pos(){return g_default.tr_Pos;}
	static	public	Transform Trans_Rot(){return g_default.tr_Rot;}
	static	public	Transform Trans_LookAt(){return g_default.tr_LookAt;}
	static	public	Transform Trans_RollAngle(){return g_default.tr_RollAngle;}

	static	public	Vector3		CurPos(){ return Trans_Pos().position; }

	public	void	Set_ThisDefault()
	{
		g_default = this;
	}

	static	public	void	LockAddKey( SJ_LERP_KEY_ACTION	type , bool b )
	{
		switch( type )
		{
			case SJ_LERP_KEY_ACTION.Pos:		g_default.layer_Pos.Lock_Add		= b; break;
			case SJ_LERP_KEY_ACTION.Rot:		g_default.layer_Rot.Lock_Add		= b; break;
			case SJ_LERP_KEY_ACTION.LookAt:		g_default.layer_LockAt.Lock_Add		= b; break;
			case SJ_LERP_KEY_ACTION.RollAngle:	g_default.layer_RollAngle.Lock_Add	= b; break;
		}
	}

	public	void	Set_CurTransValue( Vector3 p , Quaternion r , Vector3 lk , float roll )
	{
		pos_cur = p;
		rot_cur = r;
		lookAt_cur = lk;
		rollAnlge_cur = roll;

		pos_first = p;
		rot_first = r;
		lookAt_first = lk;
		rollAnlge_first = roll;
	}

	static	public	Vector3		Pos_Cur() { return g_default.pos_cur; }
	static	public	Quaternion	Rot_Cur() { return g_default.rot_cur; }
	static	public	Vector3		LookAt_Cur() { return g_default.lookAt_cur; }
	static	public	float		RollAnlge_Cur() { return g_default.rollAnlge_cur; }

	static	public	void		InitPriority()
	{
		g_default.layer_Pos.InitPriority();
		g_default.layer_Rot.InitPriority();
		g_default.layer_LockAt.InitPriority();
		g_default.layer_RollAngle.InitPriority();
	}

	public	void	AddKey( SJ_LerpKeyObj key , float _blend_time = 0 , bool resume = false  , MonoBehaviour mono = null , string func = "")
	{
		//Debug.Log( "SJ_LerpKeySys : AddKey : " + key.name );
		key.par_sys = this;

		//키추가 전에 전처리
		//OnAdd_PreKey( key , ref _blend_time );

		if( _blend_time < 0.01f ) _blend_time = blend_default;
		
		if( key.use_Pos )
		{ 
			layer_Pos.AddKey( key , _blend_time , resume , mono , func );
		}
		if( key.use_Rot )
		{ 
			layer_Rot.AddKey( key , _blend_time , resume , mono , func );
		}

		if( key.use_LookAt )
		{ 
			layer_LockAt.AddKey( key , _blend_time , resume , mono , func );
		}

		if( key.use_RollAngle )
		{ 
			layer_RollAngle.AddKey( key , _blend_time , resume , mono , func );
		}
	}


	virtual	public	void	OnAdd_PreKey( SJ_LerpKeyObj key , ref float _blend_time , SJ_LERP_KEY_ACTION t ){}

	public	void	Update_Key()
	{
		layer_Pos.PreUpdate();
		layer_Rot.PreUpdate();
		layer_LockAt.PreUpdate();
		layer_RollAngle.PreUpdate();

		layer_Pos.Update();
		layer_Rot.Update();
		layer_LockAt.Update();
		layer_RollAngle.Update();
	}

	public	void	Update_TransObj()
	{
		if( tr_Pos != null ) tr_Pos.position = pos_cur;
		if( tr_Rot != null ) tr_Rot.rotation = rot_cur;
		if( tr_LookAt != null ) tr_LookAt.LookAt( lookAt_cur );
		if( tr_RollAngle != null ) tr_RollAngle.localRotation = Quaternion.Euler( 0,0, rollAnlge_cur);
	}
}
