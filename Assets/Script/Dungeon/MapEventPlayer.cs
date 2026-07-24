using UnityEngine;

public class MapEventPlayer : MonoBehaviour
{
    static public MapEventPlayer G;

    // 싱크 이벤트를 여러개를 하위로 놓고
    // 처음 시작만 여기 등록
    // 그 후 트리거 실행은 차일드 이름 찾기
    public SJ_SimpleSyncMono syncMono_Start;


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

    static public bool StartEventPlay()
    {
        if( G == null ) return false;
        G.syncMono_Start.StartPlay();
        return true;
    }

    static public bool RunEventPlay( string evt_name )
    {
        if( G == null ) return false;

        Transform tr = G.transform.Find( evt_name );
        if( tr == null )
        {
            Debug.LogError( "이벤트 없음 : " + G.gameObject.name + "   : " + evt_name );
            return false;
        }

        SJ_SimpleSyncMono syncMono = tr.GetComponent<SJ_SimpleSyncMono>();
        if( syncMono == null )
        {
            Debug.LogError( "syncMono == null : " + G.gameObject.name + "   : " + evt_name );
            return false;
        }

        syncMono.StartPlay();
        return true;
    } 
}
