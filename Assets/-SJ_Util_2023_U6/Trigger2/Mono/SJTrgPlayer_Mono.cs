using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

[System.Serializable]
public	class	SJ_ANICLIP_NAME
{
	public	AnimationClip	ani_clip;
	public	string			ani_name;

	public	bool IsAble()
	{
		if( ani_clip != null || string.IsNullOrEmpty(ani_name) == false )
		{
			return true;
		}
		return false;
	}

}

public class SJTrgPlayer_Mono : MonoBehaviour
{
	[System.Serializable]
	public	class _SJ_ANI_SETTING
	{
		public	AnimationClip	clip;
		public	int				layer;
		public	float			speed = 1;
	}

	[Multiline]
	public	string			Desc;

	public	List<_SJ_ANI_SETTING>	list_SJ_ANI_SETTING = new List<_SJ_ANI_SETTING>();
	public	SJTrgLayer_Mono			default_layerMono;
	public	SJTrgLayer_Mono			layerMono_cur;
	public	List<SJTrgLayer_Mono>	lt_SJTrgLayer = new List<SJTrgLayer_Mono>();


	public List< SJTrgActionPlayer_Mono >	lt_childActPlayer = new List<SJTrgActionPlayer_Mono>();

	public	_SJArgVal	argVal;
	public	Animation	ani;
	public	Animator	anit;
	public	JSONClass	json = new JSONClass();

	// 
	public	List<SJTrgAction_Mono>	list_all_action = new List<SJTrgAction_Mono>();

	// 트리거 이벤트 임시 오브젝트 등록
	public	SJ_DicStr_ListObj<GameObject>	sjDicStr_RegObj = new SJ_DicStr_ListObj<GameObject>();

	// 레이어 없는 액션 오브젝트
	public	List<SJTrgAction_Mono>	list_act_Self = new List<SJTrgAction_Mono>();

	// 
	List<GameObject>		list_ReturnObj = new List<GameObject>();

	//[HideInInspector]
	public	SJTagSys_Mono	sjtagsys_mono;

	//public	bool	isDefault_TrgPlayer;
	//static	public	SJTrgPlayer_Mono	g;


	private void Awake()
	{
		//if( isDefault_TrgPlayer )g = this;
	}

	public	void	Action_OnArgStep( int idx )
	{
		foreach( SJTrgAction_Mono s in list_all_action )s.OnArgStep(idx);
	}

	public	void	Add_ReturnObj( GameObject go )
	{
		list_ReturnObj.Add(go);
	}

	public	void	Clear_ReturnObj()
	{
		list_ReturnObj.Clear();
	}

	public	void	ReturnAll_ReturnObj()
	{
		foreach( GameObject s in list_ReturnObj )
		{
			SJPool.ReturnInst(s);
		}
		list_ReturnObj.Clear();
	}

	// editer 
	public	void	Init_Editor()
	{
		OnPreInit_Editor();
		OnInit_Editor();
		Child_Init_Editor();
	}
	// editer
	public	void	Child_Init_Editor()
	{
		lt_SJTrgLayer.Clear();
		//for( int i = 0 ; i < transform.childCount ; i++ )
		//{
		//	SJTrgLayer_Mono layer_mono = transform.GetChild(i).GetComponent< SJTrgLayer_Mono >();
		//	if( layer_mono != null )
		//	{
		//		layer_mono.par_player = this;
		//		if( default_layerMono == null ) default_layerMono = layer_mono;
		//		layer_mono.Init();
		//		lt_SJTrgLayer.Add(layer_mono);
		//	}
		//}

		SJTrgLayer_Mono[] layers = transform.GetComponentsInChildren<SJTrgLayer_Mono>(true);
		foreach( SJTrgLayer_Mono s in layers )
		{
			s.par_player = this;
			if( default_layerMono == null ) default_layerMono = s;
			s.Init();
			lt_SJTrgLayer.Add(s);
		}

		list_all_action.Clear();
		SJTrgAction_Mono[] act_mono_list = GetComponentsInChildren<SJTrgAction_Mono>(true);
		foreach( SJTrgAction_Mono act in act_mono_list )
		{
			act.self_Player = this;
			act.Init_Editer();
		}

		list_all_action.AddRange( act_mono_list );
	}



