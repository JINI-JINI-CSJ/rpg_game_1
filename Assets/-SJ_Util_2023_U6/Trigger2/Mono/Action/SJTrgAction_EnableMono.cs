using UnityEngine;
using System.Collections;

public class SJTrgAction_EnableMono : SJTrgAction_Default_Mono
{
	public	MonoBehaviour[]	monos;
	public	bool			active;

	override	public	void	OnAction()
	{
		foreach( MonoBehaviour s in monos )  s.enabled = active;
	}
}
