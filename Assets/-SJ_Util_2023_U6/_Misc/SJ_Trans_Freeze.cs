using UnityEngine;
using System.Collections;

public class SJ_Trans_Freeze : MonoBehaviour
{
    public  bool    pos_x;
    public  bool    pos_y;
    public  bool    pos_z;
    public  bool    rot;

	public	bool	use_SetVal;
	public	Vector3 set_LocalPos;
	public	Vector3 set_LocalRot;
	public	float	set_lerp = 5.0f;
    public  void    Set()
    {
		if( use_SetVal )
		{
			//transform.localPosition = Vector3.Lerp( transform.localPosition , set_LocalPos ,set_lerp*Time.deltaTime );
			//transform.localRotation = Quaternion.Slerp( transform.localRotation , Quaternion.Euler( set_LocalRot ) ,set_lerp*Time.deltaTime );

			transform.localPosition = set_LocalPos;
			transform.localRotation =  Quaternion.Euler( set_LocalRot );
		} else { 

			Vector3 p = transform.position;
			if (pos_x) p.x = 0;
			if (pos_y) p.y = 0;
			if (pos_z) p.z = 0;
			transform.position = p;
			if (rot)
				transform.localRotation = Quaternion.identity;
		}
    }

	// Update is called once per frame
	void Update ()
	{
        Set();
    }

    private void LateUpdate()
    {
        Set();
    }

    private void FixedUpdate()
    {
        Set();
    }


}
