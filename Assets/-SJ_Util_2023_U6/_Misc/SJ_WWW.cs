using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;

public class SJ_WWW : SJ_Singleton_Mono
{
	static public	SJ_WWW g;
	override	public	SJ_Singleton_Mono	OnGetStatic() {return g; }
	override	public	void				OnSetStatic(SJ_Singleton_Mono s) { g = s as SJ_WWW; }

	static	public	WWW www;

	static	public	void	OpenUrl( string _url , GameObject _recv_go , string _recv_func )
	{
		g.PrcOpenUrl( _url , _recv_go , _recv_func );
	}

	IEnumerator 	PrcOpenUrl( string _url , GameObject _recv_go , string _recv_func )
	{
		www = new WWW( _url );
		yield return www;
		SJ_Unity.SendMsg( _recv_go , _recv_func );
	}


	static	public	bool	CheckError()
	{
		if( www == null ) return false;

		return false;
	}

	static	public	bool	CheckError(WWW www)
	{
		if( www == null || string.IsNullOrEmpty( www.error ) == false ) return true;
		return false;
	}


	public static string HttpGet(string url) 
	{ 
		string responseText = string.Empty; 
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); 
		request.Method = "GET"; request.Timeout = 30 * 1000; // 30초 
		//request.Headers.Add("Authorization", "BASIC SGVsbG8="); // 헤더 추가 방법 

		try
		{
			using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse()) 
			{ 
				HttpStatusCode status = resp.StatusCode; 
				Stream respStream = resp.GetResponseStream(); 
				using (StreamReader sr = new StreamReader(respStream)) 
				{ 
					responseText = sr.ReadToEnd(); 
				} 
			}			
		}
		catch (System.Exception)
		{
			
			throw;
		}


		return responseText; 
	}


}
