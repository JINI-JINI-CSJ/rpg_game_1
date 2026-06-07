using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_ChrControl_Move : MonoBehaviour 
{
	static	public	float			gravity = -9.8f;

	public	CharacterController		cc;
	public	bool					setting_gravity = true; // 공중객체면 false 
	public	bool					cur_gravity = true;		// 점프같은 잠시동안..
	public	bool					noColl_Mode;			// 콜리더하고 충돌안함.. 그리고 중력도 안함 

	public	bool		want_pos;
	public	Vector3		posNext_Want;
	public	Vector3		add_Pos;

	Vector3	recent_Mov;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public	void	Clear_AddPos()
	{
		add_Pos = Vector3.zero;
	}

	public	void	AddPos( Vector3 v )
	{
		add_Pos += v;
	}


	public	void	WantMoveDelta( Vector3 p )
	{
		want_pos = true;
		posNext_Want = p;
	}

	public	void	Move()
	{
		Vector3 v = posNext_Want;
		if( setting_gravity || cur_gravity ) v.y += gravity * Time.deltaTime;
		recent_Mov = v + add_Pos;

		//Debug.Log( "SJ_CCM : " + posNext_Want + " : " + add_Pos );
		if( noColl_Mode == false )
		{
			if( cc.enabled )
			{ 
				//Debug.Log( "SJ_CCM : " + recent_Mov );
				cc.Move( recent_Mov );
			}
			else
			{ 
			}
		}
		else 
		{
			transform.position += recent_Mov;
		}

		posNext_Want = Vector3.zero;
		want_pos = false;
	}

	public	Vector3 GetSpeed_RecentMove()
	{
		return recent_Mov / Time.deltaTime;
	}

	public	void	Active_CCM( bool b )
	{
		enabled = b;
		cc.enabled = b;
	}

	public	void	Collider_Mode( bool b )
	{
		noColl_Mode = !b;
		cc.enabled = b;
	}

	public	void	Set_Gravity( bool b )
	{
		cur_gravity = b;
	}

	private void LateUpdate()
	{

	}
}
