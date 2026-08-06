using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SJ_COMMON
{
	public delegate void Func_VOID();

	public delegate void Func_Arg( object obj );
	public delegate void Func_Arg_BOOL( bool arg );
	public delegate void Func_Arg_INT( int arg );
	public delegate void Func_Arg_STR( string arg );


	// 시계 방향
	public enum SJ_NEWS_DIR
	{
		None = -1,
		N = 0 ,
		E , 
		S , 
		W 
	}

	static public float AngleNEWS( SJ_NEWS_DIR dir )
	{
		switch( dir )
		{
			case SJ_NEWS_DIR.N: return 0; 
			case SJ_NEWS_DIR.E: return 90; 
			case SJ_NEWS_DIR.S: return 180; 
			case SJ_NEWS_DIR.W: return 270; 
		}
		return 0;
	}
}

public	class _SJ_GO_FUNC
{
	public	bool	debug;

	public	GameObject		go;
	public	MonoBehaviour	mono;
	public	string			func;

	public	object			arg_obj;

	public bool	 	Check_Func()
	{
		if( string.IsNullOrEmpty(func) ) return false;
		return true;
	}

	public	bool	IsEq( _SJ_GO_FUNC other )
	{
		if( other.go != go || other.func != func ) return false;
		return true;
	}

	public	void	Init()
	{
		go = null;mono = null;func="";arg_obj = null;
	}

	public	void	Set( GameObject _go = null , string _func = "" , object _arg = null)
	{
		go = _go;func = _func;arg_obj = _arg;
		mono = null;
	}

	public	void	SetMono( MonoBehaviour _mono = null , string _func = "" , object _arg = null)
	{
		mono = _mono;func = _func;arg_obj = _arg;
		go = null;
	}

	public	void	Func(object arg = null ) 
	{
		if( go != null )
		{ 
			if(arg_obj != null) SJ_Unity.SendMsg( go, func, arg_obj , debug );
			else				SJ_Unity.SendMsg( go ,func , arg , debug );
		}else if( mono != null ){
			if(arg_obj != null) SJ_Unity.SendMsg( mono, func, arg_obj , debug );
			else				SJ_Unity.SendMsg( mono ,func , arg , debug );
		}
	}
}

[System.Serializable]
public class SJ_MONO_FUNC
{
	public	MonoBehaviour	mono;
	public	string			func;

	public	void	Func(object arg = null ) 
	{
		if(arg != null) SJ_Unity.SendMsg( mono, func  );
		else				SJ_Unity.SendMsg( mono ,func , arg  );
	}

	public bool Check_Func()
	{
		if( mono == null || string.IsNullOrEmpty(func) ) return false;
		return true;
	}
}


public class SJ_Unity
{
	static public void SetActive(GameObject go, bool b)
	{
		if (go == null) return;
		go.SetActive(b);
	}

	static public bool SendMsg( MonoBehaviour mono , string func , object arg = null , bool debug = false )
	{
		if( mono == null || string.IsNullOrEmpty(func) )
		{
			return false; 
		}

		if( debug ) Debug.Log( "SendMsg : " + mono.name + " : " + func );

		if(arg == null)
		{
			mono.SendMessage(func , SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			mono.SendMessage(func, arg, SendMessageOptions.DontRequireReceiver);
		}
		return true;
	}

	static public bool SendMsg( GameObject mono , string func , object arg = null  , bool debug = false )
	{
		if( mono == null || string.IsNullOrEmpty(func) )
		{
			return false; 
		}

		//Debug.Log( "SendMsg : " + mono.name + " : " + func );

		if( debug ) Debug.Log( "SendMsg : " + mono.name + " : " + func );

		if(arg == null)
		{
			mono.SendMessage(func , SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			mono.SendMessage(func, arg, SendMessageOptions.DontRequireReceiver);
		}
		return true;
	}


	static	public	void	SetEqTrans( Transform self , Transform other = null , Transform par = null )
	{
		if( par != null )
		{
			self.SetParent (par);
		}

		if( other != null )
		{
			self.position = other.position;
			self.rotation = other.rotation;
			self.localScale = other.localScale;
		}
		else
		{
			self.localPosition = Vector3.zero;
			self.localRotation = Quaternion.identity;
			self.localScale = Vector3.one;
		}
	}

	public static float ObjColorLerpTime(GameObject go, float cur, float total, Color col)
	{
		if (cur < 0)
			return -1.0f;

		cur -= Time.deltaTime;
		float fRatio = 1.0f - cur / total;
		if (cur < 0.0f)
		{
			fRatio = 1.0f;
		}

		Color cur_col = Color.Lerp(col, new Color(1, 1, 1, 1), fRatio);

		SkinnedMeshRenderer[] sm_list = go.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer sm in sm_list)
		{
			sm.material.color = cur_col;
		}
		MeshRenderer[] ma_list = go.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer m in ma_list)
		{
			m.material.color = cur_col;
		}

		return cur;
	}

