


using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

static public class Menu_SJTrg
{
#if UNITY_EDITOR
	[MenuItem("SJTrg/Player 객체 링크 초기화")]
	public	static void PlayerObj_Link()
	{
		foreach( GameObject go in  Selection.gameObjects )
		{
			//GameObject go = Selection.activeGameObject;
			//if( go == null ) return;
			SJTrgPlayer_Mono player_mono = go.GetComponent<SJTrgPlayer_Mono>();
			if (player_mono != null)
			{
				player_mono.Init_Editor();
				Debug.Log("SJTrgPlayer_Mono 초기화 : " + player_mono.name);
				//return;
			}

			SJTrgActionPlayer_Mono actPlayer =go.GetComponent<SJTrgActionPlayer_Mono>();
			if( actPlayer != null )
			{
				Debug.Log("SJTrgActionPlayer_Mono 초기화 : " + actPlayer.name);
				actPlayer.Init();
			}
		}
	}


	static	GameObject Create_SJTrgActionPlayer()
	{
		GameObject go = new GameObject("ActPlayer");
		go.AddComponent<SJTrgActionPlayer_Mono>();
		return go;
	}

	[MenuItem("SJTrg/생성 Root SJTrgActionPlayer")]
	static void Create_SJTrgActionPlayer_Root()
	{
		Create_SJTrgActionPlayer();
	}

	[MenuItem("SJTrg/생성 Child SJTrgActionPlayer")]
	static void Create_SJTrgActionPlayer_Child()
	{
		GameObject sel_obj = Selection.activeObject as GameObject;
		GameObject go =	Create_SJTrgActionPlayer();
		if( sel_obj != null )
		{
			go.transform.parent = sel_obj.transform;
		}
	}



	static	GameObject Create_SJTrgAction()
	{
		GameObject go = new GameObject("act");
		go.AddComponent<SJTrgAction_Mono>();
		return go;
	}

	[MenuItem("SJTrg/생성 Root SJTrgAction")]
	static void Create_SJTrgAction_Root()
	{
		Create_SJTrgAction();
	}

	[MenuItem("SJTrg/생성 Child SJTrgAction")]
	static void Create_SJTrgAction_Child()
	{
		GameObject sel_obj = Selection.activeObject as GameObject;
		GameObject go =	Create_SJTrgAction();
		if( sel_obj != null )
		{
			go.transform.parent = sel_obj.transform;
		}
	}
#endif
}
