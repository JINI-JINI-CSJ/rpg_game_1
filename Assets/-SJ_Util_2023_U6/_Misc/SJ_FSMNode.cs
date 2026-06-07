using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상태머신기계 , 기존 액션 트리거를 단순화 및 추가기능
/// 
/// 상태이름
/// 
/// 순차실행 
/// 동시실행은 한객체에 컴포넌트추가한다.
/// 자식실행은 현재노드에서 자식으로 실행하다가 끝나면 다시 원래노드로 돌아온다
/// 
/// 
/// </summary>

public class SJ_FSMNode : MonoBehaviour
{
	public	MonoBehaviour		player;
	public	SJ_FSMNode			root;

	public int	index_node;

	public	SJ_FSMNode			par;
	public	List<SJ_FSMNode>	lt_child;

	// 이름정렬 , 중복되면 등록안댐. 이름으로 찾을꺼면 상태이름 같은 계층의 객체끼 유니크한 이름하게.
	public	Dictionary<string , SJ_FSMNode>	dic_child = new Dictionary<string, SJ_FSMNode>();
	public	List<SJ_FSMAction>	lt_act;

	public	HashSet<SJ_FSMNode>		hs_curExec = new HashSet<SJ_FSMNode>();
	public	HashSet<SJ_FSMAction>	hs_curAction = new HashSet<SJ_FSMAction>();


	public	bool	debug;

	public	List<SJ_FSMNode>	lt_node_exec_log;

	public	enum	_FUNC_EXEC_TYPE
	{ 
		None = 0,
		OneShot ,
		OneShot_Continue ,
	}

	public	_FUNC_EXEC_TYPE	func_exec_type;

	virtual	public	void OnEditor_Init() { }
	virtual	public	void OnExecStart() { }
	virtual	public	void OnExecPause() { }
	//virtual	public	void OnExecResume() { }
	virtual	public	void OnEndState() { }

	public	void	Editor_Init( SJ_FSMNode _par = null , int _idx = 0 )
	{
		par = _par;
		if( par == null )
			root = this;
		index_node = _idx;
		lt_child.Clear();
		//dic_child.Clear();
		lt_act.Clear();

		lt_act.AddRange( GetComponents<SJ_FSMAction>() );
		foreach( SJ_FSMAction s in lt_act )s.fsmNode = this;
		
		SJ_FSMNode[]	chs = GetComponentsInChildren<SJ_FSMNode>();

		int c = 0;
		foreach( SJ_FSMNode s in chs )
		{
			if( s == this ) continue;

			lt_child.Add( s );
			//dic_child[s.name] = s;
			s.player = player;
			s.root = root;
			s.Editor_Init(  this , c );
			c++;
		}
		OnEditor_Init();
		foreach( SJ_FSMAction s in lt_act )s.OnEditorInit();

		gameObject.SetActive(false);

	}

	private void Awake()
	{
		Init();
	}

	public	void	Init()
	{
		foreach( SJ_FSMNode s in lt_child )
		{
			dic_child[s.name] = s;
		}
	}

	public	void	Reg_RootExec()
	{ 
		root.hs_curExec.Add(this); 
		foreach( SJ_FSMAction s in lt_act )root.hs_curAction.Add(s);

		SJ_Unity.SendMsg( player , "OnChanged_FSM" );

	}

	public	void	Unreg_RootExec()
	{ 
		root.hs_curExec.Remove(this); 
		foreach( SJ_FSMAction s in lt_act )root.hs_curAction.Remove(s);

		SJ_Unity.SendMsg( player , "OnChanged_FSM" );
	}

	public	void	Msg_Root( string str , int val = 0 , object obj = null )
	{
		foreach( SJ_FSMNode s in hs_curExec )
			s.Recv_Msg(  str , val  ,  obj );
	}

	virtual	public	void OnRecv_Msg(string str , int val , object obj) { }
	public	void	Recv_Msg( string str , int val , object obj )
	{
		foreach( SJ_FSMAction s in lt_act )s.OnRecv_Msg( str , val , obj );
		OnRecv_Msg( str , val , obj );
	}


	public	void	SendFunc_Root( string func , string str_arg )
	{
		foreach( SJ_FSMNode s in hs_curExec )s.RecvFunc( func , str_arg );
	}

	public	void	RecvFunc( string func , string str_arg )
	{
		foreach( SJ_FSMAction s in lt_act )SJ_Unity.SendMsg( s , func , str_arg );
	}

	public	void	SendFunc_Root( string func , object arg )
	{
		foreach( SJ_FSMNode s in hs_curExec )s.RecvFunc( func , arg );
	}

	public	void	RecvFunc( string func , object arg )
	{
		foreach( SJ_FSMAction s in lt_act )SJ_Unity.SendMsg( s , func , arg );
	}