	public static void ObjRenderShowHide(GameObject go, bool bFlag)
	{
		SkinnedMeshRenderer[] sm_list = go.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer sm in sm_list)
		{
			sm.enabled = bFlag;
		}
		MeshRenderer[] ma_list = go.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer m in ma_list)
		{
			m.enabled = bFlag;
		}
	}

	public static void ObjTextureChange(GameObject go, Texture tex, string meshName = "" )
	{
		SkinnedMeshRenderer[] sm_list = go.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer sm in sm_list)
		{
			if( string.IsNullOrEmpty( meshName ) )
			{
				sm.material.SetTexture("_MainTex", tex);
				continue;
			}

			if ( sm.name == meshName) sm.material.SetTexture("_MainTex", tex);
		}
		MeshRenderer[] ma_list = go.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer m in ma_list)
		{
			if( string.IsNullOrEmpty( meshName ) )
			{
				m.material.SetTexture("_MainTex", tex);
				continue;
			}

			if (m.name == meshName)m.material.SetTexture("_MainTex", tex);
		}
	}

	public	static	void	ObjTextureChange(Renderer[] rd_list , Texture tex )
	{
		foreach (Renderer s in rd_list)
		{
			s.material.SetTexture("_MainTex", tex);
		}
	}

	public	static void		SetColor_Shader( Material mat , Color col , string param_name = "" )
	{
		if( string.IsNullOrEmpty( param_name ) )
		{
			param_name = "_MainColor";
		}

		mat.SetColor( param_name , col );
	}



	public	static	void	SetRender_RenderQ( GameObject go , int rdq )
	{
		Renderer[] rds = go.GetComponentsInChildren<Renderer>();
		foreach( Renderer s in rds ) s.material.renderQueue = rdq;
	}




	public	static	float	AnimationClip_CrossPlay( Animation ani , string ani_name , float speed=-1000.0f , float play_range=1.0f , 
		bool no_play = false , bool cross = true , bool queue = false )
	{
		if( ani == null || string.IsNullOrEmpty(ani_name)  ) return -1;

		AnimationState	st = ani[ani_name];

		if( st == null ) return -1;

		if( speed > -999.0f )
			st.speed = speed;

		AnimationClip clip = st.clip;

		float total_time = clip.length * st.speed * play_range;

		if( no_play == false )
		{
			if( queue == false )
			{
			 	if( cross )	ani.CrossFade( clip.name);
				else		ani.Play( clip.name);
			}
			else
			{
			 	if( cross )	ani.CrossFadeQueued( clip.name);
				else		ani.PlayQueued( clip.name);
			}
		}  

		return total_time;
	}

	public	static	float	AnimationClip_CrossPlay( Animation ani , AnimationClip clip , float speed=-1.0f , float play_range=1.0f , 
		bool no_play = false , bool cross = true , bool queue = false )
	{
		if( ani == null || clip == null ) return -1;
		AnimationState	st = ani[clip.name];
		float total_time = 0;
		if( st != null )
		{
			if( speed > 0 )
			{
				st.speed = speed;
			}
			else
			{
				speed = st.speed;
			}
		}
		total_time = clip.length * speed * play_range;
		total_time = Mathf.Abs(total_time);
		if( no_play == false )
		{
			if( queue == false )
			{
			 	if( cross )	ani.CrossFade( clip.name );
				else		ani.Play( clip.name);
			}
			else
			{
			 	if( cross )	ani.CrossFadeQueued( clip.name);
				else		ani.PlayQueued( clip.name);
			}
		}  
		return total_time;
	}


	// 애니메이터 스테이트 시간만큼 배속 변경해서 플레이
	// https://wergia.tistory.com/41
	public	static	void	AnimatorPlay_TotalTimeSpeed(Animator anit , AnimationClip clip , string state_name , string para_name ,  float playTime )
	{
		if( anit == null || clip == null ) return;
		float spd = clip.length / playTime;
		anit.SetFloat( para_name , spd );
		anit.Play(state_name);
	}


	public	static float	GetTime_AniClip(Animation ani , AnimationClip clip )
	{
		if( ani == null || clip == null ) return -1;
		AnimationState	st = ani[clip.name];

		if( st == null )
		{
			Debug.LogError( "Error!! : GetTime_AniClip : st == null : clip.name : " + clip.name );
			return 0;
		}

		return clip.length * st.speed;
	}

	// 애니 플래이
	// 지정된 시간 , 지정된 애니 정규화 된 시간
	// 애니의 normalize_time 만큼 time 동안 재생
	public	static	void	AnimationClip_Play_Time_Ratio( Animation ani , AnimationClip clip , float time , float normalize_time=1.0f , bool cross = false )
	{
		if( ani == null || clip == null ) return;

		AnimationState	st = ani[clip.name];
		float total_time = st.length * normalize_time;

		st.speed = total_time / time;

		if( cross )	ani.CrossFade( clip.name);
		else		ani.Play( clip.name);
	}

	static public bool Animator_CurState( Animator anit , string state_name , int layer = 0 )
	{
		AnimatorStateInfo state = anit.GetCurrentAnimatorStateInfo(layer);
	    if(	state.shortNameHash == Animator.StringToHash(state_name) )return true;
		return false;
	}

	static public bool Animator_NextState( Animator anit , string state_name , int layer = 0 )
	{
		AnimatorStateInfo state = anit.GetNextAnimatorStateInfo(layer);
	    if(	state.shortNameHash == Animator.StringToHash(state_name) )return true;
		return false;
	}

	public	static	void	Particle_Active( GameObject go , bool b )
	{
		if( go == null ) return;

		if( b ) go.SetActive(true);

		ParticleSystem[] par_list = go.GetComponentsInChildren<ParticleSystem>();
		foreach( ParticleSystem s in par_list )
		{
			if( b )
				s.Play();
			else
				s.Stop();
		}
	}


	// 랜덤 리스트 계열
	public	static	object	Array_RandomObj( object[] ar )
	{
		if(	ar.Length < 1 )return null;
		int i = UnityEngine.Random.Range( 0 , ar.Length );
		return ar[i];
	}

	// List<T> 형태로 받아서 그중에 한개를 랜덤으로 선택하고  object 타입으로 리턴
	// 만약 List 가 없으면 null 리턴

    public static object GetRandomItem<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return null;
        }

        int index = UnityEngine.Random.Range( 0 , list.Count );
        return (object)list[index];
    }



	public	static T GetArray_Random<T>( T[] arr )
	{
		if( arr.Length < 1 )
			return default(T);

		int sel = UnityEngine.Random.Range( 0 , arr.Length );
		return arr[sel];
	}


	public	static	int		List_Add_Or_Set<T>(List<T> list , T t ,int index )
	{
		if( index < 0 )
		{
			list.Add(t);
			return 1;
		}
		if(	list.Count <= index )
		{
			int add_count = (index+1) - list.Count;
			for( int i = 0 ; i < add_count ; i++ )list.Add(t);
			return add_count;
		}
		list[index] = t;
		return 1;
	}

	//
	// 리스트에 있는걸 갯수만큼 섞어서 리턴
	//
	public	static	List<T>	GetRandom_ListMix<T>( List<T> list , int count )
	{
		List<T> lt = new List<T>( list );
		List<T> lt_t = new List<T>();

		for( int i = 0 ; i < count ; i++ )
		{
			if( lt.Count < 1) return lt_t;
			int idx = UnityEngine.Random.Range( 0, lt.Count );
			T t = lt[idx];
			lt.RemoveAt( idx );
			lt_t.Add( t );
		}

		return lt_t;
	}


	public	static T GetRandom_Pop<T>( List<T> lt )
	{
		if( lt.Count < 1 ) return default(T);
		int idx = UnityEngine.Random.Range( 0, lt.Count );
		T t = lt[idx];
		lt.RemoveAt(idx);
		return t;
	}


	public static void GameObj_OneSetActive(GameObject[] go_ar, string go_name)
	{
		foreach (GameObject go in go_ar)
		{
			if (go.name == go_name) go.SetActive(true);
			else go.SetActive(false);
		}
	}
	public static void GameObj_OneSetActive(GameObject[] go_ar, int idx)
	{
		for (int i = 0; i < go_ar.Length; i++)
		{
			if (i == idx) go_ar[i].SetActive(true);
			else go_ar[i].SetActive(false);
		}
	}

	public	static	void	Child_Active(GameObject go_parent , bool b )
	{ 
		for( int i = 0 ; i < go_parent.transform.childCount ; i++ )
		{
			Transform tr_ch = go_parent.transform.GetChild(i);
			tr_ch.gameObject.SetActive(b);
		}
	}

	public	static	void	Child_OneSetActive(GameObject go_parent, string go_name)
	{
		for( int i = 0 ; i < go_parent.transform.childCount ; i++ )
		{
			Transform tr_ch = go_parent.transform.GetChild(i);
			if (tr_ch.gameObject.name == go_name) tr_ch.gameObject.SetActive(true);
			else tr_ch.gameObject.SetActive(false);
		}
	}

	public	static	void	Child_OneSetActive(GameObject go_parent, GameObject go_active)
	{
		for( int i = 0 ; i < go_parent.transform.childCount ; i++ )
		{
			Transform tr_ch = go_parent.transform.GetChild(i);
			if (tr_ch.gameObject == go_active) tr_ch.gameObject.SetActive(true);
			else tr_ch.gameObject.SetActive(false);
		}
	}

	public	static	void	Child_OneSetActive(GameObject go_parent, int idx = -1)
	{
		for( int i = 0 ; i < go_parent.transform.childCount ; i++ )
		{
			Transform tr_ch = go_parent.transform.GetChild(i);
			if (i == idx) tr_ch.gameObject.SetActive(true);
			else tr_ch.gameObject.SetActive(false);
		}
	}

	public static Transform GetChild_Random( GameObject go_parent )
	{
		List<Transform> lt_ch = new List<Transform>();
		for( int i = 0 ; i < go_parent.transform.childCount ; i++ )
		{
			Transform tr_ch = go_parent.transform.GetChild(i);
			lt_ch.Add( tr_ch );
		}
		if( lt_ch.Count == 0 ) return null;
		return GetArray_Random( lt_ch.ToArray() );
	}

	static public GameObject 	GameObj_FindActive( string path )
	{
		GameObject go = GameObject.Find(path);
		if( go == null )
		{
			Debug.LogError("Error!! : GameObj_FindActive : " + path );
			return null;
		}
		go.SetActive(true);
		return go;
	}

	public static string GetAppPathFile(string filename , bool asset_folder = false )
	{
#if UNITY_EDITOR
		string f = "";
		if (asset_folder == false)
		{
			f = Application.persistentDataPath + "/";
			f += filename;
		}
		else
		{
			f = Application.dataPath;
			f = f.Substring(0, f.LastIndexOf('/'));
			f += "/Assets/Resources/";
		}

		Debug.Log(f);

		return (f);
#else

		if (!Directory.Exists(Application.persistentDataPath ))Directory.CreateDirectory(Application.persistentDataPath);

		string path = Application.persistentDataPath + "/" + filename;

		Debug.Log("and_Path : " + path );
		return path;
#endif
	}

	public	static	StreamWriter	FileCreateWriteTxt( string filename , bool asset_folder = false )
	{
		string path_file =	GetAppPathFile( filename , asset_folder );
		StreamWriter sw =	File.CreateText( path_file );
		return sw;
	}


	public	static	StreamReader 	FileOpenReadTxt( string filename , bool asset_folder = false )
	{
		string path_file =	GetAppPathFile( filename , asset_folder );
		if( !File.Exists(path_file) )
		{
			return null;
		}
		StreamReader sr;
		sr =	File.OpenText( path_file );
		return sr;
	}

	public	static	string 	FileOpenReadTxt_All( string filename , bool asset_folder = false )
	{
		string path_file =	GetAppPathFile( filename , asset_folder );
		if( !File.Exists(path_file) )
		{
			return null;
		}
		StreamReader sr;
		sr =	File.OpenText( path_file );
		string str_data = sr.ReadToEnd();
		sr.Close();
		return str_data;
	}

	public	static	string FileOpen_ReadLine( string filename )
	{
		StreamReader sr = FileOpenReadTxt(filename);
		if (sr == null) 
		{
			return "";
		}
		string	str_list =	sr.ReadLine();
		sr.Close();
		return str_list;
	}

	public	static	bool FileCreate_WriteLine(string filename , string text , bool new_create = true )
	{
		string path_file =	GetAppPathFile( filename );

		StreamWriter sw = null;

		if( new_create )
		{
			sw =	File.CreateText( path_file );
			Debug.Log( " FileCreate_WriteList :  " + path_file );
		}
		else 
			sw =	File.AppendText( path_file );
		//Debug.Log( " FileCreate_WriteList :  " + path_file );

		if( sw == null )
		{
			Debug.Log( "Error!!!!!!! FileCreate_WriteList : " + path_file );
			return false;
		}

		sw.WriteLine( text );
		sw.Close();

		return true;
	}
	static public FileStream fs_recent = null;
	static public BinaryWriter bw_recent = null;
	static public BinaryReader br_recent = null;

	static public void CloseFileBin_Recent()
	{
		Debug.Log( "파일 닫기 시도 : " );

		if( bw_recent != null )
        {
            bw_recent.Close();
			bw_recent = null;
        }

		if( br_recent != null )
        {
            br_recent.Close();
			br_recent = null;
        }

		if( fs_recent == null ) return;
		Debug.Log( "<<<<----- 파일 닫기 성공 ~~ "  );
		fs_recent.Close();
		fs_recent = null;
	}

	public	static	BinaryWriter  FileCreate_Bin(string filename , bool cur_exe_dir = false )
	{
		Debug.Log( "----->>>> 파일 쓰기 열기 : " + filename );

		string path_file =	GetAppPathFile( filename );
		if( cur_exe_dir ) path_file = filename;
		fs_recent = File.Create( path_file );
		if( fs_recent == null ) return null;
		BinaryWriter bw = new BinaryWriter(fs_recent);
		bw_recent = bw;
		return	bw;
	}



	public	static	BinaryReader  FileLoad_Bin(string filename , bool cur_exe_dir = false )
	{
		Debug.Log( "----->>>> 파일 읽기 열기 : " + filename );

		string path_file =	GetAppPathFile( filename );
		if( cur_exe_dir ) path_file = filename;
		if( File.Exists(path_file) == false ) return null;
		fs_recent =	File.OpenRead( path_file );
		if( fs_recent == null ) return null;
		BinaryReader br = new BinaryReader( fs_recent );
		br_recent = br;
		return br;
	}

	public	static	byte[]  FileLoad_Bin_Buff(string filename )
	{
		string path_file =	GetAppPathFile( filename );
		if( File.Exists(path_file) == false ) return null;
		fs_recent =	File.OpenRead( path_file );
		if( fs_recent == null ) return null;
		if( fs_recent.Length < 1 )
		{
			fs_recent.Close();
			return null;
		}
		byte[] buff = new byte[ fs_recent.Length ];
		fs_recent.Read(buff , 0 , (int)fs_recent.Length);
		fs_recent.Close();
		return buff;
	}

	static public bool FileDelete( string filename , bool cur_exe_dir = false)
    {
		string path_file =	GetAppPathFile( filename );
		if( cur_exe_dir ) path_file = filename;
		if( File.Exists(path_file) == false ) return false;
		File.Delete(path_file);
		return true;
    }

	public static	bool	PerRandom( int ratio , int max = 100 )
	{
		int r = UnityEngine.Random.Range( 0 , max );
//		Debug.Log( "PerRandom : " + r + " : " + ratio );
		if( r <= ratio ) return true;
		return false;
	}

	public static	bool	PerRandom( float ratio , float max = 100.0f )
	{
		float r = UnityEngine.Random.Range( 0 , max );
		if( r <= ratio ) return true;
		return false;
	}

	// public static void SetList<T>( List<T> list )

	public static T Random_RangeStepList_T<T>(List<T> list)
	{
		List<int>	list_int = new List<int>();
		foreach( T s in list )
		{
			RANDOM_RANGE_STEP_BASE b = s as RANDOM_RANGE_STEP_BASE;
			list_int.Add(b.ratio);
		}
		int idx = Random_RangeStepList( list_int.ToArray() );
		return list[idx];
	}

	public static int Random_RangeStepList(int[] nRatioList)
	{
		if (nRatioList.Length < 1)
			return 0;
		int[] step_list = new int[nRatioList.Length];
		int nAcc = 0;
		for (int i = 0; i < nRatioList.Length; i++)
		{
			nAcc += nRatioList[i];
			step_list[i] = nAcc;
		}
		int nVal = UnityEngine.Random.Range(0, nAcc);
		int pre_step = 0;
		for (int i = 0; i < nRatioList.Length; i++)
		{
			if (step_list[i] != pre_step && step_list[i] > nVal)
				return i;
			pre_step = step_list[i];
		}
		return 0;
	}


	public	static	void	SetLayer_Obj( GameObject go , string layer_name , bool child )
	{
		go.layer = LayerMask.NameToLayer( layer_name );
		if( child )
		{
			for( int i = 0 ; i < go.transform.childCount ; i++ )
			{
				Transform tr_child = go.transform.GetChild(i);
				//tr_child.gameObject.layer = LayerMask.NameToLayer( layer_name );
				SetLayer_Obj( tr_child.gameObject , layer_name , child );
			}
		}
	}

	public	static	bool	Collider_RayCheck_ByWorld( Vector3 pos_world , Collider coll )
	{
		pos_world.z = -2000.0f;
		Ray ray = new Ray(pos_world ,new Vector3( 0,0,1 ) );
		RaycastHit	hit;
		if(	coll.Raycast(ray , out hit , 100000 ) )
		{
			//Debug.Log(	"레이케스트!!! Collider_RayCheck_ByWorld" );
			return true;
		}
		//Debug.Log(	"안됨~~~ 레이케스트   Collider_RayCheck_ByWorld" );
		return false;
	}

	public	static	bool	Collider_RayCheck_ByMouse( Collider coll , Camera _cam = null )
	{
		Camera cam = Camera.main;
		if( _cam != null ) cam = _cam;

		Ray ray = cam.ScreenPointToRay( Input.mousePosition );
		RaycastHit	hit;
		if(	coll.Raycast(ray , out hit , 100000 ) )
		{
			//Debug.Log(	"레이케스트!!! Collider_RayCheck_ByWorld" );
			return true;
		}
		//Debug.Log(	"안됨~~~ 레이케스트   Collider_RayCheck_ByWorld" );
		return false;
	}

	public static Vector3? Point_RayCheck_ByMouse(Collider coll, Camera _cam = null)
	{
		Camera cam = Camera.main;
		if (_cam != null) cam = _cam;

		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (coll.Raycast(ray, out hit, 100000))
		{
			return hit.point;
		}
		return null;
	}

	public	static	Collider PickMouse_RayCast( int layer_mask )
	{
		RaycastHit hit;
		Ray ray =	Camera.main.ScreenPointToRay( Input.mousePosition );
		if(	Physics.Raycast( ray , out hit , 10000 , layer_mask ) )
		{
			return hit.collider;
		}
		return null;
	}

	public	static	RaycastHit[] PickMouse_RayCastAll( int layer_mask )
	{
		Ray ray =	Camera.main.ScreenPointToRay( Input.mousePosition );
		return 	Physics.RaycastAll( ray , 10000 , layer_mask );
	}

	public static Collider2D PickMouse_RayCast2D(int layer_mask)
	{
		Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Ray2D ray = new Ray2D(wp, Vector2.zero);
		RaycastHit2D hit = Physics2D.Raycast( ray.origin , ray.direction , 10000 , layer_mask);
		//RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
		if (hit.collider != null)
		{
			return hit.collider;
		}
		return null;
	}


	public static RaycastHit2D[] PickMouse_RayCast2DAll(int layer_mask)
	{
		Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Ray2D ray = new Ray2D(wp, Vector2.zero);
		RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 10000, layer_mask);
		return hits;
	}

	public static bool  RayCast_PosTarget( Vector3 pos , Vector3 tar , float dist , int layer_mask )
	{
		Vector3 dir = tar - pos;
		float len = dir.magnitude;
		dir.Normalize();
		Ray ray = new Ray( pos , dir );
		//return Physics.RaycastAll( ray , dist , layer_mask );
	    RaycastHit[] hits =	Physics.RaycastAll( ray , dist , layer_mask );
		if( hits.Length > 0 )
		{
			foreach( var s in hits ) 
			{
				if( s.distance < len )
				{
					return true;
				}
			}
		}

		return false;
	}


	public	static	void	Delete_Child( Transform tr , bool sjpool_return = false , bool DestroyImmediate = true )
	{
		if( tr == null ) return;

		List<Transform> list_ch = new List<Transform>();

		for( int i = 0 ; i < tr.childCount ; i++ )
		{
			list_ch.Add( tr.GetChild(i) );
		}

		foreach( Transform s in list_ch )
		{
			if( sjpool_return == false )
			{
				if( DestroyImmediate )
					GameObject.DestroyImmediate(s.gameObject);
				else 
					GameObject.Destroy( s.gameObject );
			}
				
			else 
				SJPool.ReturnInst_Or_Destroy( s.gameObject );
		}
	}

	public static	Transform	FindChild_All( Transform tr , string str )
	{
		Transform tr_child = tr.Find( str );
		if( tr_child != null ) return tr_child;

		for( int i=0; i < tr.childCount ; i++ )
		{
			tr_child = FindChild_All( tr.GetChild(i) , str );
			if( tr_child != null ) return tr_child;
		}
		return null;
	}

	public static List<Transform> GetChildList(Transform tr)
	{
		List<Transform> lt = new List<Transform>();
		for( int i = 0 ; i < tr.childCount ; i++ )
		{
			lt.Add( tr.GetChild(i) );
		}
		return lt;
	}

	public static List<T> GetChildList_Component<T>(Transform tr , bool active = true)
	{
		List<Transform> trs = GetChildList( tr);

		List<T> lt = new List<T>();
		foreach( var s in trs )
		{
			// 활성화 옵션인데 , 활성화가 아니면 안함
			if( active && s.gameObject.activeSelf == false )
			{
			}
			else
			{
				T t = s.GetComponent<T>();
				if( t != null )
				{
					lt.Add(t);
				}	
			}

		}
		return lt;
	}

	static public string 	RemoveString_ForNum( string str , bool end_point_remove = true )
	{
		string str_new = str.Trim();

		str_new =	str_new.Replace( " " ,"");
		str_new =	str_new.Replace( "," ,"");
		str_new =	str_new.Replace( "\"" ,"");
		str_new =	str_new.Replace( "%" ,"");

		if( end_point_remove )
			str_new =	str_new.Replace( "." ,"");

		return str_new;
	}

	static public float TryParseFloat( string str )
	{
		float val = 0;
		float.TryParse( str , out val );
		return val;
	}

	static public int TryParseInt( string str )
	{
		if( str.Length < 1 ) return 0;

		int val = 0;
		int.TryParse( str , out val );
		try
		{
			int m = int.Parse(str);
		}
		catch (FormatException e)
		{
			Debug.Log(" TryParseInt " + "[" + str + "] " + e.Message);
		}

		return val;
	}

	static	public	bool	Check_TryParseInt(string str)
	{
		int val = -1;
		if(	int.TryParse( str , out val ) ) return true;
		return false;
	}


	static public List<string> Read_1Dim_String( string str )
	{
		List<string> list_str = new List<string>();
		if( string.IsNullOrEmpty( str ) ) return list_str;
		str =	str.Replace( "(" , "" );
		str =	str.Replace( ")" , "" );
		string[]	 str_ar = str.Split(',');
		list_str.AddRange( str_ar );
		return list_str;
	}

	static public  List<List<string>>	Read_2Dim_String( string str )
	{
		List<List<string>> list_par = new List<List<string>>();

		if (str.Length < 1)
		{
			//Debug.Log( "return!! Read_2Dim_String : str.Length < 1 " );
			return list_par;
		}

		string	str_count_1 = str.Substring( 0, 1 );
		int		count_1 = 0;
		if (int.TryParse(str_count_1, out count_1) == false)
		{
			Debug.Log( "return!! Read_2Dim_String : int.TryParse(str_count_1, out count_1) == false " );
			return null;
		}

		int find_start_idx = 0;
		for (int i = 0; i < count_1; i++)
		{
			if( str.Length <= find_start_idx ) break;

			int find_s = str.IndexOf( '(' , find_start_idx );
			int find_e = str.IndexOf( ')' , find_start_idx );

			if (find_s < 0 || find_e < 0)
			{
				Debug.Log( "return!! Read_2Dim_String : find_s < 0 || find_e < 0 " );
				break;
			}

			int read_s = find_s+1;
			int read_e = find_e;
			int read_len = read_e - read_s;
			if (read_len < 1)
			{
				Debug.Log( "return!! Read_2Dim_String : read_len < 1 " );
				Debug.Log( "find_s : " + find_s.ToString() );
				Debug.Log( "find_e : " + find_e.ToString() );
				break;
			}

			string		str_part = str.Substring( read_s , read_len );
			string[]	str_unit_ar = str_part.Split( ',' );
			List<string> str_unit_list = new List<string>();
			str_unit_list.AddRange( str_unit_ar );
			list_par.Add( str_unit_list );

			find_start_idx = find_e + 1;
		}

		return list_par;
	}


	static	public	GameObject	ResInst_Default( string _path , string _default_res , string _res )
	{
		string path = "";
		if( string.IsNullOrEmpty( _res ) == false ) path = _path + "/" + _res;
		else										path = _path + "/" + _default_res;
		GameObject prf_model = Resources.Load( path , typeof(GameObject) ) as GameObject;
		if( prf_model == null ) return null;
		GameObject go_model = GameObject.Instantiate( prf_model );
		return go_model;
	}


	static	public	void		Random_CreateBatch( Transform tr_par , Vector3 v3bb , int count , List<GameObject>	list_obj )
	{
		for( int i = 0 ; i < count ; i ++ )
		{
			Vector3 pos = SJ_Cood.Random_BoxBound(v3bb);
			GameObject go =	GetArray_Random<GameObject>(list_obj.ToArray());
			GameObject inst = GameObject.Instantiate( go );
			inst.transform.parent = tr_par;
			inst.transform.localPosition = pos;
		}
	}


	static public void 		RadnomSeed( int seed = -1 )
    {
        if( seed == -1 )
        {
            long tick = DateTime.Now.Ticks;
			UnityEngine.Random.InitState( (int)tick );
        }
    }

	// 그냥 유니티 랜덤
	static public int 		Random( int min , int max )
	{
		return UnityEngine.Random.Range( min , max );
	}

	static public float 	Random( float min , float max )
	{
		return UnityEngine.Random.Range( min , max );
	}

	static	public	bool		SetAnit_LayerName( Animator anit , string layer , float weight = 1.0f )
	{
		if( anit == null ) return false;
		int idx =	anit.GetLayerIndex(layer);
		if( idx < 0 )return false;

		//Debug.Log( "=================== SetAnit_LayerName : " + layer + " : " + weight );

		//anit.runtimeAnimatorController

		anit.SetLayerWeight(idx , weight );
		return true;
	}

	static public AnimatorClipInfo GetCurrentAnimationStateName(Animator animator, int layer = 0)
	{
		if (animator == null) return default(AnimatorClipInfo);

		AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(layer);
		if (clipInfos.Length == 0) return default(AnimatorClipInfo);

		return clipInfos[0];
	}

	static	public	bool	Check_InCamera( Vector3 pos , Camera _cam = null , float offset = 0 )
	{
		Camera cam = Camera.current;
		if( _cam != null ) cam = _cam;

		Vector3 v =	cam.WorldToViewportPoint( pos );
		if( v.z <= 0 ) return false;
		if( v.x < 0+offset || v.x > 1-offset || v.y < 0+offset || v.y > 1-offset ) return false;
		return true;
	}

	static public void Animator_ResetTriggerAll( Animator animator )
	{
		if (animator != null)
		{
			foreach (AnimatorControllerParameter param in animator.parameters)
			{
				if (param.type == AnimatorControllerParameterType.Trigger) animator.ResetTrigger(param.name);
			}  
		}
	}

	static public void Animator_ParamsInit( Animator animator )
	{
		if (animator != null)
		{
			foreach (AnimatorControllerParameter param in animator.parameters)
			{
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(param.nameHash, 0f);
                        break;
                        
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(param.nameHash, 0);
                        break;
                        
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(param.nameHash, false);
                        break;
					case AnimatorControllerParameterType.Trigger:
						animator.ResetTrigger(param.nameHash);
						break;
                }
			}  
		}
	}
	
	/// <summary>
	/// 
	/// 시작위치에서 타겟위치로 이동거리만큼 이동하고 
	/// 이동후 거리를 리턴한다.
	/// 
	/// </summary>
	/// <param name="tar"></param>
	/// <param name="start"></param>
	/// <param name="moveLen"></param>
	/// <returns></returns>

	static	public	float	CheckPlane_MoveLen( Vector3 tar , Vector3 start , float moveLen , ref Vector3 s_to_t_nor )
	{
		Vector3 t_to_s = start - tar;
		Vector3 s_to_t = tar - start;
		t_to_s.Normalize();
		s_to_t.Normalize();

		s_to_t_nor = s_to_t;

		Vector3 move_pos = start + (s_to_t*moveLen);
		Plane pl = new Plane( t_to_s , tar );
		return pl.GetDistanceToPoint( move_pos );
	}

	// 수치 범위 
	// 작은 수치부터 큰수치로 찾기
	static	public	int		Find_RangeList_INT_S_TO_L(List<int> list ,int val )
	{
		for( int i = 0 ; i < list.Count ; i++ )
		{
			int _v = list[i];
			if( _v <= val )
			{
				return i;
			}
		}
		return -1;
	}

	static	public	int		Find_RangeList_FLOAT_S_TO_L(List<float> list ,float val )
	{
		for( int i = 0 ; i < list.Count ; i++ )
		{
			float _v = list[i];
			if( _v <= val )
			{
				return i;
			}
		}
		return -1;
	}

	public	static	List<object>		ListT_TO_ListObj<T>(List<T> list )
	{
		List<object> lt_o = new List<object>();
		foreach( T t in list )
		{
			lt_o.Add( t );
		}
		return lt_o;
	}


	// 자식갯수만큼 액티브 나머지는 비 활성화
	static 	public	void 	Active_Child( Transform tr , int c )
	{
		if( tr == null ) return;
		for( int i = 0 ; i < tr.childCount ; i++ )
		{
			if( i < c )
			{
				tr.GetChild(i).gameObject.SetActive(true);
			}else{
				tr.GetChild(i).gameObject.SetActive(false);
			}
		}
	}

	// 객체의 하위 갯수 
	static public	List<GameObject>	GetChildMake_Count( GameObject go_par , int count )
	{
		if( go_par.transform.childCount < 1 ) return null;
		List<GameObject> lt = new List<GameObject>();

		if( go_par.transform.childCount > count )
		{
			List<Transform> lt_del = new List<Transform>();
			for( int i = count ; i < go_par.transform.childCount ; i++ )
			{
				lt_del.Add( go_par.transform.GetChild( i ) );
			}

			foreach( Transform s in lt_del )
			{
				GameObject.DestroyImmediate( s.gameObject );
			}
		}else if( go_par.transform.childCount < count ){
			int add = count - go_par.transform.childCount;
			for( int i = 0 ; i < add ; i++ )
			{
				GameObject inst = GameObject.Instantiate( go_par.transform.GetChild( 0 ).gameObject );
				inst.transform.parent = go_par.transform;
			}
		}

		for( int i = 0 ; i < go_par.transform.childCount ; i++ )
		{
			lt.Add( go_par.transform.GetChild( i ).gameObject );
		}
		return lt;
	}

	static public	string 	Get_FileNameTime( string _name = "" , string ext = "" )
	{
		string year  =	DateTime.Now.Year.ToString();
		string month =  DateTime.Now.Month.ToString();
		string day   =  DateTime.Now.Day.ToString();
		string hour  =  DateTime.Now.Hour.ToString();
		string min   = 	DateTime.Now.Minute.ToString();
		string sec   =  DateTime.Now.Second.ToString();
		string mil_sec= DateTime.Now.Millisecond.ToString();

		string time_s = "_" + year + "-" + month + "-" + day + "-" + hour + "-" + min + "-" + sec + "-" + mil_sec;

		return _name + time_s + ext;
	}

	// 카메라 거리
	// https://docs.unity3d.com/kr/2020.3/Manual/FrustumSizeAtDistance.html

	// 일정 거리만큼 떨어진 절두체의 높이(두 값 모두 월드 단위)는 다음 공식을 통해 구할 수 있습니다.
	//  var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
	// 또한 이 과정을 반대로 하면 특정 절두체 높이일 때의 거리를 계산할 수 있습니다.
	//  var distance = frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
	// 거리와 높이를 모두 알고 있을 때에는 FOV 각도를 계산할 수도 있습니다.
	//  var camera.fieldOfView = 2.0f * Mathf.Atan(frustumHeight * 0.5f / distance) * Mathf.Rad2Deg;
	// 각 계산식의 결과를 얻으려면 절두체의 높이를 알아야 하며, 절두체의 높이는 절두체의 너비를 이용하여 쉽게 구할 수 있습니다(절두체의 높이를 이용하여 너비를 계산할 수도 있음).
	// var frustumWidth = frustumHeight * camera.aspect;
	// var frustumHeight = frustumWidth / camera.aspect;

	static	public	float Get_CameraDistanceFrustum( Camera cam , float frustumHeight )
	{
		return frustumHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
	}


	// 유니티 랜더링 객체의 구형 바운드 사이즈 구하기
	// 객체를 인자로 넣으면 랜더림 반지름 반환
	static public float Get_Radius_Bound( GameObject go )
	{
		if( go == null ) return 0;
		Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
		if( renderers.Length < 1 ) return 0;
		float max_radius = 0;
		foreach( Renderer s in renderers )
		{
			float r = s.bounds.extents.magnitude;
			if( r > max_radius ) max_radius = r;
		}
		return max_radius;
	}

	// 유니티 랜더링 객체의 총 바운드 구하기
	// 유니티  Bounds 반환
	static public Bounds Get_Bounds( GameObject go )
	{
		if( go == null ) return new Bounds(Vector3.zero , Vector3.zero );
		Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
		if( renderers.Length < 1 ) return new Bounds(Vector3.zero , Vector3.zero );
		Bounds bounds = new Bounds(Vector3.zero , Vector3.zero );
		foreach( Renderer s in renderers )
		{
			bounds.Encapsulate( s.bounds );
		}
		return bounds;
	}

	public static T CopyComponent<T>(T original, GameObject destination) where T : Component
	{
		T copy = destination.AddComponent<T>();
		System.Type type = typeof(T);
		var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

		foreach (var field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}

		return copy;
	}

	static public Component AddComponentByType(GameObject go, System.Type type)
	{
		if (go == null) return null;
		if (type == null) return null;
		Component comp = go.GetComponent(type);
		if (comp != null) return comp;
		comp = go.AddComponent(type);
		return comp;
	}


    static public GameObject GetNearObjs(GameObject go_self, System.Type type, float len, ref float min_len)
	{
		float min = float.MaxValue;
		float range_sq = len * len;

		GameObject go_near = null;
		UnityEngine.Object[] objs = GameObject.FindObjectsByType(type, FindObjectsSortMode.None);
		foreach (var s in objs)
		{
			GameObject go = s as GameObject;
			Vector3 pos_len = go.transform.position - go_self.transform.position;
			if (pos_len.sqrMagnitude < range_sq)
			{
				if (pos_len.sqrMagnitude < min)
				{
					min = pos_len.sqrMagnitude;
					go_near = go;
				}
			}
		}

		if (go_near != null)
		{
			min_len = Mathf.Sqrt(min);
		}

		return go_near;
	}

    static public bool Check_ViewSight( Transform tr_self , Vector3 pos_tar , float view_sight_Angle , float view_sight_dist = -1 , string[] layer_view_obs = null , bool debug = false )
    { 
        Vector3 dir_tar = pos_tar - tr_self.position;
		Vector3 dir_tar_normal = dir_tar;
		dir_tar_normal.Normalize();

//		Debug.Log( tr_self.forward + " : " + dir_tar_normal );

        float angle = Vector3.Angle( tr_self.forward , dir_tar_normal );
        if( angle > view_sight_Angle )
		{
			//if(debug)Debug.Log( "Check_ViewSight : angle > : " + angle  + " : "  + view_sight_Angle );
		 	return false;
		}

		if( view_sight_dist > 0 )
		{
			if( dir_tar.sqrMagnitude > (view_sight_dist*view_sight_dist) )
			{
				//if(debug)Debug.Log( "Check_ViewSight : sqrMagnitude : " + dir_tar.sqrMagnitude  + " : "  + (view_sight_dist*view_sight_dist) );
			 	return false;
			}

			if( layer_view_obs != null && layer_view_obs.Length > 0 )
			{
				// 장애물에 걸리는지
				if( SJ_Unity.RayCast_PosTarget( pos_tar , tr_self.position , view_sight_dist , LayerMask.GetMask(layer_view_obs)  ) )
				{

					//if(debug) Debug.Log( "RayCast_PosTarget : " );

					return false;
				}				
			}
		}

        // 발견
        return true;
    }

	static public bool Check_LayerName( Collider coll , string layer )
	{
		if( coll.gameObject.layer == LayerMask.NameToLayer(layer) )
		{
			return true;
		}
		return false;
	}

	static public	void ScreenCapture_CaptureScreenshot()
	{
		string filename = "ScreenShot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
		string path = Application.dataPath + "/" + filename;
		ScreenCapture.CaptureScreenshot( path , 1 );
		Debug.Log( "screen : " + path );
	}


	static public void SetUnityAction_OneFunc( Action action_base , Action act_call )
	{
		if( act_call == null ) return;

		if( action_base?.GetInvocationList().Length > 0 )
		{
			action_base = null;
		}
		action_base += act_call;
	}

	// 타겟 방향으로 바라보고 , 거리만큼 z 스케일
	// z 방향을 바라보고 있는것이 기본
	static public void LookAt_And_DistScale( Transform tr_self , Transform tr_target )
	{
		float scl_dist = Vector3.Distance( tr_self.position , tr_target.position );
		tr_self.localScale = new Vector3(1,1,scl_dist);
		tr_self.LookAt( tr_target );
	}


	/// <summary>
	/// 에디터 함수
	/// </summary>

	static	public	void	ClearDevConsole()
	{
#if UNITY_EDITOR
		var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
		var type = assembly.GetType("UnityEditor.LogEntries");
		var method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
#endif

	}


