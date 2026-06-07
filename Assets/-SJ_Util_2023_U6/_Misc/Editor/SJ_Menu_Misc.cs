using UnityEngine;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif


static	public class SJ_Menu_Misc
{

#if UNITY_EDITOR
	[MenuItem("SJMisc/Export Texture")]
	static void Export_Texture()
	{
		Texture2D texture = Selection.activeObject as Texture2D;
		if (texture == null)
		{
			EditorUtility.DisplayDialog("Select Texture", "You Must Select a Texture first!", "Ok");
			return;
		}
   
		var bytes = texture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/exported_texture.png", bytes);
	}


	static	string fileName_ScreenShot(int width, int height)
     {
        return string.Format("screen_{0}x{1}_{2}.png",
                              width, height,
                              System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
     }

	[MenuItem("SJMisc/Camera ScreenShot")]
	static void Camera_ScreenShot()
	{

		// if (Selection.activeGameObject == null)
		// {
		// 	EditorUtility.DisplayDialog("Select Camera", "카메라 선택하셈~", "Ok");
		// 	return;
		// }
		// Camera cam = Selection.activeGameObject.GetComponent<Camera>();


		Camera cam = Camera.main;

		if (cam == null)
		{
			EditorUtility.DisplayDialog("Select Camera", "카메라 선택하셈~", "Ok");
			return;
		}
		if (cam.targetTexture == null)
		{
			EditorUtility.DisplayDialog("Select Camera", "랜더 타겟 택스쳐 없다~", "Ok");
			return;
		}
		RenderTexture currentRT = RenderTexture.active;
		RenderTexture.active = cam.targetTexture;
		cam.Render();
		Texture2D imageOverview = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.ARGB32, false);
		imageOverview.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
		imageOverview.Apply();
		RenderTexture.active = currentRT;



		//Texture2D imageOverview = new Texture2D(Screen.width, Screen.height);
		//imageOverview.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		//imageOverview.Apply();


		// Encode texture into PNG
		byte[] bytes = imageOverview.EncodeToPNG();
		// save in memory
		string filename = fileName_ScreenShot(imageOverview.width, imageOverview.height);
		string path = Application.dataPath + "/" + filename;
		System.IO.File.WriteAllBytes(path, bytes);

		Debug.Log( "screen : " + path );
	}


	// 유니티 "게임" 창 띄운 상태에서 실행
	[MenuItem("SJMisc/CaptureScreenshot")]
	static public	void ScreenCapture_CaptureScreenshot()
	{
		string filename = fileName_ScreenShot(1, 1);
		string path = Application.dataPath + "/" + filename;
		ScreenCapture.CaptureScreenshot( path , 1 );
		Debug.Log( "screen : " + path );
	}

	//=============================================================================================
	// 컴포넌트 추가
	static	public	GameObject	SJ_Component_ADD(System.Type componentType) 
	{
		if( Selection.activeGameObject == null )
		{
			EditorUtility.DisplayDialog("SJ_Component", "객체 선택하셈~", "Ok");
			return null;
		}
		if( Selection.activeGameObject.GetComponent(componentType) == null )Selection.activeGameObject.AddComponent(componentType);

		return Selection.activeGameObject;
	}

	static	public	Component	SJ_Component_Get(System.Type componentType) 
	{
		if( Selection.activeGameObject == null )
		{
			EditorUtility.DisplayDialog("SJ_Component", "객체 선택하셈~", "Ok");
			return null;
		}
		Component c = Selection.activeGameObject.GetComponent(componentType);

		if( c == null )
		{
			EditorUtility.DisplayDialog("SJ_Component", "컴포넌트 없음 : " + componentType.ToString() , "Ok");
			return null;
		}
		return c;
	}


	static	public	Component[]	SJ_Component_Gets(System.Type componentType) 
	{
		List<Component>	lt = new List<Component>();
		foreach( GameObject s in Selection.gameObjects ) 
		{
			Component c = s.GetComponent(componentType);
			if( c != null )
			{
				lt.Add(c);
			}
		}

		return lt.ToArray();
	}


	[MenuItem("SJMisc/SJ_Component/SJGoPoolObj")]
	static void SJ_Component_SJGoPoolObj(){SJ_Component_ADD( typeof(SJGoPoolObj) );}

