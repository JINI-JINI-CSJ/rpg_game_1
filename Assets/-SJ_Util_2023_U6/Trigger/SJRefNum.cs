using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public	enum SJOperatorType
{
	None = 0,
	Plus	, 
	Minus	,
	Mul		,
	Div		,
	Pow		,
	Log
}

[System.Serializable]
public	class SJOperator
{
	public	SJOperatorType	op_type;
	public	float	OP( float a , float b )
	{
		switch( op_type )
		{
			case SJOperatorType.None:	return a;
			case SJOperatorType.Plus:	return a+b;
			case SJOperatorType.Minus:	return a-b;
			case SJOperatorType.Mul:	return a*b;
			case SJOperatorType.Div:	return a/b;
			case SJOperatorType.Pow:	return a;
			case SJOperatorType.Log:	return a;
		}
		return 0.0f;
	}
}



public class SJRefNum : SJHierarchy
{
	float	val_src;
	float	val_last;

    public float	Value 
	{ 
		get { return val_last; } 
		set 
		{
			val_src = value;
			CalcLinked();
		} 
	}
	public static 	implicit operator float	( SJRefNum obj ){return obj.val_last;}
	public static 	implicit operator int 	( SJRefNum obj ){return (int)obj.val_last;}
	public static 	float operator +(SJRefNum a, SJRefNum b){return a.val_last + b.val_last ;}
	public static 	float operator -(SJRefNum a, SJRefNum b){return a.val_last - b.val_last ;}
	public static 	float operator *(SJRefNum a, SJRefNum b){return a.val_last * b.val_last ;}
	public static 	float operator /(SJRefNum a, SJRefNum b){return a.val_last / b.val_last ;}


	public	string name;
	// 연산자. 링크됬을때 상위 SJRefNum 에서 참조. 

	public	SJOperator	op = new SJOperator();

	public	float	oped_val; // 계산된후 원래 값과의 차이

	public	float OP( float val )
	{
		return op.OP( val , val_src );
	}

	List<SJRefNum>	linked = new List<SJRefNum>();

	public	void	AddLink( SJRefNum other )
	{
		linked.Add(other);
		CalcLinked();
	}

	public	void	RemoveLink( SJRefNum other )
	{
		linked.Remove(other);
	}

	public	void	CalcLinked()
	{
		float val = val_src;
		foreach( SJRefNum s in linked )
		{
			val = s.OP( val );
		}
		val_last = val;
	}

}