#if UNITY_EDITOR

	static	public	GameObject	Prefab_Load( string path , string name )
	{
		string localPath = "Assets/" + path + "/" + name + ".prefab";

		//Check if the Prefab and/or name already exists at the path
		GameObject obj = AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)) as GameObject;
		if( obj == null )
		{
			obj = new GameObject( name );
			obj = PrefabUtility.SaveAsPrefabAssetAndConnect( obj , localPath , InteractionMode.AutomatedAction );
		}
		else
		{
			obj =	GameObject.Instantiate( obj );
		}
		return obj;
	}

	static	public	void	Prefab_Apply( GameObject obj , bool delete_inst = false )
	{
		PrefabUtility.ApplyPrefabInstance( obj , InteractionMode.AutomatedAction );
		if( delete_inst )
		{
			GameObject.Destroy( obj );
		}
	}

	static	public	void	Prefab_Apply( MonoBehaviour mono )
	{
		PropertyModification[] prop_mod = PrefabUtility.GetPropertyModifications(mono);
		PrefabUtility.SetPropertyModifications(mono.gameObject, prop_mod);
	}
#endif



}
/// <summary>
/// 리스트 각 항목에  랜덤 확률을 넣고 
/// 그 확률만큼 나오게 하기
/// T 는 RANDOM_RANGE_STEP_BASE 를 상속 받은 항목
/// </summary>
public	class RANDOM_RANGE_STEP_BASE
{
	public	int		ratio = 100;

}
public class Random_RangeStepList
{

