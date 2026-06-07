using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public	enum _XYZ
{
	NONE = 0 ,
	X , Y , Z
}

public class SJ_Cood
{
    static public void RectCenterSet(ref Rect rc, float cen_x, float cen_y, float w, float h)
    {
        rc.Set(cen_x - w / 2, cen_y - h / 2, w, h);
    }

    static public void Rect_Over_NGUICood(Rect rc_l, Rect rc_s, ref float over_left, ref float over_top, ref float over_right, ref float over_bottom)
    {
        over_left = rc_l.xMin - rc_s.xMin;
        over_right = rc_s.xMax - rc_l.xMax;

        over_bottom = rc_l.yMin - rc_s.yMin;
        over_top = rc_s.yMax - rc_l.yMax;
    }

	public	static	Vector3 FitIn_BoxBound(BoxCollider bc , Vector3 pos_w , bool noTransWorld = false )
	{
		Vector3 pos_l = pos_w;

		if( noTransWorld == false )
			pos_l = bc.transform.InverseTransformPoint( pos_w );

		Vector3 s = bc.size;

		if( -s.x > pos_l.x ) pos_l.x = -s.x;
		if(  s.x < pos_l.x ) pos_l.x =  s.x;
		if( -s.y > pos_l.y ) pos_l.y = -s.y;
		if(  s.y < pos_l.y ) pos_l.y =  s.y;
		if( -s.z > pos_l.z ) pos_l.z = -s.z;
		if(  s.z < pos_l.z ) pos_l.z =  s.z;

		if( noTransWorld == false )
			pos_l =	bc.transform.TransformPoint( pos_l );
		return pos_l;
	}

	public	static	float	SphereColl_Radius( SphereCollider sp )
	{
		return sp.transform.lossyScale.x * sp.radius;
	}

	public static Vector3 Random_BoxBound(BoxCollider bc , bool local_pos = false , bool only_bound = false )
	{
		Vector3 s = bc.size * 0.5f;
		s.x = s.x * bc.transform.lossyScale.x;
		s.y = s.y * bc.transform.lossyScale.y;
		s.z = s.z * bc.transform.lossyScale.z;

		//Debug.Log("bc.size : " + bc.size);
		Vector3 v = Random_BoxBound(s);

		if( only_bound )
		{
			return v;
		}

		if( local_pos == false )
		{
			v = bc.transform.rotation * v;
			v += bc.transform.position;
		}else{
			v = bc.transform.localRotation * v;
			v += bc.transform.localPosition;
		}
		//Debug.Log("Random_BoxBound : " + v);
		return v;
	}




	public	static	Vector3	Random_BoxBound(Vector3 v)
	{
		float rx = UnityEngine.Random.Range(-v.x, v.x);
		float ry = UnityEngine.Random.Range(-v.y, v.y);
		float rz = UnityEngine.Random.Range(-v.z, v.z);
		Vector3 vr = new Vector3(rx, ry, rz);
		return vr;
	}

	public static Vector3 Random_ScaleBound(Transform tr)
	{
		float rx = UnityEngine.Random.Range(-tr.localScale.x * 0.5f , tr.localScale.x* 0.5f);
		float ry = UnityEngine.Random.Range(-tr.localScale.y * 0.5f , tr.localScale.y* 0.5f);
		float rz = UnityEngine.Random.Range(-tr.localScale.z * 0.5f , tr.localScale.z* 0.5f);

		Vector3 v = new Vector3(rx, ry, rz);
		v += tr.position;
		return v;
	}



	public static Vector3 Random_SphereBound(float radius , float min = 0)
	{
		float len = UnityEngine.Random.Range( min , radius );

		float x_ang = UnityEngine.Random.Range( 0.0f , 360.0f );
		float y_ang = UnityEngine.Random.Range( 0.0f , 360.0f );
		Vector3 v = new Vector3(0,0,len);
		v = Quaternion.Euler( x_ang , y_ang , 0.0f ) * v;
		return v;
	}

	public	static	float	SphereBound_Radius(SphereCollider sp)
	{
		return sp.radius * sp.transform.localScale.x;
	}



	public static Vector3 Random_SphereBound( SphereCollider sp , float min = 0)
	{
		Vector3 pos = Random_SphereBound(sp.radius * sp.transform.localScale.x , min );
		return	pos + sp.transform.position;
	}

	public static Collider[] OverLapBoxCollider(BoxCollider bc, string layer_name = "")
	{
		List<string> layers = new List<string>();
		if (string.IsNullOrEmpty(layer_name) == false)
		{
			layers.Add(layer_name);
		}
		return OverLapBoxCollider( bc , layers );
	}

