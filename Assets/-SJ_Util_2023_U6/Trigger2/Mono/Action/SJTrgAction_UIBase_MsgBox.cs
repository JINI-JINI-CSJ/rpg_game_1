using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_UIBase_MsgBox : SJTrgAction_Mono
{
	public enum _MSG_BOX_BT_TYPE
	{
		None = 0	,
		OK			, 
		OK_CANCEL	,
		YES_NO		,
		CUSTOM
	}

	public	struct _BT_Inf
	{
		public	string			Name;
		public	GameObject		recv;
		public	string			func;
	}

	public	class _MsgBox_Inf
	{
		public	bool	show;
		public	string	Title;
		public	string	Msg;
		public	_MSG_BOX_BT_TYPE	type = _MSG_BOX_BT_TYPE.OK;
		public	_BT_Inf[]			_bt_infs;
	}

	public	_MsgBox_Inf _msgbox_inf;

	public	GameObject	recv;
	public	string		func;

	override	public	void	OnAction()
	{
		SJ_Unity.SendMsg( recv , func , _msgbox_inf );
	}
}
