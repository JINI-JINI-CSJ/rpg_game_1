using UnityEngine;
using System.Collections;

// 태그 반드시 지정~~

public class SJ_Singleton_Mono : MonoBehaviour
{
	public	bool	dontDestroy = true;

	virtual		public	SJ_Singleton_Mono	OnGetStatic() {return null; }
	virtual		public	void				OnSetStatic(SJ_Singleton_Mono s) {}


	void	Awake()
	{
		{
			Singleton();
		}
	}


	void	Singleton()
	{
		if( OnGetStatic() == null )
		{
			// 처음 세팅
			OnSetStatic(this);
			if( dontDestroy ) GameObject.DontDestroyOnLoad(gameObject);
			OnCreate();
		}else{
			// 이미 있다면 본인은 삭제.
			Debug.Log( "삭제 : " + gameObject.name );
			GameObject.DestroyImmediate( gameObject );
		}
	}

	virtual public void		OnCreate(){ }
}
