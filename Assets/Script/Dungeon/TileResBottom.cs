using UnityEngine;

public class TileResBottom : MonoBehaviour
{
    public GameObject go_MiniMap_Fog;   // 이 객체가 미니맵 전용이라면 가리는 객체

    // 타일 배치됬을때 자동으로 세팅
    public Vector2Int cur_pos;

    // 타일 객체가 있을때만 ..
    public TileResObj tileResObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPos( int x , int y )
    {
        cur_pos.x = x;
        cur_pos.y = y;
    }

    public void OpenFog()
    {
        go_MiniMap_Fog?.SetActive( false );
    }

    public TileResObj Inst_TileResObj( TileResObj prf )
    {
        GameObject inst = GameObject.Instantiate( prf.gameObject );
        SJ_Unity.SetEqTrans( inst.transform , null , transform );
        return inst.GetComponent<TileResObj>();
    }
}
