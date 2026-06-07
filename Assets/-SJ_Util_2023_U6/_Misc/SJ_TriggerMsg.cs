using UnityEngine;
using System.Collections;

public class SJ_TriggerMsg : MonoBehaviour
{
	public	MonoBehaviour recv;

	public	string		trg_enter_func;
	public	string		trg_stay_func;
	public	string		trg_exit_func;

	public	string		Coll_enter_func;
	public	string		Coll_stay_func;
	public	string		Coll_exit_func;

	public	GameObject[]	go_Active_Enter;
	public	GameObject[]	go_Hide_Exit;

	public	SJTrgAction_Recv_Collider		actTrg_recv_coll;

	public	string		tag_filter;

	public	bool		debug;

	public bool Enter_Hide;

	bool	Check_Filter( GameObject go )
	{
		if( string.IsNullOrEmpty( tag_filter ) )return false;
		if( tag_filter == go.tag ) return true;
		return false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (debug) Debug.Log("OnTriggerEnter : " + name + " --->>> " + other.name);
		if (Check_Filter(other.gameObject)) return;
		SJ_Unity.SendMsg(recv, trg_enter_func, other, debug);
		foreach (GameObject g in go_Active_Enter) g.SetActive(true);

		if (actTrg_recv_coll != null && actTrg_recv_coll.gameObject.activeSelf) actTrg_recv_coll.OnRecv_OnTriggerEnter(other);
		
		if(  Enter_Hide )
			gameObject.SetActive( false );
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (debug) Debug.Log("OnTriggerEnter2D : " + name + " : " + collision.name);
		if (Check_Filter(collision.gameObject)) return;
		SJ_Unity.SendMsg(recv, trg_enter_func, collision, debug);
		foreach (GameObject g in go_Active_Enter) g.SetActive(true);

		//if (actTrg_recv_coll != null && actTrg_recv_coll.gameObject.activeSelf) actTrg_recv_coll.OnRecv_OnTriggerEnter(other);

		if(  Enter_Hide )
			gameObject.SetActive( false );
	}

	private void OnTriggerStay(Collider other)
	{
		if( debug ) Debug.Log( "OnTriggerStay : " + name + " : " + other.name );
		if( Check_Filter(other.gameObject) ) return;
		SJ_Unity.SendMsg( recv , trg_stay_func , other , debug );

		//foreach( GameObject g in go_Active_Enter ) g.SetActive(true);
	}


	void OnTriggerExit( Collider other )
	{
		if( debug ) Debug.Log( "OnTriggerExit : " + name + " : " + other.name );
		if( Check_Filter(other.gameObject) ) return;
		SJ_Unity.SendMsg( recv , trg_exit_func , other );
		foreach( GameObject g in go_Hide_Exit ) g.SetActive(false);
		if( actTrg_recv_coll != null && actTrg_recv_coll.gameObject.activeSelf ) actTrg_recv_coll.OnRecv_OnTriggerExit(other);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (debug) Debug.Log("OnTriggerExit2D : " + name + " : " + collision.name);
		if (Check_Filter(collision.gameObject)) return;
		SJ_Unity.SendMsg(recv, trg_exit_func, collision);
		foreach (GameObject g in go_Hide_Exit) g.SetActive(false);
		//if (actTrg_recv_coll != null && actTrg_recv_coll.gameObject.activeSelf) actTrg_recv_coll.OnRecv_OnTriggerExit(other);
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (debug) Debug.Log("OnCollisionEnter : " + name + " : " + collision.gameObject.name);

		if (Check_Filter(collision.gameObject)) return;
		SJ_Unity.SendMsg(recv, Coll_enter_func, collision);
		foreach (GameObject g in go_Active_Enter) g.SetActive(true);

		if (actTrg_recv_coll != null && actTrg_recv_coll.gameObject.activeSelf) actTrg_recv_coll.OnRecv_OnCollisionEnter(collision);
		
		if(  Enter_Hide )
			gameObject.SetActive( false );
	}

	private void OnCollisionStay(Collision collision)
	{
		if( debug ) Debug.Log( "OnCollisionStay : "+ name + " : " + collision.gameObject.name );
		if( Check_Filter(collision.gameObject) ) return;
		SJ_Unity.SendMsg( recv , Coll_stay_func , collision );
	}

	private void OnCollisionExit(Collision collision)
	{
		if( debug ) Debug.Log( "OnCollisionExit : "+ name + " : " + collision.gameObject.name );

		if( Check_Filter(collision.gameObject) ) return;
		SJ_Unity.SendMsg( recv , Coll_exit_func , collision );
		foreach( GameObject g in go_Active_Enter ) g.SetActive(false);

		if( actTrg_recv_coll != null && actTrg_recv_coll.gameObject.activeSelf ) actTrg_recv_coll.OnRecv_OnCollisionExit(collision);
	}
}

