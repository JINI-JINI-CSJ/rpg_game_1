using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/*
 *  전역에서 클래스의 함수를 문자열로 호출하기
 *  인자 : 클래스 , 함수이름 , 인자....
 *  
 * 
 * 
 */

public class SJ_GlobalCall : MonoBehaviour
{
    static public SJ_GlobalCall g;
    public List<MonoBehaviour>         lt_Mono;
    public Dictionary<string,Type>     dic_Mono;
    
    private void Awake() 
    {
        g = this;
        foreach( MonoBehaviour mono in lt_Mono )
        {
            Type type = mono.GetType();
            dic_Mono.Add( type.Name , type );
        }
    }

	static	public void Call_Func(string class_name , string func , params string[] args )
    {
		g._Call_Func(class_name, func, args);

	}

	public void _Call_Func(string class_name, string func, params string[] args)
	{
		Type type = null;
		if (dic_Mono.TryGetValue(class_name , out type) )
		{
			MethodInfo func_inf = type.GetMethod(func, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public , null , new Type[] { typeof(string[]) } , null); 
			if(func_inf != null)
			{
				func_inf.Invoke(type, args);
			}
			else
			{
				Debug.LogError("Error!!! SJ_GlobalCall : Call_Func : " + class_name + " : " + " func ");
			}
			return;
		}
		OnCall_Func(class_name, func, args);
	}

	virtual	public	void OnCall_Func(string class_name, string func, params string[] args){}
}
