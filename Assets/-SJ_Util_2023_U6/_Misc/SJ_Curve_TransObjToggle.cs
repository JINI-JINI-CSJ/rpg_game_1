using UnityEngine;
using UnityEngine.UI;


// 객체 트랜스폼 토글 애니
// 커브는 Ones 만 지원
public class SJ_Curve_TransObjToggle : MonoBehaviour
{
	public SJ_Curve sJ_Curve = new();

	public bool use_local_world; // local = false , world = true;

	public bool forward_Hide = false;

	public bool backward_Hide = true;

	public GameObject main_obj;

	public bool use_pos;
	public Vector3 pos_s = Vector3.zero;
	public Vector3 pos_e = Vector3.zero;

	public bool use_rot;
	public Quaternion rot_s = Quaternion.identity;
	public Quaternion rot_e = Quaternion.identity;

	public bool use_scl;
	public Vector3 scl_s = Vector3.one;
	public Vector3 scl_e = Vector3.one;

	public bool use_color;

	public Color color_s;
	public Color color_e;
	public Image image_Color;

	// 캔버스 그룹 알파
	public CanvasGroup canvasGroup;

	// false : 초기  , true : 완료
	// true 인 상태에서 플레이 시작하면 backward 플레이
	public bool cur_toggle;

	
	public SJ_COMMON.Func_VOID func_OnEndToggle_ON;
	public SJ_COMMON.Func_VOID func_OnEndToggle_OFF;

	public SJ_COMMON.Func_VOID func_OnEnd;

	public void Init()
	{
		//gameObject.SetActive(true);
		ActiveObj( true );
		sJ_Curve.func_Update = OnUpdateCurve;
		sJ_Curve.func_End = OnEndCurve;
		sJ_Curve.loop_type = SJ_Curve.LOOP_TYPE.None;
	}

	public bool StartToggle()
	{
		if( sJ_Curve.play ) return false;
		Init();
		sJ_Curve.StartTime_PlayDir( !cur_toggle );
		return true;
	}

	public bool isPlaying()
	{
		return sJ_Curve.play;
	}

	// 정방향 플레이 
	public void StartFunc_FWD( SJ_COMMON.Func_VOID func_end = null , bool force_start = false )
	{
		Init();
		func_OnEndToggle_ON = func_end;
		if( force_start == false && cur_toggle && sJ_Curve.play == false )
		{
			// 이미 열린 상태
			func_OnEndToggle_ON?.Invoke();
			func_OnEnd?.Invoke();		
			return;
		}
		sJ_Curve.StartTime_PlayDir( true );//그외에는 그냥 오픈 애니
		OnUpdateCurve();
	}
	public async Awaitable StartFunc_FWD_Awaitable(  bool force_start = false )
	{
		Init();
		if( force_start == false && cur_toggle && sJ_Curve.play == false )
		{	
			return;
		}
		sJ_Curve.StartTime_PlayDir( true );
		OnUpdateCurve();
		while( true )
		{
			await Awaitable.NextFrameAsync();
			if( sJ_Curve.play == false ) return;
		}
		
	}
	

	// 역방향 플레이
	public void StartFunc_BACK( SJ_COMMON.Func_VOID func_end = null , bool force_start = false )
	{
		Init();
		func_OnEndToggle_OFF = func_end;
		if( force_start == false &&  cur_toggle == false && sJ_Curve.play == false )
		{
			// 이미 닫힌 상태
			func_OnEndToggle_OFF?.Invoke();
			func_OnEnd?.Invoke();		
			return;
		}
		sJ_Curve.StartTime_PlayDir( false );
		OnUpdateCurve();
	}

	public async Awaitable StartFunc_BACK_Awaitable(  bool force_start = false )
	{
		Init();
		if( force_start == false &&  cur_toggle == false && sJ_Curve.play == false )
		{	
			return;
		}
		sJ_Curve.StartTime_PlayDir( false );
		OnUpdateCurve();
		while( true )
		{
			await Awaitable.NextFrameAsync();
			if( sJ_Curve.play == false ) return;
		}
	}


	public void OnUpdateCurve()
	{
		if( use_local_world )
		{
			if( use_pos )transform.position = Vector3.Lerp( pos_s , pos_e , sJ_Curve.curve_cur );
			if( use_rot )transform.rotation = Quaternion.Slerp( rot_s , rot_e , sJ_Curve.curve_cur );
			if( use_scl )transform.localScale = Vector3.Lerp( scl_s , scl_e , sJ_Curve.curve_cur );
		}
		else
		{
			if( use_pos )transform.localPosition = Vector3.Lerp( pos_s , pos_e , sJ_Curve.curve_cur );
			if( use_rot )transform.localRotation = Quaternion.Slerp( rot_s , rot_e , sJ_Curve.curve_cur );
			if( use_scl )transform.localScale = Vector3.Lerp( scl_s , scl_e , sJ_Curve.curve_cur );
		}
		if( use_color )
		{
			image_Color.color = Color.Lerp( color_s , color_e , sJ_Curve.curve_cur );
		}

		// 캔버스 그룹 등록 여부
		if( canvasGroup != null )
		{
			// 위의 컬러로 하자.
			Color col = Color.Lerp( color_s , color_e , sJ_Curve.curve_cur );
			canvasGroup.alpha = col.a;
		}
	}

	public void OnEndCurve()
	{
		cur_toggle = !cur_toggle;
		func_OnEnd?.Invoke();		
		if( cur_toggle )
		{
			func_OnEndToggle_ON?.Invoke();
			if( forward_Hide )ActiveObj( false );
		}
		else
		{
			func_OnEndToggle_OFF?.Invoke();
			if( backward_Hide )ActiveObj( false );
		}
	}

	public void ActiveObj( bool b )
	{
		if( main_obj != null )	main_obj.SetActive(b);
		else 					gameObject.SetActive(b);
	}

    void Update()
    {
        sJ_Curve.UpdateCurve();
    }

	static public GameObject StartPlay_FromCanvas( string go_name , SJ_COMMON.Func_VOID func_end = null , bool force_start = false , bool last_child = true)
	{
		GameObject go_canvas = GameObject.Find( "Canvas" );
		if( go_canvas == null )
		{
			Debug.LogError( "go_canvas == null" );
			return null;
		}
		 
		Transform tr_go = go_canvas.transform.Find( go_name );
		if( tr_go == null )
		{
			Debug.LogError( "tr_go == null " + go_name );
			return null;
		}

		SJ_Curve_TransObjToggle transObjToggle = tr_go.GetComponent<SJ_Curve_TransObjToggle>();
		if( transObjToggle == null )
		{
			Debug.LogError( "transObjToggle == null " + go_name );
			return null;
		}

		if( last_child )tr_go.SetAsLastSibling();
		
		transObjToggle.StartFunc_FWD( func_end , force_start );
		return tr_go.gameObject;
	}
}
