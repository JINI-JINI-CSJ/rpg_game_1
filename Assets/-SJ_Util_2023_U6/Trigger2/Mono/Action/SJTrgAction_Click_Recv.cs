using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_Click_Recv : SJTrgAction_Default_Mono
{
	public	_SJ_SEL_GAMEOBJ		_sj_sel_gameobj;
	public	Collider[]	collider_check;
	public	bool		rayCast;
	public	Camera		cam_rayCast;


	override	public	void	OnAction()
	{
		if( collider_check.Length < 1 )
		{
			collider_check = OnGet_Collriders( _sj_sel_gameobj );
			foreach( Collider s in collider_check ) s.enabled = true;
		}

		// if( rayCast == false )
		// {
		// 	foreach( Collider s in  collider_check)AddClick_Event(s);
		// }
	}

	virtual	public	Collider[]		OnGet_Collriders( _SJ_SEL_GAMEOBJ	_sj_sel_gameobj_arg )
	{
		return _sj_sel_gameobj_arg.GetGameObj(this).GetComponentsInChildren<Collider>();
	}

	// void	AddClick_Event( Collider	coll )
	// {
	// 	SJ_NGUI_EventTrigger ecttrg = coll.GetComponent<SJ_NGUI_EventTrigger>();
	// 	if( ecttrg == null )ecttrg = coll.gameObject.AddComponent<SJ_NGUI_EventTrigger>();
	// 	ecttrg.AddFunc( gameObject , "OnClick_Click_Recv" , SJ_NGUI_EventTrigger._BTActType.Click );
	// }

	public	void	OnClick_Click_Recv( GameObject go_bt )
	{
		EndAction();
	}

	private void Update()
	{
		if( rayCast )
		{
			if( Input.GetMouseButtonUp(0) )
			{
				foreach( Collider s in  collider_check)
				{ 
					if ( SJ_Unity.Collider_RayCheck_ByMouse(s) )
					{
						EndAction();
						return;
					}

					if( cam_rayCast != null )
					{
						if ( SJ_Unity.Collider_RayCheck_ByMouse(s , cam_rayCast) )
						{
							EndAction();
							return;
						}
					}
				}
			}
			
		}
	}

}
