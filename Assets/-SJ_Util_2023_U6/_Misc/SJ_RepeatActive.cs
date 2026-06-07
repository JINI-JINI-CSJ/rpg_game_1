using UnityEngine;
using System.Collections;

public class SJ_RepeatActive : MonoBehaviour
{
	public	int		repeatCount = 10;
	int				repeat_cur;
	public	float	timeTerm	= 0.3f;
	float			timeTerm_cur;
	public	GameObject[]	go_list;
	public	Renderer[]		renderers;

	public void		Start_Repeat()
	{
		enabled = true;
		timeTerm_cur = 0;
		repeat_cur = 0;
	}

	public	void	Active(bool b = true)
	{
		foreach( GameObject go in go_list )go.SetActive(b);
		foreach( Renderer s in renderers )s.enabled = b;
		enabled = false;
	}

	
	// Update is called once per frame
	void Update ()
	{
		timeTerm_cur += Time.deltaTime;
		if( timeTerm_cur >= timeTerm )
		{
			timeTerm_cur = 0;
			foreach( GameObject go in go_list )go.SetActive(!go.activeSelf);
			foreach( Renderer s in renderers )s.enabled = !s.enabled;
			
			if(	repeat_cur++ > repeatCount )
			{
				enabled = false;
				Active(false);
			}
		}
	}
}
