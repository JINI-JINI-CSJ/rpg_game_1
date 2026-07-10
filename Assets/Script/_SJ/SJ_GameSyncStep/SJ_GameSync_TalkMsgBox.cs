using System.Collections.Generic;
using UnityEngine;

// 닫을때는  use_msg == false && useID == false
public class SJ_GameSync_TalkMsgBox : SJ_GameSyncStepBase
{
    public DialogueTyper dialogueTyper;

    // 뷰박스 상위 객체
    public GameObject go_parViewBox;

    // 뷰박스 본인
    public SJ_Curve_TransObjToggle viewTextBox;

    public bool use_msg;
    // 직접 메세지
    public List<string> msg_directly;

    public bool useID;
    // 번역
    public List<SJ_LANG_ID> sJ_LANG_IDs;

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
        SJ_Unity.SetUnityAction_OneFunc( dialogueTyper.OnDialogueFinished , OnEndLine_TalkBox );

        // 아무 메시지 없는 거라면 닫기 애니
        if( use_msg == false && useID == false )
        {
            viewTextBox.StartFunc_BACK( OnEnd_ViewBoxAni_OFF );
            return;
        }
        viewTextBox.StartFunc_FWD( OnEnd_ViewBoxAni_ON );
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
        else if( useID )
        {
            if( sJ_LANG_IDs.Count < 1 )
            {
                Debug.Log( "SJ_GameSync_TalkMsgBox : sJ_LANG_IDs.Count < 1 !!!" );
                return;
            }
            dialogueTyper.StartDialogue( SJ_Language.Str( sJ_LANG_IDs ) );
        }     
    }

    public void OnEnd_ViewBoxAni_ON()
    {
        StartText();
    }

    // 모든 라인 출력이면 종료 시작
    public void OnEndLine_TalkBox()
    {
        viewTextBox.StartFunc_BACK( OnEnd_ViewBoxAni_OFF );
    }

    public void OnEnd_ViewBoxAni_OFF()
    {
        go_parViewBox.SetActive(false);
        SJ_SimpleSyncMono.NextPlay();
    }


}
