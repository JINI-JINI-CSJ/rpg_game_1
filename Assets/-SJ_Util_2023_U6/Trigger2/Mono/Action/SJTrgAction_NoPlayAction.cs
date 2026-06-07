using UnityEngine;
using System.Collections;

public class SJTrgAction_NoPlayAction : SJTrgAction_Mono
{
	public	bool	noPlay_Other = true;
	public	SJTrgAction_Mono[]	noPlay_Actions;

	override	public	void	OnAction()
	{
		foreach( SJTrgAction_Mono s in noPlay_Actions )
		{
			s.noPlay = noPlay_Other;
		}
	}
}
