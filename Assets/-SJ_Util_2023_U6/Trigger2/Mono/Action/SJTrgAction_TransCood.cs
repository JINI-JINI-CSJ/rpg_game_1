using UnityEngine;
using System.Collections;

public class SJTrgAction_TransCood : SJTrgAction_Mono
{
	public	float	speed = 1.0f;
	public	bool	lookAt;
	public	bool	localCood;
	public	bool	prcUpdate_Dir;

	public	float	wait_EndAction = -1;

	Vector3	dir;

	virtual	public	Vector3		GetTargetPos() {return Vector3.zero; }

	override public	void	OnAction()
	{
		CalTarDir();

		EndAction_Wait( wait_EndAction );
	}


	void	CalTarDir()
	{
		SJTrgPlayer_Mono player =	GetPlayerSelf();

		Vector3	tarPos = GetTargetPos();
		if( lookAt )
		{
			player.transform.LookAt( tarPos );
		}
		else
		{
			if( localCood )	dir = tarPos - player.transform.localPosition;
			else			dir = tarPos - player.transform.position;

			dir.Normalize();
		}
	}

	override	public	void	OnUpdate()
	{
		if( prcUpdate_Dir )
		{
			CalTarDir();
		}

		SJTrgPlayer_Mono player =	GetPlayerSelf();

		if( lookAt )
		{
			player.transform.Translate( player.transform.forward * speed * Time.deltaTime , Space.World );
		}
		else
		{
			if( localCood )	player.transform.Translate( dir * speed * Time.deltaTime , Space.Self );
			else			player.transform.Translate( dir * speed * Time.deltaTime , Space.World );
		}
	}


}
