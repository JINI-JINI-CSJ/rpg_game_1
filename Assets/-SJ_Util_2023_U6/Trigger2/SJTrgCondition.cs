using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SJTrgCondition
{
	public	List<SJTrgCondition>	lt_condition = new List<SJTrgCondition>();
	public	bool	Child_OR_True = false;


	public	bool	Check()
	{
		if( lt_condition.Count > 0 )
		{
			if( Child_OR_True )
			{
				foreach( SJTrgCondition c in lt_condition )
				{
					if( c.OnCheck() )return true;
				}
				return false;
			}
			else
			{
				foreach( SJTrgCondition c in lt_condition )
				{
					if( c.OnCheck() == false )return false;
				}
				return true;
			}
		}
		else
		{
			if( OnCheck() )return true;
		}

		return false;
	}

	virtual	public	bool	OnCheck()
	{
		return false;
	}
}


