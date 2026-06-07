using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJTagSys_Mono : MonoBehaviour {

	public	Dictionary< int , HashSet<SJTagObj_Mono>>		dic_IntHash = new Dictionary<int, HashSet<SJTagObj_Mono>>();
	public	Dictionary< string , HashSet<SJTagObj_Mono>>	dic_StrHash = new Dictionary<string, HashSet<SJTagObj_Mono>>();

	public	SJTrgPlayer_Mono		sjtrgplayer_mono;
	public	List<SJTagObj_Mono>		list_Remove_After;

	private void Update()
	{
		foreach( SJTagObj_Mono s in list_Remove_After )Remove_TagObj( s );
		list_Remove_After.Clear();
	}

	public	void	OnStartInstSJ()
	{
		sjtrgplayer_mono = GetComponent<SJTrgPlayer_Mono>();
		if( sjtrgplayer_mono != null )
		{
			sjtrgplayer_mono.sjtagsys_mono = this;
		}
	}

	public	void	Clear_AllTagObj()
	{
		dic_IntHash.Clear();
		dic_StrHash.Clear();
	}

	List<HashSet<SJTagObj_Mono>>	list_find = new List<HashSet<SJTagObj_Mono>>();
	public	List<HashSet<SJTagObj_Mono>>	FindNew_HashInt( HashSet<int> params_tag  )
	{
		list_find.Clear();
        foreach (int tag in params_tag)
        {
			HashSet<SJTagObj_Mono> hs_find = null;
			if(	dic_IntHash.TryGetValue( tag , out hs_find ) == false )
			{
				hs_find = new HashSet<SJTagObj_Mono>();
				dic_IntHash[tag] = hs_find;
			}
			list_find.Add( hs_find );
		}
		return list_find;
	}

	public	List<HashSet<SJTagObj_Mono>>	FindNew_HashStr( HashSet<string> params_tag  )
	{
		list_find.Clear();
        foreach (string tag in params_tag)
        {
			HashSet<SJTagObj_Mono> hs_find = null;
			if(	dic_StrHash.TryGetValue( tag , out hs_find ) == false )
			{
				hs_find = new HashSet<SJTagObj_Mono>();
				dic_StrHash[tag] = hs_find;
			}
			list_find.Add( hs_find );
		}
		return list_find;
	}

	public	bool	IsTagObj( int noTagInt = -1 , string noTagStr = "" )
	{
		if( noTagInt > -1 )
		{
			HashSet<SJTagObj_Mono> hs_find = null;
			if(	dic_IntHash.TryGetValue( noTagInt , out hs_find ) )if( hs_find.Count > 0 ) return true;
		}

		if( string.IsNullOrEmpty(noTagStr) == false )
		{
			HashSet<SJTagObj_Mono> hs_find = null;
			if(	dic_StrHash.TryGetValue( noTagStr , out hs_find ) )if( hs_find.Count > 0 ) return true;
		}

		return false;
	}



	public	bool	Insert_TagObj( SJTagObj_Mono tag_obj , int noTagInt = -1 , string noTagStr = "" , bool link_parent = true )
	{
		if( noTagInt > -1 || string.IsNullOrEmpty( noTagStr ) == false )
		{
			if( IsTagObj( noTagInt , noTagStr ) )
			{ 
				Debug.Log( "Insert_TagObj : return false : IsTagObj( noTagInt , noTagStr )" );
				return false;
			}
		}

		if( sjtrgplayer_mono != null && tag_obj.sjtrgaction_mono != null )
		{
			tag_obj.sjtrgaction_mono.self_Player = sjtrgplayer_mono;
			tag_obj.sjtrgaction_mono.OnAdd();
		}

        List<HashSet<SJTagObj_Mono>> list_hashInt = FindNew_HashInt( tag_obj.hs_tagInt );
		foreach( HashSet<SJTagObj_Mono> h in list_hashInt ) h.Add( tag_obj );

        List<HashSet<SJTagObj_Mono>> list_hashStr = FindNew_HashStr( tag_obj.hs_tagStr );
		foreach( HashSet<SJTagObj_Mono> h in list_hashStr ) h.Add( tag_obj );

		if( link_parent )
		{
			SJ_Unity.SetEqTrans( tag_obj.transform , null , transform );
		}

		tag_obj.sJTagSys_Mono_Inserted = this;

		return true;
	}

	public	void	Remove_TagObj_After( SJTagObj_Mono tag_obj )
	{
		list_Remove_After.Add( tag_obj );
	}

	public	void	Remove_TagObj( SJTagObj_Mono tag_obj )
	{
		if( tag_obj.sjtrgaction_mono != null )tag_obj.sjtrgaction_mono.OnRemove();
		
        List<HashSet<SJTagObj_Mono>> list_hashInt = FindNew_HashInt( tag_obj.hs_tagInt );
		foreach( HashSet<SJTagObj_Mono> h in list_hashInt ) h.Remove( tag_obj );

        List<HashSet<SJTagObj_Mono>> list_hashStr = FindNew_HashStr( tag_obj.hs_tagStr );
		foreach( HashSet<SJTagObj_Mono> h in list_hashStr ) h.Remove( tag_obj );

		if( tag_obj.sjgopoolobj != null ) SJPool.ReturnInst( tag_obj.gameObject );

		if( tag_obj.sjtrgaction_mono != null )tag_obj.sjtrgaction_mono.OnRemove_After();

		tag_obj.sJTagSys_Mono_Inserted = null;
	}

	public	HashSet<SJTagObj_Mono> Remove_TagObjInt( int tag )
	{
		HashSet<SJTagObj_Mono> 	hs_find = Find_TagInt(tag);
		HashSet<SJTagObj_Mono>  hs_temp = new HashSet<SJTagObj_Mono>(hs_find);
		foreach( SJTagObj_Mono s in hs_temp )Remove_TagObj( s );

		return hs_temp;
	}


	HashSet<int>	temp_hs_int = new HashSet<int>();
	public	HashSet<SJTagObj_Mono>	Find_TagInt( params int[] params_tag )
	{
		temp_hs_int.Clear();
		for(int i = 0 ; i < params_tag.Length ; i++ ) temp_hs_int.Add(params_tag[i]);
        List<HashSet<SJTagObj_Mono>> list_hashInt = FindNew_HashInt( temp_hs_int );

		if( list_hashInt.Count > 0 ) return list_hashInt[0];
		return null;
	}

	HashSet<string>	temp_hs_str = new HashSet<string>();
	public	HashSet<SJTagObj_Mono>	Find_TagStr( params string[] params_tag )
	{
		temp_hs_str.Clear();
		for(int i = 0 ; i < params_tag.Length ; i++ ) temp_hs_str.Add(params_tag[i]);
        List<HashSet<SJTagObj_Mono>> list_hashStr = FindNew_HashStr( temp_hs_str );

		if( list_hashStr.Count > 0 ) return list_hashStr[0];
		return null;
	}

	public int compare_SJTagObj_Mono( SJTagObj_Mono s1, SJTagObj_Mono s2 )
	{
		return s1.CompareTo(s2);
	}

	public	List<SJTagObj_Mono>	Find_TagInt_Sort( params int[] params_tag )
	{
		List<SJTagObj_Mono>	temp_list_SJTagObj_Mono = new List<SJTagObj_Mono>();
		HashSet<SJTagObj_Mono>	hs =	Find_TagInt( params_tag );
		if( hs == null ) return temp_list_SJTagObj_Mono;

		foreach( SJTagObj_Mono s in hs )temp_list_SJTagObj_Mono.Add(s);
		temp_list_SJTagObj_Mono.Sort( compare_SJTagObj_Mono );
		return temp_list_SJTagObj_Mono;
	}

	public	List<SJTagObj_Mono>	Find_TagStr_Sort( params string[] params_tag )
	{
		List<SJTagObj_Mono>	temp_list_SJTagObj_Mono = new List<SJTagObj_Mono>();
		HashSet<SJTagObj_Mono>	hs =	Find_TagStr( params_tag );
		if( hs == null ) return temp_list_SJTagObj_Mono;

		foreach( SJTagObj_Mono s in hs )temp_list_SJTagObj_Mono.Add(s);
		temp_list_SJTagObj_Mono.Sort( compare_SJTagObj_Mono );
		return temp_list_SJTagObj_Mono;
	}


	public	bool	OnEventRecv(string tag , int arg_i = 0 , string arg_s = "" , object obj = null )
	{
		bool b = false;
		HashSet<SJTagObj_Mono> hs_obj = Find_TagStr( tag );
		if( hs_obj == null ) return false;
		List<SJTagObj_Mono>	lt_temp = new List<SJTagObj_Mono>( hs_obj );
		foreach( SJTagObj_Mono s in lt_temp )
		{
			if(	s.sjtrgaction_mono.OnEventRecv(tag ,arg_i ,  arg_s , obj ) ) b = true;
		}
		return b;
	}


	public	bool	OnEventRecv(int tag , int arg_i = 0 , string arg_s = "" , object obj = null)
	{
		bool b = false;
		HashSet<SJTagObj_Mono> hs_obj = Find_TagInt( tag );
		if( hs_obj == null ) return false;
		foreach( SJTagObj_Mono s in hs_obj )
		{
			if(	s.sjtrgaction_mono.OnEventRecv(tag ,arg_i ,  arg_s , obj ) ) b=true;
		}
		return b;
	}


}