	[MenuItem("SJMisc/SJ_Component/SJTagObj_Mono")]
	static void SJ_Component_SJTagObj_Mono()
	{
		SJ_Component_ADD( typeof(SJTagObj_Mono) );
	}


	[MenuItem("SJMisc/SJ_Component/SJTagSys_Mono")]
	static void SJ_Component_SJTagSys_Mono()
	{
		GameObject go =	SJ_Component_ADD( typeof(SJTagSys_Mono) );
		SJTagSys_Mono		tag = go.GetComponent<SJTagSys_Mono>();
		SJTrgPlayer_Mono	player = go.GetComponent<SJTrgPlayer_Mono>();
		if( tag == null || player == null ) return;
		player.sjtagsys_mono = tag;
		tag.sjtrgplayer_mono = player;
	}


	[MenuItem("SJMisc/SJ_Component/SJTexColor")]
	static void SJ_Component_SJTexColor(){SJ_Component_ADD( typeof(SJTexColor) );}

	[MenuItem("SJMisc/SJ_Component/SJ_ReturnDestroy")]
	static void SJ_Component_SJ_ReturnDestroy(){SJ_Component_ADD( typeof(SJ_ReturnDestroy) );}


	[MenuItem("SJMisc/SJ_Component/자식에 액션플레이어 객체 추가")]
	static void SJ_Component_ADDObj_ActPlayer()
	{
		if( Selection.activeGameObject == null ) return;
		
		GameObject inst_go = new GameObject( "ActPlayer" );
		inst_go.AddComponent<SJTrgActionPlayer_Mono>();
		inst_go.transform.parent = Selection.activeGameObject.transform;
	}

	//=============================================================================================

	[MenuItem("SJMisc/SJGoPoolObj_OBJ_ID")]
	static void SJGoPoolObj_OBJ_ID()
	{
		//if( Selection.activeGameObject == null ) return;
		//Object go =	PrefabUtility.GetPrefabParent(Selection.activeGameObject);
		//if( go == null ) return;
		//Debug.Log( "SJGoPoolObj_OBJ_ID : " + go.ToString() );
		//Selection.activeGameObject = go as GameObject ; 


		GameObject res_SJ_Edit_Misc = Resources.Load( "SJ_Edit_Misc" ) as GameObject;
		SJ_Edit_Misc sj_edit_misc =	res_SJ_Edit_Misc.GetComponent<SJ_Edit_Misc>();
		GameObject	prf_Select = sj_edit_misc.prf_OBJ;

		if( prf_Select == null )
		{
			EditorUtility.DisplayDialog("SJGoPoolObj_OBJ_ID", "SJ_Edit_Misc 프리펩 선택하셈~", "Ok");
			return;
		}

		GameObject[] ls =	GameObject.FindObjectsOfType<GameObject>();

		int id_count=0;
		foreach( GameObject go in ls )
		{
			//Debug.Log( go.name );
			GameObject prf_par =	PrefabUtility.GetCorrespondingObjectFromSource(go) as GameObject;
			if( prf_Select == prf_par )
			{
				SJGoPoolObj sjgopoolobj = go.GetComponent<SJGoPoolObj>();
				sjgopoolobj.UID = id_count;
				id_count++;
			}
		}

		Debug.Log( prf_Select.name + " : id_count : " + id_count );
	}


	[MenuItem("SJMisc/SJ_RailObj/AddComponent")]
	static void SJ_RailObj_AddComponent()
	{
		// if( Selection.activeGameObject == null ) return;

		// if( Selection.activeGameObject.GetComponent<SJGoPoolObj>() == null )		Selection.activeGameObject.AddComponent<SJGoPoolObj>();
		// if( Selection.activeGameObject.GetComponent<iTweenPath>() == null )			Selection.activeGameObject.AddComponent<iTweenPath>();
		// if( Selection.activeGameObject.GetComponent<SJ_Itweens_Link>() == null )	Selection.activeGameObject.AddComponent<SJ_Itweens_Link>();

		// Debug.Log( Selection.activeGameObject.name + " SJ_RailObj_AddComponent " );
	}

