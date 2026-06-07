using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ2XColorMulty : MonoBehaviour
{
	public	Renderer		rd;
	public	Material		mat_src;
	Material				mat_sjcm;
	public	List<Color>		list_color_q;


	int		frame = 0;

	// Use this for initialization
	void Start ()
	{
		mat_src = rd.material;
		mat_sjcm = new Material( Shader.Find ("Custom/SJ2XColorMulty") );
		this.enabled = false;
	}
	

	public	void	Start_ColorQ()
	{
		rd.material = mat_sjcm;
		frame = 0;
		this.enabled = true;
	}


	// Update is called once per frame
	void Update ()
	{
		if( frame >= list_color_q.Count )
		{
			rd.material = mat_src;
			this.enabled = false;
			return;
		}
		Color col = list_color_q[frame];
		rd.material.SetColor("_Color" , col );
		frame++;
	}
}
