using UnityEngine;
using System.Collections;

public class SJTrgEvent_Collider : SJTrgEventBase
{
	public	SJTrgMode_Mono	mode_ChnageMode_Enter;
	public	SJTrgMode_Mono	mode_OnlyPlay_Enter;

	void OnTriggerEnter( Collider other )
	{
		if( mode_ChnageMode_Enter != null ) mode_ChnageMode_Enter.LayerPlay_TrgMode();
		if( mode_OnlyPlay_Enter != null )	mode_OnlyPlay_Enter.StartAction();
	}

}
