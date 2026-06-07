using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SJ_Itween_Mono : SJBase_Move
{
//     public List<Vector3> list_pos = new List<Vector3>();

//     public iTweenPath	itween_path;
//     public	bool		startFunc;

//     public	bool		lookat_moveFront;

//     public MonoBehaviour	recv;
//     public string		func;
//     //public Transform	tr_test;
//     public float		time = 1;
//     public float		time_cur;
//     public bool			play;
//     Vector3				recent_pos;


//     void Awake()
//     {
//         //this.enabled = false;
//     }

// 	void	Check_Self()
// 	{
// 		if(	tr_self == null ) tr_self = transform;
// 	}

//     void Start()
//     {
// 		if( startFunc ) Start_Arg();
//     }

//     void OnStartInstSJ()
//     {
// 		if( startFunc )Start_Arg();
//     }

//     public void Start_Arg()
//     {
//         if (play) return;

// 		Check_Self();

//         recent_pos = tr_self.position;

//         if (itween_path != null)
//         {
//             Start_Itween(itween_path.nodes, time);
//         }
//         else
//         {
//             if (list_pos.Count > 0)
//             {
//                 List<Vector3> list_copy = new List<Vector3>(list_pos);
//                 Start_Itween(list_copy, time);
//             }
//         }
//     }


//     public void SetFunc(MonoBehaviour mono, string func)
//     {
//         this.recv = mono;
//         this.func = func;
//     }

// 	public	void	SetInf(float _time, MonoBehaviour _recv, string _func )
// 	{
// 		Check_Self();

//         time = _time;
//         time_cur = 0;
//         if (_recv != null) recv = _recv;
//         if (string.IsNullOrEmpty(_func) == false) func = _func;
// 	}

//     public void Start_Itween(List<Vector3> pos, float _time, MonoBehaviour _recv = null, string _func = "")
//     {
//         list_pos.Clear();
//         list_pos.AddRange(pos);

//         if (_time < 0.00001f)
//         {
//             Debug.LogWarning("주위!!!! Start_Itween : _time  : " + _time);
//             return;
//         }

//         if (pos.Count < 1)
//         {
//             Debug.LogWarning("주위!!!! Start_Itween : pos.Count < 1 ");
//             return;
//         }

//         this.enabled = true;
//         play = true;

//         SetInf( _time , _recv , _func );
//     }

//     public void Start_Itween( float _time, MonoBehaviour _recv = null, string _func = "" , params Vector3[] pos_list )
// 	{
//         this.enabled = true;
//         play = true;
//         list_pos.Clear();
//         list_pos.AddRange(pos_list);
//         SetInf( _time , _recv , _func );
// 	}



// 	public override Vector3? Update_BasePos()
// 	{
// 		if (play == false) return Vector3.zero;
// 		float r = time_cur / time;

// 		time_cur += Time.deltaTime;
// 		r = time_cur / time;

// 		if (r > 1.0f)
// 		{
// 			time_cur = time;
// 			r = 1.0f;
// 		}

// 		return Lerp(r);
// 	}


//     public Vector3? Lerp(float r)
//     {
// 		Vector3 cur_pos = Vector3.zero;
//         if (list_pos.Count < 3) return null;

//         cur_pos = iTween.PointOnPath(list_pos.ToArray(), r);

//         if (((int)r) == 1)
//         {
//             play = false;
//             this.enabled = false;

// 			if( noUpdate_Pos == false )
// 			{
// 				if( localPos )	tr_self.localPosition = cur_pos;
// 				else			tr_self.position = cur_pos;
// 			}
//             SJ_Unity.SendMsg(recv, func);
//         }
//         else
//         {
//             if (lookat_moveFront) tr_self.LookAt(cur_pos);
// 			if( noUpdate_Pos == false )
// 			{
// 				if(localPos)	tr_self.localPosition = cur_pos;
// 				else			tr_self.position = cur_pos;
// 			}

//         }
// 		return cur_pos;
//     }


//     public void Stop()
//     {
//         play = false;
//     }

//     public void Resum()
//     {
//         play = true;
//         this.enabled = true;
//     }


//     public void LastAdd_FirstRemove(Vector3 pos)
//     {
//         list_pos.Add(pos);
//         list_pos.RemoveAt(0);
//     }

//     public void Lerp_Last(float r)
//     {
//         int last_before = list_pos.Count - 1;

//         float unit_r = 1.0f / (float)list_pos.Count;
//         float last_r = unit_r * (float)last_before;
//         float total_r = last_r + unit_r * r;

//         if (list_pos.Count < 4) return;

// 		Vector3	pos = SJ_Cood.Spline_4Vec(list_pos[0], list_pos[1], list_pos[2], list_pos[3], total_r);

// 		if(localPos)	tr_self.localPosition = pos;
//         else			tr_self.position = pos;
//     }

// 	public static Vector3 PointOnPath(iTweenPath tween, float percent)
// 	{
// 		Vector3 v = iTween.PointOnPath( tween.nodes.ToArray() , percent );
// 		if( tween.link_Transfrom )v = tween.transform.TransformPoint( v );
// 		return v;
// 	}

}