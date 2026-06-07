using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJTrgAction_RemoveInstObj_Mono : SJTrgAction_Default_Mono
{
	public	bool		use_SJObjPool = true;
	public	string		Reg_SJ_TEMP_ID;

	override	public	void	OnAction()
	{
		if( string.IsNullOrEmpty(Reg_SJ_TEMP_ID) )
		{
			SJTrgPlayer_Mono player_mono =	GetPlayerSelf();
			Return_Destroy(player_mono.gameObject);
		}
		else
		{
			List<GameObject> list_obj =	GetTempObjIDs( Reg_SJ_TEMP_ID );
			if( list_obj != null )
			{
				foreach( GameObject s in list_obj )
				{
					Return_Destroy( s );
				}
			}
		}
	}


	void	Return_Destroy(GameObject inst)
	{
		if( use_SJObjPool ) SJPool.ReturnInst( inst );
		else				GameObject.Destroy( inst  );
	}

}
