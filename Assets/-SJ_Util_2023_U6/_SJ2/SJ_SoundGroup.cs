using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 오디오 소스가 여러개 일경우
// 하위 객체마다 오디오 소스를 넣자
public class SJ_SoundGroup : MonoBehaviour
{
    [System.Serializable]
    public class SOUND_INF
    {
        public string NAME;
        public AudioSource audioSource;
        public List<AudioClip> clips;

        public float min_wait_random = 1;
        public float max_wait_random = 3;

        SJSoundObj sJSoundObj;

        public void Play( bool next_random_loop = false )
        {
            // if( clips.Count < 1 ) return;
            // if( sJSoundObj != null ) SJSound.Stop( sJSoundObj );
            // AudioClip clip = SJ_Unity.GetRandomItem( clips ) as AudioClip;
            // sJSoundObj = SJSound.PlaySound( clip , "" , false , -1 , audioSource );

            // if( next_random_loop )
            // {
            //     //  타이머의 최소값을 사운드 길이값 참조.                
            //     randomTimer.func_call.SetInst( this , "Play_RandomCall" );

            //     randomTimer.min = clip.length;
            //     randomTimer.max = clip.length + max_wait_random;
            //     randomTimer.StartRandom();       
            // }

            if( next_random_loop == false )
            {
                PlayRandomOne();
                return;
            }

            float wait_min = min_wait_random;
            float wait_max = max_wait_random;
            if( sJSoundObj != null )
            {
                wait_min += sJSoundObj.GetAudioSource().clip.length;
                wait_max += sJSoundObj.GetAudioSource().clip.length;
            }

            randomTimer.func_call.SetInst( this , "Play_RandomCall" );
            randomTimer.min = wait_min;
            randomTimer.max = wait_max;
            randomTimer.StartRandom();     
        }



        public void PlayRandomOne()
        {
            if( clips.Count < 1 ) return;
            if( sJSoundObj != null ) SJSound.Stop( sJSoundObj );
            AudioClip clip = SJ_Unity.GetRandomItem( clips ) as AudioClip;
            sJSoundObj = SJSound.PlaySound( clip , "" , false , -1 , audioSource );
        }

        public void Play_RandomCall()
        {
            PlayRandomOne();
            Play(true);
        }

        // 랜덤 타이머 사운드
        [HideInInspector]
        public SJ_RandomTimer randomTimer;

        public void Stop()
        {
            // 사운드 풀에서 가져와서 
            if( sJSoundObj != null ) SJSound.Stop( sJSoundObj );
            randomTimer.Stop();
        }

        public void UpdateTime( float time )
        {
            randomTimer.Update( time );
        }
    }
    public List<SOUND_INF> lt_SOUND_INF;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach( var s in lt_SOUND_INF )s.UpdateTime(Time.deltaTime);
    }

    public bool PlaySound( string name )
    {
        SOUND_INF inf = lt_SOUND_INF.Find( p=> p.NAME == name );
        if( inf == null )
        {
            //Debug.LogError( "PlaySound : " + name );
            return false;            
        }

        inf.Play();
        return true;
    }

    public bool StartRandom( string name )
    {
        SOUND_INF inf = lt_SOUND_INF.Find( p=> p.NAME == name );
        if( inf == null )
        {
            //Debug.LogError( "StartRandom : " + name );
            return false;            
        }
        //inf.StartRandom();
        inf.Play(true);
        return true;
    }

    public void Stop( string name = "" )
    {
        if( string.IsNullOrEmpty(name) )
        {
            foreach( var s in lt_SOUND_INF )s.Stop();
        }
        else
        {
            SOUND_INF inf = lt_SOUND_INF.Find( p=> p.NAME == name );
            if( inf == null ) return;
            inf.Stop();
        }
    }

    // Dictionary<string,AudioSource> dic_name_source = new Dictionary<string, AudioSource>();
    // public void PlaySource( string sourceName , AudioClip clip )
    // {
    //     if( clip == null ) return;
    //     if( dic_name_source.Count < 1 )
    //     {
    //         AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();            
    //         foreach( var s in audioSources )
    //         {
    //             dic_name_source[s.gameObject.name] = s;
    //         }
    //     }
    //     AudioSource find_s = null;
    //     if( dic_name_source.TryGetValue( sourceName,out find_s ) )
    //     {
    //         find_s.clip = clip;
    //         find_s.Play();
    //     }
    // }
}
