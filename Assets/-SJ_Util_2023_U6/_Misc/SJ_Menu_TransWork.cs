using UnityEngine;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;


public class SJ_Menu_TransWork : MonoBehaviour
{
    public enum TRANS_WORK_TYPE
    {
        Pos ,
        Rot ,
        Scl , 
        Copy ,
        Copy_Hide ,
        Parent_NULL
    }

    [System.Serializable]
    public class _TRANS_WORK
    {
        public bool             no_work;

        public bool             isWorld;
        public TRANS_WORK_TYPE  work_type;
        public Vector3          arg = Vector3.one;

        public void     Work( Transform tr )
        {
            if( no_work ) return;

            switch( work_type )
            {
                case TRANS_WORK_TYPE.Pos:
                {
                    if( isWorld )
                    {
                        tr.position = arg;
                    }else{
                        tr.localPosition = arg;
                    }
                }
                break;

                case TRANS_WORK_TYPE.Rot:
                {
                    if( isWorld )
                    {
                        tr.rotation = Quaternion.Euler( arg.x , arg.y , arg.z );
                    }else{
                        tr.localRotation = Quaternion.Euler( arg.x , arg.y , arg.z );
                    }
                }
                break;

                case TRANS_WORK_TYPE.Scl:
                {
                    tr.localScale = arg;
                }
                break;
                case TRANS_WORK_TYPE.Copy:
                {
                    
                    GameObject inst = GameObject.Instantiate( tr.gameObject );
                    //inst.SetActive(false);
                    inst.transform.parent = tr.parent;
                    inst.transform.position = tr.position;
                }
                break;
                case TRANS_WORK_TYPE.Copy_Hide:
                {
                    
                    GameObject inst = GameObject.Instantiate( tr.gameObject );
                    inst.SetActive(false);
                    inst.transform.parent = tr.parent;
                }
                break;

                case TRANS_WORK_TYPE.Parent_NULL:
                {
                    tr.parent = null;
                }
                break;

            }
        }
    }

    public List<_TRANS_WORK>    works;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void    Work(GameObject go)
    {
        foreach( _TRANS_WORK s in works )
        {
            s.Work(go.transform);
        }
    }

}
