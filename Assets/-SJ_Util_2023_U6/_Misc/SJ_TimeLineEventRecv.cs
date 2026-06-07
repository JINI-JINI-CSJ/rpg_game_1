using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class SJ_TimeLineEventRecv : MonoBehaviour 
{
	[System.Serializable]
	public	class _FUNC_INF
	{
		public	string		timeLine_Func;
		public	GameObject	recv;
		public	string		recv_Func;
	}
	public	List<_FUNC_INF>	list_FUNC_INF;

	public	void	TimeLine_EventRecv( string func )
	{
		string[] arr_func = func.Split(',');

		foreach( string s in arr_func )
		{
			Func_Exec(s);
		}
	}


	public	void	Func_Exec( string func )
	{
		_FUNC_INF st = null;
		foreach( _FUNC_INF s in list_FUNC_INF )
		{
			if( s.timeLine_Func == func )
			{
				st = s;
				break;
			}
		}
		if( st == null )
		{
			Debug.LogError( "Error!!! SJ_TimeLineEventRecv can't find function : " + func );
			return;
		}
		Debug.Log( "TimeLine_EventRecv : " + func );
		SJ_Unity.SendMsg( st.recv , st.recv_Func );
	}

	static public	SJ_TimeLineEventRecv g;

	public	PlayableDirector	playableDirector;

	private void Awake()
	{
		g = this;
	}

	static public	void	Set(PlayableDirector playableDirector)
	{
		g.playableDirector = playableDirector;
	}

	static public	PlayableDirector Get()
	{
		if( g.playableDirector == null )
		{
			Debug.LogError( "Error!!! SJ_TimeLineMng : not setting cutscene!! " );
			return null;
		}
		return  g.playableDirector;
	}

	public	void	Play(){_Play();}
	public	void	Pause(){_Pause();}
	public	void	Resume(){_Resume();}
	public	void	Stop(){_Stop();}

	static public	void	_Play(){if( Get() != null ) Get().Play();}
	static public	void	_Pause()
		{
		if( Get() != null )
			Get().Pause();
		}
	static public	void	_Resume(){if( Get() != null ) Get().Resume();}
	static public	void	_Stop(){if( Get() != null ) Get().Stop();}

}
