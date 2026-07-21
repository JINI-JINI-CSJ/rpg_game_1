using System.Collections.Generic;
using UnityEngine;

// 닫을때는  use_msg == false && useID == false
public class SJ_GameSync_TalkMsgBox : SJ_GameSyncStepBase
{
    public DialogueTyper dialogueTyper;
    // 뷰박스 상위 객체
    public GameObject go_parViewBox;
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
        //SJ_Unity.SetUnityAction_OneFunc( dialogueTyper.OnDialogueFinished , OnEndLine_TalkBox );

        dialogueTyper.OnDialogueFinished += OnEndLine_TalkBox;

        // 아무 메시지 없는 거라면 닫기 애니
        if( use_msg == false && use_ListID == false && use_RangeID == false )
        {
            SJ_UnityUIMng_Curve.CloseOne( OnEnd_ViewBoxAni_OFF );
            return;
        }
        SJ_UnityUIMng_Curve.Open( go_parViewBox.name , OnEnd_ViewBoxAni_ON );
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
        else
        {
            Debug.LogError( "사용 설정 없음" );
        }
    }

    public void OnEnd_ViewBoxAni_ON()
    {
        StartText();
    }

    // 모든 라인 출력이면 종료 시작
    public void OnEndLine_TalkBox()
    {
        Debug.Log( "OnEndLine_TalkBox" );
        SJ_UnityUIMng_Curve.CloseOne( OnEnd_ViewBoxAni_OFF );
    }

    public void OnEnd_ViewBoxAni_OFF()
    {
        SJ_SimpleSyncMono.NextPlay();
    }
}