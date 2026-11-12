using System.Collections.Generic;
using UnityEngine;

// 닫을때는  use_msg == false && useID == false
public class SJ_GameSync_TalkMsgBox : SJ_GameSyncStepBase
{
    public string name_Popup;

    public DialogueTyper dialogueTyper;
    // 뷰박스 상위 객체
    //public GameObject go_parViewBox;
    public bool use_msg;
    // 직접 메세지
    public List<string> msg_directly;
    public bool use_ListID;
    // 번역
    public List<SJ_LANG_ID> sJ_LANG_IDs;
    public bool     use_RangeID;
    public string   ID_PART_Range;
    public int      start_id_Range;
    public int      end_id_Range;

    public bool     closeMsgEnd;

    //public SJ_COMMON.Func_VOID func_end;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayStep()
    {
        // 아무 메시지 없는 거라면 닫기 애니
        if( use_msg == false && use_ListID == false && use_RangeID == false )
        {
            SJ_UnityUIMng_Curve.CloseOne( OnEnd_ViewBoxAni_OFF );
            return;
        }
        OpenWindow();
    }

    public void OpenWindow()
    {
        GameObject go_popup = SJ_UnityUIMng_Curve.Open( name_Popup , OnEnd_ViewBoxAni_ON );
        dialogueTyper = go_popup.GetComponentInChildren<DialogueTyper>();
        SJ_CSharpUtil.ActionFunc_CheckReg( ref dialogueTyper.OnDialogueFinished , OnEndLine_TalkBox );
    }

    public void StartText()
    {
        if( use_msg )
        {
            if( msg_directly.Count < 1 )
            {
                Debug.Log( "SJ_GameSync_TalkMsgBox : msg_directly.Count < 1 !!!" );
                return;
            }
            dialogueTyper.StartDialogue( msg_directly );
        }
        else if( use_ListID )
        {
            if( sJ_LANG_IDs.Count < 1 )
            {
                Debug.Log( "SJ_GameSync_TalkMsgBox : sJ_LANG_IDs.Count < 1 !!!" );
                return;
            }
            dialogueTyper.StartDialogue( SJ_Language.Str( sJ_LANG_IDs ) );
        }
        else if( use_RangeID )
        {
            dialogueTyper.StartDialogue( SJ_Language.STR_RangID( ID_PART_Range , start_id_Range , end_id_Range ) );
        }
        // else
        // {
        //     Debug.LogError( "사용 설정 없음" );
        // }
    }

    // 단일 메시지 열기
    public void StartText_OneStep( string msg )
    {
        OpenWindow();
        dialogueTyper.StartDialogue( msg );
    }

    public void OnEnd_ViewBoxAni_ON()
    {
        StartText();
    }

    // 모든 라인 출력이면 종료 시작
    // 단일 메세지 였다면 그냥 종료 함수
    public void OnEndLine_TalkBox()
    {
        Debug.Log( "OnEndLine_TalkBox" );
        if( closeMsgEnd )
            CloseMsgBox();
        else 
            OnEndFunc();
    }

    public void CloseMsgBox()
    {
        SJ_UnityUIMng_Curve.CloseOne( OnEnd_ViewBoxAni_OFF );
    }

    public void OnEnd_ViewBoxAni_OFF()
    {
        OnEndFunc();
    }
}