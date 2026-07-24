using UnityEngine;

// 사용 안함
// MapEventPlayer 대체
// 인트로 던전
// 
public class IntroMain : MonoBehaviour
{
    public SJ_SimpleSyncMono syncMono;

    void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void OnLoadCSV()
    {
        syncMono.StartPlay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
