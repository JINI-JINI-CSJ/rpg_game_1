using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// https://docs.google.com/spreadsheets/d/ 메인아이디/gviz/tq?tqx=out:json 하시면 됩니다. csv로 내려받을 땐 out:csv고 웹으로 볼 땐 out:html입니다
/// https://docs.google.com/spreadsheets/d/메인아이디/gviz/tq?tqx=out:csv&sheet=시트이름
/// </summary>


public class SJ_StaticCoroutine : MonoBehaviour
{
	private static SJ_StaticCoroutine mInstance = null;
 
    private static SJ_StaticCoroutine instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(SJ_StaticCoroutine)) as SJ_StaticCoroutine;
 
                if (mInstance == null)
                {
                    mInstance = new GameObject("StaticCoroutine").AddComponent<SJ_StaticCoroutine>();
                }
            }
            return mInstance;
        }
    }
 
    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this as SJ_StaticCoroutine;
        }
    }
 
    IEnumerator Perform(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
    }
 
    public static void DoCoroutine(IEnumerator coroutine)
    {
         //여기서 인스턴스에 있는 코루틴이 실행될 것이다.
        instance.StartCoroutine(instance.Perform(coroutine));    
    }


	static	public	UnityWebRequest www;
	IEnumerator CO_WebDown(string url ,GameObject recv , string func )
	{
		www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) 
		{
            Debug.Log(www.error);
			SJ_Unity.SendMsg( recv , "OnError_Web" , www.error );
        }

		SJ_Unity.SendMsg( recv , func , www );

	}

	static	public	void	WebDown( string url , GameObject recv , string func )
	{
		Debug.Log( "WebDown : " + url );
        DoCoroutine( instance.CO_WebDown(url , recv , func) );
	}

}
