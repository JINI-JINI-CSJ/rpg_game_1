using UnityEngine;
using System.Collections;

public class SJ_WaitActive : MonoBehaviour
{

	public	float	wait_time = 1.0f;

	public float reverse_wait_time = -1; //

	public GameObject go_Active;

	public GameObject go_Inverse;
	public bool startEnable = true;

	public bool flag_active = true;

	public _SJ_GO_FUNC end_func = new _SJ_GO_FUNC();

	public int fixedFrame = -1;

	int fixedFrame_cur = 0;

	void OnEnable()
	{
		if (startEnable)
		{
			StartWait();
		}
	}

	public void StartWait()
	{
		StopAllCoroutines();		
		if (go_Active == null)
		{
			Debug.LogError("SJ_WaitActive : go_Active 객체 없음!!! ");
			return;
		}
		
		if (wait_time > 0)
		{
			StartCoroutine("CO_Func");
			return;
		}
		fixedFrame_cur = 0;
	}


	private void FixedUpdate() 
	{
		if( fixedFrame >= 1 )	
		{
			fixedFrame_cur++;
			if( fixedFrame_cur > fixedFrame )
			{
				go_Active.SetActive(flag_active);
				end_func.Func(gameObject);
			}
		}
	}

	IEnumerator CO_Func()
	{
		yield return new WaitForSeconds(wait_time);
		go_Active.SetActive(flag_active);
		if( go_Inverse != null ) go_Inverse.SetActive(!flag_active);
		end_func.Func(gameObject);

		if( reverse_wait_time > 0 )
		{
			yield return new WaitForSeconds(reverse_wait_time);
			go_Active.SetActive(!flag_active);
			if( go_Inverse != null ) go_Inverse.SetActive(flag_active);
			end_func.Func(gameObject);
		}
	}
}
