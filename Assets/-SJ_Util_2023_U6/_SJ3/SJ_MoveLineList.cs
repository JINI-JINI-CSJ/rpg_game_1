using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_MoveLineList : MonoBehaviour
{
    public List<Vector3> move_line;
    public float move_time = 1;
    public Transform tr_moveObj;

    // 이동하는 방향으로 회전
    public bool     Rot_Move;
    public float    Rot_Slerp = 0.05f;

    public delegate void FUNC_END();

    public FUNC_END fund_end;

    float   elapse_cur;
    Vector3 cur_move;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Update_Pos( float elapse )
    {
        if( move_line == null || move_line.Count < 1 ) return;

        float r_b = elapse_cur / move_time;
        elapse_cur += elapse;
        float r_c = elapse_cur / move_time;

        Vector3 pos_b = GetPositionOnPath( move_line , r_b );
        Vector3 pos_c = GetPositionOnPath( move_line , r_c );
        if( tr_moveObj == null) return;
        tr_moveObj.position = pos_c;
 
        if( Rot_Move )
        {
            Vector3 dir = pos_c - pos_b;
            dir.Normalize();
            Quaternion rot_target = Quaternion.LookRotation( dir );
            tr_moveObj.rotation = Quaternion.Slerp( tr_moveObj.rotation , rot_target , Rot_Slerp );
        }

        if( elapse_cur >= move_time )
        {
            fund_end?.Invoke();
            enabled = false;
        }
    }

    public void StartMove( List<Vector3> vs , FUNC_END _func_end = null )
    {   
        move_line = vs;
        fund_end = _func_end;
        elapse_cur = 0;
        enabled = true;
    }

    public static Vector3 GetPositionOnPath(List<Vector3> points, float t)
    {
        if (points == null || points.Count == 0) return Vector3.zero;
        if (points.Count == 1) return points[0];
        
        // t를 0~1 범위로 클램프
        t = Mathf.Clamp01(t);
        
        // 전체 세그먼트 수
        int segmentCount = points.Count - 1;
        
        // t를 세그먼트 인덱스와 로컬 t로 변환
        float scaledT = t * segmentCount;
        int index = Mathf.Min((int)scaledT, segmentCount - 1);
        float localT = scaledT - index;
        
        // 두 점 사이 선형 보간
        return Vector3.Lerp(points[index], points[index + 1], localT);
    }
}
