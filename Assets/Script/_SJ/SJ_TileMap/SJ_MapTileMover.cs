using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


// 4 방향 , 값은 시계방향 
public enum _DIR_NEWS
{
    None = -1, 
    N = 0 , 
    E = 1 , 
    S = 2 , 
    W = 3 , 
}

// SJ_MapTileViewer 위에서 작동하는 이동 객체
public class SJ_MapTileMover : MonoBehaviour
{
    // 필수
    public SJ_MapTileViewer sJ_MapTile;

    // 필수
    // 현재 컴포넌트에 붙이자
    // 이동 회전 커브
    public SJ_Curve_TransObjToggle curve_trans;

    public _DIR_NEWS        cur_dir;
    public Vector2Int       cur_pos;

    public PlayerInput      playerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInputAble( bool b )
    {
        playerInput.enabled = b;
    }

    public void InitPos( Vector2Int pos , _DIR_NEWS dir = _DIR_NEWS.N )
    {
        if( CheckMapTile() == false ) return;
        transform.localRotation = Rot_DIR_NEWS(dir);
        transform.localPosition = sJ_MapTile.GetPos( pos );
        cur_pos = pos;
        cur_dir = dir;
    }

    public bool CheckMapTile()
    {
        if( sJ_MapTile == null || curve_trans == null )
        {
            Debug.LogError( "에러!!! sJ_MapTile == null  || curve_trans == null  " );
            return false;
        }
        return true;
    }

    static public float Angle_DIR_NEWS( _DIR_NEWS dir )
    {
        switch( dir )
        {
            case _DIR_NEWS.N: return 0;
            case _DIR_NEWS.S: return 180;
            case _DIR_NEWS.E: return 90;
            case _DIR_NEWS.W: return 270;
        }
        return 0;
    }

    static public Quaternion Rot_DIR_NEWS( _DIR_NEWS dir )
    {
        return Quaternion.Euler( 0 ,Angle_DIR_NEWS(dir) , 0 );
    }

    static public Vector2Int PosByDir( _DIR_NEWS dir )
    {
        switch( dir )
        {
            case _DIR_NEWS.N: return Vector2Int.up;
            case _DIR_NEWS.S: return Vector2Int.down;
            case _DIR_NEWS.E: return Vector2Int.right;
            case _DIR_NEWS.W: return Vector2Int.left;
        }
        return Vector2Int.zero;
    }

    public Vector2Int MovePosCurDir( bool front_back )
    {
        // 바라 보는 방향으로 값
        Vector2Int next_off = PosByDir( cur_dir );
        if( front_back == false )
        {
            next_off *= -1;
        }
        return next_off;
    }

    // 값1 -> 좌 : 마이너스 값 , 우 : 플러스 값
    // 값2 -> 앞뒤 전환
    static public _DIR_NEWS Offset_DIR_NEWS( _DIR_NEWS dir , int offset )
    {
        // 절대값 4 초과 에러

        int dir_int = (int)dir;
        int dir_next_int = dir_int + offset;
        dir_next_int = dir_next_int % 4; // 4방향 값 4 로 나머지
        if( dir_next_int < 0 )
        {
            dir_next_int = 4 + dir_next_int;
        }
        return ( _DIR_NEWS ) dir_next_int;
    }

    // 현재 앞의 위치
    public Vector2Int GetPos_Front()
    {
        return cur_pos + PosByDir( cur_dir );
    }

    public bool CheckMoveAblePos( Vector2Int pos )
    {
        if( CheckMapTile() == false ) return false;
        if( sJ_MapTile.OnMoveAble( pos ) == false ) return false;
        return OnCheckMoveAblePos(pos);
    }

    virtual public bool OnCheckMoveAblePos( Vector2Int pos )
    {
        return true;
    }

    virtual public void OnMoveStart(){}

    virtual public void OnMoveEnd(){}

    public bool StartMove( Vector2Int pos_tar )
    {
        if( CheckMapTile() == false ) return false;
        if( pos_tar == cur_pos ) return false;

        Vector3 pos_move_target = sJ_MapTile.GetPos( pos_tar );

        curve_trans.func_OnEnd = OnMoveEnd;
        curve_trans.use_pos = true;
        curve_trans.use_rot = false;
        curve_trans.use_scl = false;
        curve_trans.pos_s = transform.localPosition;
        curve_trans.pos_e = pos_move_target;
        curve_trans.StartFunc_FWD( null , true );
        cur_pos = pos_tar;

        OnMoveStart();

        return true;
    }

    public bool StartRot( _DIR_NEWS dir_target )
    {
        if( CheckMapTile() == false ) return false;
        if( cur_dir == dir_target )

        curve_trans.func_OnEnd = OnMoveEnd;
        curve_trans.use_pos = false;
        curve_trans.use_rot = true;
        curve_trans.use_scl = false;
        curve_trans.rot_s = transform.localRotation;
        curve_trans.rot_e = Rot_DIR_NEWS( dir_target );
        curve_trans.StartFunc_FWD( null , true );
        cur_dir = dir_target;

        OnMoveStart();

        return true;
    }

    // 이동 앞뒤
    // true : 전진 , false : 후진
    public bool StartMove_FB( bool front_back  )
    {
        Vector2Int pos = cur_pos;
        Vector2Int pos_next_off = MovePosCurDir( front_back );
        pos += pos_next_off;
        if( CheckMoveAblePos( pos ) == false ) return false;
        return StartMove( pos );
    }

    // 회전 
    // 왼쪽 : -1 , 오른쪽 : 1
    public bool StartRot_LR( int left_right  )
    {
        _DIR_NEWS dir_target = Offset_DIR_NEWS( cur_dir , left_right );
        return StartRot( dir_target );
    }

    // 테스트 메뉴
    [ContextMenu("초기화 무작위 위치")]
    public void InitRandom()
    {
        if( CheckMapTile() == false ) return;
        InitPos( sJ_MapTile.RandomAblePos() );
    }    

    [ContextMenu("전진")]
    public void MoveFront()
    {
        if( curve_trans.isPlaying() ) return;
        StartMove_FB( true );
    }

    [ContextMenu("후진")]
    public void MoveBack()
    {
        if( curve_trans.isPlaying() ) return;
        StartMove_FB( false );
    }

    [ContextMenu("좌회전")]
    public void RotLeft()
    {
        if( curve_trans.isPlaying() ) return;
        StartRot_LR( -1 );
    }

    [ContextMenu("우회전")]
    public void RotRight()
    {
        if( curve_trans.isPlaying() ) return;
        StartRot_LR( 1 );
    }

    public void OnMove(InputValue value)
    {
        _DIR_NEWS dir = MoveKey_ByVec2( value.Get<Vector2>() );
        switch( dir )
        {
            case _DIR_NEWS.N: MoveFront();break;
            case _DIR_NEWS.S: MoveBack();break;
            case _DIR_NEWS.W: RotLeft();break;
            case _DIR_NEWS.E: RotRight();break;
        }
    }

    public _DIR_NEWS MoveKey_ByVec2(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            // 좌우 이동
            if (input.x < -0.1f)    return _DIR_NEWS.W;
            if (input.x > 0.1f)     return _DIR_NEWS.E;
        }
        else
        {
            // 상하 이동
            // 유니티 인풋시스템 상하 반대
            if (input.y < -0.1f) return _DIR_NEWS.S;
            if (input.y > 0.1f)  return _DIR_NEWS.N;
        }
        return _DIR_NEWS.None;
    }

    public void InitPos_MapStart()
    {
        
    }

    //public 
}
