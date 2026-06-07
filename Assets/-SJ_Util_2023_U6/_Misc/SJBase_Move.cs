using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJBase_Move : MonoBehaviour 
{
	public	Transform	tr_self;
	public	bool		noUpdate_Pos;
	public	bool		localPos;

	virtual	public	void		OnSetBase_Inf( Transform _tr , bool _noUpdate_pos , bool _localPos)
	{
		tr_self = _tr;
		noUpdate_Pos = _noUpdate_pos;
		localPos = _localPos;
	}

	virtual	public	Vector3?	Update_BasePos()
	{
		return null;
	}

	virtual	public	void LastPos_Offset( Vector3 offset ) { }


}
