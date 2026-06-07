using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 현재 이 위치로 캐릭터 컨트롤러가 따라온다.
// 회전도 해주자
public class SJ_ChrCtrlFollower : MonoBehaviour
{
    public CharacterController chrCtrl;
    public float frameLerp = 0.9f;
    public float attach_len = 0.05f; // 이거리 이내로 오면 완전히 붙인다.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateLerp();
    }

    void LateUpdate()
    {
        UpdateLerp();
    }

    public void StartPlay( CharacterController _c = null  )
    {
        chrCtrl = _c;
    }

    public void UpdateLerp()
    {
        if( chrCtrl == null ) return;

        float dist = Vector3.Distance( transform.position , chrCtrl.transform.position );
        
        if( dist <= attach_len )
        {
            Vector3 move = transform.position - chrCtrl.transform.position;
            chrCtrl.Move( move );
            chrCtrl.transform.rotation = transform.rotation;
        }
        else
        {
            //Vector3 move = transform.position - chrCtrl.transform.position;
            Vector3 lerp_pos = Vector3.Lerp( chrCtrl.transform.position , transform.position , frameLerp );
            lerp_pos -= chrCtrl.transform.position;

            chrCtrl.Move( lerp_pos );
            chrCtrl.transform.rotation = Quaternion.Slerp(chrCtrl.transform.rotation, transform.rotation, frameLerp );
        }
    }

    public void EndPlay()
    {
        chrCtrl = null;
    }
}
