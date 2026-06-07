using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 리소스 로드 풀..
// 한번 로드한건 기억하기..
// 돈트 디스트로이도 옵션으로 가능하게..

// 택스쳐 같은것도 있을테니 그냥 오브젝트로 전환.. 

public class _RES_POOL_OBJ
{
	public	Object			obj_res;
	public Object[] objs;
	//public	GameObject		go_Inst;
	//public	GameObject		go_Res;
	public	bool			dont_Destroy;
}

[System.Serializable]
public struct _FIND_RES_OBJ
{
	public	string		str_go_name;
	public	string		str_res;
	public	Transform	tr_par;
	public	bool		dont_des;

	public	bool		res_cood;
}


public class SJ_ResPoolSys : SJ_Singleton_Mono 
{
	public	List<GameObject>		list_preLoadRes = new List<GameObject>();

	// 키 : 게임 오브젝트 이름
	Dictionary< string , _RES_POOL_OBJ >	dic_ResObj = new Dictionary<string,_RES_POOL_OBJ>();
	static	public	GameObject		g_Go;
	static	public	SJ_ResPoolSys	g;

	override	public	SJ_Singleton_Mono	OnGetStatic()
	{
		return g;
	}

	override	public	void				OnSetStatic(SJ_Singleton_Mono s)
	{
		g = s as SJ_ResPoolSys;
	}

	override public void		OnCreate()
	{
		Init_Scene_Prc();
	}

	public void Clear_All()
	{
		dic_ResObj.Clear();
	}

	// 씬에서 처음 실행할때.
	// 돈트 디스트로이 빼고 다 지운다.
	public void Init_Scene_Prc()
	{
		List<string> list_Del = new List<string>();
		foreach ( KeyValuePair<string, _RES_POOL_OBJ> pair in dic_ResObj )
			if( pair.Value.dont_Destroy == false ) list_Del.Add( pair.Key );
		foreach (string str_id in list_Del) dic_ResObj.Remove( str_id );
	}

	static public void Init_Scene()
	{
		g.Init_Scene_Prc();
	}

	static	public	Dictionary<string,_RES_POOL_OBJ>	GetDic(){return g.dic_ResObj;}


	public _RES_POOL_OBJ GetResObj_Prc( string str_res_full_path, bool dont_des=false  , System.Type t = null , bool load_all = false )
	{
		if( string.IsNullOrEmpty( str_res_full_path ) ) return null;

		_RES_POOL_OBJ res_POOL_OBJ;
		if (dic_ResObj.TryGetValue(str_res_full_path, out res_POOL_OBJ) == false)
		{
			Object		_obj_res = null;
			Object[]	_obj_res_all = null;
			if( t != null )
			{
				if( load_all )
					_obj_res_all = Resources.LoadAll(str_res_full_path , t );	
				else 
					_obj_res = Resources.Load(str_res_full_path , t );	

			}else{
				if( load_all )
					_obj_res_all = Resources.LoadAll(str_res_full_path);
				else 
					_obj_res = Resources.Load(str_res_full_path);				
			}

			if( load_all == false )
			{
				if( _obj_res == null )
				{
					Debug.LogError( "Error!!! GetResObj_Prc : " + str_res_full_path );
					return null;
				}
			}else{
				if( _obj_res_all == null )
				{
					Debug.LogError( "Error!!! GetResObj_Prc : " + str_res_full_path );
					return null;
				}
			}


			if( dont_des )
				Object.DontDestroyOnLoad( _obj_res );

			res_POOL_OBJ = new _RES_POOL_OBJ
			{
				obj_res = _obj_res, 
				dont_Destroy = dont_des ,
				objs = _obj_res_all
			};
			dic_ResObj[str_res_full_path]		= res_POOL_OBJ;
		}

		return res_POOL_OBJ;
	}

	static public Object GetResObj_PathName( string str_res_full_path, bool dont_des=false , System.Type t = null)
	{
		_RES_POOL_OBJ find = null;
		if( g == null )
		{
			Debug.LogError( "에러!!! SJ_ResPoolSys : g == null " );
			return null;
		}
		find =	g.GetResObj_Prc( str_res_full_path, dont_des , t );
		if( find != null )
		{
			return find.obj_res;
		}
		Debug.LogError( "Error!!! SJ_ResPoolSys.GetResObj_PathName : " + str_res_full_path );
		return null;
	}

	static public _RES_POOL_OBJ GetResObjs_PathName( string str_res_full_path, bool dont_des=false , System.Type t = null)
	{
		_RES_POOL_OBJ find = null;
		find =	g.GetResObj_Prc( str_res_full_path, dont_des , t  , true );
		if( find != null )
		{
			return find;
		}
		Debug.LogError( "Error!!! SJ_ResPoolSys.GetResObjs_PathName : " + str_res_full_path );
		return null;
	}
	static public Sprite GetResObjs_PathName_Sprite(string str_res_full_path)
	{
		UnityEngine.Object obj = SJ_ResPoolSys.GetResObj_PathName(str_res_full_path, false, typeof(Sprite));
		return obj as Sprite;
	}

	static public Sprite GetResObjs_PathName_MultiSpr(string str_res_full_path, string spr_name)
		{
			_RES_POOL_OBJ s = GetResObjs_PathName(str_res_full_path, false, typeof(Sprite));

			if (s != null)
			{
				foreach (object spr_o in s.objs)
				{
					Sprite spr_s = spr_o as Sprite;
					if (spr_s.name == spr_name) return spr_s;
				}
			}
			return null;
		}

	static	public	GameObject	Inst_Obj( string str_res_full_path, bool dont_des=false )
	{
		GameObject res_load = GetResObj_PathName( str_res_full_path , dont_des ) as GameObject;
		if( res_load == null )
		{

			return null;
		}
		GameObject inst =	GameObject.Instantiate( res_load ) as GameObject;
		inst.name = res_load.name;
		return inst;
	}

	static	public	void	Unload(string str_res_full_path)
	{
		_RES_POOL_OBJ res_POOL_OBJ;
		if (g.dic_ResObj.TryGetValue(str_res_full_path, out res_POOL_OBJ) )
		{
			Resources.UnloadAsset(res_POOL_OBJ.obj_res);
		}
	}

}
