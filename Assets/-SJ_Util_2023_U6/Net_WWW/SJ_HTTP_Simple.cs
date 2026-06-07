using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;

using SimpleJSON;
using System.Text;
using UnityEngine.Networking;

public	class _SJ_HTTP_SEND_Q
{
	//public	WWW			www;
	public	UnityWebRequest	uwr;
	public 	string			send_url;
	public	string			data_str;

	public	SJ_HTTP_Simple.DlgFunc_recvSelf delg_recv;
}

public class SJ_HTTP_Simple : MonoBehaviour
{
	public	string			url_Base;
	public	JSONClass		json = new JSONClass();
	public	int				packet_Count;

	public	string			field_Main;
	public	bool			use_asc = true;
	public	string			asc_Password = "";

	public	List<_SJ_HTTP_SEND_Q>	list_SendQ = new List<_SJ_HTTP_SEND_Q>();

	// 응답 받는 델리게이트 함수.
	public	delegate	void	DlgFunc_recvSelf( JSONNode jnode );


	_SJ_HTTP_SEND_Q	send_q_cur;

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


	public	void	Begin_Data()
	{
		json.ClearNode();
	}

	public	void	Send( string _url , DlgFunc_recvSelf delg_recv = null )
	{
		_SJ_HTTP_SEND_Q send_q = new _SJ_HTTP_SEND_Q();
		send_q.send_url = url_Base + _url;
		json["packet_Count"].AsInt = packet_Count;
		send_q.data_str = json.ToString();
		send_q.delg_recv = delg_recv;
		packet_Count++;
		list_SendQ.Add(send_q);
	}


	public static Dictionary<K,V> HashtableToDictionary<K,V> (Hashtable table)
	{
		Dictionary<K,V> dict = new Dictionary<K,V>();
		foreach(DictionaryEntry kvp in table)
			dict.Add((K)kvp.Key, (V)kvp.Value);
		return dict;
	}

	public void Update()
	{
		Prc_SendQ();
	}

	public	void	Prc_SendQ()
	{
		if( send_q_cur != null ) return;
		if( list_SendQ.Count < 1 )return;
		_SJ_HTTP_SEND_Q send_q = list_SendQ[0];
		list_SendQ.RemoveAt(0);

		string send_data = send_q.data_str;

		//byte[] data_utf8 =	Encoding.UTF8.GetBytes( send_data );

		//string str_utf8 = BitConverter.ToString(data_utf8); 


		

		Debug.Log( "1 Prc_SendQ : send_data : " + send_data );

		if( use_asc )
		{
			send_data =	SJ_Ase.Encrypt_128_WithJava( send_data , asc_Password );

			string dec_data = SJ_Ase.Decrypt_128_WithJava( send_data , asc_Password );

			Debug.Log( "11 Prc_SendQ : dec_data : " + dec_data );
		}

		Debug.Log( "2 Prc_SendQ : send_data : " + send_data );

		WWWForm form = new WWWForm();

		form.AddField( field_Main , send_data );

		UnityWebRequest www = UnityWebRequest.Post(send_q.send_url, form);

		send_q.uwr = www;
		send_q_cur = send_q;

		StartCoroutine( CO_Wait_Recv( send_q ) );
	}


	IEnumerator	CO_Wait_Recv(  _SJ_HTTP_SEND_Q send_q )
	{
		yield return send_q.uwr.SendWebRequest();
		if( send_q.uwr == null )
		{
			send_q_cur = null;
			yield break;
		}
		//string str = new string(send_q.uwr.);

		string recv_data = send_q.uwr.downloadHandler.text;

		Debug.Log(" 1 www : recv : " + recv_data );

		if( use_asc )
		{
			recv_data =	SJ_Ase.Decrypt_128_WithJava( recv_data , asc_Password );
		}
		Debug.Log(" 2 www : recv : " + recv_data );

		JSONNode json =	JSONClass.Parse( recv_data );

		if( send_q.delg_recv != null )
		{
			send_q.delg_recv( json );
		}

		send_q_cur = null;
	}


}
