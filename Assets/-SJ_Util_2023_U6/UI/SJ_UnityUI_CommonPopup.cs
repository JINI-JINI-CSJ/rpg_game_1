using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
//using Cysharp.Threading.Tasks;

public class SJ_UnityUI_CommonPopup : MonoBehaviour
{
    public Text text_title;
    public TMP_Text text_title_tm;
    public Text text_MSG;
    public TMP_Text text_MSG_tm;
    public Text text_OK;
    public TMP_Text text_OK_tm;
    public Text text_Cancel;
    public TMP_Text text_Cancel_tm;

    // 일반 버튼 또는 토글
    // 둘중에 한 타입만 해야 한다.
    public Button BT_OK;
    public Button BT_Cancel;
    public Toggle toggle_OK;
    public Toggle toggle_Cancel;

    public _SJ_GO_FUNC func_OK = new _SJ_GO_FUNC();
    public _SJ_GO_FUNC func_Cancel = new _SJ_GO_FUNC();

    public float wait_ko_active = -1;

    public AudioClip snd_OK;
    public AudioClip snd_Cancel;


    public SJ_CallFunc callFunc_OK = new SJ_CallFunc();
    public SJ_CallFunc callFunc_Cancel = new SJ_CallFunc();


    // TASK 
    AwaitableCompletionSource<bool> uniTask;

