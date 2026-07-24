using System.Collections;
using System.IO;
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

    virtual public void Save( BinaryWriter bw )
    {
        pos_layer.Save(bw);
        OnSave(bw);
    }
    virtual public void OnSave( BinaryWriter bw ){}

    virtual public void Load( BinaryReader br )
    {
        pos_layer.Load(br);
        OnLoad(br);
    }
    virtual public void OnLoad( BinaryReader br ){}


    virtual public void OnLoadAfter(){}

    virtual public bool OnAble_Interact(){ return false; }

    virtual public void OnPre_Interact(){}

    // 상호 작용 시작
    virtual public void OnInteract( int ID ){}

    public void WaitFunc( float wait , SJ_COMMON.Func_VOID _func )
    {
        StartCoroutine( CO_WaitFunc( wait , _func ) );
    }

    IEnumerator CO_WaitFunc( float wait , SJ_COMMON.Func_VOID _func )
    {
        yield return new WaitForSeconds( wait );
        _func?.Invoke();
    }


    
}
