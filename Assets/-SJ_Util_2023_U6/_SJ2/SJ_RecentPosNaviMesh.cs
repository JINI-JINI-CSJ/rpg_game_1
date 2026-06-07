using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

// 새로운 위치가 네비 메쉬 표면에 없으면 이전 위치 
public class SJ_RecentPosNaviMesh : MonoBehaviour
{
    Vector3 pos_recent;
    public void SetInit( Vector3 v )
    {
        pos_recent = v;
    }

    public void Check_Pos()
    {
        Vector3 cur_pos = transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(cur_pos, out hit, 2.0f, NavMesh.AllAreas))
        {
        }
        else
        {
            transform.position = pos_recent;
        }
    }

}