    public void TaskStart( bool b )
    {
        if( b )
        {
            uniTask = new AwaitableCompletionSource<bool>();
        }
        else
        {
            uniTask = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void _Active_BTCancel(bool b)
    {
        if (BT_Cancel != null)
            SJ_Unity.SetActive(BT_Cancel.gameObject, b);

        if (toggle_Cancel != null)
        {
            SJ_Unity.SetActive(toggle_Cancel.gameObject, b);
            if (b)
            {
                // 둘다 있다.
                toggle_Cancel.SetIsOnWithoutNotify(true);
            }
            else
            {
                // OK 만 있다.
                toggle_OK.SetIsOnWithoutNotify(true);
            }
        }
    }

    public void _SetText(string _text_title, string _text_msg)
    {
        if (text_title != null) text_title.text = _text_title;
        if (text_title_tm != null) text_title_tm.text = _text_title;
        if (text_MSG != null) text_MSG.text = _text_msg;
        if (text_MSG_tm != null) text_MSG_tm.text = _text_msg;
    }

    public void _SetFunc(   string _text_msg, MonoBehaviour mono = null, object obj_call = null ,
                            string func_ok = "", string func_cancel = "", 
                            object arg_ok = null , object arg_cancel = null , string _text_ok = "", 
                            string _text_cancel = "", string _text_title = "")
    {
        _SetText( _text_title , _text_msg );

        func_OK.SetMono(mono, func_ok , arg_ok );
        func_Cancel.SetMono(mono, func_cancel , arg_cancel );

        callFunc_OK.SetInst( obj_call , func_ok , arg_ok );
        callFunc_Cancel.SetInst( obj_call , func_cancel , arg_cancel );


        if (text_OK != null && string.IsNullOrEmpty(_text_ok) == false) text_OK.text = _text_ok;
        if (text_OK_tm != null && string.IsNullOrEmpty(_text_ok) == false) text_OK_tm.text = _text_ok;

        if (text_Cancel != null && string.IsNullOrEmpty(_text_cancel) == false) text_Cancel.text = _text_cancel;
        if (text_Cancel_tm != null && string.IsNullOrEmpty(_text_cancel) == false) text_Cancel_tm.text = _text_cancel;
    }


    public void Set_WaitOKActive(float wait)
    {
        StopAllCoroutines();
        if (wait <= 0) return;
        StartCoroutine(CO_WaitOKActive(wait));
    }

    IEnumerator CO_WaitOKActive(float wait)
    {
        if (BT_OK != null)
            SJ_UnityUI_Util.Button_Interactable(BT_OK, false);
        else if (toggle_OK != null)
            SJ_UnityUI_Util.Toggle_Interactable(toggle_OK, false);

        yield return new WaitForSeconds(wait);

        if (BT_OK != null)
            SJ_UnityUI_Util.Button_Interactable(BT_OK, true);
        else if (toggle_OK != null)
            SJ_UnityUI_Util.Toggle_Interactable(toggle_OK, true);
    }

    static public bool open_curve = false;

    static public SJ_UnityUI_CommonPopup OpenCommonMsg(
        string _text_msg, MonoBehaviour mono = null, object obj_call = null ,
        string func_ok = "", bool show_cancel = false, string func_cancel = "",
        object arg_ok = null , object arg_cancel = null ,
        string _text_ok = "", string _text_cancel = "",
        float _wait_ko_active = -1, string _text_title = "")
    {
        open_curve = false;
        GameObject go_SJ_UnityUI_CommonPopup = SJ_UnityUIMng.OpenPopup("SJ_UnityUI_CommonPopup");

        return SettingOpen( go_SJ_UnityUI_CommonPopup , _text_msg, mono , obj_call  ,
            func_ok ,  show_cancel ,  func_cancel ,
            arg_ok ,  arg_cancel  ,
            _text_ok ,  _text_cancel ,
            _wait_ko_active ,  _text_title );
    }

    static public SJ_UnityUI_CommonPopup OpenCommonMsg_Curve(
        string _text_msg, MonoBehaviour mono = null, object obj_call = null ,
        string func_ok = "", bool show_cancel = false, string func_cancel = "",
        object arg_ok = null , object arg_cancel = null ,
        string _text_ok = "", string _text_cancel = "",
        float _wait_ko_active = -1, string _text_title = "")
    {
        open_curve = true;
        GameObject go_SJ_UnityUI_CommonPopup = SJ_UnityUIMng_Curve.Open("SJ_UnityUI_CommonPopup");

        return SettingOpen( go_SJ_UnityUI_CommonPopup , _text_msg, mono , obj_call  ,
            func_ok ,  show_cancel ,  func_cancel ,
            arg_ok ,  arg_cancel  ,
            _text_ok ,  _text_cancel ,
            _wait_ko_active ,  _text_title );
    }

    static public SJ_UnityUI_CommonPopup SettingOpen( GameObject go_SJ_UnityUI_CommonPopup , string _text_msg, 
        MonoBehaviour mono = null, object obj_call = null ,
        string func_ok = "", bool show_cancel = false, string func_cancel = "",
        object arg_ok = null , object arg_cancel = null ,
        string _text_ok = "", string _text_cancel = "",
        float _wait_ko_active = -1, string _text_title = "" )
    {
        if (go_SJ_UnityUI_CommonPopup == null)
        {
            Debug.LogError("go_SJ_UnityUI_CommonPopup == null");
            return null;
        }
        SJ_UnityUI_CommonPopup sJ_UnityUI_CommonPopup = go_SJ_UnityUI_CommonPopup.GetComponent<SJ_UnityUI_CommonPopup>();
        if (sJ_UnityUI_CommonPopup == null)
        {
            Debug.LogError("sJ_UnityUI_CommonPopup == null");
            return null;
        }

        sJ_UnityUI_CommonPopup.TaskStart(false);

        if (string.IsNullOrEmpty(func_cancel) && string.IsNullOrEmpty(_text_cancel) && show_cancel == false)
        {
            sJ_UnityUI_CommonPopup._Active_BTCancel(false);
        }
        else
        {
            sJ_UnityUI_CommonPopup._Active_BTCancel(true);
        }
        sJ_UnityUI_CommonPopup._SetFunc(_text_msg, mono , obj_call , func_ok, func_cancel,arg_ok , arg_cancel ,  _text_ok, _text_cancel, _text_title);
        sJ_UnityUI_CommonPopup.Set_WaitOKActive(_wait_ko_active);
        return go_SJ_UnityUI_CommonPopup.GetComponent<SJ_UnityUI_CommonPopup>();
    }

    public void OnOK()
    {
        Debug.Log( "------->>>> common popup ~~~~~ OnOK" );

        SJSound.PlaySound( snd_OK );
        if( open_curve == false )
        {
            SJ_UnityUIMng.ClosePopup();        
            uniTask?.TrySetResult(true);
            func_OK.Func();
            callFunc_OK.Func();            
        }
        else
        {
            SJ_UnityUIMng_Curve.CloseOne( OnCurveCloseEnd_OK );
        }

    }

    public void OnCancel()
    {
        Debug.Log( "------->>>> common popup ~~~~~ OnCancel" );

        SJSound.PlaySound( snd_Cancel );
        if( open_curve == false )
        {
            SJ_UnityUIMng.ClosePopup();
            uniTask?.TrySetResult(false);
            func_Cancel.Func();
            callFunc_Cancel.Func();            
        }
        else
        {
            SJ_UnityUIMng_Curve.CloseOne( OnCurveCloseEnd_Cancel );
        }
    }

    public void OnCurveCloseEnd_OK()
    {
        uniTask?.TrySetResult(true);
        func_OK.Func();
        callFunc_OK.Func();    
    }

    public void OnCurveCloseEnd_Cancel()
    {
        uniTask?.TrySetResult(true);
        func_Cancel.Func();
        callFunc_Cancel.Func();      
    }

    public void OnOK_Toggle( bool b )
    {
        if(b)OnOK();
    }

    public void OnCancel_Toggle( bool b )
    {
        if(b)OnCancel();
    }

    //=============================================================================================
    // 플레이어 인풋
    // public void OnNavigate(InputValue value)
    // {
    //     Vector2 input = value.Get<Vector2>();

    //     if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
    //     {
    //         // 좌우 이동
    //         int shift_idx = 0;
    //         if (input.x < -0.1f) shift_idx = -1;
    //         if (input.x > 0.1f) shift_idx = 1;

    //         // 토글일때만
    //         if (toggle_OK != null && toggle_Cancel != null)
    //         {
    //             if (shift_idx == -1)
    //             {
    //                 toggle_OK.SetIsOnWithoutNotify(true);
    //             }
    //             else
    //             {
    //                 toggle_Cancel.SetIsOnWithoutNotify(true);
    //             }
    //         }
    //     }
    //     else
    //     {
    //     }
    // }

    public void OnSubmit(InputValue value)
    {
        // if (toggle_OK != null && toggle_Cancel != null)
        // {
        //     if (toggle_OK.isOn)
        //     {
        //         OnOK();
        //     }
        //     else if (toggle_Cancel.isOn)
        //     {
        //         OnCancel();
        //     }
        // }

        OnOK();
    }
    public void OnCancel(InputValue value)
    {
        OnCancel();
    }

    // 타스크 실행
    public async Awaitable<bool> _OpenTask( string _text_msg , bool show_cancel = false )
    {
        _SetText( "" , _text_msg );
        _Active_BTCancel( show_cancel );

        TaskStart( true );
        return await uniTask.Awaitable;
    }

    static public async Awaitable<bool> OpenTask( string _text_msg , bool show_cancel = false )
    {
        GameObject go_SJ_UnityUI_CommonPopup = SJ_UnityUIMng.OpenPopup("SJ_UnityUI_CommonPopup");
        if (go_SJ_UnityUI_CommonPopup == null)
        {
            Debug.LogError("go_SJ_UnityUI_CommonPopup == null");
            return false;
        }
        SJ_UnityUI_CommonPopup sJ_UnityUI_CommonPopup = go_SJ_UnityUI_CommonPopup.GetComponent<SJ_UnityUI_CommonPopup>();
        if (sJ_UnityUI_CommonPopup == null)
        {
            Debug.LogError("sJ_UnityUI_CommonPopup == null");
            return false;
        }

        return await sJ_UnityUI_CommonPopup._OpenTask(_text_msg, show_cancel);
    }
}
