using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_FSMAction : MonoBehaviour
{
	public	SJ_FSMNode		fsmNode;

	virtual	public	void OnEditorInit() { }

	virtual	public	void OnExecStart( object obj ) { }
	virtual	public	void OnExecPause() { }
	virtual	public	void OnExecResume(){ }
	virtual	public	void OnEndState() { }
	virtual	public	void OnRecv_Msg(string str , int val , object obj) { }

}
