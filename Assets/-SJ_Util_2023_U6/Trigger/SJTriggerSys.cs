using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//// event , trigger_list
public class SJTriggerSys : MonoBehaviour 
{
	static	public	SJTriggerSys	g;

	void Awake()
	{
		g = this;
	}



	virtual public SJTriggerExec OnGetNewInst_Exec(SJTriggerExec exec)
	{
		//GameObject new_inst = GameObject.Instantiate(exec.gameObject) as GameObject;
		//return new_inst.GetComponent<SJTriggerExec>();
		return null;
	}

	static public SJTriggerExec GetNewInst_Exec(SJTriggerExec exec)
	{
		return g.OnGetNewInst_Exec(exec);
	}

	virtual public void OnDestoryExecObj(SJTriggerExec exec)
	{
		//GameObject.Destroy(exec);

	}

	static public void DestoryExecObj(SJTriggerExec exec)
	{
		g.OnDestoryExecObj(exec);
	}



}

