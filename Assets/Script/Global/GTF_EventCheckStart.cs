using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 시작시 등등 특정 이벤트를 체크해보기 , 없으면 기본 진행
/// 
/// </summary>

public class GTF_EventCheckStart : MonoBehaviour
{
    static GTF_EventCheckStart G;

    public bool     TEST_START;
    public string   TEST_TAG;
    public int      TEST_IDS;

    void Awake()
    {
        G = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public bool CheckSceneStart()
    {
        return G._CheckSceneStart();
    }

    // 씬 시작시 체크해보자.
    public bool _CheckSceneStart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if( TEST_START )
        {
            SJ_SyncFuncLangPart.StartTagMeta( TEST_TAG , TEST_IDS );
            return true;
        }

        // 최초 시작
        // 타이틀에서 여기로 온다.
        if( currentSceneName == "InGame" )
        {
            // 완전 최초
            if( PlayerSaveFile.GetFirstPlay_Step() == 0 )
            {
                // 메타 이벤트 실행
                SJ_SyncFuncLangPart.StartTagMeta( "INTRO_GODDESS_FIRST" );
                return true;
            }

            // 2번째 공식 던전?? 등등
        }


        if( currentSceneName == "Lobby" )
        {
            // 최초 마을 입장
            if( PlayerSaveFile.GetFirstPlay_Step() == 1 )
            {
                // 메타 이벤트 실행
                SJ_SyncFuncLangPart.StartTagMeta( "INTRO_CITY_FIRST" );
                return true;
            }
        }

        return false;
    }
}
