using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJ_DymMaterial : MonoBehaviour
{
	public	Renderer[]		meshRd_Change;
	public	bool				init_Awake;
	[HideInInspector]
	public List<Material> matList_Change = new List<Material>();
	void Awake() 
	{
		if( init_Awake )
		{
			Init();
		}
	}

	public	void	Init()
	{
		if( matList_Change.Count > 0 ) return;
		if( meshRd_Change == null || meshRd_Change.Length < 1 )
		{
			meshRd_Change = GetComponentsInChildren<Renderer>(true);
		}
		foreach( Renderer s in meshRd_Change )
		{
			List<Material> matList = new List<Material>();
			foreach( Material m in s.materials )
			{
				matList.Add( new Material(m) );
			}
			s.materials = matList.ToArray();
			matList_Change.AddRange( s.materials );
		}
	}

	public List<Material> GetMaterials()
	{
		Init();
		return matList_Change;
	}
}
