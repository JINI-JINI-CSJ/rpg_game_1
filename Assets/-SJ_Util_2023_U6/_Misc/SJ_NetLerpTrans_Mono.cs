using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public	class SJ_Lerp
{
	public	float	Update_Time = 0.1f;
	float			Update_Time_cur = 0;
	public	float	lerp_cur;

	public	bool	clamp;

	public	void	Init_Lerp()
	{
		Update_Time_cur = 0;
	}

	public	void	Update_Lerp()
	{
		Update_Time_cur += Time.deltaTime;
		lerp_cur = Update_Time_cur / Update_Time;

		if( lerp_cur > 1.0f && clamp )lerp_cur = 1.0f;
		OnUpdate();
	}
	virtual	public	void	OnUpdate(){}
}

[System.Serializable]
public	class SJ_Lerp_Float : SJ_Lerp
{
	public	float	val_cur;
	float			val_start;
	float			val_next;

	public	void	Init( float v )
	{
		val_cur = val_start = val_next = v;
		Init_Lerp();
	}

	public	void	Recv( float v )
	{
		val_start = val_cur;
		val_next = v;
		Init_Lerp();
	}

	override	public	void	OnUpdate()
	{
		val_cur =	Mathf.LerpUnclamped( val_start , val_next , lerp_cur );
	}
}

[System.Serializable]
public	class SJ_Lerp_Vector3 : SJ_Lerp
{
	public	Vector3	val_cur;
	public	Vector3			val_start;
	public	Vector3			val_next;

	public	void	Init( Vector3 v )
	{
		val_cur = val_start = val_next = v;
		Init_Lerp();
	}

	public	void	Recv( Vector3 v )
	{
		val_start = val_cur;
		val_next = v;
		Init_Lerp();
	}

	override	public	void	OnUpdate()
	{
		val_cur = Vector3.Slerp( val_start , val_next , lerp_cur );
	}
}


public class SJ_NetLerpTrans_Mono : MonoBehaviour
{
	public	bool	Local_Client = false;

	public	float	Update_Time = 0.1f;
	float	Update_Time_cur = 0;

	public	bool	World_Trans = true;

	public	bool	Pos_x = true;
	public	bool	Pos_y = true;
	public	bool	Pos_z = true;
	public	bool	Rot_x = true;
	public	bool	Rot_y = true;
	public	bool	Rot_z = true;

	public	Vector3		recv_pos_cur;
	public	Quaternion	recv_rot_cur;

	public	Vector3		recv_pos_next = Vector3.zero;
	public	Quaternion	recv_rot_next = Quaternion.identity;

	public	GameObject	go_NetSend;
	public	string		func_NetSendFunc = "OnSend_SJ_NetLerpTrans_Mono";

	public	GameObject	go_NetRecv;
	public	string		func_NetRecvFunc = "OnRecv_SJ_NetLerpTrans_Mono";

	bool	first_update;

	// Use this for initialization
	void Start () {
		
	}
	
	public	void	Start_Net( float _update_time = -1 )
	{
		if( _update_time > 0 )Update_Time = _update_time;
		this.enabled = true;
		Update_Time_cur = 0;
		first_update = false;
	}

	// Update is called once per frame
	void Update ()
	{
		if( first_update == false )
		{
			first_update = true;
			return;
		}

		Update_Time_cur += Time.deltaTime;

		if( Local_Client && go_NetSend != null )
		{
			if( Update_Time_cur >= Update_Time )
			{
				SJ_Unity.SendMsg( go_NetSend , func_NetSendFunc , this );
				Update_Time_cur = 0;
			}

		}else{
			float r = Update_Time_cur / Update_Time;
			if( World_Trans )
			{
				if( Pos_x || Pos_y || Pos_z ) transform.position = Vector3.LerpUnclamped( recv_pos_cur , recv_pos_next , r );
				if( Rot_x || Rot_y || Rot_z ) transform.rotation = Quaternion.SlerpUnclamped( recv_rot_cur , recv_rot_next , r );
			}else{
				if( Pos_x || Pos_y || Pos_z ) transform.localPosition = Vector3.LerpUnclamped( recv_pos_cur , recv_pos_next , r );
				if( Rot_x || Rot_y || Rot_z ) transform.localRotation = Quaternion.SlerpUnclamped( recv_rot_cur , recv_rot_next , r );
			}
		}


	}


	public	void	Write_State( BinaryWriter bw )
	{
		Vector3 pos;
		Vector3 rot;

		if( World_Trans )
		{
			pos = transform.position;
			rot = transform.rotation.eulerAngles;
		}
		else
		{
			pos = transform.localPosition;
			rot = transform.localRotation.eulerAngles;
		}

		if( Pos_x ) bw.Write( pos.x );
		if( Pos_y ) bw.Write( pos.y );
		if( Pos_z ) bw.Write( pos.z );

		if( Rot_x ) bw.Write( rot.x );
		if( Rot_y ) bw.Write( rot.y );
		if( Rot_z ) bw.Write( rot.z );
	}

	public	void	Read_State( BinaryReader br )
	{
		Update_Time_cur = 0;
		if( World_Trans )
		{
			recv_pos_cur = transform.position;
			recv_rot_cur = transform.rotation;
		}else{
			recv_pos_cur = transform.localPosition;
			recv_rot_cur = transform.rotation;
		}

		if( Pos_x ) recv_pos_next.x = br.ReadSingle();
		if( Pos_y ) recv_pos_next.y = br.ReadSingle();
		if( Pos_z ) recv_pos_next.z = br.ReadSingle();

		Vector3 vr = Vector3.zero;
		if( Rot_x ) vr.x = br.ReadSingle();
		if( Rot_y ) vr.y = br.ReadSingle();
		if( Rot_z ) vr.z = br.ReadSingle();
		recv_rot_next = Quaternion.Euler( vr );

		SJ_Unity.SendMsg( go_NetRecv , func_NetRecvFunc , this );
	}

}
