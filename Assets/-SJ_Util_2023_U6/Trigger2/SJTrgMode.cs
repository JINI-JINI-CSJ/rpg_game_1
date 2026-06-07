using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class SJTrgMode : SJTagSys
{
	public	string	Name;

	//[Multiline]
	//public	string Desc_Mono;
	// 등록 트리거,액션
	public	List<SJTrgUnit>		lt_Unit = new List<SJTrgUnit>();



	// 시작 액션
	public	SJTrgActionPlayer	start_Action;

	// 끝 액션
	public	SJTrgActionPlayer	end_Action;

	// AI 선택 확률
	public	int			AI_Select_Per = 10;


	// 인공지능일경우
	// 시작 액션으로 순차 액션
	// 트리거로 이벤트 처리


	// 현재 진행중인 이벤트
	//..


	[HideInInspector]
	public	SJTrgLayer	par_layer;	

	[HideInInspector]
	public	SJTrgUnit	trgUnit_Cur;




	virtual	public	void	OnEndActionPlayer( SJTrgActionPlayer_Mono act_player )
	{
		//par_layer.OnEndActionPlayer( this , act_player );
	}


	public	void	ArrangeUnit()
	{
		Clear_AllTagObj();
		foreach( SJTrgUnit s in lt_Unit )
		{
			AddUnit(s);
		}
	}


	public	void	AddUnit( SJTrgUnit s )
	{
		Insert_Obj( s );
		s.OnAdd();
	}

	public	void	RemoveUnit( SJTrgUnit s )
	{
		Remove_Obj(s);
		s.OnRemove();
	}


	public	void	StartAction()
	{
		if( start_Action != null )
			start_Action.Start_Action();
	}

	public	void	EndAction()
	{
		if( end_Action != null )
			end_Action.Start_Action();
	}

	public	bool	EventPlay( params int[] params_tag )
	{
		List<SJTagObj> lt =	Find_TagInt_Sort( params_tag );
		bool	bPlayed = false;
		foreach( SJTagObj s in lt )
		{
			SJTrgUnit trg_unit = s as SJTrgUnit;
			if( trg_unit.Play() == true ) bPlayed = true;
		}
		return bPlayed;
	}

	public	bool	EventPlay( params string[] params_tag )
	{
		List<SJTagObj> lt =	Find_TagStr_Sort( params_tag );
		bool	bPlayed = false;
		foreach( SJTagObj s in lt )
		{
			SJTrgUnit trg_unit = s as SJTrgUnit;
			if( trg_unit.Play() == true ) bPlayed = true;
		}
		return bPlayed;
	}


	public	bool	GetUnit( List<SJTrgUnit> list , params int[] params_tag )
	{
		List<SJTagObj> lt =	Find_TagInt_Sort( params_tag );
		if(lt.Count < 1)return false;
		foreach( SJTagObj s in lt )
		{
			SJTrgUnit trg_unit = s as SJTrgUnit;
			list.Add(trg_unit);
		}
		return true;
	}

	public	bool	GetUnit( List<SJTrgUnit> list , params string[] params_tag )
	{
		List<SJTagObj> lt =	Find_TagStr_Sort( params_tag );
		if(lt.Count < 1)return false;
		foreach( SJTagObj s in lt )
		{
			SJTrgUnit trg_unit = s as SJTrgUnit;
			list.Add(trg_unit);
		}
		return true;
	}


}
