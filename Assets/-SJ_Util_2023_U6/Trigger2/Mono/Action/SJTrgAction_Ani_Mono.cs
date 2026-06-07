using UnityEngine;
using System.Collections;

public class SJTrgAction_Ani_Mono : SJTrgAction_Default_Mono
{
	public	AnimationClip	ani_clip;
	public	string			anit_name;

	//public	bool			stop_pre_Ani;
	public	string			anit_name_Value;
	public	float			anit_PlayTime;

	public	bool			use_clipTime_EndTime;
	public	float			anit_ani_endTime;

	public override string OnChange_Name(){return "AniClip_State";}

	override	public	void	OnAction()
	{
		if( use_clipTime_EndTime && ani_clip != null )
		{
			anit_ani_endTime = ani_clip.length;
		}

		if( string.IsNullOrEmpty(anit_name) == false )
		{
			Animator	anit = GetExecuteObj().GetComponent<Animator>();

			//if( stop_pre_Ani ) anit.enabled = false;

			Debug.Log( "Ani_Mono : anit_name : " + anit_name );

			if( string.IsNullOrEmpty( anit_name_Value ) )
				anit.Play( anit_name );
			else
				SJ_Unity.AnimatorPlay_TotalTimeSpeed( anit ,ani_clip , anit_name , anit_name_Value , anit_PlayTime );


			if( anit_ani_endTime >= 0.01f && IsSyncMode() ) EndAction_Wait( anit_ani_endTime );
		}
		else
		{
			Animation ani = GetExecuteObj().GetComponent<Animation>();
			float play_time = 0;
			if( anit_ani_endTime > 0.01f )
			{
				play_time = anit_ani_endTime;
				SJ_Unity.AnimationClip_Play_Time_Ratio( ani , ani_clip , anit_ani_endTime );
			}else{
				play_time =	SJ_Unity.AnimationClip_CrossPlay( ani , ani_clip );
			}
			if( IsSyncMode() )
			{
				EndAction_Wait( play_time );
			}
		}
	}
}