	public static Collider[] OverLapBoxCollider(BoxCollider bc, List<string> layers )
	{
		Vector3 center = bc.transform.TransformPoint(bc.center);
		Vector3 half_size = bc.size * 0.5f;
		half_size.x = half_size.x * bc.transform.lossyScale.x;
		half_size.y = half_size.y * bc.transform.lossyScale.y;
		half_size.z = half_size.z * bc.transform.lossyScale.z;
		Collider[] colls = null;
		if ( layers != null && layers.Count > 0 )
		{
			int layer_num = LayerMask.GetMask(layers.ToArray());
			colls = Physics.OverlapBox(center, half_size, bc.transform.rotation, layer_num);
		}
		else
		{
			colls = Physics.OverlapBox(center, half_size, bc.transform.rotation);
		}
		HashSet<Collider> hs = new HashSet<Collider>(colls);
		hs.Remove(bc);
		List<Collider> coll_selfs = new List<Collider>();
		foreach (var s in hs)
		{
			if (FindEqOBjParent(bc.transform, s.transform))coll_selfs.Add(s);
		}
		foreach (var s in coll_selfs)hs.Remove(s);
		return hs.ToArray();
	}


	public static Collider[] OverLapBoxCollider( Vector3 pos , Vector3 half_size , Quaternion rot , List<string> layers )
	{
		Collider[] colls = null;
		if ( layers != null && layers.Count > 0 )
		{
			int layer_num = LayerMask.GetMask(layers.ToArray());
			colls = Physics.OverlapBox(pos, half_size, rot, layer_num);
		}
		else
		{
			colls = Physics.OverlapBox(pos, half_size, rot);
		}
		return colls;
	}


	public static Collider[] OverLapSphereCollider(SphereCollider sp, string layer_name = "")
	{
		List<string> layers = new List<string>();
		if (string.IsNullOrEmpty(layer_name) == false)
		{
			layers.Add(layer_name);
		}
		return OverLapSphereCollider( sp , layers );
	}

	public static Collider[] OverLapSphereCollider(SphereCollider sp, List<string> layers)
	{
		float radius = SphereBound_Radius(sp);
		Collider[] colls = null;
		if (layers != null && layers.Count > 0)
		{
			int layer_mask = LayerMask.GetMask(layers.ToArray());
			colls = Physics.OverlapSphere(sp.transform.position, radius, layer_mask);
		}
		else
		{
			colls = Physics.OverlapSphere(sp.transform.position, radius);
		}
		HashSet<Collider> hs = new HashSet<Collider>(colls);
		hs.Remove(sp);
		List<Collider> coll_selfs = new List<Collider>();
		foreach (var s in hs)
		{
			if (FindEqOBjParent(sp.transform, s.transform))coll_selfs.Add(s);
		}
		foreach (var s in coll_selfs)hs.Remove(s);
		return hs.ToArray();
	}

	
	public static Collider[] OverLapSphereCollider(float radius , Vector3 pos , List<string> layers)
	{
		Collider[] colls = null;
		if (layers != null && layers.Count > 0)
		{
			int layer_mask = LayerMask.GetMask(layers.ToArray());
			colls = Physics.OverlapSphere(pos, radius, layer_mask);
		}
		else
		{
			colls = Physics.OverlapSphere(pos, radius);
		}
		return colls;
	}


	// tr_Find 위로 tr_Parent 가 있는지
	static public bool FindEqOBjParent(Transform tr_Parent, Transform tr_Find)
	{
		Transform tr_work = tr_Find;
		while (true)
		{
			if (tr_work == null) return false;
			if (tr_work == tr_Parent) return true;
			tr_work = tr_work.parent;
		}
		return false;
	}


	public static bool OverlapBOOL(GameObject go, List<string> layers , bool debug_log = false )
	{
		SphereCollider sp_coll_self = go.GetComponent<SphereCollider>();
		BoxCollider box_coll_self = go.GetComponent<BoxCollider>();
		Collider[] over_coll = null;

		go.SetActive(false);
		if (sp_coll_self != null && sp_coll_self.enabled == true )
		{
			over_coll = SJ_Cood.OverLapSphereCollider(sp_coll_self, layers);
		}
		else if (box_coll_self != null && box_coll_self.enabled == true )
		{
			over_coll = SJ_Cood.OverLapBoxCollider(box_coll_self, layers);
		}
		else
		{
			Debug.Log("체크할 박스나 구 콜리더 없다. ~~ : " + go.name);
			go.SetActive(true);
			return false;
		}

		go.SetActive(true);
		if (over_coll.Length > 0)
		{
			if( debug_log )
				foreach (var s in over_coll) Debug.Log("OverlapBOOL : " + s.gameObject.name);
			return true;
		}
		return false;
	}

