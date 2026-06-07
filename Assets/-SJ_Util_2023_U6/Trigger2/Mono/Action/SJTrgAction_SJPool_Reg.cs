using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_SJPool_Reg : SJTrgAction_Mono 
{
	public	GameObject[]	prfs;

	override	public	void	OnAction()
	{
		foreach( GameObject s in prfs )
		{
			SJPool.Init_PoolObj( s );
		}
	}
}
