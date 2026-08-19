using UnityEngine;

/// <summary>
/// 게임 처음 시작하면 모든 게임 데이터 생성
/// 월드맵
/// 도시
/// 메인 스토리 , 전설 스토리
/// 인물 
/// 
/// </summary>

public class MakingMain : MonoBehaviour
{
    static public MakingMain G;

    public SJ_SimpleSyncMono syncMono;

    // 새로 생성 , 또는 로드
    public int mode_make_load;

    public SJ_ID_INT_Mng idMng;

    void Awake()
    {
        G = this;
    }

    static public void StartMake(){G._StartMake();}

    static public SJ_ID_INT_Mng IDMaker(){return G.idMng;}

    static public uint Make_UID( System.Type type )
    {
        return G.idMng.Make_UID( type );
    }


    public void _StartMake()
    {
        syncMono.func_PlayStep = OnNextPlayStep;
        syncMono.func_EndALL = OnEndALL;
    }


    static public void NextMake(){}
    public void _NextMake()
    {
        SJ_SimpleSyncMono.NextPlaySelf();
    }

    public void OnNextPlayStep( object obj )
    {
        GameObject go = obj as GameObject;
        MakeBase makeBase = go.GetComponent<MakeBase>();
        if( mode_make_load == 0 )
        {
            makeBase.OnMake();
        }
        else
        {
            makeBase.OnLoad();
        }
    }

    public void OnEndALL()
    {
        MakeBase[] makes = GetComponentsInChildren<MakeBase>();
        foreach( var s in makes )
        {
            s.OnAfterWork();
        }
    }

}