	[MenuItem("SJMisc/SJ_RailObj/Create_ITweenRandObj")]
	static void SJ_RailObj_Create_ITweenRandObj()
	{
		// if( Selection.activeGameObject == null ) return;
		// SJ_RailObj sj =	Selection.activeGameObject.GetComponent<SJ_RailObj>();
		// if( sj == null ) return;

		// sj.Create_ITweenRandObj();

		// Debug.Log( sj.name + " Create_ITweenRandObj " );
	}

	[MenuItem("SJMisc/SJ_RailObj/FitHeight_Terrain_Itween")]
	static void SJ_RailObj_FitHeight_Terrain_Itween()
	{
		// if( Selection.activeGameObject == null ) return;
		// SJ_RailObj sj =	Selection.activeGameObject.GetComponent<SJ_RailObj>();
		// if( sj == null ) return;

		// sj.FitHeight_Terrain_Itween();

		// Debug.Log( sj.name + " FitHeight_Terrain_Itween " );
	}

	[MenuItem("SJMisc/SJTrgActionPlayer_Mono 정리")]
	static void SJTrgActionPlayer_Mono_Align()
	{
		if( Selection.activeGameObject == null ) return;
		SJTrgActionPlayer_Mono[] act_players = Selection.activeGameObject.GetComponentsInChildren<SJTrgActionPlayer_Mono>(true);
		if( act_players.Length < 1 ) return;

		foreach( SJTrgActionPlayer_Mono s in act_players )
			s.Align_ChildAction();
	}

	// [MenuItem("SJMisc/NGUI/All_Change_NGUI_Font")]
	// static void All_Change_NGUI_Font()
	// {
	// 	// if( Selection.activeGameObject == null ) return;
	// 	// if( Selection.activeObject == null ) return;
	// 	// UILabel[]	lb = Selection.activeGameObject.GetComponentsInChildren<UILabel>(true);
	// 	// Font font = Selection.activeObject as Font;
	// 	// Debug.Log( "폰트 선택 : " + font.name );
	// }

	// [MenuItem("SJMisc/NGUI/All_Change_Button_Color")]
	// static void All_Change_Button_Color()
	// {
	// 	GameObject go_Editor_SJ_NGUI_Ref =	GameObject.Find("Editor_SJ_NGUI_Ref");
	// 	if( go_Editor_SJ_NGUI_Ref == null )
	// 	{
	// 		Debug.LogError("Error!!! All_Change_Button_Color : Editor_SJ_NGUI_Ref no!!!!");
	// 		return;
	// 	}

	// 	Editor_SJ_NGUI_Ref editor_SJ_NGUI_Ref = go_Editor_SJ_NGUI_Ref.GetComponent<Editor_SJ_NGUI_Ref>();

	// 	UIButton[]	bts = Selection.activeGameObject.GetComponentsInChildren<UIButton>(true);
	// 	foreach( UIButton s in bts )	
	// 	{
	// 		s.defaultColor = s.hover = s.pressed = editor_SJ_NGUI_Ref.col_ButtonDefault;
	// 	}
	// 	Debug.Log( "All_Change_Button_Color : " + bts.Length );
	// }

	// 콜리더 삭제
	[MenuItem("SJMisc/Delete_ComponentAll_Collider")]
	static	public	void 	Delete_ComponentAll_Collider()
	{
		int c = 0;
		foreach( GameObject go in Selection.gameObjects )
		{
			Collider[] colls = go.GetComponentsInChildren<Collider>(true);
			foreach( Collider s in colls )
			{
				GameObject.DestroyImmediate( s );
				c++;
			}
			Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>(true);
			foreach( Rigidbody s in rbs )
			{
				GameObject.DestroyImmediate( s );
				c++;
			}
		}

		Debug.Log( "Delete_ComponentAll_Collider : " + c );
	}

	[MenuItem("SJMisc/RandomObj/SJ_RandomBatchDuplDel_CreateObj")]
	static	public	void 	SJ_RandomBatchDuplDel_CreateObj()
	{
		if( Selection.activeGameObject == null ) return;

		SJ_RandomBatchDuplDel sj_rb =	Selection.activeGameObject.GetComponent<SJ_RandomBatchDuplDel>();
		if( sj_rb == null )
		{
			Debug.LogError( "SJ_RandomBatchDuplDel 없음!!!" );
			return;
		}
		 
		sj_rb.CreateObj( true );
	}

