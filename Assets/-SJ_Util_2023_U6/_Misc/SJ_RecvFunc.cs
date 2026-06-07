using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_RecvFunc : MonoBehaviour 
{
	public	_SJ_GO_FUNC		func;

	public	void	OnRecv( string arg )
	{
		func.Func();
	}

}
