using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SJ_SimpleTransLerp : MonoBehaviour
{
    
    public Transform tr_target;

    [Header("=== 일반 러프 ===")]
    public float lerp_pos = 1.0f;
    public float lerp_rot = 1.0f;

    public bool usePos;
    public Vector3 pos_target;

    public bool useRot;
    public Vector3 rot_target;

    public bool useLookAt;
    public Transform tr_LooAt;

    [Header("=== 타임 러프 ===")]
    public bool use_startPlay_lerp;
    public float playTime = 0.3f;
    public AnimationCurve curve;

    Vector3 pos_start;

    Quaternion rot_start;

    float start_time;

    Action action_end;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( use_startPlay_lerp )
        {
            float elapse = Time.time - start_time;
            float r = elapse / playTime;
            bool end = false;
            if( r >= 1 )
            {
                r = 1;
                end = true;
            } 

            float ani_r = curve.Evaluate(r);

            if( tr_target != null )
            {
                transform.position = Vector3.Lerp( pos_start , tr_target.position , ani_r );
                transform.rotation = Quaternion.Slerp( rot_start , tr_target.rotation , ani_r );                   
            }
            if( usePos )
                transform.localPosition = Vector3.Lerp( pos_start , pos_target , ani_r );
            if( useRot )
            {
                if( useLookAt )
                {
                    Quaternion qt_look = quaternion.identity;
                    Vector3 pos_tar = tr_LooAt.position - transform.position;
                    pos_tar.Normalize();
                    qt_look.SetLookRotation( pos_tar );
                    transform.rotation = Quaternion.Slerp( transform.rotation , qt_look , ani_r );   
                }
                else
                {
                    transform.localRotation = Quaternion.Slerp( rot_start , Quaternion.Euler( rot_target ) , ani_r );                      
                }
               
            }


            if( end )
            {
                use_startPlay_lerp = false;
                action_end?.Invoke();
                enabled = false;                
            }

        }
        else
        {
            if( tr_target != null )
            {
                transform.position = Vector3.Lerp( transform.position , tr_target.position , lerp_pos );
                transform.rotation = Quaternion.Slerp( transform.rotation , tr_target.rotation , lerp_pos );                   
            }
            if( usePos )
                transform.localPosition = Vector3.Lerp( transform.localPosition , pos_target , lerp_pos );
            if( useRot )
                transform.localRotation = Quaternion.Slerp( transform.localRotation , Quaternion.Euler( rot_target ) , lerp_pos );              
        }
    }

    public void PlayStartValueMode( Action end_func = null )
    {
        if( use_startPlay_lerp ) return;

        pos_start = transform.localPosition;
        rot_start = transform.localRotation;
        useLookAt = false;

        StartPlay( end_func  );
    }

    public void PlayStartLookAt( Transform tr_target , Action end_func = null )
    {
        useLookAt = true;
        tr_LooAt = tr_target;
        StartPlay( end_func );
    }

    public void StartPlay( Action end_func = null )
    {
        enabled = true;
        use_startPlay_lerp = true;
        start_time = Time.time;
        SJ_Unity.SetUnityAction_OneFunc( action_end , end_func );
    }
}
