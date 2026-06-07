using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SJ_Child_Hide : MonoBehaviour
{
    public List<string> lt_hide;
    public bool     exec;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( exec )
        {
            Work();
            exec = false;
        }
    }

    public void     Work()
    {
        Transform[] ch = GetComponentsInChildren<Transform>();
        foreach( Transform s in ch )
        {
            if( lt_hide.Contains( s.gameObject.name ) )
            {
                s.gameObject.SetActive(false);
            }
        }
    }
}
