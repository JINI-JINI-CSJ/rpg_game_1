using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_Recv_Collider : SJTrgAction_Mono 
{

	public	void	OnRecv_OnTriggerEnter( Collider other )
	{
		EndAction();
	}

	public	void	OnRecv_OnTriggerExit( Collider other )
	{
		EndAction();
	}
	
	public void		OnRecv_OnCollisionEnter(Collision collision)
	{
		EndAction();
	}

	public void		OnRecv_OnCollisionExit(Collision collision)
	{
		EndAction();
	}

}
