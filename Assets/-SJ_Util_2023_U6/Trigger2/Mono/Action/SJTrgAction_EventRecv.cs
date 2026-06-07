using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_EventRecv : SJTrgAction_Default_Mono
{
	public	int[]		recv_tag_int;
	public	string[]	recv_tag_str;


	override	public	void	OnAction()
	{
		if( sjtagobj_mono == null )sjtagobj_mono = gameObject.AddComponent<SJTagObj_Mono>();
		
		sjtagobj_mono.sjtrgaction_mono = this;

		sjtagobj_mono.AddTag(recv_tag_int);
		sjtagobj_mono.AddTag(recv_tag_str);

		//SJTrgPlayer_Mono.Insert_TagObj_Default( sjtagobj_mono );
	}

	override	public	bool	OnEventRecv(string tag , int arg_i = 0 , string arg_s = "" , object obj = null )
	{
		foreach( string s in recv_tag_str )
		{
			if( tag == s )
			{
				EndAction();
				return true;
			}
		}

		return false;
	}

	override	public	bool	OnEventRecv(int tag , int arg_i = 0 , string arg_s = "" , object obj = null )
	{
		foreach( int s in recv_tag_int )
		{
			if( tag == s )
			{
				EndAction();
				return true;
			}
		}
		return false;
	}

	override	public	void	OnEnd()
	{
		//SJTrgPlayer_Mono.Remove_TagObj_After_Default(sjtagobj_mono);
		
	}
}
