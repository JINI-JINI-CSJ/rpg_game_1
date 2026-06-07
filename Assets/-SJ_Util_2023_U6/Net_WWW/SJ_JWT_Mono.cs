using UnityEngine;

using System.Collections;
using System.Text;
using SimpleJSON;

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public class SJ_JWT_Mono : MonoBehaviour 
{
	static	public	SJ_JWT_Mono	g;	
	static 	public	string		make_jwt;

	static	public	GameObject	recv_LoadKeyFileComplete_obj;
	static	public	string		recv_LoadKeyFileComplete_func;

	static	bool				loaded_KeyFile = false;

	static	byte[]				bt_KeyFile;
	static	X509Certificate2	certificate;

	void Awake()
	{
		g = this;
		DontDestroyOnLoad( gameObject );
	}

	static	public	void	Init_Load_KeyFile( GameObject go , string str  )
	{
		if( loaded_KeyFile ) return;
		recv_LoadKeyFileComplete_obj = go;
		recv_LoadKeyFileComplete_func = str;
		loaded_KeyFile = true;
		g.Load_KeyFile_Prc();
	}


	public	void	Load_KeyFile_Prc()
	{

		//string 	keyFilePath = "file://" + Application.dataPath + "/Resources/key.pfx";


		string	keyFilePath =	"jar:file://" + Application.dataPath + "!/key.pfx";


		WWW www = new WWW( keyFilePath );
		StartCoroutine( CO_LoadKeyFile( www ));
	}

	IEnumerator		CO_LoadKeyFile( WWW www )
	{
		yield return www;

		bt_KeyFile = www.bytes;
		certificate = new X509Certificate2( www.bytes ,"B2W=QB5HUpum" );

		//GetAccessToken();
		//Obj_misc.SendMessage( recv_LoadKeyFileComplete_obj , recv_LoadKeyFileComplete_func );
	}


	static	public string	GetAccessToken()
	{
		JSONClass	j_header	= new JSONClass();
		JSONClass	j_claimset	= new JSONClass();

		j_header["alg"] = "RS256";

		var times = GetExpiryAndIssueDate();

		j_claimset["iss"]		= "client";
		j_claimset["exp"].AsInt	= times[1];

		byte[]	headerBytes	= Encoding.UTF8.GetBytes( j_header.ToString() );
		string	headerEncoded = Base64UrlEncode(headerBytes);

		byte[]	claimsetBytes = Encoding.UTF8.GetBytes( j_claimset.ToString() );
		string	claimsetEncoded = Base64UrlEncode(claimsetBytes);

		//// input
		string	input = headerEncoded + "." + claimsetEncoded;
		byte[]	inputBytes = Encoding.UTF8.GetBytes(input);

		RSACryptoServiceProvider rsa = certificate.PrivateKey as RSACryptoServiceProvider;
		byte[]	signatureBytes = rsa.SignData(inputBytes, "SHA256");
		string	signatureEncoded = Base64UrlEncode(signatureBytes);

		//// jwt
		string jwt = headerEncoded + "." + claimsetEncoded + "." + signatureEncoded;
		Debug.Log( jwt );
		//return jwt;
		make_jwt = jwt;
		return jwt;
	}

	private static string Base64UrlEncode(byte[] input)
	{
		var output = Convert.ToBase64String(input);
		output = output.Split('=')[0]; // Remove any trailing '='s
		output = output.Replace('+', '-'); // 62nd char of encoding
		output = output.Replace('/', '_'); // 63rd char of encoding
		return output;
	}

	private static int[] GetExpiryAndIssueDate()
	{
		var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		var issueTime = DateTime.UtcNow;
		var iat = (int)issueTime.Subtract(utc0).TotalSeconds;
		var exp = (int)issueTime.AddMinutes(55).Subtract(utc0).TotalSeconds;
		return new[] { iat, exp };
	}

}
