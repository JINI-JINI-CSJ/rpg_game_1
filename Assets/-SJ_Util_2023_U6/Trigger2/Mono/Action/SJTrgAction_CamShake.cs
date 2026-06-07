using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_CamShake : SJTrgAction_Mono
{
	public	Camera	cam;
	public	float	noise = 10.0f;
	public	float	time = 1;


	public override void OnAction()
	{
		SJShakeLocalTrans	shack	= cam.GetComponent<SJShakeLocalTrans>();
		if( shack == null )	shack	= cam.gameObject.AddComponent<SJShakeLocalTrans>();

		shack.SetFirstTrans();
		shack.Shaking_Start(time,0.03f ,noise,noise,noise );

		if( IsSyncMode() )
		{
			EndAction_Wait( time );
		}

	}
}
