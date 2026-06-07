using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJGoPoolName : MonoBehaviour 
{
	public	GameObject[] 			m_ltGoPrefab;
	Dictionary<string,SJGoPoolMng> 	m_dStrPoolMng = new Dictionary<string, SJGoPoolMng>();

	public	bool			startFunc;
	public	GameObject		startFunc_activeObj;


	bool bInit = false;

	private void Start()
	{
		if( startFunc )
		{
			SJPool.InitMng();
			if( startFunc_activeObj != null )
				startFunc_activeObj.SetActive(true);
		}
	}

	public Dictionary<string,SJGoPoolMng>	GetPoolDic(){return m_dStrPoolMng;}

	public	void	InitMngObj()
	{
		if( bInit )return;

		bInit = true;

		foreach( GameObject obj in m_ltGoPrefab )
		{
			if( obj != null )
				InitMngObj_Prefab( obj );
		}
	}

	public	void 	InitMngObj_Prefab( GameObject obj )
	{
		if( obj == null ) return;
		if( Find_Mng( obj ) ) return;
		SJGoPoolObj sjgpo = obj.GetComponent<SJGoPoolObj>();

		string 		pool_name = obj.name + "_POOLMNG";
		GameObject	obj_pool = new GameObject( pool_name );
		obj_pool.AddComponent< SJGoPoolMng >();

		if( sjgpo.user_mng_Trans != null )
		{
			obj_pool.transform.parent = sjgpo.user_mng_Trans;
		}else{
			obj_pool.transform.parent = transform;		
		}

		obj_pool.transform.localScale = Vector3.one;
		
		int nObjType = 0;
		int nInstCount = 30;

		if( sjgpo != null )
		{
			nInstCount = sjgpo.m_InstCount;
			nObjType = sjgpo.m_nPoolObjType;
		}

		// // 최초 등록 
		// bool recent_atv = obj.activeSelf;
		// if( recent_atv == false )
		// 	obj.SetActive( true );
		// obj.SendMessage("OnRegFirstMng", SendMessageOptions.DontRequireReceiver);		
		// obj.SetActive( recent_atv );

		SJGoPoolMng sj_pool_mng = obj_pool.GetComponent<SJGoPoolMng>();
		sj_pool_mng.m_nPoolObjType = nObjType;
		sj_pool_mng.m_nAllocCount = nInstCount;
		sj_pool_mng.m_go_BaseObj = obj;
		sj_pool_mng.InitMng();
		m_dStrPoolMng[ obj.name ] = sj_pool_mng;
	}

	public	void	Destroy_Mng( GameObject obj )
	{
		if( obj == null ) return;
		SJGoPoolMng mng = Find_Mng_Obj(obj);
		if( mng == null ) return;
		mng.Destroy_Mng();
		GameObject.DestroyImmediate( mng );
		m_dStrPoolMng.Remove( obj.name );
	}


	public	bool	Find_Mng( GameObject go_prf )
	{
		SJGoPoolMng mng;
		if(	m_dStrPoolMng.TryGetValue( go_prf.name , out mng ) )
		{
			return true;
		}
		return false;
	}
	
	public 	SJGoPoolMng	Find_Mng_Obj(GameObject go_prf)
	{
		SJGoPoolMng mng;
		if(	m_dStrPoolMng.TryGetValue( go_prf.name , out mng ) )
		{
			return mng;
		}
		return null;
	}

	public SJGoPoolMng Find_Mng_Obj(string name)
	{
		SJGoPoolMng mng;
		if (m_dStrPoolMng.TryGetValue(name, out mng))
		{
			return mng;
		}
		return null;
	}


	public	GameObject	FindInst_ID( GameObject go_prf , int _id )
	{
		SJGoPoolMng mng = Find_Mng_Obj(go_prf);
		if( mng != null )
		{
			return mng.Find_ID(_id);
		}
		return null;
	}

	public GameObject GetNewInst(string name , bool bStartInstFunc = true)
	{
		SJGoPoolMng mng = null;
		if (m_dStrPoolMng.TryGetValue(name, out mng))
		{
			GameObject inst = mng.GetNewInst();
			if (inst != null)
			{
				if (bStartInstFunc)
				{
					inst.GetComponent<SJGoPoolObj>().StartInstSJ();
					inst.SendMessage("OnStartInstSJ", SendMessageOptions.DontRequireReceiver);
				}
				return inst;
			}
			else
			{

				Debug.LogError("GetNewInst  error GetNewInst : " + name);
				return null;
			}
		}
		Debug.LogError("GetNewInst  error No Find : " + name);
		return null;
	}

	public	GameObject GetNewInst( GameObject go_prf , bool bStartInstFunc = true )
	{
		InitMngObj_Prefab( go_prf );
		return GetNewInst(go_prf.name) ;
	}


	public	GameObject GetNewInst( GameObject go_prf , Vector3 pos , Quaternion	rot , Transform tr_par = null , bool isLocalTrans = false )
	{
		GameObject obj = GetNewInst( go_prf , false );
		if( obj != null )
		{
			if( tr_par != null )
				obj.transform.parent = tr_par;

			if( isLocalTrans )
			{
				obj.transform.localPosition = pos;
				obj.transform.localRotation = rot;
			}else{
				obj.transform.position = pos;
				obj.transform.rotation = rot;
			}

			obj.GetComponent<SJGoPoolObj>().StartInstSJ();
			obj.SendMessage( "OnStartInstSJ" , SendMessageOptions.DontRequireReceiver );
			return obj;
		}
		return null;
	}
	
	public	bool 	ReturnInst( GameObject	obj )
	{
		SJGoPoolMng mng;
		SJGoPoolObj pool_obj = obj.GetComponent<SJGoPoolObj>();

		if( pool_obj == null ) return false;
		obj.SendMessage("OnEndInstSJ", SendMessageOptions.DontRequireReceiver);
		obj.SetActive(false);

		if(	pool_obj.m_bUse == false )
		{
			//Debug.LogWarning( "����-- ReturnInst , pool_obj.m_bUse == false : " + obj.name );
			return false;
		}

		mng = pool_obj.m_cPoolMsg;
		if( mng == null )
		{
			if(	m_dStrPoolMng.TryGetValue( obj.name , out mng ) )
			{
			}else{
				Debug.LogWarning( "����-- m_dStrPoolMng.TryGetValue( obj.name , out mng ) : " + obj.name );
				return false;
			}
		}

		pool_obj.EndInstSJ();


		mng.ReturnInstDirect( obj );

		return true;
	}

	public	void	ReturnInst_All( GameObject	obj )
	{
		SJGoPoolMng mng = Find_Mng_Obj( obj );
		if( mng == null )
		{
			//Debug.LogError( "����!!! : ReturnInst_All mng == null " );
			return;
		}
		mng.ReturnAllInst();
	}

	public void ReturnInst_All()
	{
		foreach( KeyValuePair<string , SJGoPoolMng> k in m_dStrPoolMng )
		{
			k.Value.ReturnAllInst();
		}
	}

	public	void	GetInstAll_Use( GameObject	obj , List<GameObject> list )
	{
		SJGoPoolMng mng = Find_Mng_Obj( obj );
		if( mng == null )
		{
			Debug.LogError( "����!!! : ReturnInst_All mng == null " );
			return;
		}
		mng.GetInstAll_Use(list);
	}

	public	void	GetInstAll_Use( List<GameObject> list )
	{
		foreach( KeyValuePair<string , SJGoPoolMng> k in m_dStrPoolMng )
		{
			k.Value.GetInstAll_Use(list);
		}
	}

	public	void	SendMsgAll( string func , object args = null , bool all = true )
	{
		foreach( KeyValuePair<string , SJGoPoolMng> k in m_dStrPoolMng )
		{
			k.Value.SendMsgAll( func , args  , all );
		}
	}
}



