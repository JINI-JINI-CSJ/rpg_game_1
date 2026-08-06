using System.Collections.Generic;
using UnityEngine;

// 적군 위치 배열
public class BattlePartyView_Enemy : MonoBehaviour
{
    public List<BattleEnemyObjView> tr_Front;
    public List<BattleEnemyObjView> tr_Back;

    // 배치 그리드
    //public SJ_LineGridPos gridPos_front;

    //public SJ_LineGridPos gridPos_back;

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
        foreach( var s in tr_Front )s.Clear();
        foreach( var s in tr_Back )s.Clear();

        // 배틀 매인의 적군 파티를 참고해서 배치
        BattleParty bp_enemy = BattleMain.G.battleParty_Enemy;

        InitLine( tr_Front , bp_enemy.chars_Front );
        InitLine( tr_Back , bp_enemy.chars_Back );
    }

    void InitLine( List<BattleEnemyObjView> view , CharBase[] chars )
    {
        for( int i = 0 ; i < chars.Length ; i++ )
        {
            view[i].InitCharBase( chars[i] );
        }
    }

}
