using UnityEngine;
using System.Collections;

public class SJTrgAction_DestroyGameObj : SJTrgAction_Default_Mono
{
	public	string 		SJ_TEMP_ID;
	public	GameObject	go_Inst;

	override	public	void	OnAction()
	{
		GameObject del_obj = null;
		del_obj = GetTempObjID(SJ_TEMP_ID);
		if( go_Inst != null ) del_obj = go_Inst;
		GameObject.Destroy( del_obj );
	}

}
