using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJTrgLayer_Mono : MonoBehaviour
{
	[Multiline]
	public	string			Desc_Mono;

	public	SJTrgMode_Mono			startMode;
	
	public	bool					endAction_ModeSelect; // 액션 종료하면 자동 모드 선택
	public	bool					noSel_BeforeMode;
	public	SJTrgMode_Mono			mode_cur;
	public	List<SJTrgMode_Mono>	lt_SJTrgMode = new List<SJTrgMode_Mono>();

	public	SJTrgPlayer_Mono		par_player;

	public	void	Init()
	{
		Child_Init();
	}

	public	void	Child_Init()
	{
		Debug.Log( "SJTrgLayer_Mono : Child_Init " );

		lt_SJTrgMode.Clear();
		for( int i = 0 ; i < transform.childCount ; i++ )
		{
			SJTrgMode_Mono mode_mono = transform.GetChild(i).GetComponent<SJTrgMode_Mono>();
			if( mode_mono != null )
			{
				if( startMode == null ) startMode = mode_mono;
				mode_mono.par_layer = this;
				Debug.Log( "SJTrgLayer_Mono : Child_Init ADD : " + mode_mono.name );
				lt_SJTrgMode.Add( mode_mono );
				mode_mono.Init();
			}
		}
	}

	public	void	Start_Layer()
	{
		//Debug.Log( "Start_Layer : " + name );

		if( gameObject.activeSelf == false ) return;
		foreach( SJTrgMode_Mono s in lt_SJTrgMode )  s.gameObject.SetActive(false);
		if( startMode != null )
		{
			Start_Mode( startMode );
		}
	}

	public	void	Stop_AllMode()
	{
		if( mode_cur != null )
		{
			mode_cur.EndAction();
			mode_cur = null;
		}
	}


	virtual	public	void	OnEndActionPlayer( SJTrgMode_Mono mode , SJTrgActionPlayer_Mono act_player )
	{
		par_player.OnEndActionPlayer( this , mode , act_player );

		if( endAction_ModeSelect )
		{
			List<SJTrgMode_Mono> lt_mode = new List<SJTrgMode_Mono>();
			List<int>		lt_int = new List<int>();
			foreach( SJTrgMode_Mono s in lt_SJTrgMode )
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



	public	void	Start_Mode( SJTrgMode_Mono mode )
	{
		if( mode_cur != null )
		{
			mode_cur.EndAction();
			mode_cur.gameObject.SetActive(false);
		}
		mode.gameObject.SetActive(true);
		mode.StartAction();
		mode_cur = mode;
	}

}
