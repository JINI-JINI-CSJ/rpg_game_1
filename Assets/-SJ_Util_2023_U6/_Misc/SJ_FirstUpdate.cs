using UnityEngine;
using System.Collections;

public class SJ_FirstUpdate : MonoBehaviour
{
	public	_SJ_GO_FUNC		go_func;

	bool	updated = false;

	public	bool	Get_Updated() {return updated; }


	void	Prced()
	{
		if( updated == false )
		{
			updated = true;
			SJ_Unity.SendMsg( go_func.go , go_func.func );
		}
	}

	// Update is called once per frame
	void Update (){}

	private void OnGUI()
	{
		Prced();
	}

	private void OnDisable()
	{
		updated = false;
	}

}
