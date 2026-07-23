using UnityEngine;

public class PlayerMover : SJ_MapTileMover
{

    // 1. 트리거 레이어
    // 도착위치의 2번 레이어 트리거 
    // 던전 메이커의 특수 방( 보스 던전 등등 )

    // 2. 상호작용
    // 제자리 객체나 앞에 객체 가져오기
    // 일단 객체들이 2개 동시에 상호작용 하는 배치는 제외

    public override void OnMoveEnd()
    {
        
    }

}
