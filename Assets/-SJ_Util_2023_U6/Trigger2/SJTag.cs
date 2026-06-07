using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//
// 태그 객체 
// 태그를 이용한 객체 등록 , 검색
//

public class SJTagSys
{
	public	Dictionary< int , HashSet<SJTagObj>>	dic_IntHash = new Dictionary<int, HashSet<SJTagObj>>();
	public	Dictionary< string , HashSet<SJTagObj>>	dic_StrHash = new Dictionary<string, HashSet<SJTagObj>>();


	public	void	Clear_AllTagObj()
	{
		dic_IntHash.Clear();
		dic_StrHash.Clear();
	}

	List<HashSet<SJTagObj>>	list_find = new List<HashSet<SJTagObj>>();
	public	List<HashSet<SJTagObj>>	FindNew_HashInt( HashSet<int> params_tag  )
	{
		list_find.Clear();
        foreach (int tag in params_tag)
        {
			HashSet<SJTagObj> hs_find = null;
			if(	dic_IntHash.TryGetValue( tag , out hs_find ) == false )
			{
				hs_find = new HashSet<SJTagObj>();
				dic_IntHash[tag] = hs_find;
			}
			list_find.Add( hs_find );
		}
		return list_find;
	}

	public	List<HashSet<SJTagObj>>	FindNew_HashStr( HashSet<string> params_tag  )
	{
		list_find.Clear();
        foreach (string tag in params_tag)
        {
			HashSet<SJTagObj> hs_find = null;
			if(	dic_StrHash.TryGetValue( tag , out hs_find ) == false )
			{
				hs_find = new HashSet<SJTagObj>();
				dic_StrHash[tag] = hs_find;
			}
			list_find.Add( hs_find );
		}
		return list_find;
	}

	public	void	Insert_Obj( SJTagObj tag_obj )
	{
        List<HashSet<SJTagObj>> list_hashInt = FindNew_HashInt( tag_obj.hs_tagInt );
		foreach( HashSet<SJTagObj> h in list_hashInt ) h.Add( tag_obj );

        List<HashSet<SJTagObj>> list_hashStr = FindNew_HashStr( tag_obj.hs_tagStr );
		foreach( HashSet<SJTagObj> h in list_hashStr ) h.Add( tag_obj );
	}

	public	void	Remove_Obj( SJTagObj tag_obj )
	{
        List<HashSet<SJTagObj>> list_hashInt = FindNew_HashInt( tag_obj.hs_tagInt );
		foreach( HashSet<SJTagObj> h in list_hashInt ) h.Remove( tag_obj );

        List<HashSet<SJTagObj>> list_hashStr = FindNew_HashStr( tag_obj.hs_tagStr );
		foreach( HashSet<SJTagObj> h in list_hashStr ) h.Remove( tag_obj );
	}

	HashSet<int>	temp_hs_int = new HashSet<int>();
	public	HashSet<SJTagObj>	Find_TagInt( params int[] params_tag )
	{
		temp_hs_int.Clear();
		for(int i = 0 ; i < params_tag.Length ; i++ ) temp_hs_int.Add(params_tag[i]);
        List<HashSet<SJTagObj>> list_hashInt = FindNew_HashInt( temp_hs_int );

		if( list_hashInt.Count > 0 ) return list_hashInt[0];
		return null;
	}

	HashSet<string>	temp_hs_str = new HashSet<string>();
	public	HashSet<SJTagObj>	Find_TagStr( params string[] params_tag )
	{
		temp_hs_str.Clear();
		for(int i = 0 ; i < params_tag.Length ; i++ ) temp_hs_str.Add(params_tag[i]);
        List<HashSet<SJTagObj>> list_hashStr = FindNew_HashStr( temp_hs_str );

		if( list_hashStr.Count > 0 ) return list_hashStr[0];
		return null;
	}

	public int compare_SJTagObj( SJTagObj s1, SJTagObj s2 )
	{
		return s1.CompareTo(s2);
	}


	//List<SJTagObj>	temp_list_sjtagObj = new List<SJTagObj>();
	public	List<SJTagObj>	Find_TagInt_Sort( params int[] params_tag )
	{
		List<SJTagObj>	temp_list_sjtagObj = new List<SJTagObj>();
		HashSet<SJTagObj>	hs =	Find_TagInt( params_tag );
		if( hs == null ) return temp_list_sjtagObj;

		foreach( SJTagObj s in hs )temp_list_sjtagObj.Add(s);
		temp_list_sjtagObj.Sort( compare_SJTagObj );
		return temp_list_sjtagObj;
	}

	public	List<SJTagObj>	Find_TagStr_Sort( params string[] params_tag )
	{
		List<SJTagObj>	temp_list_sjtagObj = new List<SJTagObj>();
		HashSet<SJTagObj>	hs =	Find_TagStr( params_tag );
		if( hs == null ) return temp_list_sjtagObj;

		foreach( SJTagObj s in hs )temp_list_sjtagObj.Add(s);
		temp_list_sjtagObj.Sort( compare_SJTagObj );
		return temp_list_sjtagObj;
	}


}


public	class SJTagObj
{
	public	object				obj;
	public	int					Priority;

	public	HashSet<int>		hs_tagInt = new HashSet<int>();
	public	HashSet<string>		hs_tagStr = new HashSet<string>();

	public	int		CompareTo( SJTagObj other )
	{
		if( Priority > other.Priority )	return -1;
		else if( Priority < other.Priority )	return 1;
		return 0;
	}

	public	void	AddTag( params int[] params_tag )
	{
		for(int i = 0 ; i < params_tag.Length ; i++) hs_tagInt.Add( params_tag[i] );
	}

	public	void	AddTag( params string[] params_tag )
	{
		for(int i = 0 ; i < params_tag.Length ; i++) hs_tagStr.Add( params_tag[i] );
	}

	public	void	DelTag( params int[] params_tag )
	{
		for(int i = 0 ; i < params_tag.Length ; i++) hs_tagInt.Remove( params_tag[i] );
	}

	public	void	DelTag( params string[] params_tag )
	{
		for(int i = 0 ; i < params_tag.Length ; i++) hs_tagStr.Remove( params_tag[i] );
	}
}