	public static bool RayCastLine(Vector3 origin, Vector3 dir, out RaycastHit hit, string layer = "", float max_dist = 1000)
	{
		if (string.IsNullOrEmpty(layer))
		{
			List<string> layers = new List<string>();
			layers.Add(layer);
			return RayCastLineLayers(origin, dir, out hit, layers, max_dist);
		}
		else
		{
			return RayCastLineLayers(origin, dir, out hit);
		}
	}

	public static bool RayCastLineLayers(Vector3 origin, Vector3 dir, out RaycastHit hit, List<string> layers = null, float max_dist = 1000)
	{
		//RaycastHit[] hits = null;
		
		dir.Normalize();
		if (layers != null && layers.Count > 0)
		{
			int layer_mask = LayerMask.GetMask(layers.ToArray());
			return Physics.Raycast(origin, dir, out hit , max_dist , layer_mask);
		}
		else
		{
			return Physics.Raycast(origin, dir, out hit , max_dist);
		}
	}

	public static RaycastHit FindNear_RaycastHits(RaycastHit[] hits)
	{
		RaycastHit hit_near = new RaycastHit();
		hit_near.distance = float.MaxValue;
		foreach (var s in hits)
		{
			if (s.distance < hit_near.distance)
			{
				hit_near = s;
			}
		}
		return hit_near;
	}

	public static Vector3 Random_UpVector(float ang_rand, Quaternion? rot = null, bool front = false)
	{
		Vector3 vDir = Vector3.zero;

		if (front == false)
		{
			float x = UnityEngine.Random.Range(0.0f, ang_rand);
			float y = UnityEngine.Random.Range(0.0f, 360.0f);
			vDir = Quaternion.Euler(x, 0, 0) * Vector3.up;
			vDir = Quaternion.Euler(0, y, 0) * vDir;
		}
		else
		{
			float y = UnityEngine.Random.Range(0.0f, ang_rand);
			float z = UnityEngine.Random.Range(0.0f, 360.0f);
			vDir = Quaternion.Euler(0, y, 0) * Vector3.forward;
			vDir = Quaternion.Euler(0, 0, z) * vDir;
		}

		if (rot != null)
		{
			vDir = rot.Value * vDir;
		}

		return vDir;
	}


	// 목표위치만큼 Z 스케일 
	public	static	void	Z_Sclae_TartgetPos( Transform tr_obj ,  Vector3 pos_start , Vector3 pos_target )
	{
		Vector3 pos_len = pos_target - pos_start;
		float	len = pos_len.magnitude;

		tr_obj.position = pos_start;

		Vector3 scl = tr_obj.localScale;
		scl.z = len;
		tr_obj.localScale = scl;
		tr_obj.LookAt( pos_target );
	}


	public	static float	GetAngle(Vector3 src_dir , Vector3 tar_dir , _XYZ axis = _XYZ.Y )
	{
		float tar_ang =	Vector3.Angle( src_dir , tar_dir );
		Vector3 crs =	Vector3.Cross( src_dir , tar_dir );

		//Debug.Log("GetAngle : " + src_dir + "   : " + tar_dir + " : " + tar_ang );

		switch( axis )
		{
			case _XYZ.Y:
			if( crs.y < 0 ) tar_ang *= -1;
			break;

			case _XYZ.Z:
			if( crs.z < 0 ) tar_ang *= -1;
			break;
		}
		//Debug.Log("GetAngle : " + src_dir + "   : " + tar_dir + " : " + tar_ang );
		return tar_ang;
	}

	public static float GetAngle( float x , float y )
	{
	   return	Mathf.Atan2(x, y) * Mathf.Rad2Deg;
	}

