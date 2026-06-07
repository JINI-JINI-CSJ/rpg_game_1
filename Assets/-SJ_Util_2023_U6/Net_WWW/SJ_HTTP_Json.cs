using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;

using System.Threading;

//using System.Text;
using System.Net;

using System.Security.Cryptography.X509Certificates; 


public class _SJ_SEND_DATA_Q
{
	public	bool		post;

	public	WWW			www;
	public 	string		send_url;

	public	string		data_str;

	public	byte[]		data;
	public	Hashtable	header;

	public	Dictionary<string,string>	dic_header;

	public	GameObject	recv_gameObj; 
	public	string 		recv_func;
	public	SJ_HTTP_Json.DlgFunc_recvSelf	dlgFunc_recvSelf;

	public	void		InitValue()
	{
		post			= false;
		send_url		= "";
		data			= null;
		header			= null;
		recv_gameObj	= null;
		recv_func		= "";
		dlgFunc_recvSelf = null;
	}


	public	void	Copy( _SJ_SEND_DATA_Q other )
	{
		this.www			= other.www;
		this.post			= other.post;
		this.send_url		= other.send_url;
		this.data			= other.data;
		this.header			= other.header;
		this.recv_gameObj	= other.recv_gameObj;
		this.recv_func		= other.recv_func;
		this.dlgFunc_recvSelf = other.dlgFunc_recvSelf;
	}
}

public class RequestState
{
    public WebRequest			webRequest;
    public string				errorMessage;
	public	_SJ_SEND_DATA_Q		send_q;

    public RequestState ()
    {
        webRequest = null;
        errorMessage = null;
		send_q = null;
    }
}


public	class _SJ_HTTPS_SEND_ARG
{
	public	_SJ_SEND_DATA_Q		send_q;
	public	X509Certificate2	certificate_arg;
}




public class SJ_HTTP_Json : MonoBehaviour 
{
	public	JSONClass		json = new JSONClass();
	bool	bSended			= false;
	string	send_packet_name;

	public	string		url;

	public	bool		testServer;
	public	string		url_testServer;

	public	bool		encryptData;
	public	string		encryptData_Key;

	public	bool		use_JWT;
	public	bool		use_HTTPS_Class;
	public	int			JWT_Update_Second = 40;

	bool				first_sended = false;
	DateTime			time_last_JWT;
	string				jwt_str;

	public	string		tag_packet		= "packet";
	public	string		tag_packetCount = "packetCount";

	public	GameObject	recv_gameObj;
	public	string		recv_func;
	
	// 응답 받는 델리게이트 함수.
	public	delegate	void	DlgFunc_recvSelf( JSONNode jnode );
	public	DlgFunc_recvSelf	dlgFunc_recvSelf;

	public	bool			offlineMode;
	public	bool			autoPacketCount = true; 
	public	int				curPacketCount;

	// 아예 네트웍 연결이 안대있다
	public	GameObject		go_NotReachable;

	// 보내고 로딩 화면
	public	GameObject		go_Wait_Show;
	public	bool			noHide_Wait_Show;

	// 몇초 이상 못 받았을때 보여주는거
	public	float			recv_Wait_Time = 5.0f;
	public	GameObject		go_NoRecv_Error;
	public	bool			post_send = true;
	public	string			url_add;
	public	string			url_add_sended;

	public	bool			debug_log;


	static	public	bool			error_Critical = false;

	static	public	SJ_HTTP_Json	sj_HTTP_Json_recent_sended = null;

	_SJ_SEND_DATA_Q			recent_send_q = new _SJ_SEND_DATA_Q();

	virtual	public	void	On_LockAction(){}
	virtual	public	void	On_UnlockAction(){}

	public	delegate	void	Dlg_Offline_Json( JSONNode	j ,  _SJ_SEND_DATA_Q send_q );

	static	Dlg_Offline_Json	dlg_Offline_Json;


