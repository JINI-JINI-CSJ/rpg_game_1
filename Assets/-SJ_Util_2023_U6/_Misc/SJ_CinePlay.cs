using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
//using Cinemachine;
using UnityEngine.Playables;

public class SJ_CinePlay : MonoBehaviour 
{
	public	PlayableDirector playableDirector;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		playableDirector.Play();
	}
}
