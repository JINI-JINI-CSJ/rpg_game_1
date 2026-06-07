using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UVTexture : MonoBehaviour 
{
	[System.Serializable]
	public	class _UVMat
	{
		public string 		tex_name = "_MainTex";

		public	Renderer	rd;
		public 	Material	mat;
		public	float		time_u;
		public  float		time_v;

		public	void		Update()
		{
			Vector2 v = new Vector2( time_u  ,time_v );

			if( rd != null && rd.material != null )	rd.material.SetTextureOffset( tex_name , v*Time.time );
			if( mat != null )						mat.SetTextureOffset( tex_name , v*Time.time );
			
		}
	}

	public	List<_UVMat>	list_UVMat;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach( _UVMat s in list_UVMat )
		{
			s.Update();
		}
	}
}
