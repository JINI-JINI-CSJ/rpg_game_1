using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public	class _SJSound_SyncOrder
{
	public	AudioClip	clip;
	public	string		bgmName;
}


[System.Serializable]
public		class _SJSound_Play
{
	public	AudioClip[]	clips;
	public	float 		volume = 1.0f;

	public	void	Play()
	{
		SJSound.PlaySound( clips , "",false , volume );
	}
}


public class SJSoundMng : MonoBehaviour 
{
	static		public	SJSoundMng	g;

	GameObject						prf_snd_obj;
	Dictionary<string,SJSoundObj> 	dic_PlayBGM = new Dictionary<string, SJSoundObj>();
	
	public	float			BGM_volume = 0.45f;
	public	float			EFF_volume = 0.45f;
	public	bool 			playBGM = true;
	public	bool 			playEff = true;
	HashSet<string> 		usedSoundFile = new HashSet<string>();

	bool	init = false;


	public	void 	InitSoundPool()
	{
		if( init ) return;

		g = this;
		init = true;

		prf_snd_obj = new GameObject( "SJSoundObj" );
		prf_snd_obj.AddComponent<AudioSource>();
		prf_snd_obj.AddComponent<SJSoundObj>();
		//prf_snd_obj.AddComponent<TweenVolume>();
		AudioSource snd_c = prf_snd_obj.GetComponent<AudioSource>();
		snd_c.playOnAwake = false;

		//TweenVolume tw_vol = prf_snd_obj.GetComponent<TweenVolume>();
		// tw_vol.from		= 0.01f;
		// tw_vol.duration = 0.2f;
		// tw_vol.enabled  = false;
		// tw_vol.to		= BGM_volume;

		prf_snd_obj.SetActive(false);
		SJPool.Init_PoolObj( prf_snd_obj.GetComponent<SJGoPoolObj>() );
	}
	

	// Update is called once per frame
	void Update () {
	
	}


	public void SoundPool_All_Volume( float vol_eff , float vol_bgm )
	{
		BGM_volume = vol_bgm;
		EFF_volume = vol_eff;

		SJSoundObj[] snd_objs = GameObject.FindObjectsByType<SJSoundObj>(FindObjectsSortMode.None);

		foreach( var s in snd_objs )
        {
			AudioSource audioSource = s.GetAudioSource();

            if( s.bBGM )
            {
				audioSource.volume = BGM_volume;
            }
            else
            {
                audioSource.volume = EFF_volume;
            }
        }

		//foreach (GameObject go_sound in m_ltAllocObj)
		//{
		//	TweenVolume tw_vol = go_sound.GetComponent<TweenVolume>();
		//	tw_vol.to = vol;
		//}
	}


	public	void 	OnEnd_SoundObj( SJSoundObj sj_snd_obj )
	{
		if( sj_snd_obj.clip_next != null )
		{
			PlaySound( sj_snd_obj.clip_next , sj_snd_obj.bgmName_next );
		}

		SJSoundObj bgm_before = null;
		if(	dic_PlayBGM.TryGetValue( sj_snd_obj.gameObject.name , out bgm_before ) )dic_PlayBGM.Remove( sj_snd_obj.gameObject.name );

		SJPool.ReturnInst( sj_snd_obj.gameObject );
	}


	static public void OnEnd_SoundObj_S(SJSoundObj sj_snd_obj)
	{
		g.OnEnd_SoundObj(sj_snd_obj);
	}


