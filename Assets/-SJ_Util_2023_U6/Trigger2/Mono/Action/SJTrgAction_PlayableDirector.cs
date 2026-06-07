using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
//using Cinemachine;

public class SJTrgAction_PlayableDirector : SJTrgAction_Mono
{
	public	PlayableDirector	playableDirector;
	public	float				timer_end = -1;
	public	bool				stop;

	[Header ("종료시 메인캠을 지정된캠의 값으로 바꿈") ]
	public	CinemachineCamera	End_curCam_State_ByCam;

	[Header ("종료시 컷신 객체 비활성화") ]
	public	bool						End_Hide_playableDirector;

	bool	prced_End_curCam_State_ByCam;

	public override string OnChange_Name(){return "PlayableDirector";}

	override	public	void	OnAction()
	{
		prced_End_curCam_State_ByCam = false;

		if( stop == false )
		{
			playableDirector.gameObject.SetActive(true);
			playableDirector.Play();
		}
		else
		{
			playableDirector.Stop();
			playableDirector.gameObject.SetActive(false);
		}

		if( timer_end > 0.001f )
			StartCoroutine(CO_Wait_Stop());
	}


	IEnumerator	CO_Wait_Stop()
	{
		yield return new WaitForSeconds( timer_end );

		//Debug.Log( "CO_Wait_Stop" );

		playableDirector.Stop();
	}


	public override void OnUpdate()
	{
		if ( playableDirector.state == PlayState.Paused )
		{
			if( prced_End_curCam_State_ByCam == false && End_curCam_State_ByCam != null )
			{
				Debug.Log( "Cam Val : src : " + End_curCam_State_ByCam.name );

				//SJ_Unity.SetEqTrans(AS_PlayerArea.g.cineCamMain.transform , End_curCam_State_ByCam.transform );
				//AS_PlayerArea.g.cineCamMain.gameObject.SetActive(true);

				prced_End_curCam_State_ByCam = true;
			}

			if( End_Hide_playableDirector )
			{
				Debug.Log( "PlayableDirector : End_Hide_playableDirector" );

				playableDirector.gameObject.SetActive(false);
			}

			if( IsSyncMode() )
				EndAction();

		}
	}

}