public class SJPool
{
	public static	SJGoPoolName g_pool_global = null;
	
	public static	void 	InitMng()
	{
		if( g_pool_global != null ) return;

		// GameObject pool_global =	GameObject.Find("_SJPoolGlobal");
		// if( pool_global == null )
		// {
		// 	Debug.Log( "SJPool Not Find SJPoolGlobal obj" );
		// 	return;
		// }
		// g_pool_global = pool_global.GetComponent<SJGoPoolName>();
		SJGoPoolName[] sJGoPoolNames = GameObject.FindObjectsByType<SJGoPoolName>( FindObjectsSortMode.None );

		if (sJGoPoolNames.Length < 1)
		{
			GameObject go_mng = new GameObject();
			SJGoPoolName sJGoPoolName = go_mng.AddComponent<SJGoPoolName>();
			go_mng.name = "_SJPoolGlobal";
			g_pool_global = sJGoPoolName;
		}
		else
		{
			g_pool_global = sJGoPoolNames[0];

			if (sJGoPoolNames.Length > 1)
			{
				for (int i = 1; i < sJGoPoolNames.Length; i++)
				{
					GameObject.DestroyImmediate(sJGoPoolNames[i].gameObject);
				}
			}
		}
		g_pool_global.InitMngObj();
	}

	

