using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_RailEventObj : MonoBehaviour 
{
	public	float	active_progress;

	public	bool	play=false;

	// Use this for initialization
	void Start () {
		
	}

	virtual	public	void	OnReady(){}
	virtual	public	void	OnPlay_Start(){}
	virtual	public	void	OnEnd(){}
	
	public	void	Ready()
	{
		gameObject.SetActive(false);
		play = false;
		OnReady();
	}

	public	bool	Check_Active( float cur_prog )
	{
		if( play )return false;

		if( cur_prog >= active_progress )
		{
			play = true;
			gameObject.SetActive(true);
			OnPlay_Start();
			return true;
		}
		return false;
	} 

	public	void	End()
	{
		gameObject.SetActive(false);
		play = false;
		OnEnd();
	}

}
