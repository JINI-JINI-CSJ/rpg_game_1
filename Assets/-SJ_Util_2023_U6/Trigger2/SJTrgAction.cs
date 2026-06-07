using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public	class _SJTRG_PLAYER_ID
{
	public	int		tag_int;
	public	string	tag_str;
	
	public	virtual	SJTrgPlayer	OnGetPlayer( SJTrgAction action )
	{
		if( tag_int == 0 && string.IsNullOrEmpty( tag_str ) )
		{
			return	action.GetPlayerSelf();
		}
		if( tag_int > 0 ) return SJTrg.GetPlayer(tag_int);
		if( string.IsNullOrEmpty( tag_str ) == false ) return SJTrg.GetPlayer(tag_str);

		return null;
	}

	public	JSONBinaryTag	value_Type_Get;
	public	string			value_Name_Get;

	public	int		GetValue_Int( SJTrgAction action  )
	{
		SJTrgPlayer player = OnGetPlayer(action);
		if( player != null ) return	player.json[value_Name_Get].AsInt;
		return 0;
	}

	public	float		GetValue_Float(SJTrgAction action)
	{
		SJTrgPlayer player = OnGetPlayer(action);
		if( player != null ) return	player.json[value_Name_Get].AsFloat;
		return 0;
	}
}

[System.Serializable]
public class SJTrgAction
{
	public	bool				noSync;		// 바로 리턴 , 다음 액션
	public	_SJTRG_PLAYER_ID	playerTag;
	public	SJTrgUnit			par_trgUnit;


	public	bool	IsSyncMode()
	{
		if(	par_trgUnit.syncAction && noSync == false )
		{
			return true;
		}
		return false;
	}

#if UNITY_STANDALONE
	[HideInInspector]
	public	SJTrgAction_Mono	mono;
#endif

	public	void	Action()
	{
#if UNITY_STANDALONE
		mono.OnAction();
#endif
		OnAction();
	}
	public	void	EndAction()
	{
		OnEnd();
#if UNITY_STANDALONE
		mono.OnEnd();
#endif
		par_trgUnit.Next_Action();
	}

	public	SJTrgPlayer	GetPlayerSelf()
	{
		return par_trgUnit.par_trgMode.par_layer.par_player;
	}

	virtual	public	void	OnAdd() { }
	virtual	public	void	OnRemove() { }

	virtual public	void	OnAction() { }

	virtual	public	void	OnStart() { }
	virtual	public	void	OnUpdate() { }
	virtual	public	void	OnFixUpdate() { }
	virtual	public	void	OnEnd() { }

}





// 트리거 모드 바꾸기
// 객체 속성 관련
public	enum SJ_TRG_Action_Base
{
	None = 0 ,
	TrgMode_Change ,
	PropValue_Set ,
	PropValue_Add ,
	PropValue_Sub ,
}

[System.Serializable]
public class SJTrgAction_Base : SJTrgAction
{
	public	SJ_TRG_Action_Base	func;

	public	string			trgLayerName;
	public	string			Change_trgModeName;

	public	JSONBinaryTag	value_Type;
	public	string			value_Name;
	public	int				value_Int;
	public	float			value_Float;
	public	string			value_String;
	public	object			value_obj;


	override	public	void	OnAdd()
	{
		if( func == SJ_TRG_Action_Base.TrgMode_Change ) return;

		SJTrgPlayer player  =	playerTag.OnGetPlayer(this);
		if( player == null )	return;
		switch( func )
		{
			case SJ_TRG_Action_Base.PropValue_Add:
				{
					JSONNode j = player.json;
					switch( value_Type )
					{
						case JSONBinaryTag.IntValue:	j[value_Name].AsInt		+= value_Int;	break;
						case JSONBinaryTag.FloatValue:	j[value_Name].AsFloat	+= value_Float;	break;
					}
				}
				break;

				case SJ_TRG_Action_Base.PropValue_Sub:
				{
					JSONNode j = player.json;
					switch( value_Type )
					{
						case JSONBinaryTag.IntValue:	j[value_Name].AsInt		-= value_Int;	break;
						case JSONBinaryTag.FloatValue:	j[value_Name].AsFloat	-= value_Float;	break;
					}
				}
				break;
		}

		player.OnValueChaged( value_Name );
	}

	override	public	void	OnRemove()
	{
		if( func == SJ_TRG_Action_Base.TrgMode_Change ) return;

		SJTrgPlayer player  =	playerTag.OnGetPlayer(this);
		if( player == null )	return;
		switch( func )
		{
			case SJ_TRG_Action_Base.PropValue_Add:
				{
					JSONNode j = player.json;
					switch( value_Type )
					{
						case JSONBinaryTag.IntValue:	j[value_Name].AsInt		-= value_Int;	break;
						case JSONBinaryTag.FloatValue:	j[value_Name].AsFloat	-= value_Float;	break;
					}
				}
				break;

				case SJ_TRG_Action_Base.PropValue_Sub:
				{
					JSONNode j = player.json;
					switch( value_Type )
					{
						case JSONBinaryTag.IntValue:	j[value_Name].AsInt		+= value_Int;	break;
						case JSONBinaryTag.FloatValue:	j[value_Name].AsFloat	+= value_Float;	break;
					}
				}
				break;
		}

		player.OnValueChaged( value_Name );
	}

	override public	void	OnAction()
	{
		SJTrgPlayer player  =	playerTag.OnGetPlayer(this);

		if( player == null )	return;

		switch( func )
		{
			case SJ_TRG_Action_Base.TrgMode_Change:
				{
					//SJTrgLayer trgLayer = player.Find_TrgLayer(trgLayerName);
					//if( trgLayer == null )return;
					//trgLayer.Start_Mode( Change_trgModeName );
				}
				break;

			case SJ_TRG_Action_Base.PropValue_Set:
				{
					JSONNode j = player.json;
					switch( value_Type )
					{
						case JSONBinaryTag.IntValue:	j[value_Name].AsInt		= value_Int;	break;
						case JSONBinaryTag.FloatValue:	j[value_Name].AsFloat	= value_Float;	break;
						case JSONBinaryTag.String:		j[value_Name].Value		= value_String; break;
						//case JSONBinaryTag.obejct: j[value_Name].AsInt = value_Int; break;
					}

					player.OnValueChaged( value_Name );
				}
				break;
		}
	}
}