	public	SJSoundObj 	PlaySound( AudioClip	clip , string bgmName = "" , bool bOrder = false , float vol = -1.0f , AudioSource src_user = null )
	{
		if( playEff == false && string.IsNullOrEmpty(bgmName) ) return null; // 사운드 토글

		if( clip == null ) return null;

		//if( clip != null )			Debug.Log( "PlaySound : " + clip.name );

		GameObject	inst = SJPool.GetNewInst( prf_snd_obj.gameObject );
		//AudioSource snd_c = inst.GetComponent<AudioSource>();

		SJSoundObj 	sj_snd_inst = inst.GetComponent<SJSoundObj>();

		sj_snd_inst.SetUserAudioSrc( src_user );

		AudioSource snd_c = sj_snd_inst.GetAudioSource();

		snd_c.time = 0;		
		snd_c.clip = clip;
		sj_snd_inst.bOrder = bOrder;
		sj_snd_inst.sndMng = this;


		if( string.IsNullOrEmpty(bgmName) )
		{
			// 일반 사운드.
			sj_snd_inst.bBGM = false;
			snd_c.loop = false;
			if( vol < 0.0f ) vol = EFF_volume; // 사운드 볼륨
			snd_c.volume = vol;
	
			snd_c.Play();
		}else{
			// 비지엠.
			sj_snd_inst.bBGM = true;
			snd_c.loop = true;
			if( vol < 0.0f ) vol = BGM_volume; // 사운드 볼륨
			if( playBGM ) 	snd_c.volume = vol; // bgm 토글
			else 			snd_c.volume = 0.0f;

			// 같은 비지엠..
			SJSoundObj bgm_pair = null;
			if (dic_PlayBGM.TryGetValue(bgmName, out bgm_pair))
			{
				AudioSource source_pair = bgm_pair.GetAudioSource();
				if( source_pair.clip == clip )
				{
					//Debug.Log( "주의!!! SJSOUND 이미 같은 BGM : " + bgmName );
					SJPool.ReturnInst( inst );
					return null;
				}
			}

			snd_c.Play();
			StopBGM( bgmName );
			dic_PlayBGM[bgmName] = sj_snd_inst;
		}

		return sj_snd_inst;
	}

	public	bool 	Check_BGM( string bgmName)
	{
		SJSoundObj bgm_pair = null;
		if(	dic_PlayBGM.TryGetValue( bgmName , out bgm_pair ) )
		{
			return true;
		}	
		return false;
	}


	public	void 	StopBGM( string bgmName )
	{
		SJSoundObj bgm_pair = null;
		if(	dic_PlayBGM.TryGetValue( bgmName , out bgm_pair ) )
		{
			dic_PlayBGM.Remove( bgmName );
			OnEnd_SoundObj( bgm_pair );
		}
	}

	public void 	Stop( SJSoundObj sJSoundObj )
    {
        OnEnd_SoundObj( sJSoundObj );
    }

	public	void 	ToggleSoundEff( bool auto_toggle = true , bool user_toggle = false )
	{
		if( auto_toggle )
		{
			if( playEff )	playEff = false;
			else 			playEff = true;
		}else{
			playEff = user_toggle;
		}
	}


	public	void 	ToggleBGM( bool auto_toggle = true , bool user_toggle = false )
	{
		if( auto_toggle )
		{
			if( playBGM )	playBGM = false;
			else 			playBGM = true;
		}else{

			if( playBGM == user_toggle ) return;

			playBGM = user_toggle;
		}

		//Debug.Log( " ToggleBGM : " + playBGM.ToString() );

		foreach( KeyValuePair< string,SJSoundObj> pair in dic_PlayBGM )
		{
			AudioSource source = pair.Value.GetAudioSource();
			if(playBGM)
			{
				// TweenVolume tw_v = pair.Value.GetComponent<TweenVolume>();
				// tw_v.enabled = true;
				// tw_v.PlayForward();
				// pair.Value.audio_src.gameObject.SetActive(true);
				source.Play();
				//Debug.Log( "Play : " + pair.Value.audio_src.clip.name );
			}
			else
			{
				// TweenVolume tw_v = pair.Value.GetComponent<TweenVolume>();
				// tw_v.enabled = true;
				// tw_v.PlayReverse();
				source.Stop();
				//Debug.Log( "Stop : " + pair.Value.audio_src.clip.name );
			}
		}
	}

	public void Wait_ToggleBGM(float fWait)
	{
		ToggleBGM();
		StartCoroutine(CO_ToggleBGM( fWait ));
	}

	public void Wait_ToggleBGM( AudioClip snd_time )
	{
		Wait_ToggleBGM( snd_time.length + 1.0f );
	}

	IEnumerator 	CO_ToggleBGM( float wait )
	{
		yield return new WaitForSeconds(wait);
		ToggleBGM();
	}

	public	void StopAll_Init()
	{
		//Debug.Log( "StopAll_Init ->>>> "  );
		// foreach( KeyValuePair< string,SJSoundObj> pair in dic_PlayBGM )
		// {
		// 	pair.Value.audio_src.Stop();
		// 	Debug.Log( "StopAll_Init : " + pair.Key );
		// }

		dic_PlayBGM.Clear();
		//ReturnAllInst();
		SJPool.ReturnInst_All( prf_snd_obj );
	}

}

