using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJTrgMode_Mono : MonoBehaviour
{
	[Multiline]
	public	string		Desc_Mono;
	//public	bool		default_ChildStartAction = true;

	// 시작 액션
	public	SJTrgActionPlayer_Mono	start_Action;

	// 끝 액션
	//public	SJTrgActionPlayer_Mono	end_Action;

    public  List<SJTrgActionPlayer_Mono>    lt_Action;

	// AI 선택 확률
	public	int			AI_Select_Per = 10;
	
	//[HideInInspector]
	public	SJTrgLayer_Mono	par_layer;	

	//[HideInInspector]
	public	SJTrgUnit_Mono	trgUnit_Cur;


	SJTagSys_Mono	tagSys;

	void	Awake()
	{
		tagSys = GetComponent<SJTagSys_Mono>();
	}

	public	void	Init()
	{
		SetChild_StartAction();
	}

	public	void	SetChild_StartAction()
	{
		Debug.Log( "SJTrgMode_Mono : SJTrgActionPlayer_Mono ~~~ " );
		SJTrgActionPlayer_Mono[] act_player_mono = transform.GetComponentsInChildren<SJTrgActionPlayer_Mono>(true);

		foreach( SJTrgActionPlayer_Mono s in act_player_mono )
			//if( act_player_mono != null )
		{
			Debug.Log( "SJTrgMode_Mono : SJTrgActionPlayer_Mono : " + s.gameObject.name );
			s.par_mode = this;
			s.Init();
			//start_Action = act_player_mono;
			if( start_Action == null )
				start_Action = s;
			SJTrgUnit_Mono unit_mono = s.GetComponent<SJTrgUnit_Mono>();
			if( unit_mono != null )
			{
				unit_mono.par_trgMode = this;
			}
		}

        lt_Action.Clear();
        lt_Action.AddRange( act_player_mono );
    }

	public	void	LayerPlay_TrgMode()
	{
		par_layer.Start_Mode( this );
	}

	// 인공지능일경우
	// 시작 액션으로 순차 액션
	// 트리거로 이벤트 처리


	// 현재 진행중인 이벤트
	//..


	virtual	public	void	OnEndActionPlayer( SJTrgActionPlayer_Mono act_player )
	{
		par_layer.OnEndActionPlayer( this , act_player );
	}

	public	void	AddUnit( SJTrgUnit_Mono s )
	{
		SJTagObj_Mono tag_obj = s.GetComponent<SJTagObj_Mono>();
		if( tag_obj == null )return;
		tagSys.Insert_TagObj( tag_obj );
		s.OnAdd();
	}

	public	void	RemoveUnit( SJTrgUnit_Mono s )
	{
		SJTagObj_Mono tag_obj = s.GetComponent<SJTagObj_Mono>();
		if( tag_obj == null )return;
		tagSys.Remove_TagObj(tag_obj);
		s.OnRemove();
	}


	public	void	StartAction()
	{
		//Debug.Log( "시작 모드 : " + name );

		if( start_Action != null )
			start_Action.Start_Action();
	}

	public	void	EndAction()
	{
        foreach(SJTrgActionPlayer_Mono s in lt_Action)
        {
            s.End_AllAction();
        }

		//start_Action.End_AllAction();

		//if( end_Action != null )
		//	end_Action.Start_Action();
	}

	public bool EventPlay(params int[] params_tag)
	{
		List<SJTagObj_Mono> lt = tagSys.Find_TagInt_Sort(params_tag);
		bool bPlayed = false;
		foreach (SJTagObj_Mono s in lt)
		{
			SJTrgUnit_Mono trg_unit = s.GetComponent<SJTrgUnit_Mono>();
			if (trg_unit.Play() == true) bPlayed = true;
		}
		return bPlayed;
	}

	public bool EventPlay(params string[] params_tag)
	{
		List<SJTagObj_Mono> lt = tagSys.Find_TagStr_Sort(params_tag);
		bool bPlayed = false;
		foreach (SJTagObj_Mono s in lt)
		{
			SJTrgUnit_Mono trg_unit = s.GetComponent<SJTrgUnit_Mono>();
			if (trg_unit.Play() == true) bPlayed = true;
		}
		return bPlayed;
	}


	public bool GetUnit(List<SJTrgUnit_Mono> list, params int[] params_tag)
	{
		List<SJTagObj_Mono> lt = tagSys.Find_TagInt_Sort(params_tag);
		if (lt.Count < 1) return false;
		foreach (SJTagObj_Mono s in lt)
		{
			SJTrgUnit_Mono trg_unit = s.GetComponent<SJTrgUnit_Mono>();
			list.Add(trg_unit);
		}
		return true;
	}

	public bool GetUnit(List<SJTrgUnit_Mono> list, params string[] params_tag)
	{
		List<SJTagObj_Mono> lt = tagSys.Find_TagStr_Sort(params_tag);
		if (lt.Count < 1) return false;
		foreach (SJTagObj_Mono s in lt)
		{
			SJTrgUnit_Mono trg_unit = s.GetComponent<SJTrgUnit_Mono>();
			list.Add(trg_unit);
		}
		return true;
	}


}