	[MenuItem("SJMisc/RandomObj/SJ_RandomActive_ShowActive")]
	static	public	void 	SJ_RandomActive_ShowActive()
	{
		SJ_RandomActive c = SJ_Component_Get( typeof( SJ_RandomActive ) ) as SJ_RandomActive;
		if( c != null ) c.ShowActive( true );
	}

	[MenuItem("SJMisc/RandomObj/Random_PosOnly_BoxColl")]
	static	public	void 	Random_PosOnly_BoxColl()
	{
		SJ_RandomObjBatch c = SJ_Component_Get( typeof( SJ_RandomObjBatch ) ) as SJ_RandomObjBatch;
		if( c != null ) c.Random_PosOnly_BoxColl();
	}

	[MenuItem("SJMisc/현재 객체들의 부모 객체생성")]
	static	public	void 	Make_ParentObj()
	{
		foreach( GameObject go in Selection.gameObjects )
		{
			GameObject go_par = new GameObject( go.name );			
			go_par.transform.position = go.transform.position;
			go.transform.parent = go_par.transform;
		}
	}

	[MenuItem("SJMisc/현재 객체들의 부모 객체생성(원점위치)")]
	static	public	void 	Make_ParentObj_ZERO_POS()
	{
		foreach( GameObject go in Selection.gameObjects )
		{
			GameObject go_par = new GameObject( go.name );		
			go_par.transform.position = Vector3.zero;
			go_par.transform.rotation = Quaternion.identity;
			go_par.transform.localScale = Vector3.one;	
			go.transform.parent = go_par.transform;
		}
	}

	[MenuItem("SJMisc/하위 객체 모두 삭제")]
	static	public	void 	Delete_ChildObj()
	{
		//if( Selection.activeGameObject == null ) return;
		//SJ_Unity.Delete_Child( Selection.activeGameObject.transform );
		foreach( GameObject go in Selection.gameObjects )
		{
			SJ_Unity.Delete_Child( go.transform );
		}
	}

	[MenuItem("SJMisc/SJ_GridPos 그리드 정렬")]
	static	public	void 	SJ_GridPos_AlignChild()
	{
		// SJ_GridPos c = SJ_Component_Get( typeof( SJ_GridPos ) ) as SJ_GridPos;
		// if( c != null ) c.AlignChild();

		Component[] cs = SJ_Component_Gets( typeof( SJ_GridPos ) );

		foreach( Component s in cs )
		{
			SJ_GridPos c = s as SJ_GridPos;
			c.AlignChild();
		}
	}

	[MenuItem("SJMisc/SJ_Menu_TransWork 실행")]
	static void SJ_Menu_TransWork()
	{
        GameObject menu_TransWork = GameObject.Find( "SJ_Menu_TransWork" );
        if( menu_TransWork == null )
        {
            Debug.LogError( "없음!!! SJ_Menu_TransWork 객체" );
            return;
        }

        SJ_Menu_TransWork work = menu_TransWork.GetComponent<SJ_Menu_TransWork>();
        if( work == null )
        {
            Debug.LogError( "없음!!! SJ_Menu_TransWork 컴포넌트" );
            return;
        }

		foreach( GameObject go in Selection.gameObjects )
		{
            work.Work( go );
		}
	}

	[MenuItem("SJMisc/SJ_AllChangeFont 레거시 폰트 모두 바꾸기")]
	static void SJ_AllChangeFont()
	{
		if( Selection.activeGameObject == null ) 
		{
			Debug.LogError( "없음!!! Selection.activeGameObject 객체" );
			return;
		}

	    Text tx = Selection.activeGameObject.GetComponent<Text>();
		if( tx == null ) 
		{
			Debug.LogError( "없음!!! 기준 폰트가 있는 Text 컴포넌트" );
			return;
		}

		Font font = tx.font;

		GameObject Canvas = GameObject.Find( "Canvas" );

		Text[] tx_all = Canvas.GetComponentsInChildren<Text>(true);

		foreach( var s in tx_all )
		{
			s.font = font;
		}
	}

