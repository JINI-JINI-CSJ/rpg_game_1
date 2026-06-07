using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_AniMove_MultyObj : SJTrgAction_Default_Mono
{
	public	SJ_AniMove_MultyObj sJ_AniMove_MultyObj;

	public override void OnAction()
	{
		sJ_AniMove_MultyObj.endFunc.Set(gameObject , "OnEnd_SJ" );
	}

	public	void	OnEnd_SJ()
	{
		EndAction();
	}

}