	public	void	SJ_Init()
	{
		foreach( _SJ_ANI_SETTING s in list_SJ_ANI_SETTING )
		{

			if( ani == null )
			{
				Debug.LogError( "!!! SJTrgPlayer_Mono : Animation 컴포넌트 업다!!! " );
			}

			AnimationState at =	ani[ s.clip.name ];

            if( at == null )
            {
                Debug.Log("!!! SJTrgPlayer_Mono : at == null : clip.name : " + s.clip.name + " : " + gameObject.name + " : 레거시 타입 체크요망~" );
                continue;
            }

			at.layer = s.layer;
			at.speed = s.speed;
		}
		foreach( SJTrgAction_Mono s in list_all_action ) s.Init_Inst();

		foreach (SJTrgAction_Mono s in list_act_Self)
		{
			AddAction_Self(s,true);
		}
	}
	
	public	void	Play_DefaultLayer()
	{
		argVal.SetValue();
		if( default_layerMono != null )
		{
			default_layerMono.Start_Layer();
			layerMono_cur = default_layerMono;
		}
	}

	public	void	Stop_AllLayer()
	{
		if( layerMono_cur != null ) layerMono_cur.Stop_AllMode();
		foreach( SJTrgLayer_Mono s in lt_SJTrgLayer )
		{
			s.Stop_AllMode();
		}
	}

	public	void	Play_Layer( string layerName )
	{
		Stop_AllLayer();
		foreach( SJTrgLayer_Mono s in lt_SJTrgLayer )
		{
			if( s.gameObject.name.Contains( layerName ) )
			{
				s.Start_Layer();
				layerMono_cur = s;
				return;
			}
		}
		Debug.LogError("Error! Play_Layer : " + gameObject.name + " : " +layerName);
	}

	virtual	public	void	OnPreInit_Editor() { }
	virtual	public	void	OnInit_Editor() { }


	public	Animation		GetAni() {return ani; }

	// over
	virtual	public	void	OnEventPlayEnd_INT( SJTrgMode_Mono mode , params int[] event_tag ) {}
	virtual	public	void	OnEventPlayEnd_STR( SJTrgMode_Mono mode , params string[] event_tag ){ }
	virtual	public	void	OnValueChaged( string value_name ) { }


	virtual	public	void	OnEndActionPlayer( SJTrgLayer_Mono layer , SJTrgMode_Mono mode , SJTrgActionPlayer_Mono act_player  ) { }

	virtual	public	void	OnAddTrgUnit( SJTrgUnit_Mono unit ){}
	virtual	public	void	OnRemoveTrgUnit( SJTrgUnit_Mono unit ){}


	public	void	JVal_Set( string valueName , int	val , int type = -1 )	{ json[valueName].AsInt = val;		OnValueChanged(valueName,type); }
	public	void	JVal_Set( string valueName , float	val , int type = -1 )	{ json[valueName].AsFloat = val;	OnValueChanged(valueName,type); }
	public	void	JVal_Set( string valueName , string  val , int type = -1 )	{ json[valueName] = val;			OnValueChanged(valueName,type); }

	public	void	JVal_Add( string valueName , int	val , int type = -1 )	{json[valueName].AsInt += val;OnValueChanged(valueName,type);}
	public	void	JVal_Add( string valueName , float	val , int type = -1 )	{ json[valueName].AsFloat += val;	OnValueChanged(valueName,type); }
	public	void	JVal_Add( string valueName , string  val , int type = -1 )	{ json[valueName] += val;			OnValueChanged(valueName,type); }

	public	void	JVal_Sub( string valueName , int	val , int type = -1 )	{ json[valueName].AsInt -= val;		OnValueChanged(valueName,type); }
	public	void	JVal_Sub( string valueName , float	val , int type = -1 )	{ json[valueName].AsFloat -= val;	OnValueChanged(valueName,type); }
	public	void	JVal_Sub( string valueName , string  val , int type = -1)	{  }

	public	void	JVal_Mul( string valueName , int	val , int type = -1)	{ json[valueName].AsInt *= val;		OnValueChanged(valueName,type); }
	public	void	JVal_Mul( string valueName , float	val , int type = -1)	{ json[valueName].AsFloat *= val;	OnValueChanged(valueName,type); }
	public	void	JVal_Mul( string valueName , string  val , int type = -1)	{  }

	public	void	JVal_Div( string valueName , int	val , int type = -1)	{ json[valueName].AsInt /= val;		OnValueChanged(valueName,type); }
	public	void	JVal_Div( string valueName , float	val , int type = -1)	{ json[valueName].AsFloat /= val;	OnValueChanged(valueName,type); }
	public	void	JVal_Div( string valueName , string  val , int type = -1)	{  }