	public	void	Set_Recv_Func( GameObject _go_recv , string _func ) { recv_gameObj = _go_recv; recv_func = _func;  }

	public	void	Set_Wait_Show( GameObject go ){ go_Wait_Show = go; }

	public	JSONClass	Start_Packet( string adding_url , string pakect_name = "")
	{
		url_add = adding_url;
		json.ClearNode();
		if( string.IsNullOrEmpty( pakect_name ) == false )json[tag_packet] = pakect_name;

		if( autoPacketCount )
		{
			curPacketCount++;
			Set_KeyInt( tag_packetCount , curPacketCount);
		}
		send_packet_name = pakect_name;
		return json;
	}

	//------------------------------------------------------------------------------------------------
	// json
	public	void	Set_KeyValue( string key , string val ){json[key] = val;}
	public	void	Set_KeyJNode( string key , JSONNode	json_node ){json[key] = json_node;}
	public	void	Set_KeyInt( string key , int val ){json[key].AsInt = val;}
	public	void	Set_KeyInt64( string key , Int64 val ){json[key].AsInt64 = val;}
	public	void	Set_KeyIntList( string key , string tag_key , List<int> list_val )
	{
		int i = 0;
		foreach( int val in list_val )
		{
			json[key][i].AsInt = val;
			i++;
		}
	}
	public	void	GetValueList_INT( JSONNode jdata , List<int> list)
	{
		list.Clear();
		foreach( JSONNode jvalue in jdata.Childs ) {list.Add( jvalue.AsInt );}
	}
	public	string	Get_Key_Str( JSONNode json , string key )
	{
		JSONNode j_find = json[key];
		if( j_find == null )return "";
		return	j_find;
	}
	public	int	Get_Key_Int( JSONNode json , string key )
	{
		JSONNode j_find = json[key];
		if( j_find == null )return 0;
		return	j_find.AsInt;
	}
	//------------------------------------------------------------------------------------------------

	public	void	LockAction()
	{
		//if(recv_gameObj != null )	Obj_misc.GameObj_SetActive( go_Wait_Show , true );

		On_LockAction();
	}

	public	void	UnlockAction()
	{
		//if( noHide_Wait_Show == false ) Obj_misc.GameObj_SetActive( go_Wait_Show , false );

		On_UnlockAction();
	}

	public	void	Set_NoHide_WaitShow( bool b )
	{ 
		if( b )
		{
			noHide_Wait_Show = true;
		}else{
			noHide_Wait_Show = false;
			//Obj_misc.GameObj_SetActive( go_Wait_Show , false );
		}
	}


	public static Dictionary<K,V> HashtableToDictionary<K,V> (Hashtable table)
	{
		Dictionary<K,V> dict = new Dictionary<K,V>();
		foreach(DictionaryEntry kvp in table)
			dict.Add((K)kvp.Key, (V)kvp.Value);
		return dict;
	}

