using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class SJAllocObjMng <T> where T : ISJAllocObj
{
	List<T>		m_ObjList;
	Queue<T>	m_ObjList_UseAble;	
	
	public	void	AllocObj( T t , int nCount )
	{
		m_ObjList = new List<T>();
		m_ObjList_UseAble = new Queue<T>();
		
		for(int i = 0 ; i < nCount ; i++ )
		{	
			T obj = (T)t.NewObj();
			
			obj.m_ID = i;
			obj.m_isUsing = false;
			
			obj.OnAlloc();
			
			m_ObjList.Add( obj );
			m_ObjList_UseAble.Enqueue( obj );
		}
	}
	
	
	public	T 		GetNewObj()
	{
		T obj = m_ObjList_UseAble.Dequeue();
		
		if( obj == null )
		{
			return null;
		}
		obj.m_isUsing = true;
		return obj;
	}
	
	public	void	ReturnObj(T obj)
	{
		obj.m_isUsing = false;
		m_ObjList_UseAble.Enqueue(obj);
	}
	
	public	void	ReturnObjAll()
	{
		m_ObjList_UseAble.Clear();
		foreach( T t in m_ObjList )
		{
			m_ObjList_UseAble.Enqueue( t );
		}
	}
	
	public	T		GetObj_Idx( int nID )
	{
		if( m_ObjList.Count <= nID )
			return null;
		return	m_ObjList[ nID ];
	}
}

public class ISJAllocObj
{
	public	int 	m_ID;
	public	bool	m_isUsing;
	
	public	GameObject	go_obj;
	
	virtual	public	ISJAllocObj NewObj()
	{
		return new 	ISJAllocObj();
	}
	
	virtual	public  void OnAlloc()
	{
		//Debug.Log( "ISJAllocObj" );
	}
}

