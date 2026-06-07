using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public	enum TRANS_TYPE 
{
	POS = 0,
	ROT,
	SCL
}


public	enum SJSHAKING_TYPE
{
	FIXED = 0,
	RANDOM_FREE,
	RANDOM_BALANCE
}



public class SJShakeLocalTrans : MonoBehaviour 
{
	static	public	SJShakeLocalTrans	g;

	public	float 			m_TotalTime = 1;
	public	float 			m_TermTime = 0.05f;
	public	Vector3			m_RangePos = Vector3.one;
	public	TRANS_TYPE		m_TransType = TRANS_TYPE.POS;
	public	SJSHAKING_TYPE 	m_shaking_type = SJSHAKING_TYPE.RANDOM_BALANCE;
	public	bool 			m_RelativeCam = true;

	public	bool			startFunc = false;
	public	bool			m_AutoRestart = false;


	
	List<Vector3>	m_ltKeyValue = new List<Vector3>();
	bool 			m_IsPlay=false;
	public	float 			m_CurTotalElapse=0;
	
	Vector3			m_FirstPos;
	Quaternion		m_FirstRot;
	Vector3			m_FirstScl;

	float 			m_delay = 0;

	public			GameObject	recv_obj;
	public			string		func;

	private void Awake()
	{
		g = this;
	}

	// Use this for initialization
	void Start()
	{
		SetFirstTrans();

		if( startFunc )
		{
			MakeStart();
			StartPlay();
		}
	}
	
	public	void SetFirstTrans()
	{
		g = this;

		if( m_IsPlay )
			return;

		m_FirstPos = transform.localPosition;
		m_FirstRot = transform.localRotation;
		m_FirstScl = transform.localScale;
	}
	
	// Update is called once per frame
	void Update() 
	{
		//if( m_IsPlay && gameObject.GetComponent<Camera>() == null )
		//{
		//	UpdateShaking();
		//}
	}
	
	
	void LateUpdate()
	{
		//if( m_IsPlay && gameObject.GetComponent<Camera>() != null )
		if( m_IsPlay )
		{
			UpdateShaking();
		}
	}

	public	void SetDelay( float f ){m_delay = f;}
	
	static		public	void Shaking( 	float total_time, 	float term_time=0.03f, 
							float range_x=1.0f, float range_y=1.0f, float range_z=1.0f,		bool start_play=true,
							SJSHAKING_TYPE 		shaking_type=SJSHAKING_TYPE.RANDOM_BALANCE, bool relativeCam = false , 
							TRANS_TYPE 			trans_type=TRANS_TYPE.POS)
	{
		g.Shaking_Start(total_time,  term_time,  range_x,  range_y,  range_z, start_play,shaking_type,  relativeCam, trans_type);
	}

	public	void Shaking_Start( 	float total_time, 	float term_time=0.03f, 
							float range_x=1.0f, float range_y=1.0f, float range_z=1.0f,		bool start_play=true,
							SJSHAKING_TYPE 		shaking_type=SJSHAKING_TYPE.RANDOM_BALANCE, bool relativeCam = false , 
							TRANS_TYPE 			trans_type=TRANS_TYPE.POS)
	{
		m_TotalTime 	= total_time;
		m_TermTime 		= term_time;
		m_RangePos.x 	= range_x;m_RangePos.y = range_y;m_RangePos.z = range_z;
		m_TransType 	= trans_type;
		m_shaking_type  = shaking_type;
		m_RelativeCam 	= relativeCam;
		MakeStart();
		if( start_play )StartPlay();
	}

	public	void	MakeStart()
	{
		int 	count = 0;
		float 	cur_time = 0;
		Vector3 recent_pos = new Vector3(0,0);
		Vector3	amount = new Vector3( m_RangePos.x, m_RangePos.y, m_RangePos.z );
		m_ltKeyValue.Clear();
		m_ltKeyValue.Add(new Vector3(0,0,0));
		while(true)
		{
			cur_time += m_TermTime;
			if( cur_time >= m_TotalTime )
			{
				m_ltKeyValue.Add(new Vector3(0,0,0));
				break;
			}
			
			if( m_shaking_type == SJSHAKING_TYPE.FIXED )
			{
				float f = 1.0f;
				if( count % 2 == 1 )f = -1.0f;
				m_ltKeyValue.Add( amount * f );
			}
			else if( m_shaking_type == SJSHAKING_TYPE.RANDOM_FREE )
			{
				float x = UnityEngine.Random.Range( -amount.x , amount.x );
				float y = UnityEngine.Random.Range( -amount.y , amount.y );
				float z = UnityEngine.Random.Range( -amount.z , amount.z );
				m_ltKeyValue.Add( new Vector3(x,y,z) );
			}
			else if( m_shaking_type == SJSHAKING_TYPE.RANDOM_BALANCE )
			{
				float x = BalanceRandom( amount.x , recent_pos.x );
				float y = BalanceRandom( amount.y , recent_pos.y );
				float z = BalanceRandom( amount.z , recent_pos.z );
				recent_pos.x=x;
				recent_pos.y=y;
				recent_pos.z=z;
				m_ltKeyValue.Add( recent_pos );
			}
		}
	}

	
	float BalanceRandom( float src , float recent )
	{
		if( recent < 0 )
		{
			return UnityEngine.Random.Range( recent , src );
		}//else
		return UnityEngine.Random.Range( -src , recent );
	}
	
	public	void StartPlay()
	{
		m_IsPlay=true;
		m_CurTotalElapse=0;
	}
	
	
	void UpdateShaking()
	{	
		m_delay -= Time.deltaTime;

		if( m_delay > 0 )return;
		m_delay = 0.0f;

		m_CurTotalElapse += Time.deltaTime;
		float f = m_CurTotalElapse / m_TotalTime;
		if( f >= 1.0f )	
		{
			m_IsPlay = false;
			if( m_TransType == TRANS_TYPE.POS )
				transform.localPosition = m_FirstPos;
			else if( m_TransType == TRANS_TYPE.ROT )
				transform.localRotation = m_FirstRot;
			else if( m_TransType == TRANS_TYPE.SCL )
				transform.localScale 	= m_FirstScl;
			
			SJ_Unity.SendMsg( recv_obj , func );

			if( m_AutoRestart )
			{
				MakeStart();
				StartPlay();
			}

			return;
		}

		Vector3 val = SJMiscUtil_1.Interp_Catmull_Rom( m_ltKeyValue.ToArray(), f );

		if( m_RelativeCam ){}

		if( m_TransType == TRANS_TYPE.POS )
			transform.localPosition = m_FirstPos + val;
		else if( m_TransType == TRANS_TYPE.ROT )
			transform.localRotation = Quaternion.Euler( val );
		else if( m_TransType == TRANS_TYPE.SCL )
			transform.localScale 	= val;
	}
}
