using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
//using System.Net;

public class SJNetRecv_Base : MonoBehaviour 
{
	_SJRecv_Ref 	sj_recv_ref = new _SJRecv_Ref();
	_SJNetHead		sj_net_head = new _SJNetHead();
	
	GameObject		go_eventWait;
	string 			str_eventMsg ,	str_eventFuncWait , str_eventFuncWait_Arg;
	bool 			bWaitMode = false;
	float			fWaitTime;
	float 			fShowDelayTime;
	bool 			bShowEvent = false;

	//KOJ2
	protected float	fMaxConnectTime = 10.0f;
	protected float fNowConnectTime = 0.0f;
	
	//-------------------------------------------------------------------------
	public	void Update_PrcNetEvent()
	{
		System.Object 			sync_obj =	SJNetPrc.Instance.m_SyncPacket;
		List< _SJ_NET_EVENT >	event_list = SJNetPrc.Instance.m_SJNetEvent;
		
		_SJ_NET_EVENT	net_event;

		lock( sync_obj )
		{
			if( event_list.Count > 0 )
			{
				net_event = event_list[0];

				switch( net_event )
				{
				case _SJ_NET_EVENT.SUCC:
					if( SJNetPrc.Instance.go_Recv_obj_sub != null )
					{
						SJNetPrc.Instance.go_Recv_obj_sub.SendMessage( "OnConnectServer" , 0 , SendMessageOptions.DontRequireReceiver );
					}
					break;
					
				case _SJ_NET_EVENT.FAILE:
					if( SJNetPrc.Instance.go_Recv_obj_sub != null )
						SJNetPrc.Instance.go_Recv_obj_sub.SendMessage( "OnConnectServer_Error" , SendMessageOptions.DontRequireReceiver );
					break;
				}
				
				//Debug.Log( "event_list a : " + event_list.Count );
				
				event_list.Remove( net_event );					
				//AndroidManager.Instance.LogAndroid( "event_list b : " + event_list.Count.ToString() );
				//Debug.Log( "event_list b : " + event_list.Count );
			}
		}
	}
	
	
	public	void Update_PrcNetRecv()
	{
		System.Object sync_obj =	SJNetPrc.Instance.m_SyncPacket;
		List< SJNetBuffPrc >		packet_list = SJNetPrc.Instance.m_RecvPacket;
		SJNetBuffPrc				packet;
		while(true)
		{
			packet = null;
			lock( sync_obj )
			{
				if( packet_list.Count > 0 )
				{
					packet = packet_list[0];
					
					SJNetPrc.Instance.m_RecvPacketMng.ReturnObj( packet );
					packet_list.Remove( packet );					
				}
			}
			if( packet == null )
				break;
			
			//Debug.Log( "패킷 리시프 : h : " +  packet.m_high + "  l : " + packet.m_low );

			// default
			if( packet.m_high == 255 &&  packet.m_low == 254 )
			{
				OnRecv_Ping_Kw();
			}
			
			//
			// recv object
			//
			sj_net_head.high = packet.m_high;
			sj_net_head.low = packet.m_low;
			
			_SJRecvObjFunc 	recv_obj;
			
			if(	SJNetPrc.Instance.m_dOnRecvObj.TryGetValue( sj_net_head , out recv_obj ) )
			{
				if( recv_obj.recv_obj != null )
				{
					sj_recv_ref.head = sj_net_head;
					sj_recv_ref.br = packet.m_br;
					recv_obj.recv_obj.SendMessage( recv_obj.func , sj_recv_ref , SendMessageOptions.DontRequireReceiver );

					//KOJ2
					fNowConnectTime = 0;
				}
			}
			
			//Debug.Log( " RECV~~>>> packet.m_high : " + packet.m_high + "     packet.m_low : " + packet.m_low );

			OnPrcNetRecvPacket( packet.m_br , packet.m_tcp_udp , packet.m_high , packet.m_low , packet.m_result  );
			packet.CloseStream();
		}
	}
	
	
	void UpdateWaitMode()
	{
		if( bWaitMode == false )
			return;
		
		fWaitTime -= Time.deltaTime;
		
		if( fWaitTime < 0.0f )
		{
			bWaitMode = false;
			if( go_eventWait != null && string.IsNullOrEmpty(str_eventFuncWait) == false )
			{
				go_eventWait.SendMessage( str_eventFuncWait , str_eventFuncWait_Arg ,  SendMessageOptions.DontRequireReceiver );
			}
		}

		if( bShowEvent == false )
		{
			fShowDelayTime -= Time.deltaTime;
			if( fShowDelayTime <= 0.0f )
			{
				bShowEvent = true;
				OnWaitRecv_ShowEvent( str_eventMsg );
			}
		}
		
	}
	// Update is called once per frame
	void Update () 
	{
		Update_NetRecv_Base();

		SJNetPrc.Instance.SJ_AverageScore.Update();

		//KOJ2
		//if (SJNetPrc.Instance.m_bOnline) 
		//{
		//	fNowConnectTime += Time.deltaTime;
		//	if ( fNowConnectTime > fMaxConnectTime )
		//	{
		//		SJNetPrc.Instance.Disconnect();
		//		fNowConnectTime = 0;
		//	}
		//}
	}

	void Update_NetRecv_Base()
	{
		//Debug.Log( "Update_NetRecv_Base~~~" );
		Update_PrcNetEvent();
		Update_PrcNetRecv();
		UpdateWaitMode();
	}
	
	//
	// ping
	//
	public	void	OnRecv_Ping_Kw()
	{
		//UInt32 nSendSize = 0;
		//BinaryWriter bw = SJNetPrc.Instance.GetTCP_Buff();

		//nSendSize += 6; // head

		//bw.Write( (byte)255 );	//	1
		//bw.Write( (byte)253 );	//	1
		//bw.Write( (UInt32)IPAddress.HostToNetworkOrder( (int)nSendSize ) );	//	4 

		//SJNetPrc.Instance.SendTCPServer_Buff( (int)nSendSize );

		//Debug.Log( "OnRecv_Ping_Kw" );

		BinaryWriter bw  =	SJNetPrc.Instance.GetSJBuff_BW();
		SJNetPrc.Instance.SendSJBuff( (byte)255 , (byte)253 );

	}

	public	void 	StopWaitMode()
	{
		bWaitMode = false;
	}

//-------------------------------------------------------------------------
	virtual public	void OnPrcNetRecvPacket( BinaryReader br , byte tcp_udp , byte high , byte low , int result ){}
	virtual	public	void OnWaitRecv_Start( string msg , GameObject go_obj ,string eventFuncWait , string eventFuncWait_Arg , float waitTime , float showDelayTime){}
	virtual	public	void OnWaitRecv_End(){}
	virtual	public	void OnWaitRecv_ShowEvent(string msg ){}
	
	public 	void 	WaitRecv_Start( string msg="", GameObject go_obj=null ,string eventFuncWait=""  ,
	                             string eventFuncWait_Arg="", float waitTime=10.0f , float showDelayTime = 0.0f )
	{
		bWaitMode = true;
		bShowEvent = false;
		str_eventMsg = msg;
		go_eventWait = go_obj;
		str_eventFuncWait = eventFuncWait;
		str_eventFuncWait_Arg = eventFuncWait_Arg;
		fWaitTime = waitTime;
		fShowDelayTime = showDelayTime;
		OnWaitRecv_Start( msg, go_obj , eventFuncWait  , eventFuncWait_Arg, waitTime , showDelayTime );
	}

	

	public	void 	WaitRecv_End()
	{
		OnWaitRecv_End();
	}
	
}
