using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_ParticleActive : MonoBehaviour 
{
	private void OnEnable()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		particleSystem.Play(true);
	}
}