	[MenuItem("SJMisc/객체 랜더링 반지름")]
	static void SJ_GameObjRender_Radius()
	{
		if( Selection.activeGameObject == null ) return;
		float radius = SJ_Unity.Get_Radius_Bound( Selection.activeGameObject );

		Debug.Log( "랜더링 반지름 : " + Selection.activeGameObject.name + " : " + radius );
	}

	[MenuItem("SJMisc/메쉬 노멀 반전")]
	static void InvertSelectedMesh()
    {

		GameObject obj = Selection.activeGameObject;

        if (obj == null || obj.GetComponent<MeshFilter>() == null)
        {
            Debug.LogWarning("선택한 오브젝트에 MeshFilter가 없습니다.");
            return;
        }

        MeshFilter mf = obj.GetComponent<MeshFilter>();
        Mesh originalMesh = mf.sharedMesh;

        if (originalMesh == null)
        {
            Debug.LogWarning("MeshFilter에 메시가 없습니다.");
            return;
        }

        // 메시 복사
        Mesh newMesh = GameObject.Instantiate(originalMesh);
        newMesh.name = originalMesh.name + "_Inverted";

        // 삼각형 winding order 반전
        int[] triangles = newMesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        newMesh.triangles = triangles;

        // 노멀 반전
        Vector3[] normals = newMesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        newMesh.normals = normals;

        // 메시 저장 경로 지정
        string originalPath = AssetDatabase.GetAssetPath(originalMesh);
        string directory = Path.GetDirectoryName(originalPath);
        string newPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directory, newMesh.name + ".asset"));

        // 에셋으로 저장
        AssetDatabase.CreateAsset(newMesh, newPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 새 메시 적용
        mf.sharedMesh = newMesh;

        Debug.Log($"반전된 메시가 저장되었습니다: {newPath}");
    }

	[MenuItem("SJMisc/콜리더 오버랩 테스트")]
	static void Check_OverLapCollider()
    {
		if( Selection.activeGameObject == null )
		{
			Debug.Log( "선택 없음~~~" );
		 	return;			
		}
		
		GameObject obj = Selection.activeGameObject;

		BoxCollider 	boxCollider = obj.GetComponent<BoxCollider>();
		SphereCollider 	sphereCollider = obj.GetComponent<SphereCollider>();

		Collider[] colliders = null;
		if( boxCollider != null )
			colliders = SJ_Cood.OverLapBoxCollider( boxCollider );
		else if( sphereCollider != null )
			colliders = SJ_Cood.OverLapSphereCollider( sphereCollider );
		else
		{
			Debug.Log( "콜리더 없다~~" );
			return;
		}
		foreach( var s in colliders )
		{
			Debug.Log( "감지 : " + s.name ) ;
		}
	}

    [MenuItem("SJMisc/누락 스크립트 제거")]
    private static void RemoveAllMissingScriptComponents()
    {
        var selectedGameObjects = Selection.gameObjects;
        int totalComponentCount = 0;
        int totalGameObjectCount = 0;

        foreach (var gameObject in selectedGameObjects)
        {
            // int missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);

            // if (missingScriptCount > 0)
            // {
            //     Undo.RegisterCompleteObjectUndo(gameObject, "Remove Missing Scripts");
            //     GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);

            //     totalComponentCount += missingScriptCount;
            //     totalGameObjectCount++;
            // }
			RemoveAllMissingScriptComponents_Child( gameObject );
        }

        //Debug.Log($"Removed {totalComponentCount} missing script component(s) from {totalGameObjectCount} game object(s).");
    }

	public static void RemoveAllMissingScriptComponents_Child( GameObject go )
	{
		int missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
		if (missingScriptCount > 0)
		{
			Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");
			GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
		}
		for( int i = 0 ; i < go.transform.childCount ; i++ )
		{
			RemoveAllMissingScriptComponents_Child( go.transform.GetChild(i).gameObject );
		}
	}

	[MenuItem("SJMisc/오디오 소스 플레이")]
	public static void SoundPlay_AudioSrc()
    {
		if( Selection.activeGameObject == null )
		{
			Debug.Log( "선택 없음~~~" );
		 	return;			
		}
		GameObject obj = Selection.activeGameObject;
		AudioSource source = obj.GetComponent<AudioSource>();
		if( source == null )
        {
			Debug.Log( "AudioSource 없음~~~" );
		 	return;	
        }

		source.Play();
    }

#endif

}
