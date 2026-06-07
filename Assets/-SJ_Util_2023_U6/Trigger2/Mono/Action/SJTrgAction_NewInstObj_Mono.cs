using UnityEngine;
using System.Collections;

public class SJTrgAction_NewInstObj_Mono : SJTrgAction_Default_Mono
{
	public	int					instCount = 1;

	public	GameObject			go_Prf;
	public	Transform			par_obj;
	public	bool				pos_selfPlayer;
	public	Transform			tr_pos;
	public	BoxCollider			pos_Random_Box;
	public	SphereCollider		pos_Random_Sphere;

	override	public	void	OnAction()
	{
		for( int i = 0 ; i < instCount ; i++ )
		{
			NewInst();
		}

	}

	virtual	public	bool	OnGetPos( ref Vector3 r_pos )
	{
		return false;
	}

	public	void	NewInst()
	{
		Vector3	pos = new Vector3();

		if( tr_pos != null ) pos = tr_pos.position;
		else	if( pos_Random_Box != null )	pos = SJ_Cood.Random_BoxBound( pos_Random_Box );
		else	if( pos_Random_Sphere != null ) pos = SJ_Cood.Random_SphereBound( pos_Random_Sphere.radius );
		else	if( pos_selfPlayer )
		{
			SJTrgPlayer_Mono player = GetPlayerSelf();
			pos = player.transform.position;
		}

		OnGetPos( ref pos );

		GameObject	inst_obj = SJPool.GetNewInst_Or_Create( go_Prf );
		if( par_obj != null )
		{
			inst_obj.transform.parent = par_obj;
			inst_obj.transform.localPosition = pos;
		}else
		{
			inst_obj.transform.position = pos;
		}
	}

}
