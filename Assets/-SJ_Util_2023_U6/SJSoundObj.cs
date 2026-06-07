using UnityEngine;
using System.Collections;

public class SJSoundObj : SJGoPoolObj 
{
	public	AudioSource			audio_src;
	public	AudioSource			audio_user; // 유저가 직접 넘긴 오디오 소스
	[HideInInspector]
	public	SJSoundMng			sndMng;

	[HideInInspector]
	public	bool 				bOrder = false;

	bool 	bStartFrame = false;

	[HideInInspector]
	public	AudioClip 			clip_next;
	[HideInInspector]
	public	string 				bgmName_next;

	public	bool				bBGM = false;


	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () 
	{
		AudioSource source = GetAudioSource();
		if( bBGM == false && source.isPlaying == false )
		{
			if( bStartFrame )
			{
				bStartFrame = false;
				return;
			}
			SJSoundMng.OnEnd_SoundObj_S(this);
		}
	}

	override	public	void 	AllocInstSJ( GameObject prf )
	{
		audio_src = GetComponent<AudioSource>();
	}

	override	public void 	StartInstSJ()
	{
		bStartFrame = true;
		clip_next = null;
		bgmName_next = "";
		audio_user = null;

		audio_src.Stop();
		if( audio_user != null )audio_user.Stop();
	}


	public	void NextInput_Sound( AudioClip clip , string bgm_name )
	{
		clip_next = clip;
		bgmName_next = bgm_name;
	}

	override	public void 	EndInstSJ()
	{
		GetAudioSource().Stop();
	}

	public void SetUserAudioSrc( AudioSource source )
    {
        audio_user = source;
    }

	public AudioSource GetAudioSource() 
	{
		if( audio_user != null ) return audio_user;
		return audio_src;
	}

}
