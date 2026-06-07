using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;


public enum SJ_VALUE_TYPE { INT = 0 , FLOAT = 10 , STRING = 20  }
public enum SJ_VALUE_ACT_TYPE { None = 0, Set , Add , Sub , Mul , Div }

public class SJTrgAction_PlayerValue : SJTrgAction_Mono
{


	public	class _VALUE_ACT
	{
		public	SJ_VALUE_ACT_TYPE	actType;
		public	SJ_VALUE_TYPE		valType;
		public	string				name_val;
		public	int					val_int;
		public	float				val_float;
		public	string				val_str;

		public	void	In( SJTrgPlayer_Mono	player )
		{
			switch( actType )
			{
				case SJ_VALUE_ACT_TYPE.Set:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Set(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Set(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Set(name_val,val_str);	break;
						}
					}
					break;

				case SJ_VALUE_ACT_TYPE.Add:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Add(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Add(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Add(name_val,val_str);	break;
						}
					}
					break;

				case SJ_VALUE_ACT_TYPE.Sub:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Sub(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Sub(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Sub(name_val,val_str);	break;
						}
					}
					break;

				case SJ_VALUE_ACT_TYPE.Mul:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Mul(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Mul(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Mul(name_val,val_str);	break;
						}
					}
					break;

				case SJ_VALUE_ACT_TYPE.Div:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Div(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Div(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Div(name_val,val_str);	break;
						}
					}
					break;
			}
		}

		public	void	Out( SJTrgPlayer_Mono	player )
		{
			switch( actType )
			{
				// 반대로 액션..

				//case SJ_VALUE_ACT_TYPE.Set:
				//	{
				//		switch( valType )
				//		{
				//			case SJ_VALUE_TYPE.INT:		player.JVal_Set(name_val,val_int);	break;
				//			case SJ_VALUE_TYPE.FLOAT:	player.JVal_Set(name_val,val_float);break;
				//			case SJ_VALUE_TYPE.STRING:	player.JVal_Set(name_val,val_str);	break;
				//		}
				//	}
				//	break;

				case SJ_VALUE_ACT_TYPE.Add:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Sub(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Sub(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Sub(name_val,val_str);	break;
						}
					}
					break;

				case SJ_VALUE_ACT_TYPE.Sub:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Add(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Add(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Add(name_val,val_str);	break;
						}
					}
					break;

				case SJ_VALUE_ACT_TYPE.Mul:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Div(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Div(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Div(name_val,val_str);	break;
						}
					}
					break;

				case SJ_VALUE_ACT_TYPE.Div:
					{
						switch( valType )
						{
							case SJ_VALUE_TYPE.INT:		player.JVal_Mul(name_val,val_int);	break;
							case SJ_VALUE_TYPE.FLOAT:	player.JVal_Mul(name_val,val_float);break;
							case SJ_VALUE_TYPE.STRING:	player.JVal_Mul(name_val,val_str);	break;
						}
					}
					break;
			}
		}

	}

	public	bool	addEvent;
	public	bool	removeEvent;

	public	List<_VALUE_ACT>	list_valAct = new List<_VALUE_ACT>();


	override	public	void	OnAction()
	{
		if( addEvent == false ) ValueAction( true );
	}
	

	override	public	void	OnAdd()
	{
		if( addEvent ) ValueAction( true );
	}


	override	public	void	OnRemove()
	{
		if( removeEvent ) ValueAction( false );
	}


	public	void	ValueAction( bool in_play )
	{
		foreach( _VALUE_ACT s in list_valAct )
		{
			if( in_play )	s.In( self_Player );
			else			s.Out( self_Player );
		}
	}
}
