using UnityEngine;

public class SJ_TileCoordBase : MonoBehaviour
{
    public SJ_MAP_LAYER_TILE_COORD pos_layer = new();

    public bool noMove;

    // 현재 제자리에서 상호작용 여부
    public bool nowPosInterAble;

    // 바로 앞에서 상호 작용 가능 여부
    public bool frontInterAble;


    public void SetPosLayer( Vector2Int _p , int layer )
    {
        pos_layer.pos = _p;
        pos_layer.layer = layer;
    }

    virtual public void OnFrontPlayer(){}
    // 상호 작용 시작
    virtual public void OnInteract( int ID ){}


}
