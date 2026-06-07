using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
//using Cinemachine;

public class SJ_CineCam : MonoBehaviour 
{
	static	public	SJ_CineCam	g;

	public	CinemachineCamera	cam_Main;
	public	CinemachineCamera	cam_Recent;

	public	class _CineCam_Inf_Backup
	{
		public	NoiseSettings		noise;
		public	LensSettings		lens;
	}


	
}
