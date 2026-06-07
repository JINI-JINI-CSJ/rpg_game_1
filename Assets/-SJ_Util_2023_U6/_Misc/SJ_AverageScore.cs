using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_AverageScore
{
	public	class	_AverageScore
	{
		public	float	Influence;		// 총 계산 영향도
		public	float	time_Rating;	// 평가 시간

		public	int		score;			// 현재 점수

		public	_AverageScore	before;

		float	time_cur = 0;


		public	void	Update()
		{
			if( Influence < 0 )return;

			time_cur += Time.deltaTime;
			if( time_cur >= time_Rating )
			{
				Calc();
				time_cur = 0;
			}
		}

		public	void	Calc()
		{
			// 이전꺼와 계산한다.
			if( before != null )
			{
				score = (score + before.score) /2;
				before.score = 0;
			}
		}

		public	int		Average()
		{
			if( Influence < 0 )return 0;
			return (int)((float)score * Influence);
		}

	}
	public	List<_AverageScore>		list_AverageScore = new List<_AverageScore>();

	public	void	Clear()
	{
		list_AverageScore.Clear();
	}



	public	void	Add_AverageScore( 	float	Influence , float	time_Rating )
	{
		_AverageScore s = new _AverageScore();
		s.Influence = Influence;
		s.time_Rating = time_Rating;
		list_AverageScore.Add( s );
	}

	public	void	Ready()
	{
		_AverageScore recent_s = null;
		foreach( _AverageScore s in list_AverageScore )
		{
			if( recent_s != null )
			{
				s.before = recent_s;
			}
			recent_s = s;
		}
	}


	public	void	Add_Score(int score)
	{
		list_AverageScore[0].score += score;
	}

	public	int 	Calc_All()
	{
		int		average = 0;
		foreach( _AverageScore s in list_AverageScore )
		{
			average += s.Average();
		}
		return average;
	}

	public	void	Update()
	{
		foreach( _AverageScore s in list_AverageScore )
		{
			s.Update();
		}
	}
}
