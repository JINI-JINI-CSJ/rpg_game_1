using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_GlobalArg : MonoBehaviour
{
	public	bool	global;
	static	public	SJ_GlobalArg g_global;

	public	Dictionary<string,object>	dic_str_obj = new Dictionary<string, object>();

	private void Awake()
	{
		if( global )
		{
			g_global = this;
		}
	}

	static	public	void	SetValue( string str , object obj )
	{
        //Debug.Log("SJ_GlobalArg:SetValue[" + str + "]");

		g_global.dic_str_obj[str] = obj;
	}

	static	public	object	GetValue( string str )
	{
        //Debug.Log("SJ_GlobalArg:GetValue[" + str + "] " + " c : " + g_global.dic_str_obj.Count);

		object f_obj;

        if (g_global.dic_str_obj.TryGetValue(str, out f_obj))
        {
            return f_obj;
        }
        return	null;
	}

}
