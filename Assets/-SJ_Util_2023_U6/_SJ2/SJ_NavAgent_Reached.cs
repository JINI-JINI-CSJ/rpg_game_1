using UnityEngine.AI;
using UnityEngine;

public class SJ_NavAgent_Reached
{
    public NavMeshAgent agent;
    public bool hasReachedDestination;
    public float move_time_max = 20;
    float move_time_start;

    public SJ_CallFunc func_Reached = new SJ_CallFunc();
    public SJ_CallFunc func_TimeOver = new SJ_CallFunc();


    public bool GetRandomPointOnNavMesh( Transform tr , ref Vector3 pos , float random_pos_range = 10 )
    {
        for (int i = 0; i < 30; i++) // 최대 30번 시도
        {
            // 
            Vector3 sp_random = Random.insideUnitSphere * random_pos_range;
            sp_random.y *= 0.2f; // 고저차는 줄이고

            Vector3 randomPos = tr.position + sp_random;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                pos = hit.position;
                return true;
            }
        }
        // 실패 시 중심 좌표 반환
        return false;
    }

    public bool MoveRandom( Transform tr , float random_pos_range = 10 )
    {
        Vector3 pos = Vector3.zero;
        if( !GetRandomPointOnNavMesh( tr , ref pos , random_pos_range ) )
        {
            return false;
        }
        SetStart( pos );
        return true;
    }


    public void SetStart( Vector3 pos , bool noStop = false )
    {
        if( agent.enabled == false ) return;

        if( noStop == false )
            Stop();

        //Debug.Log( "SetStart ~~~~~~~~~ : " + pos );

        move_time_start = Time.time;
        agent.SetDestination(pos);
    }

    public void Stop( bool disable_agent = false )
    {
        if( disable_agent )
            agent.enabled = false;
        hasReachedDestination = false;
        if (agent != null && agent.enabled )
        {
            agent.ResetPath();
        }
            
    }

    public void Update()
    {
        if( agent.enabled == false ) return;

        //Debug.Log( "agent.transform : " + agent.transform.position );

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    if (!hasReachedDestination)
                    {
                        hasReachedDestination = true;
                        func_Reached.Func();
                    }
                }
            }
            else
            {
                hasReachedDestination = false;

                if( move_time_max > 0 )
                {
                    if( Time.time >= (move_time_start + move_time_max) )
                    {
                        agent.ResetPath();
                        func_TimeOver.Func();
                    }                    
                }
            }
        }
    }
}