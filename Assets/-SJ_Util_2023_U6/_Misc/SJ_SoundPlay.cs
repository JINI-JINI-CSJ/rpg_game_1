using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_SoundPlay : MonoBehaviour 
{
	public	AudioClip	clip;
	public	float		vol = 1.0f;
	public	string		bgm_Name;

	public	float		repeat;

	public	bool		start_func = true;

	public void OnEnable()
	{
	}

	public void Start()
	{
		if( start_func ) PlayStart();
	}

	public	void	PlayStart()
	{
		if( repeat < 0.001f )
			SoundPlay();
		else
			InvokeRepeating( "SoundPlay" , 0, repeat );	
	}
	public	void	SoundPlay()
	{
		SJSound.PlaySound( clip , bgm_Name , false , vol );
	}
	private void OnDisable()
	{
		CancelInvoke();
	}

}