	public	bool	Send()
	{
		_SJ_SEND_DATA_Q send_q_cur = new _SJ_SEND_DATA_Q();
		send_q_cur.InitValue();
		send_q_cur.recv_gameObj = recv_gameObj;
		send_q_cur.recv_func = recv_func;
		send_q_cur.dlgFunc_recvSelf = dlgFunc_recvSelf;

		if( offlineMode )
		{
			Recv_Prc("" , send_q_cur );
			return true;
		}

		if( Application.internetReachability == NetworkReachability.NotReachable ) 
		{ 
			// 인터넷 연결 안됨 ( 핸드폰 망 / 와이파이 둘다 안될 때 ) 
			OnError_NoReachable();
			return false;
		}

		LockAction();

		if(	bSended )
		{
			if(debug_log)Debug.Log( "주의! 먼저 보낸 패킷 도착 안했음! : 시도한 패킷 :" +  url_add );
		}
		bSended = true;
		WWW www = null;

		recent_send_q.InitValue();
		if( post_send )
		{
			// [Unity] JSON Type 웹서버로 POST
			// http://collabocean.blogspot.kr/2014/12/unity-www-json.html

			string	json_str = json.ToString();

			byte[] data = null;

			if( encryptData )
			{
				string str_enc = "";
				str_enc = SJ_Ase.Encrypt_128_WithJava( json_str , encryptData_Key );
				data = Encoding.UTF8.GetBytes( str_enc );
			}else{
				data = Encoding.UTF8.GetBytes( json_str.ToCharArray());
			}

			Hashtable header = new Hashtable();
			header.Add("Content-Type", "text/json");
			header.Add("Content-Length", data.Length);

			if( use_JWT )
			{
				bool new_jwt = false;
				if( first_sended )
				{
					TimeSpan time_span = DateTime.Now - time_last_JWT;
					if(debug_log)Debug.Log( "JWT 갱신 체크 : DateTime.Now			: "	+ DateTime.Now );
					if(debug_log)Debug.Log( "JWT 갱신 체크 : time_last_JWT			: "	+ time_last_JWT );
					if(debug_log)Debug.Log( "JWT 갱신 체크 : time_span.TotalSeconds	: " + time_span.TotalSeconds );

					if( time_span.TotalSeconds >= JWT_Update_Second )
					{
						if(debug_log)Debug.Log( "JWT 갱신 한닷!  " );
						new_jwt = true;
						time_last_JWT = DateTime.Now;
					}else{
						if(debug_log)Debug.Log( "JWT 갱신 안함~~ " );
					}
				}else{
					new_jwt = true;
					time_last_JWT = DateTime.Now;
				}

				if( new_jwt ) jwt_str = SJ_JWT.GetAccessToken("","","");
				header.Add("Authorization", jwt_str );
			}

			string send_url = url;
			if( testServer ) send_url = url_testServer;
			if( string.IsNullOrEmpty(url_add) == false )send_url += "/" + url_add;
			
			Debug.Log( "SEND : " + send_url + " : " + json_str);

			send_q_cur.post		= true;
			send_q_cur.send_url	= send_url;
			send_q_cur.data		= data;
			send_q_cur.header	= header;


			Dictionary<string,string> dic_header = HashtableToDictionary<string,string>(header);
			send_q_cur.dic_header = dic_header;

			if( use_HTTPS_Class )
			{
				StartCoroutine(	"CO_Send_HttpWebRequest" , send_q_cur);
			}else{
				www = new WWW( send_url , data,  dic_header );
			}

			send_q_cur.www		= www;

		}else{
			string get_arg = "?";
			
			// 최상위 키 데이터만.
			bool first = true;
			foreach( KeyValuePair<string, JSONNode>  k in json )
			{
				if( first )
				{
					first = false;
				}else{
					get_arg += "&";
				}

				string k_s = k.Key;
				string k_v = k.Value;

				if(debug_log)Debug.Log( " GET :  k : " + k_s + "  : " + k_v );

				get_arg += k_s + "=" + k_v;
			}

			string send_url = url;
			if( testServer ) send_url = url_testServer;
			if( string.IsNullOrEmpty(url_add) == false )send_url += "/" + url_add;

			send_url += get_arg;

			if(debug_log)Debug.Log( "SJ_HTTP_Json : URL : " + send_url );

			www = new WWW( send_url);

			send_q_cur.www		= www;
			send_q_cur.post		= false;
			send_q_cur.send_url	= send_url;
		}

		recent_send_q.Copy( send_q_cur );

		sj_HTTP_Json_recent_sended = this;

		if( use_HTTPS_Class == false )
			StartCoroutine( "CO_Wait_Recv" ,  send_q_cur );

		first_sended = true;
		if(recv_gameObj != null && use_HTTPS_Class == false )
			StartCoroutine( "CO_Wait_TimeOut" );

		return true;
	}


	//const string	STR_ERROR_CERT = "";

