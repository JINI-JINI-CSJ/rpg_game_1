using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
//using Cinemachine;
using UnityEngine.Playables;

public class SJ_SimpleTimePlay : MonoBehaviour
{
    [System.Serializable]
    public  class _TIME_PLAY
    {
        public  bool        play;
        public  bool        active_start;
        public  float       time_wait = 0;
        public  float       time_after = 0;
        public	PlayableDirector playableDirector;
        public  GameObject      go;
        public  _SJ_GO_FUNC     func_end;        

        float   time_cur;
        public  void    Start()
        {
            play = true;
            active_start = false;
            time_cur = 0;
        }

        public  void    Update( float t )
        {
            if( play == false ) return;

            if( active_start == false )
            {
                time_cur += t;                
                if( time_cur >= time_wait )
                {
                    active_start = true;
                    go.SetActive(true);
                    if( playableDirector != null )
                    {
                        playableDirector.Play();
                    }
                    time_cur = 0;
                }                
            }

            if( active_start )
            {
                time_cur += t;
                if( time_cur >= time_after )
                {
                    play = false;
                    go.SetActive(false);
                    func_end.Func();
                }
            }
        }
    }

    public  List<_TIME_PLAY>    lt_TIME_PLAY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach( _TIME_PLAY s in lt_TIME_PLAY )
        {
            s.Update(Time.deltaTime);
        }
    }

    public  void    Play()
    {
        gameObject.SetActive(true);
        foreach( _TIME_PLAY s in lt_TIME_PLAY )
        {
            s.Start();
        }
    }
}
