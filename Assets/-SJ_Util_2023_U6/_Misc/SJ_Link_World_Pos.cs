using UnityEngine;
using System.Collections;

public class SJ_Link_World_Pos : MonoBehaviour 
{
	public	bool	use_x = true;
	public	bool	use_y = true;
	public	bool	use_z = true;

    public  Vector3 offset_pos;

	public Transform	tr_world_pos;
    public GameObject   recv_Update_After;

    //public  bool noRot = false;
    public  bool noLateUpdate = false;

	public	bool copyTransVal;

	public	Vector3	pos_world;

	public bool Start_NoParent;

	public Transform tr_LinkParent;

	private void Awake()
	{
		pos_world = transform.position;
	}

	private void LateUpdate()
	{
        if(noLateUpdate == false) Update_Pos();
	}

    void OnEnable()
    {	
        if( Start_NoParent ) transform.parent = null;
		if( tr_LinkParent != null ) transform.parent = tr_LinkParent;
    }

    public void Set_Pos_Obj( Transform tr_pos )
	{
		tr_world_pos = tr_pos;
		Update_Pos();
	}

	public void Update_Pos()
	{
		if( tr_world_pos == null )
		{
			transform.position = pos_world;
			return;
		}


		if( copyTransVal )
		{
			SJ_Unity.SetEqTrans( transform , tr_world_pos );
			return;
		}

		Vector3 pos = Vector3.zero;
		if( use_x ) pos.x = tr_world_pos.position.x;
		if( use_y ) pos.y = tr_world_pos.position.y;
		if( use_z ) pos.z = tr_world_pos.position.z;

        pos += offset_pos;
		transform.position = pos;
        SJ_Unity.SendMsg(recv_Update_After , "On_SJ_Link_World_Pos_Update_After");
	}


}
