using UnityEngine;
using System.Collections;

// OnAllocInstSJ

public class SJGoPoolObj : MonoBehaviour 
{
	public	SJGoPoolMng m_cPoolMsg;
	public	int 		m_nPoolObjType = 0;
	public	int 		m_InstCount = 30;
	public	float		addInst_Ratio = 0.3f;
	public	int 		UID;
	public	bool 		m_bUse = true;

	public	Transform	user_mng_Trans; // 매니저 객체 유저 지정

	public	int			new_Inst_First_Last;

	virtual	public	void 	AllocInstSJ( GameObject prf ){}
	virtual	public void 	StartInstSJ(){}
	virtual	public void 	EndInstSJ(){}

	public GameObject		GetPrefabObj()
	{
		return m_cPoolMsg.m_go_BaseObj;
	}

}