	static	public	void	Init_PoolObj( SJGoPoolObj sjpoolobj )
	{
		g_pool_global.InitMngObj_Prefab( sjpoolobj.gameObject );
	}

	static	public	void	Init_PoolObj( GameObject go )
	{
		if( go != null )
			g_pool_global.InitMngObj_Prefab( go );
	}

	static	public	void	Destroy_Mng( GameObject go )
	{
		if( go != null )
			g_pool_global.Destroy_Mng( go );
	}

	static	public	GameObject	FindInst_ID( GameObject go_prf , int _id )
	{
		return 	g_pool_global.FindInst_ID( go_prf , _id );
	}

	public	static GameObject GetNewInst( GameObject go_prf )
	{
		return 	g_pool_global.GetNewInst( go_prf );
	}

	public	static GameObject GetNewInst_InitMng( GameObject go_prf )
	{
		Init_PoolObj( go_prf );
		return 	g_pool_global.GetNewInst( go_prf );
	}


	public static GameObject GetNewInst(string name)
	{
		return g_pool_global.GetNewInst(name);
	}

	public	static	GameObject GetNewInst( GameObject go_prf , Vector3 pos , Quaternion	rot , Transform tr_par = null , bool isLocalTrans = false )
	{
		if( go_prf == null ) return null;
		return g_pool_global.GetNewInst( go_prf , pos , rot , tr_par , isLocalTrans );
	}
	
	public	static GameObject	GetNewInst_Or_Create( GameObject go_prf )
	{
		if( go_prf == null ) return null;
		if( g_pool_global != null && g_pool_global.Find_Mng( go_prf ) )
		{
			return GetNewInst(go_prf);
		}
		return GameObject.Instantiate( go_prf ) as GameObject;
	}

	public	static	void ReturnInst( GameObject obj )
	{
		if( obj == null )
		{
			//Debug.LogWarning( " ReturnInst : obj == null " );
			return;
		}
		g_pool_global.ReturnInst( obj );
	}


	public	static	void	ReturnInst_Child( GameObject obj_par )
	{
		for( int i = 0 ; i < obj_par.transform.childCount ; i++ )
		{
			Transform ch = obj_par.transform.GetChild(i);
			ReturnInst(ch.gameObject);
		}
	}

	public	static	bool ReturnInst_Or_Destroy( GameObject obj )
	{
		if( obj == null ) return false;

		if( g_pool_global == null )
		{
			GameObject.Destroy( obj );
			return true;
		}

		if(	g_pool_global.ReturnInst( obj ) == false )
		{
			GameObject.Destroy( obj );
			return false;
		}
		return true;
	}


	public	static	void	ReturnInst_All( GameObject obj )
	{
		g_pool_global.ReturnInst_All( obj );
	}

	public static void ReturnInst_All()
	{
		g_pool_global.ReturnInst_All();
	}

	public	static	void	GetInstAll_Use( GameObject	obj , List<GameObject> list )
	{
		g_pool_global.GetInstAll_Use( obj , list );
	}


	public	static	void	GetInstAll_Use( List<GameObject> list )
	{
		g_pool_global.GetInstAll_Use( list );
	}

	public	static bool 	FindMng( GameObject obj )
	{
		return	g_pool_global.Find_Mng( obj );
	}

	static	public	void	SendMsgAll( string func , object args = null , bool all = true )
	{
		g_pool_global.SendMsgAll( func , args  , all );
	}

}