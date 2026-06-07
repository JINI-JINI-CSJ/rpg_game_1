using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJ_DicStr_ListObj<T>
{
	public	Dictionary< string , List<T> >	dic = new Dictionary<string, List<T>>();

	public	void	Add( string str , T t )
	{
		List<T>	find_list = null;
		if( dic.TryGetValue( str , out find_list ) == false )
		{
			find_list = new List<T>();
			dic[str] = find_list;
		}
		find_list.Add( t );
	}

	public	List<T>	Find_List( string str )
	{
		List<T>	find_list = null;
		if( dic.TryGetValue( str , out find_list ) == false )
		{
			return null;
		}
		return find_list;
	}

	public void		Clear_All()
	{
		dic.Clear();
	}

	public	void	Clear_OnlyObj()
	{
		foreach( KeyValuePair< string , List<T>> s in dic )
		{
			s.Value.Clear();
		}
	}

}
