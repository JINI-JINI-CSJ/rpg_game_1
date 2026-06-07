using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public	enum SJ_Hierarchy_ID_TYPE
{
	Num = 0 ,
	String = 1
}

[System.Serializable]
public	class _SJ_Hierarchy_ID
{
	public	SJ_Hierarchy_ID_TYPE  id_type;
	public	int			id_int;
	public	string		id_str;
}


public class SJHierarchy
{
	public	List<_SJ_Hierarchy_ID>			list_Hierarchy_path = new List<_SJ_Hierarchy_ID>(); // 상위 경로
	public	_SJ_Hierarchy_ID				self_id = new _SJ_Hierarchy_ID(); // 본인 
	public	Dictionary<int,SJHierarchy>		dic_int_obj = new Dictionary<int,SJHierarchy>();
	public	Dictionary<string,SJHierarchy>	dic_str_obj = new Dictionary<string,SJHierarchy>();

	public	GameObject	gameObject;

	public	void	SetTagNum( int id )
	{
		self_id.id_type = SJ_Hierarchy_ID_TYPE.Num;
		self_id.id_int = id;
	}

	public	void	SetTagString( string str )
	{
		self_id.id_type = SJ_Hierarchy_ID_TYPE.String;
		self_id.id_str = str;
	}


	public	void	AddChild( SJHierarchy	sj_obj )
	{
		if( sj_obj.self_id.id_type == SJ_Hierarchy_ID_TYPE.Num )
		{
			dic_int_obj[sj_obj.self_id.id_int] = sj_obj;
		}else{
			dic_str_obj[sj_obj.self_id.id_str] = sj_obj;
		}
	}


	public	void	RemoveChild( SJHierarchy	sj_obj )
	{
		if( sj_obj.self_id.id_type == SJ_Hierarchy_ID_TYPE.Num )
		{
			dic_int_obj.Remove(sj_obj.self_id.id_int);
		}else{
			dic_str_obj.Remove(sj_obj.self_id.id_str);
		}
	}

	public	SJHierarchy	FindChild( _SJ_Hierarchy_ID sj_id )
	{
		SJHierarchy find_obj = null;
		if( sj_id.id_type == SJ_Hierarchy_ID_TYPE.Num )
		{
			dic_int_obj.TryGetValue( sj_id.id_int , out find_obj );
		}else{
			dic_str_obj.TryGetValue( sj_id.id_str , out find_obj );
		}
		return find_obj;
	}

	public	SJHierarchy	FindHierarchyPath( List<_SJ_Hierarchy_ID> list_path )
	{
		SJHierarchy find_obj = this;
		foreach( _SJ_Hierarchy_ID id in list_path )
		{
			SJHierarchy find_child = find_obj.FindChild( id );
			if( find_child == null ) return null;
			find_obj = find_child;
		}
		return find_obj;
	}


	public	bool	RegHierarchy( SJHierarchy sj_obj )
	{
		SJHierarchy		find_parent = FindHierarchyPath( sj_obj.list_Hierarchy_path );
		if( find_parent == null )
		{
			return false;
		}
		find_parent.AddChild( sj_obj );
		return true;
	}

	public	bool	RemoveHierarchy( SJHierarchy sj_obj )
	{
		SJHierarchy		find_parent = FindHierarchyPath( sj_obj.list_Hierarchy_path );
		if( find_parent == null )
		{
			return false;
		}
		find_parent.RemoveChild( sj_obj );
		return true;
	}
}
