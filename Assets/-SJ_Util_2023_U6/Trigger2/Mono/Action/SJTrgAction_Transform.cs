using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_Transform : SJTrgAction_Mono 
{
	public	bool		world;

	
	public	Vector3		pos;
	public	Vector3		rot;
	public	Vector3		scl;


	override public	void	OnAction()
	{
		Transform	tr = executeObj_Name.Get().transform;

		if( world )
		{
			tr.position = pos;
			tr.rotation = Quaternion.Euler( rot );
		}
		else
		{
			tr.localPosition = pos;
			tr.localRotation = Quaternion.Euler( rot );
		}
	}
}
