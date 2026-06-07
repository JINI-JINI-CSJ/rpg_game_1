using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_TimerFunc
{
    public SJ_CallFunc callFunc = new SJ_CallFunc();
	public float TimeMAX;
	public float time_cur;
	public bool play;

	public void Init( float max = -1 )
    {
        time_cur = 0;
		play = true;
		if( max > 0 ) TimeMAX = max;
    }

	public void InitFunc( object _obj , string _func , float max )
    {
        callFunc.SetInst( _obj , _func );
		Init(max);
    }

	public bool UpdateFixTime( float elapse )
    {
        if( play == false ) return false;
		time_cur += elapse;
		if( time_cur >= TimeMAX )
        {
			play = false;
			callFunc.Func();
            return true;
        }
		return false;
    }

	public bool ReTimerMax( float add_max )
    {
        TimeMAX += add_max;
		return UpdateFixTime(0);
    }

}

public class SJ_TimerFuncMono : MonoBehaviour
{
	_SJ_GO_FUNC sfunc = null;

	public	float	start_time;
	float	time_cur;


	// Use this for initialization
	void Start () {
		this.enabled = false;
	}
	
	public	void	Time_Start( float _start_time = -1 )
	{
		if( _start_time > 0 ) start_time = _start_time;
		this.enabled = true;
		time_cur = start_time;
	}

	// Update is called once per frame
	void Update ()
	{
		time_cur -= Time.deltaTime;

		if( time_cur <= 0 )
		{
			time_cur = 0;
			SJ_Unity.SendMsg(sfunc.go , sfunc.func , time_cur );
			this.enabled = false;
			return;
		}
		SJ_Unity.SendMsg(sfunc.go , sfunc.func , time_cur );
	}
}
