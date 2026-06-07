using UnityEngine;
using System.Collections;

public class SJ_WaitSetParent : MonoBehaviour
{
	Transform par;
	public	ParticleSystem[]	particle_systems;

	public	float	wiat_time = 2;

	public	void	Start_Particle()
	{
		foreach( ParticleSystem s in particle_systems ) s.Play();
	}

	public	void	SetWaitParent( float wait = -1 )
	{
		par = transform.parent;
		transform.parent = null;

		if( wait < 0 ) wait = wiat_time;

		foreach( ParticleSystem s in particle_systems ) s.Stop();
		StartCoroutine( CO_WaitParent(wait) );
	}

	IEnumerator	CO_WaitParent( float wait )
	{
		yield return new WaitForSeconds(wait);
		transform.parent = par;
	}
}
