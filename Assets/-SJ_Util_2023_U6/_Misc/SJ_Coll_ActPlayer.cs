using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_Coll_ActPlayer : MonoBehaviour 
{

	public	SJTrgActionPlayer_Mono	actionPlayer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		actionPlayer.Start_Action();
	}

}