	IEnumerator	CO_Send_HttpWebRequest( _SJ_SEND_DATA_Q send_q )
	{
if(debug_log)Debug.Log( "비동기 use_HTTPS_Class 시작**********************>>>>>>>>>>>>>>>>>>>>>>>>" );


		StartCoroutine( "CO_Wait_TimeOut" );

		DateTime start_http_time = DateTime.Now;

	    HttpWebRequest	request = (HttpWebRequest) WebRequest.Create( send_q.send_url );
	    request.Method = "POST";
	    request.ClientCertificates.Add( SJ_JWT.certificate );
	    request.Timeout = 20000;


	    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors ) => 
		{
			if(debug_log)Debug.Log( "서버 인증서 콜백 : " + sslPolicyErrors.ToString() );

			return true;
		};

	    string jwt_str = send_q.header[ "Authorization" ].ToString();
	    request.Headers.Add( "Authorization" , jwt_str );

		//request.Headers.Add( "Connection" , "Close" );
	    //request.Connection = "Close";


	    request.ContentType = "text/json";
	    request.ContentLength = send_q.data.Length;

		//// 
		RequestState requestState = new RequestState();
		requestState.webRequest = request;
		requestState.send_q = send_q;

if(debug_log)Debug.Log( "비동기 use_HTTPS_Class 1");
		IAsyncResult asyncResult_req = null;
		try
		{
			if(debug_log)Debug.Log( "샌드 리퀘스트 스트링 header :"	+ request.Headers.ToString() );

			// 비동기 리퀘스트 스트림
			asyncResult_req = (IAsyncResult)request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), requestState );
		}
		catch(WebException e)
		{
			if(debug_log)Debug.Log( "익솁션 : BeginGetRequestStream !!!!!!!!!!!!" );
			if(debug_log)Debug.Log( "WebException : " + e.Message );
			if( e.Message.Contains( "401" ) )
			{
				error_Critical = true;
				OnError_CertError();
				StopCoroutine( "CO_Wait_TimeOut" );
			}
			else								OnErrorMsg( e.Message );
			StopCoroutine( "CO_Send_HttpWebRequest" );

		}


		// Wait until the the call is completed
		while (!asyncResult_req.IsCompleted) 
		{ 
			yield return null; 
		}


if(debug_log)Debug.Log( "비동기 use_HTTPS_Class 1~1 ");

		try
		{
			//// End the operation
			Stream postStream = request.EndGetRequestStream(asyncResult_req);
			postStream.Write( send_q.data,0,send_q.data.Length );
			postStream.Close();
		}
		catch(Exception e)
		{
		    if(debug_log)Debug.Log( "Exception : " + e.Message );
			if( e.Message.Contains( "401" ) )
			{
				error_Critical = true;
				OnError_CertError();
				Wait_TimeOut_Prc(false);
			}
		    else if( e.Message.Contains( "ConnectFailure" ) )
			{
				error_Critical = true;
				OnError_NoReachable();
				Wait_TimeOut_Prc(false);
			}else{
				OnErrorMsg( e.Message );
			}
		    StopCoroutine( "CO_Send_HttpWebRequest" );

		}

if(debug_log)Debug.Log( "비동기 use_HTTPS_Class 2");
		// 비동기 리스폰 스트림

		IAsyncResult asyncResult_rep = null;

		try
		{
			asyncResult_rep = (IAsyncResult) request.BeginGetResponse(new AsyncCallback(GetResponseCallback), requestState);
		}
		catch(WebException e)
		{
			if(debug_log)Debug.Log( "WebException : " + e.Message );
			if( e.Message.Contains( "401" ) )
			{
				error_Critical = true;
				OnError_CertError();
				StopCoroutine( "CO_Wait_TimeOut" );
			}
			else								OnErrorMsg( e.Message );
			StopCoroutine( "CO_Send_HttpWebRequest" );

		}


		while (!asyncResult_rep.IsCompleted) 
		{ 
			yield return null; 
		}

		WebResponse res = null;		



