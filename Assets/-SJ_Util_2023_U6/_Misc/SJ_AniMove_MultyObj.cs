using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_AniMove_MultyObj : MonoBehaviour 
{
	public	AnimationCurve		curve;
	public	Transform			tr_Player;

	public	class _TransTime
	{
		public	Transform	tr;
		public	float		time;
	}
	public	List<_TransTime>	list_TransTime;
	public	_SJ_GO_FUNC			endFunc;
	List<_TransTime>			list_TransTime_play = new List<_TransTime>();

	Vector3		pos_before;
	_TransTime	cur_TransTime;
	float		time_cur;
	bool		play;

	public	void	StartTrans( Transform _tr = null )
	{
		list_TransTime_play.Clear();
		list_TransTime_play.AddRange( list_TransTime );
		if( _tr != null )
		{
			tr_Player.position = _tr.position;

		}
		else
		{
			cur_TransTime = list_TransTime_play[0];
			list_TransTime_play.RemoveAt(0);
			tr_Player.position = cur_TransTime.tr.position;
		}

		cur_TransTime = null;
		play = true;

		pos_before = tr_Player.localPosition;
		cur_TransTime = list_TransTime_play[0];
		list_TransTime_play.RemoveAt(0);
		time_cur = 0;
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( play == false )	return;
		time_cur += Time.deltaTime;

		float r = time_cur / cur_TransTime.time;
		if( time_cur >= cur_TransTime.time ) r = 1;
		float ev = curve.Evaluate(r);
		tr_Player.localPosition = Vector3.Lerp( pos_before , cur_TransTime.tr.localPosition , ev);

		if( time_cur >= cur_TransTime.time )
		{
			if( list_TransTime_play.Count < 1 )
			{
				endFunc.Func();
				play = false;
				return;
			}
			pos_before = tr_Player.localPosition;
			cur_TransTime = list_TransTime_play[0];
			list_TransTime_play.RemoveAt(0);
			time_cur = 0;
		}
	}
}
