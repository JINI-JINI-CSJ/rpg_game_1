using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class SJTrgAction_LoadScene : SJTrgAction_Default_Mono
{
	public	string	sceneName;
	public	GameObject	go_Loading;
	public	float		delay = 0.2f;

	override	public	void	OnAction()
	{
		if( delay > 0 )
			SceneManager.LoadScene( sceneName );
		else
			StartCoroutine( CO_LoadScene() );
	}

	IEnumerator	CO_LoadScene()
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene( sceneName );
	}

}
