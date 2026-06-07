using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_SoundPlay : SJTrgAction_Mono 
{
	public	AudioClip	clip;
	public	float		vol = 1;
	public	bool		bgm;

	override	public	string		OnChange_Name(){return "Sound_Play";}

	override public	void	OnAction()
	{
		if( bgm )
			SJSound.PlaySound( clip , "MAIN_BGM" , false , vol );
		else
			SJSound.PlaySound( clip ,"",false , vol );

	}
}
