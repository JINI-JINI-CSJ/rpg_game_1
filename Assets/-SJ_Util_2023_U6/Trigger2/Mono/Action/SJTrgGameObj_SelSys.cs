using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public	class _SJ_SelObjName
{
	public	string		Name;
	public	GameObject	go_Sel;
	public	GameObject	Get()
	{
		if( go_Sel != null ) return go_Sel;
		return SJTrgGameObj_SelSys.FindObj( Name );
	}
}

public class SJTrgGameObj_SelSys : MonoBehaviour 
{
	static	public	SJTrgGameObj_SelSys	g_sel;

	public	class _Name_Obj
	{
		public	string			Name;
		public	GameObject		go;
	}

	public	Dictionary<string ,_Name_Obj>	dic_Name_Obj = new Dictionary<string, _Name_Obj>();

	private void Awake()
	{
		g_sel=this;
	}

	virtual	public	GameObject	OnFindObj( string str )
	{
		return null;
	}

	static	public	void		SetObj( string str , GameObject		go )
	{
		_Name_Obj s = new _Name_Obj();
		s.Name = str;
		s.go = go;
		g_sel.dic_Name_Obj[ str ] = s;
	}

	static	public	GameObject	FindObj( string str )
	{
		GameObject go = g_sel.OnFindObj(str);

		if( go != null ) return go; 

		_Name_Obj	f = null;
		if( g_sel.dic_Name_Obj.TryGetValue( str , out f ) )
		{
			return f.go;
		}
		return null;
	}

}
