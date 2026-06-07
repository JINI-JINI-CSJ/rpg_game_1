using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_TimeLineEventRecv_TrgAct : MonoBehaviour
{
	[System.Serializable]
	public	class _FUNC_INF
	{
		public	string				timeLine_Func;
		public	SJTrgAction_Mono	act;
		public	string				arg_str;
	}
	public	List<_FUNC_INF>	list_FUNC_INF;

	public	void	TimeLine_EventRecv( string func )
	{
		_FUNC_INF st = null;
		foreach( _FUNC_INF s in list_FUNC_INF )
		{
			if( s.timeLine_Func == func )
			{
				st = s;
				break;
			}
		}
		if( st == null )
		{
			Debug.LogError( "Error!!! SJ_TimeLineEventRecv can't find function : " + func );
			return;
		}
		
	}
}
