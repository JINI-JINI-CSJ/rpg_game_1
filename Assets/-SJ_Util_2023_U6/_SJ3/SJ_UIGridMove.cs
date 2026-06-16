using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 타일 그리드 형태로 유아이 객체들을 나열한다.
// 상하좌우로 커서 이동한다.
// 각 객체에 활성토글 신호
public class SJ_UIGridMove : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;

    // 1개 행
    [System.Serializable]
    public class _ROW_GO
    {
        public List<GameObject> arr_x = new List<GameObject>();   

        public int FixMoveX( int x , bool find = true )
        {
            if( x < 0 ) x = 0;
            if( x >= arr_x.Count ) x =  arr_x.Count-1;
            return x;
        }

        public int Find( GameObject go )
        {
            return arr_x.FindIndex( x => x == go );
        }
    }

    public List<_ROW_GO> _ROW_s;
    public Vector2Int cur_pos;
    public GameObject recent_active;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        Init();
    }

    // List<_ROW_GO> 를 참조해서 격자 만들기
    public void Init()
    {
        recent_active = null;
        CurActive();
    }

    void CurActive()
    {
        if( recent_active != null )
        {
            SJ_Unity.SendMsg( recent_active , "OnGrid_MOVE" , false );
        }

        if( _ROW_s.Count < 1 )return;

        _ROW_GO cur_r = _ROW_s[cur_pos.y];
        recent_active = cur_r.arr_x[cur_pos.x];
        SJ_Unity.SendMsg( recent_active , "OnGrid_MOVE" , true );
    }

    // 좌 -1 , 우 1
    public void MoveX( int dir )
    {
        int x = cur_pos.x + dir;
        _ROW_GO cur_r = _ROW_s[cur_pos.y];
        cur_pos.x = cur_r.FixMoveX( x );
        CurActive();
    }

    // 위 -1 , 아래 1
    public void MoveY( int dir )
    {
        int y = cur_pos.y + dir;
        if( y < 0 ) y = 0;
        if( y >= _ROW_s.Count )y = _ROW_s.Count - 1;
        cur_pos.y = y;
        CurActive();
    }

    public void Align_By_GridLayoutGroup( bool clear_row = true )
    {
        if( gridLayoutGroup == null ) return;

        if( clear_row )
        {
            _ROW_s.Clear();
        }

        if( _ROW_s.Count > 0 ) return;

        // 현재는 픽스 컬럼 카운트만 지원
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            int y_pre = -1;
            _ROW_GO row = null;
            int columnCount = gridLayoutGroup.constraintCount;
            for( int i = 0 ; i < gridLayoutGroup.transform.childCount ; i++ )
            {
                int y = i / columnCount;
                int x = i % columnCount;
                if( y_pre != y )
                {
                    y_pre = y;
                    row = new _ROW_GO();
                    _ROW_s.Add(row);
                }
                Transform tr_ch = gridLayoutGroup.transform.GetChild(i);
                row.arr_x.Add(tr_ch.gameObject);
            }
        }
    }

    public void SelectByGameObj( GameObject go )
    {
        for( int y = 0 ; y < _ROW_s.Count ; y++ )
        {
            _ROW_GO row = _ROW_s[y];
            int x = row.Find( go );
            if( x >= 0 )
            {
                cur_pos.x = x;
                cur_pos.y = y;
                CurActive();
                return;
            }
        }
    }
}
