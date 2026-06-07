using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public	enum _SJ_JVal_Operator
{
	None , 
	Set , 
	Add , 
	Sub ,
	Mul ,
	Div ,
	User
}

public class SJTrgAction_JValue : SJTrgAction_Default_Mono
{
	public	enum _VALUE_TYPE
	{
		INT = 0,
		FLOAT , 
		STRING
	}

	public	string				str_JValName;
	public	int					int_JValName;
	public	_VALUE_TYPE			value_type;
	public	_SJ_JVal_Operator	eOperator; 
	public	bool				fwd = true;

	public	int					val_int;
	public	float				val_float;
	public	string				val_str;


	virtual	public	void 	OnSet_JValName(){}

	override	public	void	OnAction()
	{
		Operating();
	}

	public	void	Operating( SJTrgPlayer_Mono _player , string _str_valName , int _int_valName , _VALUE_TYPE _value_type , _SJ_JVal_Operator _eop , bool _fwd , object val )
	{
		value_type = _value_type;
		eOperator = _eop;
		str_JValName = _str_valName;
		int_JValName = _int_valName;

		switch(value_type)
		{
			case _VALUE_TYPE.INT:		val_int = (int)val; break;
			case _VALUE_TYPE.FLOAT:		val_float = (float)val; break;
			case _VALUE_TYPE.STRING:	val_str = (string)val; break;
		}
		Operating( _player );
	}

	public	void	Operating( SJTrgPlayer_Mono _player = null )
	{
		SJTrgPlayer_Mono player =	GetPlayerSelf();

		if( _player != null ) player = _player;

		if( player == null )
		{
			Debug.Log( "!!! Operating return : player == null " );
			return;
		}
		//Debug.Log( " Operating  : player : " + player.name );
		switch( eOperator )
		{
			case _SJ_JVal_Operator.Set:
				{
					switch( value_type )
					{
						case _VALUE_TYPE.INT:	player.JVal_Set( str_JValName , val_int , int_JValName );break;
						case _VALUE_TYPE.FLOAT: player.JVal_Set( str_JValName , val_float , int_JValName );break;
						case _VALUE_TYPE.STRING:player.JVal_Set( str_JValName , val_str , int_JValName );break;
					}
				}
				break;

			case _SJ_JVal_Operator.Add:
				{
					if( fwd )
					{
						switch( value_type )
						{
								case _VALUE_TYPE.INT:	player.JVal_Add( str_JValName , val_int , int_JValName);break;
								case _VALUE_TYPE.FLOAT: player.JVal_Add( str_JValName , val_float, int_JValName );break;
								case _VALUE_TYPE.STRING:player.JVal_Add( str_JValName , val_str, int_JValName );break;
						}
					}else{
						switch( value_type )
						{
								case _VALUE_TYPE.INT:	player.JVal_Sub( str_JValName , val_int , int_JValName);break;
								case _VALUE_TYPE.FLOAT: player.JVal_Sub( str_JValName , val_float, int_JValName );break;
								case _VALUE_TYPE.STRING:player.JVal_Sub( str_JValName , val_str, int_JValName );break;
						}
					}

				}
				break;
			case _SJ_JVal_Operator.Sub:
				{
					if( fwd )
					{
						switch( value_type )
						{
							case _VALUE_TYPE.INT:	player.JVal_Sub( str_JValName , val_int, int_JValName );break;
							case _VALUE_TYPE.FLOAT: player.JVal_Sub( str_JValName , val_float, int_JValName );break;
							case _VALUE_TYPE.STRING:player.JVal_Sub( str_JValName , val_str, int_JValName );break;
						}
					}else{
						switch( value_type )
						{
							case _VALUE_TYPE.INT:	player.JVal_Add( str_JValName , val_int , int_JValName);break;
							case _VALUE_TYPE.FLOAT: player.JVal_Add( str_JValName , val_float, int_JValName );break;
							case _VALUE_TYPE.STRING:player.JVal_Add( str_JValName , val_str, int_JValName );break;
						}
					}
				}
				break;

			case _SJ_JVal_Operator.Mul:
				{
					if( fwd )
					{
						switch( value_type )
						{
							case _VALUE_TYPE.INT:	player.JVal_Mul( str_JValName , val_int, int_JValName );break;
							case _VALUE_TYPE.FLOAT: player.JVal_Mul( str_JValName , val_float, int_JValName );break;
							case _VALUE_TYPE.STRING:player.JVal_Mul( str_JValName , val_str, int_JValName );break;
						}
					}else{
						switch( value_type )
						{
							case _VALUE_TYPE.INT:	player.JVal_Div( str_JValName , val_int, int_JValName );break;
							case _VALUE_TYPE.FLOAT: player.JVal_Div( str_JValName , val_float, int_JValName );break;
							case _VALUE_TYPE.STRING:player.JVal_Div( str_JValName , val_str, int_JValName );break;
						}
					}
				}
				break;

			case _SJ_JVal_Operator.Div:
				{
					if( fwd )
					{
						switch( value_type )
						{
							case _VALUE_TYPE.INT:	player.JVal_Div( str_JValName , val_int, int_JValName );break;
							case _VALUE_TYPE.FLOAT: player.JVal_Div( str_JValName , val_float, int_JValName );break;
							case _VALUE_TYPE.STRING:player.JVal_Div( str_JValName , val_str, int_JValName );break;
						}
					}else{
						switch( value_type )
						{
							case _VALUE_TYPE.INT:	player.JVal_Mul( str_JValName , val_int, int_JValName );break;
							case _VALUE_TYPE.FLOAT: player.JVal_Mul( str_JValName , val_float, int_JValName );break;
							case _VALUE_TYPE.STRING:player.JVal_Mul( str_JValName , val_str, int_JValName );break;
						}
					}
				}
				break;

		}
	}


}
