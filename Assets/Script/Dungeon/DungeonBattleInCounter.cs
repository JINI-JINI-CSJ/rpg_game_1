using UnityEngine;

// 던전 배틀 인카운터
// 일반 JRPG 처럼 걸을수록 인카운트 확률 올라감
// 초반 몇 걸음은 노 카운트?

[System.Serializable]
public class DUNGEON_INCOUNT_INF
{
    public int      NoCount_Move = 10;

    // 걸음당 추가 확률
    public float    AddPer_OneMove = 0.5f;

    // 최대 확률 
    public float    MaxPer = 30;    
}

public class DungeonBattleInCounter : MonoBehaviour
{
    public DUNGEON_INCOUNT_INF incount_inf;

    // 현재 확률
    public float    cur_per;

    // 현재 걸음수
    public int      cur_move;

    public Mng_X128SS mng_X128;

    public SJ_COMMON.Func_VOID func_InCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        
    }

    public void Clear()
    {
        cur_per = 0;
        cur_move = 0;
    }

    public void OnMoveEnd()
    {
        cur_move++;
        cur_per += incount_inf.AddPer_OneMove;
        if( cur_per >= incount_inf.MaxPer ) cur_per = incount_inf.MaxPer;

        if( Check_InCount() )
        {
            // 전투 시작
            func_InCount?.Invoke();
        }
    }

    bool Check_InCount()
    {
        return mng_X128.RandomFloat_Per( cur_per * 0.01f );
    }

}
