using UnityEngine;

// 타일맵 
// 기존 타일 뷰어에서 레이어 1이상 데이터를 가져온다.

// 레이어 0 : 지형 : 타일 0 만 사용
// 레이어 1 : 포털 등등 중요 객체 , 타일 1~10
// 레이어 2 : 이벤트 11~20


public class GTF_TileMap : SJ_MapTileViewer_claude
{
    static public GTF_TileMap G;

    public MapEventPlayer mapEvent;

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

    static public void StartMap()
    {
        if( G == null )
        {
            Debug.LogError( "주 타일맵 없음" );
            return;
        }
        G._StartMap();
    }

    public void _StartMap()
    {
        // 1. 타일맵 로드 및 생성
        // 2. 시작점 배치
        // 3. 트리거 이벤트 있으면 실행
        MENU_Load();
        PlayerStartPos();
    }

    public void PlayerStartPos()
    {
        
    }
}
