using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_Follow_Mono : MonoBehaviour 
{
	public	float			saveTerm = 0.1f;
	public	int				maxSave = 300;

	int		ref_c = 0;

	public List<Vector3>	lt_pos = new List<Vector3>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public	void	Start_Follow()
	{
		InvokeRepeating( "AddCurPos" ,0,saveTerm );
	}

	public	void	Stop_Follow()
	{
		CancelInvoke();
	}

	public	void	AddCurPos()
	{
		lt_pos.Insert( 0, transform.position );
		if( lt_pos.Count >= maxSave )
		{
			lt_pos.RemoveAt(lt_pos.Count-1);
		}
	}

	public	Vector3		GetPos( int idx )
	{
		if( idx >= lt_pos.Count )
		{
			return lt_pos[lt_pos.Count-1];
		}
		return lt_pos[idx];
	}

}
