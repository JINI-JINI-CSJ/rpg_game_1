using UnityEngine;
using System.Collections;

/*
	트리거 
	* 지향방향
		- 간단
		- 유연
	* 트리거 플레이어
		- 트리거 모드
			- 트리거 유닛
				- 이벤트
				- 조건 
				- 실행	

			- 순차 실행

	* 조건 및 실행
		- 객체, 지정자, 게임 오브젝트
			- 변수 시스템
		- 행동 , 액션

	* 객체 관리
*/

//public	class SJTrg{}
//public	class SJTrgPlayer { }
//public	class SJTrgMode { }
//public	class SJTrgUnit	{ }
//public	class SJTrgEvent { }
//public	class SJTrgCondition { }
//public	class SJTrgAction { }

//public	class SJObjID { }
//public	class SJVar { }



public	class SJTrg
{
	static	public	SJTrg	g;

	//static	public	SJTrgPlayer		player_cur;


	virtual	public	SJTrgPlayer	OnGetPlayer(int		tag) {return null; }
	virtual	public	SJTrgPlayer	OnGetPlayer(string	tag) {return null; }

	static	public	SJTrgPlayer	GetPlayer( int		tag ) { return g.OnGetPlayer(tag); }
	static	public	SJTrgPlayer	GetPlayer( string	tag ) { return g.OnGetPlayer(tag); }
}