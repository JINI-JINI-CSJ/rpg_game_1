using UnityEngine;
using System.Collections;

public class SJ_FrameActive : MonoBehaviour
{
	public	int		viewFrame = 2;

	int				viewFrame_cur;

	void OnEnable()
	{
		viewFrame_cur = viewFrame;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( --viewFrame_cur < 0 )
		{
			gameObject.SetActive(false);
		}
	}
}
