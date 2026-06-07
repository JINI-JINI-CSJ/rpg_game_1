using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_ReturnDestory : SJTrgAction_Default_Mono 
{
	public	SJ_ReturnDestroy	sj_rd;
	override	public	void	OnAction()
	{
		SJTrgPlayer_Mono self =	GetPlayerSelf();
		sj_rd.go_return = self.gameObject;
		sj_rd.Return_Start();
	}

}