public	class SJSound
{
	public	static	SJSoundMng		g_Mng = null;

	//public	static	void 	InitSound( float bgm_vol = 1.0f )
	//{
	//	if( g_Mng == null )
	//	{
	//		GameObject 	go_mng = GameObject.Find( "/_SJSoundMng" );
	//		if( go_mng == null )
	//		{
	//			go_mng =	GameObject.Instantiate( Resources.Load("_SJSoundMng") ) as GameObject ;
	//			go_mng.name = "_SJSoundMng";
	//			g_Mng = go_mng.GetComponent<SJSoundMng>();
	//			g_Mng.BGM_volume = bgm_vol;
	//			g_Mng.InitSoundPool();
	//		}

	//	}
	//	g_Mng.StopAll_Init();
	//}

	public	static	void	Init()
	{
		SJSoundMng[] sJSoundMngs = GameObject.FindObjectsByType<SJSoundMng>( FindObjectsSortMode.None );

		if (sJSoundMngs.Length < 1)
		{
			GameObject go_mng = new GameObject();
			SJSoundMng sJSoundMng = go_mng.AddComponent<SJSoundMng>();
			go_mng.name = "_SJSoundMng";
			g_Mng = sJSoundMng;
		}
		else
		{
			g_Mng = sJSoundMngs[0];

			if (sJSoundMngs.Length > 1)
			{
				for (int i = 1; i < sJSoundMngs.Length; i++)
				{
					GameObject.Destroy(sJSoundMngs[i].gameObject);
				}
			}
		}
		g_Mng.InitSoundPool();
	}

	static	public	SJSoundObj 	PlaySound( AudioClip	clip , string bgmName = "" , bool bOrder = false  , float vol = -1.0f , AudioSource src_user = null )
	{
		return	g_Mng.PlaySound( clip , bgmName , bOrder , vol , src_user );
	}

	static	public	SJSoundObj 	PlaySound( AudioClip[]	snd_list , string bgmName = "" , bool bOrder = false  , float vol = -1.0f , AudioSource src_user = null)
	{
		if( snd_list.Length < 1 )
			return null;
		int idx = UnityEngine.Random.Range( 0 , snd_list.Length );
		return	g_Mng.PlaySound( snd_list[idx] , bgmName , bOrder , vol , src_user );
	}

	static	public	SJSoundObj 	PlaySound_OneShot_Random( AudioClip[] snd_list )
	{
		if( snd_list.Length < 1 )
			return null;
		int idx = UnityEngine.Random.Range( 0 , snd_list.Length );
		return	PlaySound( snd_list[idx] );
	}


	static public SJSoundObj PlaySound( string snd_file , string res_path = "" , string bgmName = "", bool bOrder = false, float vol = 1.0f)
	{
		return null;
	}

	static public SJSoundObj PlaySound( string[] snd_file_list , string res_path = "" , string bgmName = "", bool bOrder = false)
	{
		return null;
	}

	static public SJSoundObj PlaySound_OneShot_Random( string[] snd_file_list , string res_path = "" )
	{
		return null;
	}


	static	public	bool 	Check_BGM(string bgmName)
	{
		return g_Mng.Check_BGM( bgmName );
	}


	static	public	void 	StopBGM( string bgmName )
	{
		g_Mng.StopBGM( bgmName );
	}

	static public void Stop( SJSoundObj sJSoundObj )
    {
        g_Mng.Stop( sJSoundObj );
    }


	static	public	void 	StopAll_Init()
	{
		g_Mng.StopAll_Init();
	}


	static	public	void 	ToggleSoundEff( bool auto_toggle = true , bool user_toggle = false )
	{
		g_Mng.ToggleSoundEff( auto_toggle ,  user_toggle );
	}

	static	public	void 	ToggleBGM( bool auto_toggle = true , bool user_toggle = false  )
	{
		g_Mng.ToggleBGM( auto_toggle , user_toggle );
	}

	static	public void Wait_ToggleBGM(AudioClip snd_time)
	{
		g_Mng.Wait_ToggleBGM( snd_time );
	}

	static public 	bool 	Get_ToggleBGM(){return g_Mng.playBGM;}
	static public	bool 	Get_ToggleBGMSndEff(){return g_Mng.playEff;}

	static	public void SoundPool_All_Volume( float vol_eff , float vol_bgm )
    {
        g_Mng.SoundPool_All_Volume( vol_eff ,  vol_bgm );
    }

}

