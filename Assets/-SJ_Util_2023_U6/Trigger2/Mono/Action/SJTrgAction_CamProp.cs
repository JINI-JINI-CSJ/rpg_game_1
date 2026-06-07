using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_CamProp : SJTrgAction_Mono 
{
	public	Camera	cam;

	public	CameraClearFlags	cameraClearFlags;


	public override void OnAction()
	{
		cam.clearFlags = cameraClearFlags;
	}

}
