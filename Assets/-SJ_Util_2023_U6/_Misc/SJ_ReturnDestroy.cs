using UnityEngine;
using System.Collections;

public class SJ_ReturnDestroy : MonoBehaviour
{
	public	float	timer = 1;
	//public	bool	startFunc;
	public	bool	start_Enable=false;

	public	GameObject	go_return;

	public	int 		debug_state;

	private void OnEnable()
	{
		if( start_Enable )
		{
			debug_state = 1;
			StartCoroutine( "CO_End" );
		}
	}

	public	void	Return_Start( float _timer = -1 )
	{
		if( _timer > 0 ) timer = _timer;
		enabled = true;

		if( timer > 0 )
		{
			//Debug.Log( "Return_Start : " +  timer);
			StartCoroutine( "CO_End" );
		}
		else
			Return_Obj();
	}

	IEnumerator CO_End()
	{
		debug_state = 2;

		//Debug.Log( "Return_Start ----->>>> : " +  timer + " : " + Time.time ); 

		yield return new WaitForSeconds(timer);

		//Debug.Log( "Return_Start ----<<<<< : " +  timer + " : " + Time.time ); 

		Return_Obj();
		debug_state = 3;
	}


	public	void	Return_Obj()
	{
		//Debug.Log( "Return_Start  ===== Return_Obj " );
		debug_state = 4;
		if( go_return != null )	SJPool.ReturnInst_Or_Destroy(go_return);
		else					SJPool.ReturnInst_Or_Destroy(gameObject);
	}

}
