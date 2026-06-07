using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SJTriggerExec : MonoBehaviour
{
	public		bool				add_DuplableAble;

	public		SJTriggerPlayer		sjTriggerPlayer;


	virtual public	SJTriggerExecSelPlayer	OnGet_SJTriggerExecSelPlayer()	{return null; }
	virtual public	SJTriggerExecCondition	OnGet_SJTriggerExecCondition()	{return null; }
	virtual public	SJTriggerExecAction		OnGet_SJTriggerExecAction()		{return null; }


	public	void Play( SJTriggerPlayer		_sjTriggerPlayer )
	{
		sjTriggerPlayer = _sjTriggerPlayer;
		SJTriggerExecSelPlayer	sel			= OnGet_SJTriggerExecSelPlayer();
		SJTriggerExecCondition	condition	= OnGet_SJTriggerExecCondition();
		SJTriggerExecAction		act			= OnGet_SJTriggerExecAction();

		if( sel != null )		sel.OnSelect();
		if( condition != null )
		{ 
			if(	condition.Check() == false )
			{
				return;
			}
		}
		if( act != null )		act.Action( null , this );
	}

	virtual		public	void		GetEventList( List<int> event_list ){}
	virtual		public	void		OnAdd(){}
	virtual		public	void		OnRemove(){}
	virtual		public	void		OnDuplicated(){}

	virtual		public	int			OnTriggerMessage_Play			( int event_id , object		arg_obj = null  ,int val_int=0 , float val_float=0, string val_str=null ){return 0;}
	virtual		public	bool		OnTriggerMessage_Quary_Bool		( int event_id , ref bool	ref_bool	,object arg_obj=null , int val_int=0 , float val_float=0, string val_str=null ){return false;}
	virtual		public	bool 		OnTriggerMessage_Quary_Int		( int event_id , ref int	ref_int		,object arg_obj=null , int val_int=0 , float val_float=0, string val_str=null ){return false;}
	virtual		public	bool		OnTriggerMessage_Quary_Float	( int event_id , ref float	ref_float	,object arg_obj=null , int val_int=0 , float val_float=0, string val_str=null ){return false;}
	virtual		public	bool		OnTriggerMessage_Quary_String	( int event_id , ref string	ref_str		,object arg_obj=null , int val_int=0 , float val_float=0, string val_str=null ){return false;}
}
