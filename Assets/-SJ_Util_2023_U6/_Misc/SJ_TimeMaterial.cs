using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 새 재질로 변경후 
public class SJ_TimeMaterial : MonoBehaviour 
{
	[System.Serializable]
	public	class _RDS_MAT
	{
		public	Renderer			rd;
		List<Material>				list_mat_src;
		public	List<Material>		list_mat_tar;
		List<Material>				list_mat_tar_inst;

		public	void	NewInst()
		{
			if( list_mat_tar_inst.Count > 0 ) return;

			foreach( Material s in rd.materials )
			{
				list_mat_src.Add(s);
			}

			foreach( Material s in list_mat_tar )
			{
				Material inst_mat = new Material( s );
				list_mat_tar_inst.Add(inst_mat);
			}
		}

		public	void	Change( bool b )
		{
			List<Material>	list_m = list_mat_src;
			if( b ) list_m = list_mat_tar_inst;
			rd.materials = list_m.ToArray();
		}

		public	void	SetFloat( string name , float f )
		{
			foreach( Material s in rd.materials )
			{
				s.SetFloat( name , f );
			}
		}
	}

	public	List<_RDS_MAT>		list_RDS_MAT;
	public	_SJ_CurveTime		sj_cvtime;
	public	string				change_MatVal;
	public	_SJ_GO_FUNC			endFunc;

	bool play;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( play == false )	 return;

		sj_cvtime.Update();
		foreach( _RDS_MAT s in list_RDS_MAT )
		{
			s.SetFloat( change_MatVal , sj_cvtime.cv_ratio );
		}

		if( sj_cvtime.Check_End() )
		{
			play = false;
			foreach( _RDS_MAT s in list_RDS_MAT )
			{
				s.Change(false);
			}
			endFunc.Func();
		}
	}

	public	void	NewInst()
	{
		foreach( _RDS_MAT s in list_RDS_MAT )
		{
			s.NewInst();
		}
	}


	public	void	Start_Mat( bool instMat = true )
	{
		if( instMat ) NewInst();

		play = true;
		sj_cvtime.Start();
		foreach( _RDS_MAT s in list_RDS_MAT )
		{
			s.Change(true);
		}
	}

}
