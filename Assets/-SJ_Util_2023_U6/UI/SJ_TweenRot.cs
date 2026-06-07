using UnityEngine;

public class SJ_TweenRot : SJ_UITweenBase
{
    public Vector3 rot_start;
    public Vector3 rot_end;
    public Transform tr_obj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FrameMove( Time.deltaTime );
    }

    public override void OnFrameMove()
    {
        Vector3 rot = Vector3.Lerp( rot_start , rot_end , ratio_cur );

        //Debug.Log( rot );

        if( tr_obj != null )
        {
            tr_obj.localRotation = Quaternion.Euler( rot );
        }else{
            transform.localRotation = Quaternion.Euler( rot );
        }
    }
}