	public	int		GetJVal_INT(string valueName) {return json[valueName].AsInt; }
	public	float	GetJVal_FLOAT(string valueName) {return json[valueName].AsFloat; }
	public	string 	GetJVal_STR(string valueName) {return json[valueName]; }

	virtual	public	void	OnValueChanged( string valueName , int type) { }

	public	bool	Play_Ani( SJ_ANICLIP_NAME sj_ani )
	{
		if( sj_ani.ani_clip != null )
		{
			SJ_Unity.AnimationClip_CrossPlay( ani , sj_ani.ani_clip );
			return true;
		}
		else if ( string.IsNullOrEmpty( sj_ani.ani_name ) == false )
		{
			anit.Play( sj_ani.ani_name );
			return true;
		}
		return false;
	}

	public	void	Play_Ani( string anit_name )
	{
		anit.Play( anit_name );
	}

	public	void	Stop_Ani(  )
	{
		if( ani != null )	ani.Stop();
		else if( anit )		anit.StopPlayback();
	}


	public	void	Add_ChildActionPlayer( SJTrgActionPlayer_Mono actPlayer )
	{
		lt_childActPlayer.Add( actPlayer );
		actPlayer.OnAddEvent( this );
	}


	public	void	Remove_ChildActionPlayer( SJTrgActionPlayer_Mono actPlayer )
	{
		lt_childActPlayer.Remove( actPlayer );
		actPlayer.OnRemoveEvent( this );
	}


	public	void	AddAction_Self( SJTrgAction_Mono act , bool noAdd = false )
	{
		//Debug.Log( "AddAction_Self" );

		act.self_Player = this;
		SJ_Unity.SetEqTrans( act.transform , null , transform );

		if( noAdd == false )
			list_act_Self.Add( act );

		if( act.sjtagobj_mono != null )
		{
			if( act.sjtagobj_mono.AddUser() > 0 )
			{
				Insert_TagObj( act.sjtagobj_mono );
			}
		}
		act.OnAdd();
	}

	public	void	RemoveAction_Self( SJTrgAction_Mono act , bool return_inst )
	{
		//Debug.Log( "RemoveAction_Self" );

		list_act_Self.Remove( act );
		act.OnRemove();
		if( return_inst ) SJPool.ReturnInst( act.gameObject );
	}


	public	void	AddLayer( SJTrgLayer_Mono layer )
	{

	}

	public	void	AddTrgUnit( SJTrgUnit_Mono unit , string layer_name = "" , string mode_name = "" )
	{

	}

	public	void	RemoveTrgUnit( SJTrgUnit_Mono unit , string layer_name = "" , string mode_name = "" )
	{

	}


	public	void	EventPlay( object obj = null , params int[] event_tag )
	{
		foreach( int s in event_tag )	OnEventRecv(s , 0, "" ,obj );
	}

	public	void	EventPlay( object obj = null , params string[] event_tag )
	{
		foreach( string s in event_tag )	OnEventRecv(s , 0,"",obj);
	}


	public	void	Wait_ReturnInst( float wait ) 
	{
		StartCoroutine( CO_ReturnInst(wait) );
	}

	IEnumerator CO_ReturnInst( float wait )
	{
		yield return new WaitForSeconds(wait);
		SJPool.ReturnInst_Or_Destroy( gameObject );
	}


	// 태그 시스템 함수
	public	bool	IsTagObj( int noTagInt = -1 , string noTagStr = "" )
	{
		if( sjtagsys_mono == null ) return false;
		return sjtagsys_mono.IsTagObj( noTagInt , noTagStr );
	}

	public	bool	Insert_TagObj( SJTagObj_Mono tag_obj, int noTagInt = -1 , string noTagStr = "" , bool link_parent = true   )
	{
		if( sjtagsys_mono == null )
		{
			Debug.LogError( "!!! Insert_TagObj : return false : sjtagsys_mono == null" );
			return false;
		}
		return 	sjtagsys_mono.Insert_TagObj( tag_obj , noTagInt , noTagStr , link_parent );
	}
	
	public	void	Remove_TagObj( SJTagObj_Mono tag_obj )
	{
		if( sjtagsys_mono == null ) return;
		sjtagsys_mono.Remove_TagObj( tag_obj );
	}

	public	void	Remove_TagObj_After( SJTagObj_Mono tag_obj )
	{
		if( sjtagsys_mono == null ) return;
		sjtagsys_mono.Remove_TagObj_After( tag_obj );
	}

	public	HashSet<SJTagObj_Mono>	Remove_TagObjInt( int tag )
	{
		if( sjtagsys_mono == null ) return null;
		return	sjtagsys_mono.Remove_TagObjInt( tag );
	}

