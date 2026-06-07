using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJTexColor : MonoBehaviour
{
	public	bool			changeShader;

	public	Renderer[]		rds;
	[HideInInspector]
	public	List<Shader>	list_rds_src_Shader = new List<Shader>(); // 원래 셰이더
	static	public	Shader	shader_SJTexColor;

	[System.Serializable]
	public	class	_SJTexColor_Order_Q
	{
		public	Color					col_add;
		public	Color					col_mul;

		public	void	Init( Renderer	rd , Color add , Color mul )
		{
			rd.material.SetColor( "_AColor" , add );
			rd.material.SetColor( "_MColor" , mul );
		}

		public	void	FuncOrder( Renderer	rd )
		{
			rd.material.SetColor( "_AColor" , col_add );
			rd.material.SetColor( "_MColor" , col_mul );
		}
	}
	int		idx_order = -1;
	public	List<_SJTexColor_Order_Q>	list_order_q = new List<_SJTexColor_Order_Q>();
	
	public	Color	init_Color_Add = Color.black;
	public	Color	init_Color_Mul = Color.white;

	static	public	void	Init_Load_Shader()
	{
		shader_SJTexColor = Shader.Find( "Custom/SJColAddmul_MoDf" );
		//shader_SJTexColor = Shader.Find( "Custom/SJTexColor" );
	}

	void	Awake()
	{
		if( changeShader == false )
		{ 
		}
		else { 
			list_rds_src_Shader.Clear();
			foreach( Renderer s in rds )list_rds_src_Shader.Add( s.material.shader );
		}

	}

	public	void	Init()
	{
		idx_order = -1;
		Init_Color();
	}

	public	void	Favorites_ColorAni()
	{
		list_order_q.Clear();

		_SJTexColor_Order_Q s1 = new _SJTexColor_Order_Q();
		s1.col_add = Color.white;
		s1.col_mul = Color.white;
		list_order_q.Add(s1);

		_SJTexColor_Order_Q s2 = new _SJTexColor_Order_Q();
		s2.col_add = Color.black;
		s2.col_mul = Color.black;
		list_order_q.Add(s2);

	}

	public	void	Init_Color()
	{
		int i=0;
		if( changeShader == false )
		{
			foreach( Renderer s in rds )
			{
				foreach( _SJTexColor_Order_Q os in list_order_q )
				{ 
					os.Init(s , init_Color_Add ,init_Color_Mul );
				}
			}
		}else{ 
			foreach( Renderer s in rds ) s.material.shader = list_rds_src_Shader[i++];
		}

	}

	public	void Start_Color()
	{
		idx_order = 0;
		if( changeShader )
		{
			foreach( Renderer s in rds ) s.material.shader = shader_SJTexColor;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if( list_order_q.Count == 0 )
			return;

		if( idx_order == -1 )
		{
			Init_Color();
			return;
		}
		if( idx_order >= list_order_q.Count )
		{
			Init_Color();
			return;
		}
		foreach( Renderer s in rds ) list_order_q[idx_order].FuncOrder(s);
		idx_order++;
	}

	public	void	SetColor_Add( Color col )
	{
		foreach( Renderer s in rds )s.material.SetColor( "_AColor" , col );
	}

	public	void	SetColor_Mul( Color col )
	{
		foreach( Renderer s in rds )s.material.SetColor( "_MColor" , col );
	}

}