	static public List<int> temp_int = new List<int>();
	static public object obj_list_set;

	public static void SetList<T>(List<T> list)
	{
		obj_list_set = (object)list;
		int nAcc = 0;
		temp_int.Clear();
		foreach (T s in list)
		{
			RANDOM_RANGE_STEP_BASE b = s as RANDOM_RANGE_STEP_BASE;
			if (b.ratio == 0) continue;
			nAcc += b.ratio;
			temp_int.Add(nAcc);
		}
	}

	// 특정인덱스를 전체에 비례해서 원하는 비율로 다시 설정
	public static void SetIndexRatio<T>(int idx, float percent)
	{
		if (obj_list_set == null) return;

		List<T> list_cur = obj_list_set as List<T>;
		if (idx < 0 || idx >= list_cur.Count || list_cur.Count < 2 ) return;

		// 특정 인덱스의 값을 제외한 나머지 값들을 합산후에
		// 그 값을 기준으로 비율을 재설정
		int val_other_total = 0;
		int c = 0;
		foreach (T s in obj_list_set as List<T>)
		{
			RANDOM_RANGE_STEP_BASE b = s as RANDOM_RANGE_STEP_BASE;
			if (c == idx) continue;
			val_other_total += b.ratio;
			c++;
		}

		// 이제 val_other_total 에는 (1.0f - percent) 에 해당하는 값이 들어있다.
		// val_other_total 값에 의해 100% 에 해당하는 값을 구한다.
		// val_other_total = ? * (1.0f - percent) , 이므로 양변을 (1.0f - percent) 로 나누면
		// ? = val_other_total / (1.0f - percent)

		float val_100_per = val_other_total / (1.0f - percent);
		int val_cur_total = (int)val_100_per;

		c = 0;
		foreach (T s in obj_list_set as List<T>)
		{
			RANDOM_RANGE_STEP_BASE b = s as RANDOM_RANGE_STEP_BASE;
			if (c == idx)
			{
				b.ratio = val_cur_total;
				break;
			}
			c++;
		}
		SetList<T>( list_cur );
	}

