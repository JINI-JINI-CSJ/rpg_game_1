using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;


// 트리거 플레이어
//		-트리거 레이어
//			-- 트리거 모드
//				--- 트리거 유니트
//					---- 트리거 조건 액션

public class SJTrgPlayer
{
	public	List<SJTrgLayer>	lt_SJTrgLayer = new List<SJTrgLayer>();

	//
	public	JSONNode	json = new JSONNode();


//#if UNITY_STANDALONE
//	public	SJTrgPlayer_Mono	mono;
//#endif

	virtual	public	void	OnEventPlayEnd_INT( SJTrgMode mode , params int[] event_tag )
	{
//#if UNITY_STANDALONE
//	mono.OnEventPlayEnd_INT( mode , event_tag );
//#endif
	}

	virtual	public	void	OnEventPlayEnd_STR( SJTrgMode mode , params string[] event_tag )
	{
//#if UNITY_STANDALONE
//	mono.OnEventPlayEnd_STR( mode , event_tag );
//#endif
	}

	virtual	public	void	OnEndActionPlayer( SJTrgLayer layer , SJTrgMode mode , SJTrgActionPlayer act_player  ) { }

	virtual	public	void	OnAddTrgUnit( SJTrgUnit unit ){}
	virtual	public	void	OnRemoveTrgUnit( SJTrgUnit unit ){}


	public	int		GetVal_INT(string valueName) {return json[valueName].AsInt; }
	public	float	GetVal_FLOAT(string valueName) {return json[valueName].AsFloat; }
	public	string 	GetVal_STR(string valueName) {return json[valueName]; }

	virtual	public	void	OnValueChaged( string value_name )
	{
//#if UNITY_STANDALONE
//	mono.OnValueChaged( value_name );
//#endif
	}


	public	void	AddLayer( SJTrgLayer layer )
	{
		if( Find_TrgLayer( layer.Name ) != null ) return;
		lt_SJTrgLayer.Add(layer);
		layer.par_player = this;
	}


	public	SJTrgLayer	Find_TrgLayer( string name = "" )
	{
		if( string.IsNullOrEmpty( name ) )
		{
			if( lt_SJTrgLayer.Count < 1 )
			{
				lt_SJTrgLayer.Add( new SJTrgLayer() );
			}
			return lt_SJTrgLayer[0];
		}
		
		foreach( SJTrgLayer s in lt_SJTrgLayer )if( s.Name == name ) return s;
		return null;
	}
	

	public	void	AddTrgUnit( SJTrgUnit unit , string layer_name = "" , string mode_name = "" )
	{

	}

	public	void	RemoveTrgUnit( SJTrgUnit unit , string layer_name = "" , string mode_name = "" )
	{

	}


	public	void	EventPlay( string layer_name = "" , string mode_name = "" , params int[] event_tag )
	{

	}

	public	void	EventPlay( string layer_name = "" , string mode_name = "" , params string[] event_tag )
	{

	}


}
