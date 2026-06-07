using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Net;
using System.Collections.Generic;


public class SJSerialBuffer
{
	byte[]			m_buff = null;
	UInt32			m_totalSize = 0;
	
	MemoryStream	m_ms = null;
	BinaryWriter	m_bw = null;
	
	
	public	void 	Create( int nSize = 10240 )
	{
		m_buff = new byte[ nSize ];
		
		m_ms = new MemoryStream(m_buff ,true);
		m_bw = new BinaryWriter(m_ms);
	}
	
	
	public void 	Destroy()
	{
		m_buff = null;
		m_ms = null;
		m_bw = null;
	}
	
	public	void 	Start()
	{
		m_ms.Seek(0,SeekOrigin.Begin);
	}
	
	public	BinaryWriter	GetBW()
	{
		return m_bw;
	}
	
	public	BinaryWriter	GetBW_Begin()
	{
		m_ms.Seek(6,SeekOrigin.Begin);

		return m_bw;
	}
	
	public	void 	BuffPacket_End( byte h , byte l )
	{
		m_totalSize = (UInt32)m_ms.Position;
		
		m_ms.Seek(0,SeekOrigin.Begin);
		m_bw.Write( h );
		m_bw.Write( l );
		m_bw.Write( m_totalSize );
	}
	
	public	byte[]	GetBuff(){return m_buff;}
	public	long	GetTotalWriteSize(){return m_ms.Position;}
	public	long	GetPacket_End(){return m_totalSize;}
}