if(debug_log)Debug.Log( "비동기 use_HTTPS_Class 3");
		string recv_str = null;
		try
		{
			res = 	request.EndGetResponse( asyncResult_rep );
			using(	Stream			res_sr		=	res.GetResponseStream() )
			{
				using(	StreamReader	res_sr_read =	new StreamReader( res_sr ) )
				{
					res_sr.Flush();
					recv_str = res_sr_read.ReadToEnd();
				}
			}
		}
		catch(WebException e)
		{
			if(debug_log)Debug.Log( "익셉션 !! request.EndGetResponse : " + e.Message );
			if(debug_log)Debug.Log( "WebException : " + e.Message );

			if( e.Message.Contains( "401" ) )
			{
				error_Critical = true;
				OnError_CertError();
				StopCoroutine( "CO_Wait_TimeOut" );
			}
			else								OnErrorMsg( e.Message );
			StopCoroutine( "CO_Send_HttpWebRequest" );
		}

		string str_recv = recv_str;
		if( encryptData )str_recv = SJ_Ase.Decrypt_128_WithJava( str_recv , encryptData_Key );
		Debug.Log( "RECV : " + str_recv );
		Recv_Prc( str_recv , send_q );

		StopCoroutine( "CO_Wait_TimeOut" );
		UnlockAction();


