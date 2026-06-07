using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SJ_UpdateQueueAct_Mono : MonoBehaviour
{
	public	class UPDATE_Q_ACT
	{
		public	UPDATE_Q_ACT( string f , GameObject g )
		{
			func = f;go = g;
		}
		public	string		func;
		public	GameObject	go;
	}

	public	List< UPDATE_Q_ACT >	list_q = new List<UPDATE_Q_ACT>();

	bool	play_Q = false;

	public	void	AddQueue( string func , GameObject go = null )
	{
		list_q.Add( new UPDATE_Q_ACT( func , go ) );
	}


	public	void	PlayQ()
	{
		play_Q = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( play_Q )
		{
			play_Q = false;
			PrcPlayQ();
		}
	}

	public	void	PrcPlayQ()
	{
		if( list_q.Count < 1  ) return;

		UPDATE_Q_ACT q = list_q[0];
		list_q.RemoveAt(0);
		if( q.go == null )
		{
			SJ_Unity.SendMsg( this , q.func );
		}
		else
		{
			SJ_Unity.SendMsg( q.go , q.func );
		}
	}
}
