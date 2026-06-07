using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public	class _SJ_SEL_GAMEOBJ
{
	public	GameObject	go_this;
	public	string		find_name;
	public	string		find_tag;
	public	bool		cur_sel_ActPlayer;

	public	bool		save_curSel;

	public	GameObject	GetGameObj( SJTrgAction_Mono act )
	{
		GameObject _go = null;
		if( go_this != null )_go = go_this;
		else if( string.IsNullOrEmpty( find_name ) == false ) _go =  GameObject.Find(find_name);
		else if( string.IsNullOrEmpty( find_tag ) == false ) _go =  GameObject.FindWithTag(find_tag);
		else if( cur_sel_ActPlayer ) _go =  act.par_actPlayer.go_curSelect;

		if( save_curSel ) act.par_actPlayer.go_curSelect = _go;

		return _go;
	}


}

public class SJTrgAction_SelectGameObj : SJTrgAction_Default_Mono
{
	public	_SJ_SEL_GAMEOBJ	_sj_sel_gameobj;

	override	public	void	OnAction()
	{
		par_actPlayer.go_curSelect = _sj_sel_gameobj.GetGameObj(this);
	}
}
