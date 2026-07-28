using UnityEngine;

public class BattleMain : MonoBehaviour
{
    static public BattleMain G;
    public GameObject go_CameraBattle; // 배틀 카메라
    public BattlePartyView_Enemy view_Enemy;

    public BattleParty battleParty_Enemy;

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

    // 적군을 세팅해서 넘겨주기
    public void InitBattle( BattleParty bp_enemy )
    {
        battleParty_Enemy = bp_enemy;
        view_Enemy.Init();

        go_CameraBattle.SetActive(true);
    }
}
