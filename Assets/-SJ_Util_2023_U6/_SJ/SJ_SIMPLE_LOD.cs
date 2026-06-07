using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class SJ_SIMPLE_LOD : MonoBehaviour
{
    [System.Serializable]
    public class _LOD
    {
        public float        dist;
        public GameObject   go;
        public void SetActive(bool b)
        {
            if(go!= null)go.SetActive(b);
        }
    }
    public List<_LOD> lt_LOD;

    int recent_step = -1;

    public float dist_cur;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() 
    {
        UpdateLod();
    }

    [ContextMenu("UpdateLod")]
    public void UpdateLod()
    {
        if( recent_step < 0 )
        {
            foreach( _LOD l in lt_LOD )
            {
                l.SetActive(false);
            }
        }

        dist_cur = Vector3.Distance(transform.position , Camera.main.transform.position);

        //if( 0.001f > Mathf.Abs( recent_dist - dist_cur ) ) return;

        int cur_step = lt_LOD.Count - 1;
        for( int  i = 0; i < lt_LOD.Count - 1 ; i++ )
        {
            if(dist_cur < lt_LOD[i].dist )
            {
                cur_step = i;
                break;
            }
        }

        if( recent_step == cur_step ) return;

        if( recent_step >= 0 )lt_LOD[recent_step].SetActive(false);
        lt_LOD[cur_step].SetActive(true);

        recent_step = cur_step;        

    }
}
