using UnityEngine;
using System.Collections;

public class SJ_LogFile 
{
	static	public	bool	noLog = false;

	static	public	string	str_fileName = "sj_log.txt";

	static	public	void	LogFile( string msg , bool new_create = false )
	{
		if(noLog)return;
		SJ_Unity.FileCreate_WriteLine( str_fileName , msg , new_create );
	}

}
