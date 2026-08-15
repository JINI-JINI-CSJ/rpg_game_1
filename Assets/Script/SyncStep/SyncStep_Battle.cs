using UnityEngine;

public class SyncStep_Battle : SJ_GameSyncStepBase
{
    public _ENEMY_BATTLE_INIT enemy_inf;

    public void PlayStep()
    {
        BattleMain.G.InitBattle( enemy_inf.Make_BattleParty() );
        SJ_SimpleSyncMono.NextPlaySelf();
    }
}
