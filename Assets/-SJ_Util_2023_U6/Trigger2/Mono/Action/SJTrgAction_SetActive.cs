using UnityEngine;
using System.Collections;

public class SJTrgAction_SetActive : SJTrgAction_Mono
{
	public	GameObject[]	go_active;
	public	GameObject[]	go_hide;

	public	string			find_name;

	public	bool			active;

	override	public	string		OnChange_Name(){return "SetActive";}

	override	public	void	OnAction()
	{
		GameObject go =	executeObj_Name.Get();
		if(go != null)
		{
			go.SetActive(active);
		}

		foreach( GameObject g in go_hide )		g.SetActive(false);
		foreach( GameObject g in go_active )	g.SetActive(true);

		if( string.IsNullOrEmpty(find_name) == false )
		{ 
			GameObject go_find = GameObject.Find( find_name );
			if( go_find != null )
			{
				go_find.SetActive( active );
			}
			else
			{
				Debug.LogError("error!!! SJTrgAction_SetActive can't find : " + find_name );
			}
		}
	}
}
