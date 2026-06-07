using UnityEngine;
using System.Collections;

public class SJTrgAction_LookAt : SJTrgAction_Default_Mono
{
	public	Transform	tr_Tar;
	public	float		time = -1;
	SJ_LookAt			sj_LookAt;

	public override string OnChange_Name()
	{
		return "LookAt";
	}

	override	public	void	OnAction()
	{
		SJTrgPlayer_Mono player= GetPlayerSelf();

		if( time < 0.0001f )
		{
			player.transform.LookAt( tr_Tar );
			return;
		}
		sj_LookAt = player.GetComponent<SJ_LookAt>();
		sj_LookAt.Start_LookAt( time , tr_Tar.position );

		if( noSync == false )
		{
			EndAction_Wait( time );
		}
	}




}