	public	HashSet<SJTagObj_Mono>	Find_TagInt( params int[] params_tag )
	{
		if( sjtagsys_mono == null ) return null;
		return	sjtagsys_mono.Find_TagInt( params_tag );
	}

	public	HashSet<SJTagObj_Mono>	Find_TagStr( params string[] params_tag )
	{
		if( sjtagsys_mono == null ) return null;
		return	sjtagsys_mono.Find_TagStr( params_tag );
	}
	

	// string tag
	public	bool	OnEventRecv(string tag , int arg_i = 0 , string arg_s = "" , object obj = null )
	{
		bool b = false;
		HashSet<SJTagObj_Mono> hs_obj = Find_TagStr( tag );
		if( hs_obj == null ) return false;

		List<SJTagObj_Mono>	lt_temp = new List<SJTagObj_Mono>( hs_obj );

		foreach( SJTagObj_Mono s in lt_temp )
		{
			if(	s.sjtrgaction_mono.OnEventRecv(tag ,arg_i ,  arg_s , obj ) ) b = true;
		}
		return b;
	}

	public	int 	FixValue( string tag , string arg_name , int val )
	{
		HashSet<SJTagObj_Mono> hs_obj = Find_TagStr( tag );
		if( hs_obj == null ) return val;
		foreach( SJTagObj_Mono s in hs_obj )val =	s.sjtrgaction_mono.FixValue(tag ,arg_name ,  val );
		return val;
	}

	public	float 	FixValue( string tag , string arg_name , float	val )
	{
		HashSet<SJTagObj_Mono> hs_obj = Find_TagStr( tag );
		if( hs_obj == null ) return val;
		foreach( SJTagObj_Mono s in hs_obj )val =	s.sjtrgaction_mono.FixValue(tag ,arg_name ,  val );
		return val;
	}

	public	bool	QueryOne( string tag , string arg_name , ref bool ref_val )
	{
		HashSet<SJTagObj_Mono> hs_obj = Find_TagStr( tag );
		if( hs_obj == null ) return false;
		foreach( SJTagObj_Mono s in hs_obj )
			if(	s.sjtrgaction_mono.QueryOne(tag ,arg_name , ref ref_val ) ) return true;
		return false;
	}

	// int tag
	virtual	public	bool	OnEventRecv(int tag , int arg_i = 0 , string arg_s = "" , object obj = null)
	{
		bool b = false;
		HashSet<SJTagObj_Mono> hs_obj = Find_TagInt( tag );
		if( hs_obj == null ) return false;
		foreach( SJTagObj_Mono s in hs_obj )
		{
			if(	s.sjtrgaction_mono.OnEventRecv(tag ,arg_i ,  arg_s , obj ) ) b=true;
		}
		return b;
	}

	virtual	public	int 	FixValue(	int tag , string arg_name , int		val )
	{
		HashSet<SJTagObj_Mono> hs_obj = Find_TagInt( tag );
		if( hs_obj == null ) return val;
		foreach( SJTagObj_Mono s in hs_obj )val =	s.sjtrgaction_mono.FixValue(tag ,arg_name ,  val );
		return val;
	}

	virtual	public	float 	FixValue(	int tag , string arg_name , float	val )
	{
		HashSet<SJTagObj_Mono> hs_obj = Find_TagInt( tag );
		if( hs_obj == null ) return val;
		foreach( SJTagObj_Mono s in hs_obj )val =	s.sjtrgaction_mono.FixValue(tag ,arg_name ,  val );
		return val;
	}

	virtual	public	bool	QueryOne(	int tag , string arg_name , ref bool ref_val )
	{
		HashSet<SJTagObj_Mono> hs_obj = Find_TagInt( tag );
		foreach( SJTagObj_Mono s in hs_obj )
			if(	s.sjtrgaction_mono.QueryOne(tag ,arg_name , ref ref_val ) ) return true;
		return false;
	}

	virtual	public	bool	QueryOne_RefReturn(	int tag , string arg_name  = "" )
	{
		bool ref_b = false;
		QueryOne(  tag , arg_name , ref ref_b );
		return  ref_b;
	}

	public	void	Tag_SendMsg( int tag , string func , object arg = null )
	{
		HashSet<SJTagObj_Mono> hs_obj = Find_TagInt( tag );
		foreach( SJTagObj_Mono s in hs_obj )SJ_Unity.SendMsg( s.gameObject , func , arg );
	}

}

