using UnityEngine;

[System.Serializable]
public class SJ_RandomTimer
{
    public bool play;
    public float min;
    public float max;
    public float cur_timer;
    public float elapse;

    public SJ_CallFunc func_call = new SJ_CallFunc();

    public void Init( float _min , float _max , object obj_call , string func , bool _play )
    {
        min = _min;
        max = _max;
        func_call.SetInst( obj_call , func );
        if( _play )
        {
            StartRandom();
        }
    }

    public void StartRandom()
    {
        play = true;
        elapse = 0;
        cur_timer = UnityEngine.Random.Range( min , max );
    }

    public void Update( float time )
    {
        if( play == false ) return;
        elapse += time;
        if( elapse >= cur_timer )
        {
            elapse = 0;
            func_call.Func();
            StartRandom();
        }
    }

    public void Stop()
    {
        play = false;
    }
}