	public	void	ExecStart( object obj = null )
	{
		if( root.debug )
		{
			Debug.Log( "ExecStart : " + name );
			root.lt_node_exec_log.Add(this);
		}

		gameObject.SetActive(true);
		foreach( SJ_FSMAction s in lt_act)s.OnExecStart(obj);
		OnExecStart();

		Reg_RootExec();

		if( func_exec_type != _FUNC_EXEC_TYPE.None )
		{
			if( func_exec_type == _FUNC_EXEC_TYPE.OneShot )
			{
				Exec_CurEnd_Next();
			}
			else if( func_exec_type == _FUNC_EXEC_TYPE.OneShot_Continue )
			{
				Exec_CurEnd_Next( true );
			}
		}

	}

	public	void	ExecPause()
	{
		Unreg_RootExec();
		gameObject.SetActive(false);
		foreach( SJ_FSMAction s in lt_act)s.OnExecPause();
		OnExecPause();
	}

	//public	void	ExecResume()
	//{
	//	Reg_RootExec();
	//	gameObject.SetActive(true);
	//	foreach( SJ_FSMAction s in lt_act)s.OnExecResume();
	//	OnExecResume();
	//}

	public	void	EndState()
	{
		foreach( SJ_FSMAction s in lt_act)s.OnEndState();
		foreach( SJ_FSMNode s in lt_child )
		{
			if( s.gameObject.activeSelf )
			{
				s.ExecPause();
			}
		}
		OnEndState();
	}

	// 같은 부모아래있는 본인의 다음 노드
	public	void Exec_CurEnd_Next(bool noStop = false)
	{
		if( noStop == false )ExecPause();

		// 자식이 있다면 자식에게 흐름 넘김
		if( lt_child.Count > 0 )
		{
			lt_child[0].ExecStart();
			return;
		}

		Exec_CurEnd_Next_AndParent();
	}

	// 
	public	void	Exec_CurEnd_Next_AndParent()
	{
		SJ_FSMNode next =	Find_SelfNext();
		if( next == null )
		{
			if( par != null )
				par.Exec_CurEnd_Next_AndParent();
			return;
		}
		next.ExecStart();
	}

	// 본인 노드의 다음 노드 
	// 부모에서만 찾는다.
	public	SJ_FSMNode	Find_SelfNext()
	{
		if( par == null ) return null;
		return par.Find_ChildNext( this );
	}

	public	SJ_FSMNode	Find_ChildNext( SJ_FSMNode node )
	{
		int next_idx = node.index_node + 1;
		if( lt_child.Count <= next_idx )
		{
			return lt_child[next_idx];
		}
		return null;
	}

	public	void		End_ChildNode_NextExec( SJ_FSMNode node, object obj = null)
	{
		int next_idx = node.index_node + 1;
		if( lt_child.Count <= next_idx )
		{
			EndState();
			if( par != null )
			{
				par.End_ChildNode_NextExec(this);
			}
			return;
		}

		SJ_FSMNode next_node = lt_child[next_idx];
		next_node.ExecStart(obj);
	}


	public	void Exec_CurEnd_Other( string str_node , object obj = null , bool noStop = false )
	{
		if( noStop == false )ExecPause();

		if( par == null )
		{
			return;
		}
		SJ_FSMNode other =	par.FindChild(str_node);
		other.ExecStart( obj );
	}

	// 부모 실행 
	public void	Exec_Parent( string _name = "" , object obj = null , bool noStop = false  )
	{
		if( noStop == false )ExecPause();
		SJ_FSMNode par = FindParent( _name );
		if( par == null )
		{
			return;
		}
		par.ExecStart(obj);
	}

	public	SJ_FSMNode	FindParent(string _name)
	{
		if( par == null ) return null;

		if( string.IsNullOrEmpty(_name) )
		{
			return par;
		}

		if( par.name == _name )
		{
			return par;
		}
		return par.FindParent(_name);
	}

	public	SJ_FSMNode	FindChild( string _name )
	{
		SJ_FSMNode f = null;
		if( dic_child.TryGetValue(_name , out f) )
		{
			return f;
		}
		return null;
	}

	public	void	FindChild_All( string _name , List<SJ_FSMNode> list )
	{
		SJ_FSMNode f = null;
		if( dic_child.TryGetValue(_name , out f) )
		{
			list.Add(f);
		}

		foreach( SJ_FSMNode s in lt_child )
		{
			s.FindChild_All( _name , list );
		}
	}

	public	void	Stop_AllState()
	{
		List<SJ_FSMNode> lt = new List<SJ_FSMNode>();
		foreach( SJ_FSMNode s in root.hs_curExec )
		{ 
			//s.ExecPause();
			lt.Add(s);
		}

		foreach( SJ_FSMNode s in lt )
		{
			s.ExecPause();
		}
	}

	public	void	Exec_StateRoot( string _name  , object obj = null , SJ_FSMNode cur_stop = null )
	{
		if( root.debug )
		{ 
			Debug.Log( "Exec_StateRoot : " + _name );
		}
		
		if( cur_stop != null )
		{
			cur_stop.ExecPause();
		}

		List<SJ_FSMNode> list = new List<SJ_FSMNode>();
		root.FindChild_All( _name , list );
		foreach( SJ_FSMNode s in list )
		{ 
			s.ExecStart(obj);
		}
	}







}
