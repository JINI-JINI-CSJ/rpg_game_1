using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_AniPlayEvent : MonoBehaviour
{
    public  Animator            anit;
    public  AnimationClip       clip;
    public  _SJ_GO_FUNC         go_FUNC;

    public  GameObject          cam_hide;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void    Play( MonoBehaviour mono , string func )
    {
        anit.enabled = true;
        anit.Play(clip.name);
        go_FUNC.SetMono(mono ,func);
        StartCoroutine( CO_PlayEnd( clip.length ) );
    }

    IEnumerator CO_PlayEnd( float t )
    {
        yield return new WaitForSeconds( t );
        anit.enabled = false;
        cam_hide.SetActive(false);
        go_FUNC.Func();
    }
}
