using UnityEngine;
using System.Collections;

// 첨으로 업그레이드 되는 시스템에 대한 
// 튜토리얼 이벤트
public class SJTrgAction_RecvEvent_CreateObj : SJTrgAction_Default_Mono
{
	public	string	recvEvent;
	public	int		arg_int;
	public	string	arg_string;

	public	GameObject	prf;

	override	public	void	OnAction()
	{
		SetTagRecvEvent( recvEvent );
	}

	override	public	bool	OnEventRecv(string tag, int arg_i = 0 , string arg_s = "", object obj = null )
	{
		if( arg_int == arg_i && arg_string == arg_s )
		{
			GameObject.Instantiate( prf );
			return true;
		}
		return false;
	}
}