	public	static	void	RotationTrans_Ang( Transform tr , Vector3 tar_dir , float ang , float time , _XYZ axis = _XYZ.Y , bool all_ang = false , bool dir_src_trans = false )
	{
		Vector3 dir_src = Vector3.forward;
		switch( axis )
		{
			case _XYZ.Y:dir_src = Vector3.forward;	break;
			case _XYZ.Z:dir_src = Vector3.up;		break;
		}

		if( dir_src_trans )
		{
			dir_src = tr.rotation * dir_src;
		}

		float tar_ang =	GetAngle( dir_src , tar_dir , axis );

		//Debug.Log( "RotationTrans_Ang : tar_ang : " + tar_ang );

		if(Mathf.Abs( tar_ang ) < 0.01f )
		{
			//Debug.Log( "return tar_ang < 0.01f" );
			return;
		}

		//Debug.Log( "ang : " + ang );
		float ang_cur = ang * time;

		if( all_ang )
		{
			ang_cur = tar_ang;
		}else {
			if( tar_ang < 0.0f ) ang_cur *= -1f;
			if( Mathf.Abs( tar_ang ) < Mathf.Abs( ang_cur ) ) ang_cur = tar_ang;
		}

		switch( axis )
		{
			case _XYZ.Y:
			tr.Rotate(0,ang_cur,0,Space.World);
				break;
			case _XYZ.Z:
			tr.Rotate(0,0,ang_cur,Space.World);
				break;

		}
	}

	static	public	Vector2		Rect_Clamp( Rect rc , Vector2 v )
	{
		v.x =	Mathf.Clamp( v.x , rc.xMin , rc.xMax );
		v.y =	Mathf.Clamp( v.y , rc.yMin , rc.yMax );


		return v;
	}

	static	public	bool	Rect_Contain(Rect rc , Vector2 v)
	{
		if( rc.xMin > v.x || rc.xMax < v.x ) return false;
		if( rc.yMin > v.y || rc.yMax < v.y ) return false;
		return true;
	}


    static  public  Vector3     Spline_4Vec(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 ret = 0.5f * ((2.0f * p1) +
        (-p0 + p2) * t +
        (2.0f * p0 - 5.0f * p1 + 4 * p2 - p3) * t2 +
        (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);

        return ret;
    }

	static	public	float		Lerp_Float( float s , float e , float lerp )
	{
		return s + (e - s) * lerp;
	}

	static	public	int			Random_PlusMinus()
	{
		int i = UnityEngine.Random.Range(0,2);
		if(i==0)return -1;
		return 1;
	}

	static 	public	bool 		EqFloat( float a , float b , float len = 0.0001f )
	{
		float f = Mathf.Abs( a-b );
		if( f < len ) return true;
		return false;
	}


	// Bound

	static public	Bounds		GetAllBounds( GameObject go )
	{
		Bounds bd = InitBounds();
		Renderer[] rds = go.GetComponentsInChildren<Renderer>();
		foreach( Renderer s in rds )
		{
			AddBounds( ref bd , s.bounds );
		}
		return bd;
	}

	static public	Bounds 		InitBounds()
	{
		Bounds bd = new Bounds();
		bd.SetMinMax( new Vector3( 100000,100000,10000 ) , new Vector3( -100000 , -100000 , -10000 ) );
		return bd;
	}

	static public	void 		AddBounds( ref Bounds bd , Bounds add )
	{
		Vector3 min = bd.min;
		Vector3 max = bd.max;
		if( min.x > add.min.x ) min.x = add.min.x;
		if( min.y > add.min.y ) min.y = add.min.y;
		if( min.z > add.min.z ) min.z = add.min.z;

		if( max.x < add.max.x ) max.x = add.max.x;
		if( max.y < add.max.y ) max.y = add.max.y;
		if( max.z < add.max.z ) max.z = add.max.z;

		bd.SetMinMax( min , max );
	}

	static public 	void 	AddBounds( ref Bounds bd , Vector3 p )
	{
		Vector3 min = bd.min;
		Vector3 max = bd.max;
		if( min.x > p.x ) min.x = p.x;
		if( min.y > p.y ) min.y = p.y;
		if( min.z > p.z ) min.z = p.z;

		if( max.x < p.x ) max.x = p.x;
		if( max.y < p.y ) max.y = p.y;
		if( max.z < p.z ) max.z = p.z;

		bd.SetMinMax( min , max );
	}

	// 전체 스크린에서 객체 사각형 안쪽으로 가야 하는 거리 계산
	static public Vector2Int Fit_Screen_RECT( Rect rc_scr , Rect rc_obj )
	{
		Vector2Int v = new Vector2Int();
		if( rc_obj.xMin < rc_scr.xMin  )v.x = (int)rc_scr.xMin - (int)rc_obj.xMin;
		if( rc_obj.xMax > rc_scr.xMax  )v.x = (int)rc_scr.xMax - (int)rc_obj.xMax;
		if( rc_obj.yMin < rc_scr.yMin  )v.y = (int)rc_scr.yMin - (int)rc_obj.yMin;
		if( rc_obj.yMax > rc_scr.yMax  )v.y = (int)rc_scr.yMax - (int)rc_obj.yMax;
		return v;
	}	


	
}