	public static T GetRandom<T>()
	{
		int nVal = UnityEngine.Random.Range(0, temp_int[temp_int.Count - 1]);

		int sel = 0;
		for (int i = 0; i < temp_int.Count; i++)
		{
			int max_step = temp_int[i];
			int pre_step = 0;
			if (i > 0)
			{
				pre_step = temp_int[i - 1];
			}

			if (pre_step <= nVal && max_step > nVal)
			{
				sel = i;
				break;
			}

		}

		List<T> list = obj_list_set as List<T>;
		return list[sel];
	}
}


// 위의 float 버전
// 그리고 인터페이스도 좀 단순하게

public	class Random_RangeStepList_FLOAT
{
	public class _UNIT
	{
		public object obj;
		public float ratio;
	}

	static public List<_UNIT> temp_float = new List<_UNIT>();

	static public void Clear()
	{
		temp_float.Clear();
	} 

	// 순차적으로 추가
	// 반드시 높은 확률에서 낮은 확률로 입력
	// 이전 값을 자동으로 추가한다. 
	static public void AddSYNC( object obj , float ratio )
	{
		_UNIT u = new _UNIT();
		u.obj = obj;
		u.ratio = ratio;

		if( temp_float.Count > 0 )
		{
			_UNIT u_pre = temp_float[temp_float.Count-1];
			u.ratio += u_pre.ratio;
		}

		temp_float.Add(u);
	}

	static public object GetRandom()
	{
		if( temp_float.Count < 1 ) return null;

		_UNIT u_last = temp_float[temp_float.Count-1];

		float nVal = UnityEngine.Random.Range(0, u_last.ratio);

		for (int i = 0; i < temp_float.Count; i++)
		{
			float max_step = temp_float[i].ratio;
			float pre_step = 0;
			if( i > 0 )
			{
				pre_step = temp_float[i-1].ratio;
			}

			if (pre_step <= nVal && max_step > nVal)
			{
				
				return temp_float[i].obj;
			}

		}

		return u_last;
	}
}
