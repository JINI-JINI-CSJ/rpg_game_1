using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SJ_ObjActive : MonoBehaviour
{
	[System.Serializable]
	public	class _OBJ_PART
	{
		public	string	Name;
		public	List<GameObject>	list_obj = new List<GameObject>();
		public	List<MonoBehaviour>	list_mono = new List<MonoBehaviour>();

		public	void	Active( bool b )
		{
			foreach( GameObject s in list_obj ) s.SetActive(b);
			foreach( MonoBehaviour s in list_mono ) s.enabled = b;
		}
	}
	public	List<_OBJ_PART>		list_OBJ_PART = new List<_OBJ_PART>();

	public	void	SetActive_Part(bool b , string Name = "" )
	{
		foreach( _OBJ_PART s in list_OBJ_PART )
		{
			if( string.IsNullOrEmpty( Name ) == false ) 
			{
				if( s.Name == Name )
				{
					s.Active(b);
					break;
				}
			}
			else
			{
				s.Active(b);
			}
		}
	}

	public	void	SetActive_Part_On() { SetActive_Part( true ); }
	public	void	SetActive_Part_Off(){ SetActive_Part( false ); }
}
