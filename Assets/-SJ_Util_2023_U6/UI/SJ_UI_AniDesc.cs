using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UI_AniDesc : MonoBehaviour
{
    public GameObject go_Parent;
    public Animator animator;
    public string str_ani_Open = "ANI_GUI_POPUP_Open";
    public string str_ani_Close = "ANI_GUI_POPUP_Close";

    public AnimationClip animationClipOpen;
    public AnimationClip animationClipClose;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LinkCreate( GameObject obj_par )
    {
        go_Parent = obj_par;

        //Transform[] tr_child = obj_par.GetComponentsInChildren<Transform>( true );
        List<Transform> tr_child = new List<Transform>();
        for( int i = 0 ; i < obj_par.transform.childCount ; i++ )
        {
            Transform tr = obj_par.transform.GetChild( i );
            tr_child.Add( tr );
        }

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2( 0 , 0 );
        SJ_Unity.SetEqTrans( transform , null , obj_par.transform );

        foreach( Transform tr in tr_child )
        {
            tr.parent = transform;
        }

        gameObject.SetActive( true );
    }


    public void OpenAni()
    {
        //SJ_UnityUIMng.ANI_playing = 1;

        if( animator == null )animator = GetComponent<Animator>();

        if( go_Parent != null )
        {
            go_Parent.SetActive(true);
        }

        animator.Play( str_ani_Open );

        if( animationClipOpen != null )
        {
            float time = animationClipOpen.length;
            StartCoroutine( Wait_Open( time ) );
        }
    }

    IEnumerator Wait_Open( float time )
    {
        yield return new WaitForSeconds( time );
        //SJ_UnityUIMng.ANI_playing = 0;
        SJ_UnityUIMng.End_CurAniQueue();
        SJ_UnityUIMng.Check_NextQueue();
    }

    public void CloseAni()
    {
        //SJ_UnityUIMng.ANI_playing = 1;

        if( animator == null )animator = GetComponent<Animator>();

        animator.Play("ANI_GUI_POPUP_Close");

        if( animationClipClose != null )
        {
            float time = animationClipClose.length;
            StartCoroutine( Wait_Hide( time ) );
        }
    }

    IEnumerator Wait_Hide( float time )
    {
        yield return new WaitForSeconds( time );
        //SJ_UnityUIMng.ANI_playing = 0;
        //Debug.Log( "Wait_Hide : " + time );
        // if( go_Parent != null )
        // {
        //     SJ_UnityUIMng.g.Close_Prc( go_Parent );            
        //     go_Parent.SetActive(false);
        // }
        SJ_UnityUIMng.g.Close_Prc( go_Parent );
        SJ_UnityUIMng.End_CurAniQueue();
        SJ_UnityUIMng.Check_NextQueue();
    }
}
