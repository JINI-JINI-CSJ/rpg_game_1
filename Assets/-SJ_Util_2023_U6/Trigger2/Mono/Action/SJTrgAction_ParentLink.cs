using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_ParentLink : SJTrgAction_Default_Mono 
{
	public	Transform	tr_Parent;
	public	Transform	tr_Self;

	public	bool		zeroCood;

	public override string OnChange_Name(){return "ParentLink";}

	override	public	void	OnAction()
	{
		if( zeroCood ) 
		{
			SJ_Unity.SetEqTrans( tr_Self , null , tr_Parent );
		}
		else
		{
			tr_Self.parent = tr_Parent;
		}
	}
	

}
