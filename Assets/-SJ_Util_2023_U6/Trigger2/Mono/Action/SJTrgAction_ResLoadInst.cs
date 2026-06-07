using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_ResLoadInst : SJTrgAction_Default_Mono
{
	public	string		path_file;
	public	string 		objName_par;

	override	public	string		OnChange_Name(){return "ResLoadInst";}

	public override void OnAction()
	{
		GameObject inst_obj =	SJ_ResPoolSys.Inst_Obj( path_file );
		if( string.IsNullOrEmpty(objName_par) == false )
		{
			GameObject go_par =	GameObject.Find( objName_par );
			if( go_par == null )
			{
				Debug.LogError( "error!! : SJTrgAction_ResLoadInst : " + objName_par );
				return;
			}
			SJ_Unity.SetEqTrans( inst_obj.transform , null , go_par.transform );
		}

		SJTrgGameObj_SelSys.SetObj(path_file  , inst_obj );
	}
}