if(debug_log)Debug.Log( "비동기 use_HTTPS_Class 완료**********************<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<" );
	}

    private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
    {
		//HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

		//// End the operation
		//Stream postStream = request.EndGetRequestStream(asynchronousResult);


		//Console.WriteLine("Please enter the input data to be posted:");
		//string postData = Console.ReadLine();

		//// Convert the string into a byte array.
		//byte[] byteArray = Encoding.UTF8.GetBytes(postData);

		//// Write to the request stream.
		//postStream.Write(byteArray, 0, postData.Length);
		//postStream.Close();

		//// Start the asynchronous operation to get the response
		//request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);

	}



	static private void GetResponseCallback (IAsyncResult asyncResult) 
	{
		//RequestState requestState = (RequestState) asyncResult.AsyncState;
		//WebRequest webRequest = requestState.webRequest;

		//try 
		//{
		//    webRequest.EndGetResponse(asyncResult);


		//} 
		//catch (WebException webException) 
		//{
		//    requestState.errorMessage = webException.Message;
		//}




    }

    static private void ScanTimeoutCallback (object state, bool timedOut)  
	{ 
        if (timedOut)  
		{
            RequestState requestState = (RequestState)state;
            if (requestState != null) 
                requestState.webRequest.Abort();
        } 
		else 
		{
            RegisteredWaitHandle registeredWaitHandle = (RegisteredWaitHandle)state;
            if (registeredWaitHandle != null)
                registeredWaitHandle.Unregister(null);
        }
    }




	public	void	Send_Retry_Prc()
	{
		if(debug_log)Debug.Log( "패킷 다시 보냄" );
		LockAction();

		if( use_HTTPS_Class == false )
		{
			if( recent_send_q.post )
			{
				WWW www = new WWW( recent_send_q.send_url , recent_send_q.data, recent_send_q.dic_header );
			}else{
				WWW www = new WWW( recent_send_q.send_url);
			}
			StartCoroutine( "CO_Wait_Recv" ,  recent_send_q );
			if(recv_gameObj != null)
				StartCoroutine( "CO_Wait_TimeOut" );
		}
		else
		{
		    StartCoroutine(	"CO_Send_HttpWebRequest" , recent_send_q);
		}
	}

	static	public	void	Send_Retry()
	{
		if( sj_HTTP_Json_recent_sended != null )
		{
			sj_HTTP_Json_recent_sended.Send_Retry_Prc();
		}
	}

	//StopCoroutine 쓸 때 주의점
	//http://spowder.egloos.com/9613919
	//StartCoroutine에서 메서드 명을 string값으로 쓴것만 StopCoroutine으로 제어가 된다.
	//StopCoroutine을 한번만 실행해도 똑같은 메서드명으로 여러번 실행했던 루틴이 모두 종료된다.

	IEnumerator CO_Wait_TimeOut()
	{
	    yield return new WaitForSeconds(recv_Wait_Time);
	    Wait_TimeOut_Prc();
	}

	public	void	Wait_TimeOut_Prc( bool view_NoRecv = true )
	{
//	    Debug.LogError("SJ_HTTP_Json : 타임 아웃!!!!!!");
	    StopCoroutine( "CO_Wait_Recv" );
		StopCoroutine( "CO_Send_HttpWebRequest" );
	    bSended = false;
		UnlockAction();

		if(view_NoRecv)
		{
			OnError_RecvTimeOut();
		}
	}

	IEnumerator	CO_Wait_Recv(  _SJ_SEND_DATA_Q send_q )
	{
		if( send_q.www == null )
		{
			StopCoroutine( "CO_Wait_TimeOut" );
			UnlockAction();
			OnError_NULL_WWW();
			yield break;
		}
		yield return send_q.www;
		StopCoroutine( "CO_Wait_TimeOut" );


		//if (!string.IsNullOrEmpty(send_q.www.error))
		//{
		//    Debug.Log(" www 에러 : " +  send_q.www.error);
		//    UnlockAction();
		//    if( send_q.www.error.Contains("401 Unauthorized"))
		//    {
		//        OnError_CertError();
		//    }
		//    else
		//    {
		//        OnErrorMsg( send_q.www.error );
		//    }
		//    yield	return 0;
		//}

		if( WWWError(send_q.www) == false )
		{
			yield break;
		}


		string str_recv = send_q.www.text;
		if( encryptData )str_recv = SJ_Ase.Decrypt_128_WithJava( str_recv , encryptData_Key );
		Debug.Log( "RECV : " + str_recv );
		UnlockAction();

		Recv_Prc( str_recv , send_q );
	}


	public	bool	WWWError( WWW www , GameObject go_retry = null , string func_retry = "" )
	{
		if( www == null )
		{
			Debug.Log(" www error : www == null!!!!" );
			UnlockAction();
			OnError_NULL_WWW( go_retry , func_retry );
			return false;
		}

		if (!string.IsNullOrEmpty(www.error))
		{
            Debug.Log(" www error : " +  www.error);
			UnlockAction();
			if( www.error.Contains("401 Unauthorized"))
			{
				OnError_CertError( go_retry , func_retry );
			}
			else
			{
				OnErrorMsg( www.error , go_retry , func_retry );
			}
			return false;
		}
		return true;
	}


	public	void	Recv_Prc( string recv_str , _SJ_SEND_DATA_Q send_q )
	{
		JSONNode recv_json = JSON.Parse( recv_str );

		if( offlineMode )
		{
			if( dlg_Offline_Json != null ) dlg_Offline_Json( recv_json  ,send_q );
		}

		OnRecv_WWW( recv_json );
		if( send_q.dlgFunc_recvSelf != null )send_q.dlgFunc_recvSelf( recv_json );
		bSended = false;
		
		// json 추가
		SJ_Unity.SendMsg( send_q.recv_gameObj , send_q.recv_func , recv_json );
	}

	virtual	public	void	OnRecv_WWW( JSONNode recv_json ){}

	virtual	public	void	OnErrorMsg( string error , GameObject go_retry = null , string func_retry = "" ){}
	virtual	public	void	OnError_NoReachable	(GameObject go_retry = null , string func_retry = ""){}
	virtual	public	void	OnError_NULL_WWW	(GameObject go_retry = null , string func_retry = ""){}
	virtual	public	void	OnError_RecvTimeOut	(GameObject go_retry = null , string func_retry = ""){}
	virtual	public	void	OnError_CertError	(GameObject go_retry = null , string func_retry = ""){}
}
