using UnityEngine;
using System.Collections;

public class SJTrgAction_MovePos : SJTrgAction_Mono
{
	public	float		time = 1;
	public	Transform	tr_Obj;
	public	bool		local = true;
	public	Transform	tr_tar;

	public	AnimationClip	ani_clip;
	public	bool			end_ani_clip_stop;

	float		time_cur;
	Vector3		start_pos;
	Vector3		end_pos;

	override public	void	OnAction()
	{
		time_cur = 0;
		if( local )
		{
			start_pos = tr_Obj.localPosition;
			end_pos = tr_tar.localPosition;
		}
		else
		{
			start_pos = tr_Obj.position;
			end_pos = tr_tar.position;
		}

		PlayAni( ani_clip );
	}

	override	public	void	OnUpdate()
	{
		time_cur += Time.deltaTime;
		float r = time_cur / time;

		if(  r >= 1.0f ) r = 1.0f;

		Vector3 v =	Vector3.Lerp( start_pos , end_pos , r );

		if( local ) tr_Obj.localPosition = v;
		else		tr_Obj.position = v;
		
		if( (int)r  == 1 )
		{
			if( end_ani_clip_stop ) StopAni( ani_clip );
			EndAction();
		}

	}

}
