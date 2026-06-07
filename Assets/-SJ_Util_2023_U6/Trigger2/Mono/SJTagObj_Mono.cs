using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SJTagObj_Mono : MonoBehaviour
{
	// 자동으로 등록될때..
	public	List<int>			lt_UserTag_int;
	public	List<string>		lt_UserTag_str;

	public	int					Priority;
	public	HashSet<int>		hs_tagInt = new HashSet<int>();
	public	HashSet<string>		hs_tagStr = new HashSet<string>();

	public	SJGoPoolObj			sjgopoolobj;
	public	SJTrgAction_Mono	sjtrgaction_mono;

	public	SJTagSys_Mono		sJTagSys_Mono_Inserted;	// 이벤트 저장된 TagSys

	private void Awake()
	{

	}

	public	void	OnStartInstSJ()
	{

	}

	public	void	Init_Editer()
	{
		sjgopoolobj = GetComponent<SJGoPoolObj>();
		sjtrgaction_mono = GetComponent<SJTrgAction_Mono>();
		if( sjtrgaction_mono != null ) sjtrgaction_mono.sjtagobj_mono = this;
	}


	public	int		CompareTo( SJTagObj_Mono other )
	{
		if( Priority > other.Priority )	return -1;
		else if( Priority < other.Priority )	return 1;
		return 0;
	}

	public	int	AddUser()
	{
		AddTag( lt_UserTag_int.ToArray() );
		AddTag( lt_UserTag_str.ToArray() );

		return lt_UserTag_int.Count + lt_UserTag_str.Count;
	}

	public	void	AddTag( params int[] params_tag )
	{
		if( params_tag == null ) return;
		for(int i = 0 ; i < params_tag.Length ; i++) hs_tagInt.Add( params_tag[i] );
	}

	public	void	AddTag( params string[] params_tag )
	{
		if( params_tag == null ) return;
		for(int i = 0 ; i < params_tag.Length ; i++) hs_tagStr.Add( params_tag[i] );
	}

	public	void	DelTag( params int[] params_tag )
	{
		if( params_tag == null ) return;
		for(int i = 0 ; i < params_tag.Length ; i++) hs_tagInt.Remove( params_tag[i] );
	}

	public	void	DelTag( params string[] params_tag )
	{
		if( params_tag == null ) return;
		for(int i = 0 ; i < params_tag.Length ; i++) hs_tagStr.Remove( params_tag[i] );
	}
}
