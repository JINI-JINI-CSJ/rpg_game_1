using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//[System.Serializable]
public class SJTrgLayer
{
	public	string	Name;

	public	SJTrgMode		mode_cur;
	public	List<SJTrgMode>	lt_SJTrgMode = new List<SJTrgMode>();

	public	SJTrgPlayer	par_player;


	public	bool	endAction_ModeSelect; // 액션 종료하면 자동 모드 선택
	public	bool	noSel_BeforeMode;

	virtual	public	void	OnEndActionPlayer( SJTrgMode mode , SJTrgActionPlayer act_player )
	{
		par_player.OnEndActionPlayer( this , mode , act_player );

		if( endAction_ModeSelect )
		{
			List<SJTrgMode> lt_mode = new List<SJTrgMode>();
			List<int>		lt_int = new List<int>();
			foreach( SJTrgMode s in lt_SJTrgMode )
			{
				if( noSel_BeforeMode )
				{
					if( s == mode )
						continue;
				}

				if( s.AI_Select_Per < 1 )continue;

				lt_mode.Add( s );
				lt_int.Add( s.AI_Select_Per );
			}

			int sel =	SJ_Unity.Random_RangeStepList( lt_int.ToArray() );
			Start_Mode( lt_mode[sel] );
		}
	}


	public	SJTrgMode	Find_TrgMode( string name = "" )
	{
		if( string.IsNullOrEmpty( name ) )
		{
			if ( lt_SJTrgMode.Count < 1 )
			{
				lt_SJTrgMode.Add( new SJTrgMode() );
			}
			return lt_SJTrgMode[0];
		}

		foreach( SJTrgMode s in lt_SJTrgMode )if( s.Name == name ) return s;
		return null;
	} 

	public	void	Start_Mode( SJTrgMode mode )
	{

		if( mode_cur != null ) mode_cur.EndAction();
		mode.StartAction();
		mode_cur = mode;
	}




}
