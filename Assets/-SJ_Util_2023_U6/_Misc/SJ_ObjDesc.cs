using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_ObjDesc : MonoBehaviour 
{
	[System.Serializable]
	public class _OBJ_DESC
	{
		public	string		Desc;
		public	GameObject	go;
	}
	public	List<_OBJ_DESC>		list_OBJ_DESC;
}
