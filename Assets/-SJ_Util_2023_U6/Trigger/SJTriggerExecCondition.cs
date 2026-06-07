using UnityEngine;
using System.Collections;

public	enum SJ_TRG_CONDITION_Logical
{
	Normal = 0 ,
	AND = 10 ,
	OR = 20
}

public	class SJTriggerExecCondition_Mono
{
	virtual public	SJTriggerExecCondition	OnGet_SJTriggerExecCondition() {return null;}
}

public class SJTriggerExecCondition  
{
	public	SJ_TRG_CONDITION_Logical	logical;


	public	bool	Check()
	{
		if( logical == SJ_TRG_CONDITION_Logical.Normal )
		{
			return OnCheck();
		}

		SJTriggerExecCondition_Mono[] mono_list = OnGet_Child();
		if( mono_list == null )
		{
			Debug.LogError( "error!!! mono_list == null" );
			return false;
		}

		if( logical == SJ_TRG_CONDITION_Logical.OR )
		{
			foreach( SJTriggerExecCondition_Mono m in mono_list )
			{
				if( m.OnGet_SJTriggerExecCondition().OnCheck() )return true;
			}
			return false;
		}
		else if( logical == SJ_TRG_CONDITION_Logical.AND )
		{
			foreach( SJTriggerExecCondition_Mono m in mono_list )
			{
				if( m.OnGet_SJTriggerExecCondition().OnCheck() == false )return false;
			}
			return true;
		}

		return false;
	}

	virtual public	bool OnCheck() { return false; }

	virtual public	SJTriggerExecCondition_Mono[] OnGet_Child()
	{
		return null;
	}


}
