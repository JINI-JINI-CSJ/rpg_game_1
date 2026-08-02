using UnityEngine;

// 던전 배틀 인카운터
// 일반 JRPG 처럼 걸을수록 인카운트 확률 올라감
// 초반 몇 걸음은 노 카운트?
public class DungeonBattleInCounter : MonoBehaviour
{
    // 노 카운트 최소 걸음
    public int NoCount_Move = 10;

    // 걸음당 추가 확률
    public float AddPer_OneMove = 0.1f;

    // 최대 확률 
    public float MaxPer = 30;

    // 현재 확률
    public float cur_per;

    // 현재 걸음수
    public int cur_move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
