using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_AnimatorList : MonoBehaviour
{
    // [System.Serializable]
    // public class _STR_ANI
    // {
    //     public string   strName;
    //     //public string   aniName;        // 애니매이터 
    //     public AnimationClip    ani_clip;  // 애니메이션
    // }
    //public List<_STR_ANI>   lt_STR_ANI;
    //public Dictionary<string,_STR_ANI> dic_STR_ANI = new Dictionary<string, _STR_ANI>();

    public Animator animator;
    //public Animation ani;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool     Play( string strName )
    {
        if( animator == null )
        {
            animator = GetComponentInChildren<Animator>();
            if( animator == null )
            {
                Debug.LogError( "에러!!! animator 없음!!!" + gameObject.name );
                return false;
            }            
        }

        animator.Play( strName );

        // if( lt_STR_ANI.Count < 1 ) return false;
        // if( dic_STR_ANI.Count < 1 )
        // {
        //     foreach( _STR_ANI s in lt_STR_ANI )
        //     {
        //         dic_STR_ANI[s.strName] = s;
        //     }
        // }

        // _STR_ANI s_find = null;
        // if( dic_STR_ANI.TryGetValue( strName , out s_find ) == false ) return false;

        // if( animator != null )
        // {
        //     animator.Play( s_find.strName );            
        // }else if( ani != null )
        // {
        //     SJ_Unity.AnimationClip_CrossPlay( ani , s_find.ani_clip );
        // }

        return true;
    }

}
