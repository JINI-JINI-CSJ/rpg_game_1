using UnityEngine;
using System.Collections;

[System.Serializable]
public	class _SJ_CreateByRes_INF
{
	public	bool		pool_inst = true;
	public	GameObject	prf_Res;
	public	string 		Reg_SJ_TEMP_ID;
	public	string		Name_SelSys;
	public	Transform	tr_Parent;
	public	string		find_Parent;
	public	bool		add_SelfPlayer_ReturnObj;
	public	float		time_ReturnObj = -1;
	public	bool		nextAct_time_ReturnObj;
	public	bool		ngui_worldobjpos;
	public	Transform	tr_ngui_worldobjpos;
}

public class SJTrgAction_CreateByRes : SJTrgAction_Default_Mono
{
	public	_SJ_CreateByRes_INF	_inf;

	[HideInInspector]
	public	GameObject	new_Inst;

	override	public	void	OnAction()
	{
		new_Inst =	NewInst();
		// if( _inf.ngui_worldobjpos )
		// {
		// 	SJ_NGUI_WorldObjPos sj =	new_Inst.GetComponent<SJ_NGUI_WorldObjPos>();
		// 	sj.tr_worldObj = _inf.tr_ngui_worldobjpos;
		// }
	}

	public	GameObject	NewInst()
	{
		GameObject new_go = null;
		if( _inf.pool_inst )	new_go = SJPool.GetNewInst( _inf.prf_Res );
		else 					new_go = Instantiate( _inf.prf_Res );
		if( string.IsNullOrEmpty( _inf.find_Parent ) == false )
		{
			GameObject find_go = GameObject.Find( _inf.find_Parent );
			_inf.tr_Parent = find_go.transform;
		}
		SJ_Unity.SetEqTrans( new_go.transform , null , _inf.tr_Parent );

		SetTempObjID( new_go , _inf.Reg_SJ_TEMP_ID );
		if( string.IsNullOrEmpty( _inf.Name_SelSys ) == false )
		{
			SJTrgGameObj_SelSys.SetObj(_inf.Name_SelSys , new_go );
		}

		if( _inf.add_SelfPlayer_ReturnObj )
		{
			self_Player.Add_ReturnObj( new_go );
		}

		SJ_Unity.SendMsg(new_go , "OnCreateByRes");

		return new_go;
	}

	IEnumerator CO_ReturnInst()
	{
		yield return new WaitForSeconds(_inf.time_ReturnObj);
		SJPool.ReturnInst( new_Inst );
		if( _inf.nextAct_time_ReturnObj ) EndAction();
	}

}
